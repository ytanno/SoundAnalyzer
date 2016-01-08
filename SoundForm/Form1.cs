//date 2016/01/04
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Numerics;

// Nuget NAudio
using NAudio.Wave;

//ref http://wildpie.hatenablog.com/entry/2014/09/24/000900
//ref http://stackoverflow.com/questions/3929389/naudio-convert-input-byte-array-to-an-array-of-doubles
//ref http://pastie.org/10256216#14

namespace SoundForm
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private WaveIn _wi = null;
		private WaveFileWriter _writer = null;
	
		//raw sound plot
		PlotUtil _p1 = null;

		//processing sound plot
		PlotUtil _p2 = null;

		//data
		private List<float> _recorded1 = new List<float>();

		//if you used FFT, 2^N. ex 1024, 2048, 4096
		private int _takeDataSize = 1024;

		//sound sampleRate
		private int _sampleRate = 8000;

		//test mode of FFT
		private bool _testMode = true;
	

		private void Form1_Load(object sender, EventArgs e)
		{
			InitSound();
			InitPlot();

			if (_testMode) TestPlot();
		}

		private void TestPlot()
		{
			
			List<float> _6Hz = new List<float>();
			List<float> _2Hz = new List<float>();
			RecordButton.Enabled = false;
			var angle = 360.0;
			var testRate = 1024; //sound rate is 8000 but take data is 1024
			var c = angle / testRate;
			var hz6 = c * 6;
			var hz2 = c * 2;
			var hz400 = c * 400;
			for (int i = 0; i < testRate; i++)
			{
				var hz6v = (float)Math.Sin(hz6 * i * (Math.PI / 180));
				var hz2v = (float)Math.Sin(hz2 * i * (Math.PI / 180));
				var hz400v = (float)Math.Sin(hz400 * i * (Math.PI / 180));
				_2Hz.Add(hz2v);
				_6Hz.Add(hz6v);

				//sum wave
				_recorded1.Add(hz6v + hz2v);
				//_recorded1.Add(hz400v);
			}

			_p1.UpdatePlot(_recorded1);
			//_p2.UpdateFFTPlot(_recorded1,testRate); // FFT Test. plot size xMax = 10.
			_p2.PlotInvFFT(_recorded1, testRate); //if you can see, change plot size xMax = 1024.
		}


		private void InitSound()
		{
			var deviceIndex = 0;
			
			//Device Info
			if (WaveIn.DeviceCount > 0)
			{
				var deviceInfo = WaveIn.GetCapabilities(0);
				this.label1.Text = String.Format("Device {0}: {1}, {2} channels",
					deviceIndex, deviceInfo.ProductName, deviceInfo.Channels);
				_wi = new WaveIn() { DeviceNumber = deviceIndex };

				//mike receive data
				_wi.DataAvailable += (ss, ee) =>
				{
					for (int index = 0; index < ee.BytesRecorded; index += 2)
					{
						short sample = (short)((ee.Buffer[index + 1] << 8) | ee.Buffer[index + 0]);
						float sample32 = 0;
						if (sample > 0) sample32 = sample / 32767f;
						if (sample < 0) sample32 = sample / 32768f;

						_recorded1.Add(sample32);

						if (_recorded1.Count == _takeDataSize)
						{
							_p1.UpdatePlot(_recorded1);
							_p2.UpdateFFTPlot(_recorded1, _sampleRate);
							_recorded1.Clear();
						}

						//Analyze Do here 
						//ee.Buffer[index + 1] = AnalyzedData;
						//ee.Buffer[index + 0] = AnalyzedData;

					}
					_writer.Write(ee.Buffer, 0, ee.BytesRecorded);
					_writer.Flush();
				};

				_wi.WaveFormat = new WaveFormat(sampleRate: _sampleRate, channels: deviceInfo.Channels);

				//save file
				var sPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\file.wav";
				_writer = new WaveFileWriter( sPath, _wi.WaveFormat);
			}
			else RecordButton.Enabled = false;
		}

		private void InitPlot()
		{
			this.SuspendLayout();

			var size = new Size(400, 150);
			var startPoint = new Point(10, 40);
			var spaceHeight = 20;

			_p1 = new PlotUtil(this, "p1", startPoint, size);
			startPoint = new Point(startPoint.X, startPoint.Y + size.Height + spaceHeight);

			if(!_testMode)_p2 = new PlotUtil(this, "p2", startPoint, size, 0, 15);
			if(_testMode) _p2 = new PlotUtil(this, "p2", startPoint, size, 0, 0, 0, 1024);
		}

		private void RecordButton_Click(object sender, EventArgs e)
		{
			if(RecordButton.Text == "Start")
			{
				//init data
				_recorded1 = new List<float>();
				_p1.UpdatePlot(_recorded1);
				_p2.UpdatePlot(_recorded1);

				RecordButton.Text = "Stop";
				_wi.StartRecording();
			}
			else 
			{
				End();
			}
		}

		private void End()
		{
			RecordButton.Enabled = false;
			_wi.StopRecording();

			if (_writer != null)
			{
				_writer.Dispose();  _writer = null;
			}

			if (_wi != null)
			{
				_wi.Dispose(); _wi = null;
			}

			InitSound();
			RecordButton.Text = "Start";
			RecordButton.Enabled = true;
		}
	}
}