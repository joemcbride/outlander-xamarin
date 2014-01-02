using System;
using System.Collections.Generic;

namespace Pathfinder.Core.Client
{
	public interface IHighlighter
	{
		IEnumerable<TextTag> Highlight(TextTag text);
	}
}
