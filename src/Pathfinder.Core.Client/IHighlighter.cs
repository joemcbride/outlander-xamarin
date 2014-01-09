using System;
using System.Collections.Generic;

namespace Outlander.Core.Client
{
	public interface IHighlighter
	{
		IEnumerable<TextTag> Highlight(TextTag text);
	}
}
