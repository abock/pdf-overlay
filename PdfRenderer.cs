//
// PdfRenderer.cs
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
using System.Drawing;

using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace PdfOverlay
{
	public class PdfRenderer : IDisposable
	{
		public NSColor Color { get; set; }
		public NSFont Font { get; set; }
		public RectangleF Bounds { get; set; }
		public NSTextAlignment HAlign { get; set; }
		public NSTextAlignment VAlign { get; set; }

		CGContextPDF context;
		bool isTemplated;
		bool ensured;

		public PdfRenderer ()
		{
		}

		public void Open (string outputFile)
		{
			context = new CGContextPDF (NSUrl.FromFilename (outputFile));
		}

		void Ensure ()
		{
			if (ensured) {
				return;
			}

			ensured = true;

			if (!isTemplated) {
				context.BeginPage (Bounds);
			}
		}

		public void Dispose ()
		{
			if (context != null) {
				context.EndPage ();
				context.Dispose ();
				context = null;
			}
		}

		public void SetFont (string fontDesc)
		{
			string name;
			float size = 10;

			var i = fontDesc.LastIndexOf (' ');
			if (i < 0) {
				name = fontDesc;
			} else {
				name = fontDesc.Substring (0, i);
				size = Single.Parse (fontDesc.Substring (i + 1));
			}

			Font = NSFont.FromFontName (name, size);
		}

		public void SetBounds (string boundsDesc)
		{
			var bounds = RectangleF.Empty;
			var parts = boundsDesc.Split (new [] { ',' }, 4);

			for (int i = 0; i < parts.Length; i++) {
				var val = Single.Parse (parts [i].Trim ());
				switch (i) {
				case 0:
					bounds.X = val;
					break;
				case 1:
					bounds.Y = val;
					break;
				case 2:
					bounds.Width = val;
					break;
				case 3:
					bounds.Height = val;
					break;
				}
			}

			Bounds = bounds;
		}

		public void SetColor (string colorDesc)
		{
			Color = ColorParser.Parse (colorDesc, true);
		}

		NSTextAlignment ParseAlignment (string alignDesc)
		{
			switch (alignDesc.Trim ().ToLower ()) {
			case "center":
			case "middle":
				return NSTextAlignment.Center;
			case "right":
			case "bottom":
				return NSTextAlignment.Right;
			case "left":
			case "top":
			default:
				return NSTextAlignment.Left;
			}
		}

		public void SetHAlign (string alignDesc)
		{
			HAlign = ParseAlignment (alignDesc);
		}

		public void SetVAlign (string alignDesc)
		{
			VAlign = ParseAlignment (alignDesc);
		}

		public void DrawTemplatePage (string templateFile)
		{
			if (isTemplated) {
				return;
			}

			isTemplated = true;

			using (var template = CGPDFDocument.FromFile (templateFile)) {
				var templatePage = template.GetPage (1);
				var templateBounds = templatePage.GetBoxRect (CGPDFBox.Crop);
				context.BeginPage (templateBounds);
				context.DrawPDFPage (templatePage);
			}
		}

		public void DrawRect ()
		{
			Ensure ();

			context.SetFillColor (Color.CGColor);
			context.FillRect (Bounds);
		}

		public void DrawText (string text)
		{
			Ensure ();

			var gc = NSGraphicsContext.FromGraphicsPort (context, false);
			gc.SaveGraphicsState ();
			NSGraphicsContext.CurrentContext = gc;

			var str = new NSAttributedString (text, foregroundColor: Color, font: Font);
			var size = str.Size;

			var box = Bounds;

			switch (HAlign) {
			case NSTextAlignment.Right:
				box.X += box.Width - size.Width;
				break;
			case NSTextAlignment.Center:
				box.X += (box.Width - size.Width) / 2;
				break;
			}

			switch (VAlign) {
			case NSTextAlignment.Left:
				box.Y += box.Height - size.Height;
				break;
			case NSTextAlignment.Center:
				box.Y += (box.Height - size.Height) / 2;
				break;
			}

			str.DrawString (box.Location);

			NSGraphicsContext.GlobalRestoreGraphicsState ();
		}
	}
}