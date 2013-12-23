using System;
using System.Text;

namespace Pathfinder.Core.Tests
{
	public class InMemoryLogger : ILog
	{
		public StringBuilder Builder = new StringBuilder();

		public void Info(string message)
		{
			Builder.AppendLine(message);
		}

		public void Warn(string message)
		{
			Builder.AppendLine(message);
		}

		public void Error(Exception exception)
		{
			Builder.AppendLine(exception.ToString());
		}
	}
}
