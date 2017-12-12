using Bridge.jQuery2;

namespace Client {
	public class PaneComponent : IComponent {
		public jQuery RootElement { get; }
		readonly jQuery contentElement;

		public IComponent Content {
			set {
				contentElement.Contents().Remove();
				contentElement.Append(value);
			}
		}
		
		public PaneComponent(string heading=null) {
			RootElement = new jQuery("<div>").AddClass("panel", "panel-default");
			if(heading != null) {
				var he = new jQuery("<div>").AddClass("panel-heading");
				he.Append(new jQuery("<h3>").AddClass("panel-title").Text(heading));
				RootElement.Append(he);
			}
			RootElement.Append(contentElement = new jQuery("<div>").AddClass("panel-body"));
		}
	}
}