using System;

namespace Outlander.Core.Client
{
	public class Token
	{
		public string Type { get; set; }
		public string Value { get; set; }
		public string Text { get; set; }
		public bool Ignore { get; set; }
	}
}
