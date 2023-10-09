using System.Diagnostics;

namespace PyBox.PyRunner
{
	public class Runner : IDisposable
	{
		private string executionPath, executable, scriptName;
		private Process process;
		public bool IsBusy { get; private set; } = false;
		public bool IsKilled { get; private set; } = false;
		public string Result { get; private set; } = string.Empty;
		public string Errors { get; private set; } = string.Empty;
		public bool NotInstalled { get; private set; } = false;

		public Runner(byte[] script, string parameters)
		{
			executionPath = Path.Combine(AppContext.BaseDirectory, "pypy");
			if (!Directory.Exists(executionPath))
				return;
			executable = Path.Combine(executionPath, "pypy.exe");
			if (!File.Exists(executable))
				return;
			scriptName = $"{Guid.NewGuid()}.py";
			File.WriteAllBytes(Path.Combine(executionPath, scriptName), script);
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				FileName = executable,
				Arguments = string.Format("\"{0}\" {1}", scriptName, parameters),
				WorkingDirectory = executionPath,
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};
			process = new Process() { StartInfo = startInfo };
		}
		public async Task Run()
		{
			if (Directory.Exists(executionPath))
			{
				try
				{
					IsBusy = true;
					process.Start();
				}
				catch (Exception ex)
				{
					IsBusy = false;
					Errors = ex.ToString();
					return;
				}
				await process.WaitForExitAsync();
				string[] data = !IsKilled ? GetData() : new string[] { "", "" };
				Result = data[0];
				Errors = data[1];
				IsBusy = false;
			}
			else
			{
				Result = string.Empty;
				Errors = "Not all application components are installed!\n" +
					"Call the \"/configurations/install\" endpoint to complete the installation and try running the script again.";
				NotInstalled = true;
				IsBusy = false;
			}
		}

		private string[] GetData()
		{
			try
			{
				string result = "";
				using (StreamReader reader = process.StandardOutput)
					result = reader.ReadToEnd();
				string errors = process.StandardError.ReadToEnd();
				if (!string.IsNullOrEmpty(result))
					result = result.Replace(executionPath, "").Replace(scriptName, "<scriptName>");
				if (!string.IsNullOrEmpty(errors))
					errors = errors.Replace(executionPath, "").Replace(scriptName, "<scriptName>");
				return new string[] { result, errors };
			}
			catch (Exception ex)
			{
				return new string[] { "Read error", ex.ToString() };
			}
		}

		public void Kill()
		{
			try
			{
				process.Kill();
			}
			catch { }
			IsKilled = true;
		}

		public void Dispose()
		{
			File.Delete(Path.Combine(executionPath, scriptName));
			process.Dispose();
			process = null;
		}
	}
}