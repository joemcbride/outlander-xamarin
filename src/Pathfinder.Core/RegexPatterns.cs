using System;

namespace Pathfinder.Core
{
	public class RegexPatterns
	{
		public const string Label = "^([\\w\\.-]+):$";
		public const string MonsterBold = "(<pushBold\\/>(.*?)<popBold\\/>)";
	}
}
