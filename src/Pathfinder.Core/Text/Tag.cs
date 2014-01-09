using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Outlander.Core.Text
{
	public class Tag
	{
		private string _text;

		public Tag()
		{
		}

		public Tag(string text)
		{
			Text = text;
		}

		public string Text {
			get {
				return _text;
			}
			private set {
				_text = value;
				OnTextSet();
			}
		}

		protected virtual void OnTextSet()
		{
		}

		public override string ToString()
		{
			return string.Format("[Tag: Text={0}]", Text);
		}

		public static TTag For<TTag>(string text) where TTag : Tag, new()
		{
			return new TTag(){ Text = text };
		}
	}	
}
