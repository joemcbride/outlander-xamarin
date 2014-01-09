using System;

namespace Outlander.Core.Client.Scripting
{
	public class ScriptToken : Token
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string[] Args { get; set; }
	}
}
