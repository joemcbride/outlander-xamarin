using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Pathfinder.Core
{
	public class SimpleFileLogger : ILog
	{
		private readonly string _filename = "log.txt";
		private readonly string _errorsFilename = "errors.txt";

		private static object LockObject = new object();

		public SimpleFileLogger()
			: this("log.txt", "errors.txt")
		{
		}

		public SimpleFileLogger(string fileName, string errorsFileName)
		{
			_filename = fileName;
			_errorsFilename = errorsFileName;
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
			Write(_errorsFilename, string.Format("Error: {0}", exception));
		}

		private void Write(string filename, string data)
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents/Pathfinder");
			var fullPath = Path.Combine(path, filename);

			try {
				lock (LockObject) {
					using (var writer = File.AppendText(fullPath)) {
						writer.Write(data);
					}
				}
			} catch (Exception exc) {
				Debug.WriteLine(exc);
			}
		}
	}
}
