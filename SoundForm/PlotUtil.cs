using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

//Install-Package OxyPlot.WindowsForms -Pre
//I do not know how to add from tool box in windowsForm 
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace SoundForm
{
	public class PlotUtil
	{
		public PlotView _pv = null;
		private LineSeries _lineSeries = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setForm">Add WinForm</param>
		/// <param name="name">Graph Name</param>
		/// <param name="location">Graph Layout Start Point</param>
		/// <param name="viewSize">Graph Size</param>
		public PlotUtil(Form1 setForm, string name, Point location, Size viewSize)
		{
			_pv = new PlotView();
			_lineSeries = new LineSeries();

			PlotModel pm = new PlotModel();

			LinearAxis lax1 = new LinearAxis { Position = AxisPosition.Bottom };
			LinearAxis lax2 = new LinearAxis
			{
				Maximum = 1.0,
				Minimum = -1.0,
				Position = AxisPosition.Left
			};

			pm.Axes.Add(lax1);
			pm.Axes.Add(lax2);
			pm.Series.Add(_lineSeries);

			_pv.Dock = DockStyle.Bottom;
			_pv.Location = location;
			_pv.Name = name;
			_pv.PanCursor = Cursors.Hand;
			_pv.Size = new Size(400, 150);
			_pv.TabIndex = 0;
			_pv.Text = name;
			_pv.ZoomHorizontalCursor = Cursors.SizeWE;
			_pv.ZoomRectangleCursor = Cursors.SizeNWSE;
			_pv.ZoomVerticalCursor = Cursors.SizeNS;
			_pv.Model = pm;

			setForm.Controls.Add(_pv);
		}

		public void UpdatePlot(List<float> dataList)
		{
			var points = dataList.Select((v, index) =>
						new DataPoint((double)index, v)
					).ToList();
			_lineSeries.Points.Clear();
			_lineSeries.Points.AddRange(points);

			_pv.InvalidatePlot(true);
			dataList.Clear();
		}

	}
}
