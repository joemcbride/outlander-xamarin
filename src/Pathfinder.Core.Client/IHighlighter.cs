using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public interface IHighlighter
	{
		IEnumerable<TextTag> Highlight(TextTag text);
	}
}
