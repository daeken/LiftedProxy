using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;
using MoreLinq;
using Bridge.jQuery2;

namespace Client {
	public class TableComponent : IComponent {
		public jQuery RootElement { get; }
		jQuery TBody;

		public TableComponent(IEnumerable<IComponent> headings=null) {
			RootElement = new jQuery("<table>").AddClass("table", "table-bordered");
			if(headings != null)
				RootElement.Append(new jQuery("<thead>").Append(new jQuery("<tr>").Append(headings.Select(x => new jQuery("<th>").Append(x)))));
			RootElement.Append(TBody = new jQuery("<tbody>"));
		}

		public TableComponent(IEnumerable<string> headings) : this(headings.Select(x => (TextComponent) x)) {
		}

		public void AddRow(IEnumerable<IComponent> row) {
			TBody.Append(new jQuery("<tr>").Append(row.Select(x => new jQuery("<td>").Append(x))));
		}
		public void AddRow(IEnumerable<string> row) {
			AddRow(row.Select(x => (TextComponent) x));
		}
		public void AddRow(params IComponent[] row) {
			AddRow(row.ToEnumerable<IComponent>());
		}
		public void AddRow(params string[] row) {
			AddRow(row.ToEnumerable<string>());
		}
	}
}