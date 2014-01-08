using System;
using System.Collections.Generic;

namespace Pathfinder.Core
{
	public interface IDataTracker<T> : IObservable<T>
	{
		void Publish(T item);
		void EndTransmission();
	}

	public class DataTracker<T> : IDataTracker<T>
	{
		protected List<IObserver<T>> _observers;

		public DataTracker()
		{
			_observers = new List<IObserver<T>>();
		}

		IDisposable IObservable<T>.Subscribe(IObserver<T> observer) 
		{
			if (!_observers.Contains(observer)) 
				_observers.Add(observer);

			return new Unsubscriber(_observers, observer);
		}

		public virtual void Publish(T item)
		{
			foreach (var observer in _observers.ToArray()) {
				observer.OnNext(item);
			}
		}

		public void EndTransmission()
		{
			foreach (var observer in _observers.ToArray())
				if (_observers.Contains(observer))
					observer.OnCompleted();

			_observers.Clear();
		}

		private class Unsubscriber : IDisposable
		{
			private List<IObserver<T>>_observers;
			private IObserver<T> _observer;

			public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
			{
				this._observers = observers;
				this._observer = observer;
			}

			public void Dispose()
			{
				if (_observer != null && _observers.Contains(_observer))
					_observers.Remove(_observer);
			}
		}
	}

	public interface IDataReporter<T> : IObserver<T>
	{
	}

	public class DataReporter<T> : IDataReporter<T>
	{
		private IDisposable _unsubscriber;
		private string _Id;

		public DataReporter(string id)
		{
			_Id = id;
		}

		public string Id
		{
			get { return _Id; }
		}

		public virtual void Subscribe(IObservable<T> provider)
		{
			if (provider != null)
				_unsubscriber = provider.Subscribe(this);
		}

		public virtual void OnCompleted()
		{
			Unsubscribe();
		}

		public virtual void OnError(Exception e)
		{
		}

		public virtual void OnNext(T value)
		{
		}

		public virtual void Unsubscribe()
		{
			if(_unsubscriber != null)
				_unsubscriber.Dispose();
		}
	}
}
