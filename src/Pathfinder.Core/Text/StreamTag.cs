using System;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class StreamTag : Tag
	{
		private const string Id_Regex = "id=\"(\\w+)\"";
		private const string Value_Regex = "";

		public string Id { get; private set; }
		public string Value { get; private set; }

		protected override void OnTextSet()
		{
			var idMatch = Regex.Match(Text, Id_Regex);

			if (idMatch.Success) {
				Id = idMatch.Groups[1].Value;
				Value = Regex.Replace(Text, "<[^>]*>", string.Empty).TrimStart();
			}
		}
	}
}
