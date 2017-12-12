using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;
using MoreLinq;
using Bridge.jQuery2;

namespace Client {
	public class TableComponent<RowT> : IComponent {
		public jQuery RootElement { get; }
		readonly jQuery TBody;

		public event EventHandler<RowT> Select;
		public event EventHandler<RowT> Click;
		public event EventHandler<RowT> DoubleClick;

		Dictionary<int, RowT> rowBindings = new Dictionary<int, RowT>();

		public TableComponent(IEnumerable<IComponent> headings=null) {
			RootElement = new jQuery("<table>").AddClass("table", "table-bordered");
			if(headings != null)
				RootElement.Append(new jQuery("<thead>").Append(new jQuery("<tr>").Append(headings.Select(x => new jQuery("<th>").Append(x)))));
			RootElement.Append(TBody = new jQuery("<tbody>"));
		}

		public TableComponent(IEnumerable<string> headings) : this(headings.Select(x => (TextComponent) x)) {
		}

		void Clicked(jQueryMouseEvent evt) {
			var tr = new jQuery(evt.Target).Closest("tr");
			if(tr.Length == 0)
				return;
			var idx = (int) tr.Data("row-id");
			if(!rowBindings.ContainsKey(idx))
				return;
			var obj = rowBindings[idx];
			Click?.Invoke(this, obj);
			if(Select != null) {
				tr.Parent().Children(".table-row-selected").RemoveClass("table-row-selected");
				tr.AddClass("table-row-selected");
				Select(this, obj);
			}
		}

		void DoubleClicked(jQueryMouseEvent evt) {
			var tr = new jQuery(evt.Target).Closest("tr");
			if(tr.Length == 0)
				return;
			var idx = (int) tr.Data("row-id");
			if(!rowBindings.ContainsKey(idx))
				return;
			var obj = rowBindings[idx];
			DoubleClick?.Invoke(this, obj);
		}

		public void AddRow(RowT obj, IEnumerable<IComponent> row) {
			TBody.Append(new jQuery("<tr>").Append(row.Select(x => new jQuery("<td>").Append(x))).Data("row-id", rowBindings.Count).Click(Clicked).DblClick(DoubleClicked));
			rowBindings[rowBindings.Count] = obj;
		}
		public void AddRow(RowT obj, IEnumerable<string> row) {
			AddRow(obj, row.Select(x => (TextComponent) x));
		}
		public void AddRow(RowT obj, params IComponent[] row) {
			AddRow(obj, row.ToEnumerable<IComponent>());
		}
		public void AddRow(RowT obj, params string[] row) {
			AddRow(obj, row.ToEnumerable<string>());
		}
	}
}