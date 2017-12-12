using System.Collections.Generic;
using System.Linq;
using Bridge.jQuery2;

namespace Client {
	public static class Extensions {
		public static jQuery Append(this jQuery root, IComponent component) {
			return root.Append(component.RootElement);
		}

		public static jQuery Append(this jQuery root, IEnumerable<IComponent> components) {
			foreach(var comp in components)
				root.Append(comp.RootElement);
			return root;
		}

		public static jQuery Append(this jQuery root, IEnumerable<jQuery> components) {
			foreach(var comp in components)
				root.Append(comp);
			return root;
		}

		public static jQuery AddClass(this jQuery root, params string[] classes) {
			foreach(var cls in classes)
				root = root.AddClass(cls);
			return root;
		}
	}
}