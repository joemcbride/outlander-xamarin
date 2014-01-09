using System;
using System.Globalization;

namespace Outlander.Core.Client
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

			c = (float)Math.Round((decimal)c, 2);
			y = (float)Math.Round((decimal)y, 2);
			m = (float)Math.Round((decimal)m, 2);
			k = (float)Math.Round((decimal)k, 2);
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

		public static void FromHexToOSXRGB(this string hexColor, out float red, out float green, out float blue)
		{
			int r;
			int g;
			int b;

			FromHexToRGB(hexColor, out r, out g, out b);

			red = (1.0F * r) / 255.0f;
			green = (1.0F * g) / 255.0f;
			blue = (1.0F * b) / 255.0f;
		}
	}
}
