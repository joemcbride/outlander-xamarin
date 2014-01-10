using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client
{
	public interface IVariableReplacer
	{
		string Replace(string data, ScriptContext context);
	}

	public class VariableReplacer : IVariableReplacer
	{
		private const string Local_Vars_Regex = "%([a-zA-z0-9\\.]+)";
		private const string Global_Vars_Regex = "\\$([a-zA-z0-9\\.]+)";

		public string Replace(string data, ScriptContext context)
		{
			data = SetArguments(data, context);
			data = SetLocalVars(data, context);
			data = SetGlobalVars(data, context);

			return data;
		}

		private string SetLocalVars(string data, ScriptContext context)
		{
			if(context.LocalVars != null) {
				var matches = Regex.Matches(data, Local_Vars_Regex);
				foreach(Match match in matches) {
					var value = context.LocalVars.Get(match.Groups[1].Value);
					if(!string.IsNullOrWhiteSpace(value))
						data = Regex.Replace(data, match.Groups[0].Value, value);
				}
			}

			return data;
		}

		private string SetGlobalVars(string data, ScriptContext context)
		{
			var matches = Regex.Matches(data, Global_Vars_Regex);
			foreach(Match match in matches) {
				var value = context.GlobalVar.Get(match.Groups[1].Value);
				if(!string.IsNullOrWhiteSpace(value))
					data = Regex.Replace(data, match.Groups[0].Value.Replace("$", "\\$"), value);
			}

			return data;
		}

		private string SetArguments(string data, ScriptContext context)
		{
			if(context.CurrentArgs != null)
			{
				var args = string.Join(" ", context.CurrentArgs.Select(t => {
					if(t.Contains(" "))
					{
						t = "\"{0}\"".ToFormat(t);
					}
					return t;
				}));
				data = ReplaceArgument(data, 0, args);

				for(int i = 0; i < context.CurrentArgs.Length; i++) {
					data = ReplaceArgument(data, i + 1, context.CurrentArgs[i]);
				}
			}

			return data;
		}

		private string ReplaceArgument(string data, int argument, string value)
		{
			return data.Replace("$" + argument.ToString(), value);
		}
	}
}
