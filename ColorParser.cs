//
// ColorParser.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;

using MonoMac.AppKit;

namespace PdfOverlay
{
	public static class ColorParser
	{
		public static NSColor Parse (string color)
		{
			Func<string, int, int, byte> parseHex = (s, i, l) => {
				var t = Convert.ToByte (s.Substring (i, l), 16);
				if (l == 1) {
					return (byte)((t << 4) | t);
				} else {
					return t;
				}
			};

			color = color.Trim ();

			if (color.Length > 0 && color [0] == '#') {
				byte r = 0, g = 0, b = 0, a = 0xff;

				if (color.Length == 4 || color.Length == 5) {
					r = parseHex (color, 1, 1);
					g = parseHex (color, 2, 1);
					b = parseHex (color, 3, 1);
					if (color.Length == 5) {
						a = parseHex (color, 4, 1);
					}
				} else if (color.Length == 7 || color.Length == 9) {
					r = parseHex (color, 1, 2);
					g = parseHex (color, 3, 2);
					b = parseHex (color, 5, 2);
					if (color.Length == 9) {
						a = parseHex (color, 7, 2);
					}
				}

				return NSColor.FromSrgb (r / 255f, g / 255f, b / 255f, a / 255f);
			}

			return (
				from list in NSColorList.AvailableColorLists
				from name in list.AllKeys ()
				where String.Equals (name, color, StringComparison.InvariantCultureIgnoreCase)
				select list.ColorWithKey (name)
			).FirstOrDefault ();
		}

		public static NSColor Parse (string color, bool throwOnError)
		{
			var c = Parse (color);
			if (c == null) {
				throw new Exception ("Invalid color: " + color);
			}
			return c;
		}
	}
}