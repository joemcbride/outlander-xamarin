using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Outlander.Core.Text
{
	public interface ITagTransformer
	{
		bool Matches(Tag tag);
		Tag Transform(Tag tag);
	}
	
}
