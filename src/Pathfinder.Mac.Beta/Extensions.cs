using System;
using System.Globalization;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Pathfinder.Core.Client;

namespace Pathfinder.Mac.Beta
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

	public static class Extensions
	{
		public static NSAttributedString CreateString(this string text, NSColor color)
		{
			var defaults = new DefaultSettings ();
			return CreateString (text, color, defaults.Font);
		}

		public static NSAttributedString CreateString(this string text, NSColor color, NSFont font)
		{
			NSObject[] objects = new NSObject[] { color, font };
			NSObject[] keys = new NSObject[] { NSAttributedString.ForegroundColorAttributeName, NSAttributedString.FontAttributeName };
			NSDictionary attributes = NSDictionary.FromObjectsAndKeys (objects, keys);

			return new NSAttributedString (text, attributes);
		}

		public static NSColor ToNSColor(this string hexColor)
		{
			int red;
			int green;
			int blue;

			hexColor.FromHexToRGB(out red, out green, out blue);

			//return NSColor.FromDeviceRgba(red, green, blue, 1.0f);

			float c = 0.6168f;
			float m = 0.0000f;
			float y = 1.0000f;
			float k = 0.5804f;
			float alpha = 1.0f;

			ColorHelpers.FromRGBToCMYK(red, green, blue, out c, out m, out y, out k);

			return NSColor.FromDeviceCymka(c, m, y, k, alpha);
		}
	}
}
