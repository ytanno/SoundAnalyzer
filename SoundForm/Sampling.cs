using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

//Nuget Math.NET Numerics
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;

namespace SoundForm
{
	public static class Sampling
	{
		public static List<float> FFTFilter(List<float> data, int windowsize)
		{
			List<float> dst = new List<float>();

			Complex[] temp;

			//set hamming window
			var window = Window.Hamming(windowsize);
			var l = data.Select((v, i) => v * (float)window[i]).ToList();

			//codinate data
			temp = l.Select(x => new Complex(x, 0.0)).ToArray();

			//FFT 
			Fourier.Forward(temp, FourierOptions.Matlab);

			//Filter


			//FFTInv
			Fourier.Inverse(temp, FourierOptions.Matlab);

			//codinate Inv
			dst = temp.Select((v, i) => v / (float)(window[i])).Select(x => (float)x.Real).ToList();

			return dst;
		}

		public static Complex[] HammingFFT(List<float> data, int windowsize)
		{
			//set hamming window
			var window = Window.Hamming(windowsize);
			var l = data.Select((v, i) => v * (float)window[i]).ToList();

			//codinate data
			Complex[] temp = l.Select(x => new Complex(x, 0.0)).ToArray();

			//FFT 
			Fourier.Forward(temp, FourierOptions.Matlab);
			return temp;
		}




	}
}
