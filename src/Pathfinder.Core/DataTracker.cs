using System;
using System.Collections.Generic;

namespace Pathfinder.Core
{
	public class DataTracker<T> : IObservable<T>
	{
		private List<IObserver<T>> _observers;

		public DataTracker()
		{
			_observers = new List<IObserver<T>>();
		}

		public IDisposable Subscribe(IObserver<T> observer) 
		{
			if (!_observers.Contains(observer)) 
				_observers.Add(observer);

			return new Unsubscriber(_observers, observer);
		}

		public void Publish(T item)
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

	public class DataReporter<T> : IObserver<T>
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
