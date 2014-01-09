using System;

namespace Outlander.Core
{
	public class RegexPatterns
	{
		public const string Arguments = "(?<match>[\\w:]+)|\"(?<match>[\\S\\s]*)\"";
		public const string Label = "^([\\w\\.-]+):$";
		public const string MonsterBold = "(<pushBold\\/>(.*?)<popBold\\/>)";
	}
}
