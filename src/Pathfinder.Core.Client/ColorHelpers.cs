using System;
using System.Globalization;

namespace Pathfinder.Core.Client
{
	public static class ColorHelpers
	{
		public static void FromRGBToCMYK(float red, float green, float blue, out float c, out float m, out float y, out float k)
		{
			c = 1.0f - (red / 255);
			m = 1.0f - (green / 255);
			y = 1.0f - (blue / 255);
			k = Math.Min(c, Math.Min(m, y));

			c = (c - k) / (1 - k);
			m = (m - k) / (1 - k);
			y = (y - k) / (1 - k);

			c = float.Parse(c.ToString("0.0000"));
			y = float.Parse(y.ToString("0.0000"));
			m = float.Parse(m.ToString("0.0000"));
			k = float.Parse(k.ToString("0.0000"));
		}

		public static void FromHexToRGB(this string hexColor, out int red, out int green, out int blue)
		{
			//Remove # if present
			if (hexColor.IndexOf('#') != -1)
				hexColor = hexColor.Replace("#", "");

			red = 0;
			green = 0;
			blue = 0;

			if (hexColor.Length == 6)
			{
				//#RRGGBB
				red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
				green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
				blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
			}
			else if (hexColor.Length == 3)
			{
				//#RGB
				red = int.Parse(hexColor[0].ToString() + hexColor[0].ToString(), NumberStyles.AllowHexSpecifier);
				green = int.Parse(hexColor[1].ToString() + hexColor[1].ToString(), NumberStyles.AllowHexSpecifier);
				blue = int.Parse(hexColor[2].ToString() + hexColor[2].ToString(), NumberStyles.AllowHexSpecifier);
			}
		}
	}
}
