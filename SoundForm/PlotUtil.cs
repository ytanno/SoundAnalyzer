using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;

//Install-Package OxyPlot.WindowsForms -Pre
//I do not know how to add from tool box in windowsForm 
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

//Nuget Math.NET Numerics
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;

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
		public PlotUtil(Form1 setForm, string name, Point location, Size viewSize, double yMin, double yMax)
		{
			SetPlot(setForm, name, location, viewSize, yMin, yMax);
		}

		public PlotUtil(Form1 setForm, string name, Point location, Size viewSize)
		{
			SetPlot(setForm, name, location, viewSize, 0, 0);
		}

		private void SetPlot(Form1 setForm, string name, Point location, Size viewSize, double yMin, double yMax)
		{
			_pv = new PlotView();
			_lineSeries = new LineSeries();

			PlotModel pm = new PlotModel();

			LinearAxis lax1 = new LinearAxis { Position = AxisPosition.Bottom };
			LinearAxis lax2 = new LinearAxis { Position = AxisPosition.Left };

			if (yMax != yMin)
			{
				lax2 = new LinearAxis
				{
					//Maximum = 1.0,
					//Minimum = -1.0,
					Maximum = yMax,
					Minimum = yMin,
					Position = AxisPosition.Left
				};
			}

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
		}

		public void UpdateFFTPlot(List<float> dataList)
		{
			var windowsize = dataList.Count();
			var window = Window.Hamming(windowsize);
			var l = dataList.Select((v, i) => v * (float)window[i]).ToList();

			//FFT
			var c = l.Select(x => new Complex(x, 0.0)).ToArray();
			Fourier.Forward(c, FourierOptions.Matlab);

			var s = windowsize * (1.0 / 8000.0);
			var point = c.Take(c.Count() / 2).Select((v, index) =>
					new DataPoint((double)index / s,
			  Math.Sqrt(v.Real * v.Real + v.Imaginary * v.Imaginary))
			).ToList();

			_lineSeries.Points.Clear();
			_lineSeries.Points.AddRange(point);

			var xm = point.Max(d => d.X);
			var ym = point.Max(d => d.Y);

			_pv.InvalidatePlot(true);
		}


	}
}
