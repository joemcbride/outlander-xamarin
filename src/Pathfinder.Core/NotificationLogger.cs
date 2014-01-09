using System;

namespace Outlander.Core
{
	public class NotificationLogger : ILog
	{
		public Action<Exception> OnError { get; set; }

		public void Info(string message)
		{
		}

		public void Warn(string message)
		{
		}

		public void Error(Exception exc)
		{
			var onError = OnError;
			if(onError != null)
			{
				onError(exc);
			}
		}
	}
}
