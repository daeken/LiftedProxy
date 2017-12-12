using Bridge.jQuery2;

namespace Client {
	public enum SplitDirection {
		Vertical, Horizontal
	}
	public class SplitComponent : IComponent {
		public jQuery RootElement { get; }

		readonly jQuery firstContentElement, secondContentElement;
		public IComponent FirstContent {
			set {
				firstContentElement.Contents().Remove();
				firstContentElement.Append(value);
			}
		}

		public IComponent SecondContent {
			set {
				secondContentElement.Contents().Remove();
				secondContentElement.Append(value);
			}
		}

		public SplitComponent(SplitDirection direction, float splitPercentage = 50) {
			var horiz = direction == SplitDirection.Horizontal;
			splitPercentage = 100 - splitPercentage;
			RootElement = new jQuery("<div>").AddClass("split-pane", horiz ? "horizontal-percent" : "vertical-percent");
			RootElement.Append(firstContentElement = new jQuery("<div>").AddClass("split-pane-component").Css(horiz ? "bottom" : "right", $"{splitPercentage}%"));
			RootElement.Append(new jQuery("<div>").AddClass("split-pane-divider").Css(horiz ? "bottom" : "right", $"{splitPercentage}%"));
			RootElement.Append(secondContentElement = new jQuery("<div>").AddClass("split-pane-component").Css(horiz ? "height" : "width", $"{splitPercentage}%"));
			((dynamic) RootElement).splitPane();
		}
	}
}