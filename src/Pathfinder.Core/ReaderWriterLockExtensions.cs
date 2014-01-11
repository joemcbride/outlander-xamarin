using System;
using System.Threading;

namespace Outlander.Core
{
	public static class ReaderWriterLockExtensions
	{
		public static void Write(this ReaderWriterLockSlim rwLock, Action action)
		{
			rwLock.EnterWriteLock();
			try
			{
				action();
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		public static void Read(this ReaderWriterLockSlim rwLock, Action action)
		{
			rwLock.EnterReadLock();
			try
			{
				action();
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public static T Read<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
		{
			rwLock.EnterReadLock();
			try
			{
				return func();
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}
	}
}
