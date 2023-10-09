using PyBox.Shared.Services.Interops;
using System.IO.Compression;

namespace PyBox.PyRunner
{
    public static class DependencyInstaller
    {
        public static async Task<InteropsResult> Check()
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, "pypy");
                if (!Directory.Exists(path) || !File.Exists(Path.Combine(path, "pypy.exe")))
                {
                    return new InteropsResult()
                    {
                        Error = "Application directory not found. Please start the installation process using the api endpoint: \"configurations/install\"",
                        NotInstalled = true,
                        Result = "Application directory not found. Please start the installation process using the api endpoint: \"configurations/install\""
                    };
                }
            }
            catch (Exception ex)
            {
                return new InteropsResult()
                {
                    Error = ex.Message,
                    NotInstalled = true,
                    Result = "An error occurred while trying to check the files"
                };
            }
            return new InteropsResult()
            {
                Error = string.Empty,
                NotInstalled = false,
                Result = "All dependencies are installed"
            };
        }
        public static async Task<InteropsResult> InstallDependencyAsync()
        {
            string tempFile = Path.GetTempFileName() + ".zip";
            try
            {
                Uri uri = new Uri("https://downloads.python.org/pypy/pypy3.10-v7.3.13-win64.zip");
                using (HttpClient client = new HttpClient())
                {
                    using (Stream s = await client.GetStreamAsync(uri))
                    {
                        using (FileStream fs = new FileStream(tempFile, FileMode.Create))
                        {
                            await s.CopyToAsync(fs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new InteropsResult()
                {
                    Error = ex.Message,
                    NotInstalled = true,
                    Result = "An error occurred while trying to download dependencies"
                };
            }
            string destination = Path.Combine(AppContext.BaseDirectory, "pypy");
            if (Directory.Exists(tempFile))
            {
                try
                {
                    Directory.Delete(destination, true);
                }
                catch (Exception ex)
                {
                    return new InteropsResult()
                    {
                        Error = ex.Message,
                        NotInstalled = true,
                        Result = "An error occurred while trying to delete the old pypy folder"
                    };
                }
            }
            try
            {
                ZipFile.ExtractToDirectory(tempFile, AppContext.BaseDirectory);
            }
            catch (Exception ex)
            {
                return new InteropsResult()
                {
                    Error = ex.Message,
                    NotInstalled = true,
                    Result = "An error occurred while trying to unzip the files"
                };
            }
            try
            {
                Directory.Move(Path.Combine(AppContext.BaseDirectory, "pypy3.10-v7.3.13-win64"), destination);
            }
            catch (Exception ex)
            {
                return new InteropsResult()
                {
                    Error = ex.Message,
                    NotInstalled = true,
                    Result = "An error occurred while trying to rename the extracted folder"
                };
            }
            try
            {
                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                return new InteropsResult()
                {
                    Error = ex.Message,
                    NotInstalled = false,
                    Result = "An error occurred while trying to remove the temporary files"
                };
            }
            return new InteropsResult()
            {
                Error = string.Empty,
                NotInstalled = false,
                Result = "Installation completed successfully"
            };
        }
    }
}
