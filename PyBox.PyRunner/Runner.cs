using System.Diagnostics;

namespace PyBox.PyRunner
{
	public class Runner : IDisposable
	{
		private string? executionPath, executable, scriptName;
		private Process? process;
		public bool IsBusy { get; private set; } = false;
		public bool IsKilled { get; private set; } = false;
		public string Result { get; private set; } = string.Empty;
		public string Errors { get; private set; } = string.Empty;
		public bool NotInstalled { get; private set; } = false;
		private bool HaveParameters = false;

		public Runner(byte[] script, bool haveParameters)
		{
			HaveParameters = haveParameters;
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
				Arguments = string.Format($"\"{scriptName}\""),
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
					process!.Start();
				}
				catch (Exception ex)
				{
					IsBusy = false;
					Errors = ex.ToString();
					return;
				}
				Stopwatch sw = Stopwatch.StartNew();
				while (!process.HasExited)
				{
					if (sw.ElapsedMilliseconds > 10000)
					{
						Kill();
					}
				}
				if (!IsKilled)
				{
					string[] data = GetData();
					Result = data[0];
					Errors = data[1];
				}
				else
				{
					Result = "";
					Errors = "TimeOut error. Your script exceeded the maximum execution time (10 seconds)";
				}
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
				using (StreamReader reader = process!.StandardOutput)
					result = reader.ReadToEnd();
				string errors = process.StandardError.ReadToEnd();
				if (!string.IsNullOrEmpty(result))
					result = result.Replace(executionPath!, "").Replace(scriptName!, "<scriptName>");
				if (!string.IsNullOrEmpty(errors))
				{
					errors = errors.Replace(executionPath!, "").Replace(scriptName!, "<scriptName>");
					if (errors.Contains(", line ") && HaveParameters)
					{
						try
						{
							int line = int.Parse(errors.Split(", line ")[1].Split(',')[0]);
							errors = errors.Replace($", line {line}", $", line {line - 1}");
						}
						catch { }
					}
				}
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
				process!.Kill();
			}
			catch { }
			IsKilled = true;
		}

		public void Dispose()
		{
			File.Delete(Path.Combine(executionPath!, scriptName!));
			process!.Dispose();
			process = null;
		}
	}
}