using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Pathfinder.Core.Client.Scripting
{
	public class ScriptToken : Token
	{
		public string Name { get; set; }
		public string[] Args { get; set; }
	}
}
