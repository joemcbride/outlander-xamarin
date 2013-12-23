using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;

namespace Pathfinder.Core
{
	public class SimpleFileLogger : ILog
	{
		private readonly string _filename = "log.txt";
		private readonly string _errorsFilename = "errors.txt";

		private IServiceLocator _services;

		private static object LockObject = new object();

		public SimpleFileLogger()
			: this("log.txt", "errors.txt", null)
		{
		}

		public SimpleFileLogger(string fileName, string errorsFileName, IServiceLocator services)
		{
			_filename = fileName;
			_errorsFilename = errorsFileName;
			_services = services;
		}

		public void Info(string data)
		{
			Write(_filename, data + "^---------------------^");
		}

		public void Warn(string data)
		{
			Write(_errorsFilename, data);
		}

		public void Error(Exception exception)
		{
			Write(_errorsFilename, string.Format("Error at {0}: {1}\n\n\n", DateTime.UtcNow, exception));
		}

		private void Write(string filename, string data)
		{
			var appSettings = _services.Get<AppSettings>();

			var fullPath = Path.Combine(appSettings.HomeDirectory, AppSettings.LogFolder, filename);

			lock (LockObject) {
				WriteTextAsync(fullPath, data);
			}
		}

		private async void WriteTextAsync(string filePath, string text)
		{
			try {
				byte[] encodedText = Encoding.Unicode.GetBytes(text);

				using (FileStream sourceStream = new FileStream(filePath,
					FileMode.Append, FileAccess.Write, FileShare.ReadWrite,
                        bufferSize: 4096, useAsync: true)) {
					await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
				}
			} catch (Exception exc) {
				Debug.WriteLine(exc);
			}
		}
	}
}
