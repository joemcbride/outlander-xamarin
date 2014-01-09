using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Outlander.Core.Client.Scripting
{
	public class IfBlocks
	{
		public IfBlocks()
		{
			IfEvalLineNumber = -1;
			IfBlockLineNumber = -1;
			ElseIfLineNumber = -1;
			ElseIfBlockLineNumber = -1;
			ElseBlockLineNumber = -1;
		}

		public int IfEvalLineNumber { get; set; }
		public string IfEval { get; set; }

		public int IfBlockLineNumber { get; set; }
		public string IfBlock { get; set; }

		public int ElseIfLineNumber { get; set; }
		public string ElseIf { get; set; }

		public int ElseIfBlockLineNumber { get; set; }
		public string ElseIfBlock { get; set; }

		public int ElseBlockLineNumber { get; set; }
		public string ElseBlock { get; set; }

		public int LastLineNumber
		{
			get
			{
				var lastLine = -1;
				var block = string.Empty;

				if(ElseBlockLineNumber > -1)
				{
					lastLine = ElseBlockLineNumber;
					block = ElseBlock;
				}
				else if(ElseIfBlockLineNumber > -1)
				{
					lastLine = ElseIfBlockLineNumber;
					block = ElseIfBlock;
				}
				else if(IfBlockLineNumber > -1)
				{
					lastLine = IfBlockLineNumber;
					block = IfBlock;
				}

				// want to be on the last line since the for loop
				// will ++ this number
				lastLine += (CountLineNumbers(block) - 1).Normalize();

				return lastLine;
			}
		}

		public int CountLineNumbers(string block)
		{
			block = block.EnsureEmpty();
			var count = 1;
			count += block.Where(c => c == '\n').Count();
			return count;
		}
	}
	
}
