using System;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public interface IVariableReplacer
	{
		string Replace(string data, ScriptContext context);
	}

	public class VariableReplacer : IVariableReplacer
	{
		private const string Local_Vars_Regex = "%(\\d+)";
		private const string Global_Vars_Regex = "\\$([a-zA-z0-9\\.]+)";

		public string Replace(string data, ScriptContext context)
		{
			var matches = Regex.Matches(data, Global_Vars_Regex);
			foreach(Match match in matches) {
				var value = context.GlobalVar.Get(match.Groups[1].Value);
				if(!string.IsNullOrWhiteSpace(value))
					data = Regex.Replace(data, match.Groups[0].Value.Replace("$", "\\$"), value);
			}

			if(context.LocalVars != null)
			{
				matches = Regex.Matches(data, Local_Vars_Regex);
				foreach(Match match in matches) {
					var value = context.LocalVars.Get(match.Groups[1].Value);
					if(!string.IsNullOrWhiteSpace(value))
						data = Regex.Replace(data, match.Groups[0].Value, value);
				}
			}

			return data;
		}
	}
}
