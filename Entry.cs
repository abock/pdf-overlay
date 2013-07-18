//
// Entry.cs
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

using Mono.Options;

namespace PdfOverlay
{
	public class Entry
	{
		static int Main (string[] args)
		{
			MonoMac.AppKit.NSApplication.Init ();

			var showHelp = false;
			var renderer = new PdfRenderer ();

			var options = new OptionSet {
				{ "o|output=", "Set the output PDF file (must be specified first)", v => renderer.Open (v) },
				{ "t|template=", "Draw the first page of a template PDF (must be the first draw command)", v => renderer.DrawTemplatePage (v) },
				{ "rect", "Draw a rectangle", v => renderer.DrawRect () },
				{ "text=", "Draw some text", v => renderer.DrawText (v) },
				{ "bounds=", "Set the current bounding box (X,Y,W,H)", v => renderer.SetBounds (v) },
				{ "font=", "Set the current context font ('<Name> <Size>')", v => renderer.SetFont (v) },
				{ "color=", "Set the current color (name, #rgb, #rgba, #rrggbb, #rrggbbaa)", v => renderer.SetColor (v) },
				{ "halign=", "Set the current horizontal alignment (left, center ,right)", v => renderer.SetHAlign (v) },
				{ "valign=", "Set the current vertical alignment (top, middle, bottom)", v => renderer.SetVAlign (v) },
				{ "help", "Show this help", v => showHelp = v != null }
			};

			try {
				if (args.Length > 0) {
					options.Parse (args);
				} else {
					showHelp = true;
				}
			} catch (Exception e) {
				Console.WriteLine ("Invalid option: {0}", e.Message);
				Console.WriteLine (e);
				return 1;
			}

			if (showHelp) {
				Console.WriteLine ("Usage: pdf-overlay [OPTIONS]+");
				Console.WriteLine ();
				Console.WriteLine ("Options:");

				options.WriteOptionDescriptions (Console.Out);

				return 1;
			}

			renderer.Dispose ();

			return 0;
		}
	}
}