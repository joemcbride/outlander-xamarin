using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public interface ITagTransformer
	{
		bool Matches(Tag tag);
		Tag Transform(Tag tag);
	}
	
}
