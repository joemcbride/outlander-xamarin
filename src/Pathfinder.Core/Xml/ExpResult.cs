using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core
{

	public class ExpResult : ComponentResult
	{
		public string Skill { get; set; }
		public double Rank { get; set; }

		public override bool IsValid()
		{
			return base.IsValid() && !string.IsNullOrWhiteSpace(Skill);
		}
	}
}
