using System;
using Bridge.Html5;
using Bridge.jQuery2;

namespace Client {
	public class TextComponent : IComponent {
		public string Value {
			get { return RootElement.Text(); }
			set { RootElement.Text(value); }
		}
		public jQuery RootElement { get; }

		public TextComponent(string value = null) {
			RootElement = new jQuery(Document.CreateTextNode(value ?? ""));
		}

		public static implicit operator TextComponent(string value) {
			return new TextComponent(value);
		}
	}
}