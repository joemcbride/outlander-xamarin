using System;
using MonoMac.AppKit;

namespace Outlander.Mac.Beta
{
	public class DefaultSettings
	{
		public DefaultSettings ()
		{
			Font = NSFont.FromFontName("Geneva", 12);
			MonoFont = NSFont.FromFontName("Andale Mono", 12);
		}

		public NSFont Font { get; set; }
		public NSFont MonoFont { get; set; }
	}
}
