using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// http://caliburnmicro.codeplex.com/
namespace Outlander.Core
{
    /// <summary>
    /// A logger.
    /// </summary>
    public interface ILog {
        /// <summary>
        /// Logs the message as info.
        /// </summary>
		/// <param name="message">A formatted message.</param>
        void Info(string message);

        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
		/// <param name="message">A formatted message.</param>
		void Warn(string message);

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Error(Exception exception);
    }

    /// <summary>
    /// Used to manage logging.
    /// </summary>
    public static class LogManager {
        static readonly ILog NullLogInstance = new NullLog();

        /// <summary>
        /// Creates an <see cref="ILog"/> for the provided type.
        /// </summary>
        public static Func<Type, ILog> GetLog = type => NullLogInstance;
    }

	public class NullLog : ILog {
		public void Info(string message) { }
		public void Warn(string message) { }
		public void Error(Exception exception) { }
	}

	public class CompositeLog : ILog
	{
		private IList<ILog> _loggers = new List<ILog>();

		public CompositeLog()
			: this(Enumerable.Empty<ILog>())
		{
		}

		public CompositeLog(IEnumerable<ILog> loggers)
		{
			loggers.Apply(_loggers.Add);
		}

		public void Add(ILog logger)
		{
			_loggers.Add(logger);
		}

		public void Info(string message)
		{
			_loggers.Apply(logger => {
				logger.Info(message);
			});
		}

		public void Warn(string message)
		{
			_loggers.Apply(logger => {
				logger.Warn(message);
			});
		}

		public void Error(Exception exception)
		{
			_loggers.Apply(logger => {
				logger.Error(exception);
			});
		}
	}

	public class ConsoleLog : ILog
	{
		public void Info(string message)
		{
			Console.Write(message);
		}

		public void Warn(string message)
		{
			Console.Write(message);
		}

		public void Error(Exception exception)
		{
			Console.Write(exception);
		}
	}

	public class DebugLog : ILog
	{
		public void Info(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		public void Warn(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		public void Error(Exception exception)
		{
			System.Diagnostics.Debug.WriteLine(exception);
		}
	}
}
