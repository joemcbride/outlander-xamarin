using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Pathfinder.Core;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Xml;

namespace Pathfinder.Mac.Beta
{

	public class KeyUtil
	{
		public static NSKey GetKeys(NSEvent theEvent)
		{
			var nskey = Enum.ToObject (typeof(NSKey), theEvent.KeyCode);
			if ((theEvent.ModifierFlags & NSEventModifierMask.FunctionKeyMask) > 0) {
				var chars = theEvent.Characters.ToCharArray ();
				var thekey = chars [0];
				if (theEvent.KeyCode != (char)NSKey.ForwardDelete) {

					nskey = (NSKey)Enum.ToObject (typeof(NSKey), thekey);
				}
			}

			return (NSKey)nskey;
		}
	}
}
