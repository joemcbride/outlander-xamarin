using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Outlander.Core;

namespace Outlander.Mac.Beta
{
	[Register("VitalsBar")]
	public class VitalsBar : NSView
	{
		private float _value = 100.0f;
		private string _label = "Health";
		private string _backgroundColor = "#00004b";
		private PointF _textOffset = new PointF(40, 2);
		private NSFont _font = NSFont.FromFontName("Geneva", 11);

		public VitalsBar()
			: base()
		{
		}

		public VitalsBar(RectangleF rect)
			: base(rect)
		{
		}

		public VitalsBar(NSCoder rect)
			: base(rect)
		{
		}

		public VitalsBar(NSObjectFlag rect)
			: base(rect)
		{
		}

		public VitalsBar(IntPtr ptr)
			: base(ptr)
		{
		}

		public string Label {
			get {
				return _label;
			}
			set {
				_label = value;
				NeedsDisplay = true;
			}
		}

		public float Value {
			get {
				return _value;
			}
			set {
				if(value < 0)
					_value = 0;
				else if(_value > 100)
					_value = 100;
				else
					_value = value;

				NeedsDisplay = true;
			}
		}

		public string BackgroundColor {
			get {
				return _backgroundColor;
			}
			set {
				_backgroundColor = value;
				NeedsDisplay = true;
			}
		}

		public PointF TextOffset {
			get {
				return _textOffset;
			}
			set {
				_textOffset = value;
			}
		}

		public NSFont Font {
			get {
				return _font;
			}
			set {
				_font = value;
				NeedsDisplay = true;
			}
		}

		public override void DrawRect (RectangleF dirtyRect)
		{
			var height = base.Bounds.Height;
			var calcValue = base.Bounds.Width * (Value * 0.01f);

			var context = NSGraphicsContext.CurrentContext.GraphicsPort;
			context.SetStrokeColor("#999999".ToCGColor());
			context.SetLineWidth (1.0F);

			context.StrokeRect(new RectangleF(0, 0, base.Bounds.Width, height));

			context.SetFillColor("#999999".ToCGColor());
			context.FillRect(new RectangleF(0, 0, base.Bounds.Width, height));

			context.SetFillColor(BackgroundColor.ToCGColor());
			context.FillRect(new RectangleF(0, 0, calcValue, height));

			var attrStr = "{0}".ToFormat(Label).CreateString("#ffffff".ToNSColor(), Font);

			var storage   = new NSTextStorage();
			storage.SetString(attrStr);

			var layout    = new NSLayoutManager();
			var container = new NSTextContainer();

			layout.AddTextContainer(container);
			storage.AddLayoutManager(layout);

			// Get size
//			var size = layout.GetUsedRectForTextContainer (container).Size;
//			var width = size.Width / 2.0f;
			var width = (base.Bounds.Width / 2.0f) - (TextOffset.X);

			storage.DrawString(new PointF(width, TextOffset.Y));
		}
	}
}
