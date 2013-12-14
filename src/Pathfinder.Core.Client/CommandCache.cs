using System;
using System.Collections.Generic;

namespace Pathfinder.Core.Client
{
	public class CommandCache
	{
		private List<string> _commands = new List<string>();

		private int _currentIndex = -1;
		private int _maxSize;

		public CommandCache()
		{
			MaxSize = 20;
			CommandMinLength = 3;
		}

		public int CommandMinLength
		{
			get;
			set;
		}

		public int MaxSize
		{
			get { return _maxSize; }
			set
			{
				_maxSize = value;
				if(_maxSize < 0)
				{
					_maxSize = 0;
				}

				var diff = _commands.Count - _maxSize;
				if(diff > 0)
				{
					for(int i = 0; i < diff; i++) {
						_commands.RemoveAt(0);
					}
				}
			}
		}

		public void Add(string command)
		{
			_currentIndex = -1;

			if( string.IsNullOrWhiteSpace(command)
				|| command.Length < CommandMinLength
				|| command.Equals(LastCommand()))
				return;

			_commands.Add(command);

			if(_commands.Count > MaxSize)
			{
				_commands.RemoveAt(0);
			}
		}

		public string LastCommand()
		{
			var index = _commands.Count - 1;
			if(index < 0)
				return string.Empty;

			return _commands[index];
		}

		public string MovePrevious()
		{
			if(CurrentIndex == -1) {
				CurrentIndex = _commands.Count - 1;
			} else {
				CurrentIndex--;
			}
			return Current;
		}

		public string MoveNext()
		{
			if(CurrentIndex > -1) {
				CurrentIndex++;
			}

			return Current;
		}

		public int CurrentIndex
		{
			get { return _currentIndex; }
			private set
			{
				if(value >= _commands.Count)
					_currentIndex = -1;
				else if(value < 0)
					_currentIndex = -1;
				else
					_currentIndex = value;
			}
		}

		public string Current
		{
			get
			{
				if(_currentIndex < 0 || _currentIndex > _commands.Count)
					return string.Empty;

				return _commands[_currentIndex];
			}
		}

		public int Count
		{
			get { return _commands.Count; }
		}
	}
}
