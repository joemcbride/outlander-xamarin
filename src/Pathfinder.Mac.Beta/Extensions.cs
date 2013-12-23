using System;
using System.Globalization;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Pathfinder.Core.Client;

namespace Pathfinder.Mac.Beta
{
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
			float red;
			float green;
			float blue;

			hexColor.FromHexToOSXRGB(out red, out green, out blue);

			return NSColor.FromDeviceRgba(red, green, blue, 1.0f);
		}
	}
}
