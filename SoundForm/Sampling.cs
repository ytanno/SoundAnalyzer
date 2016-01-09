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
        public static List<float> FFTFilter(List<float> data, int windowsize, int samplingRate,
                                            int lowPass, int highPass)
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
            var s = windowsize * (1.0 / samplingRate);
            var half = temp.Count() / 2;

            //lowPass Filter
            for (int i = 0; i < half; i++)
            {
                var v = (double)i / (double)s;
                if (v < lowPass)
                {
                    temp[i] = new Complex(0, 0);
                    temp[temp.Count() - i - 1] = new Complex(0, 0);
                }

                if (v > highPass)
                {
                    temp[i] = new Complex(0, 0);
                    temp[half + i] = new Complex(0, 0);
                }
            }

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

        public static Complex[] TestPlot(List<float> data, int windowsize, int samplingRate,
                                            int lowPass, int highPass)
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
            var s = windowsize * (1.0 / samplingRate);
            var half = temp.Count() / 2;

            //lowPass Filter
            for (int i = 0; i < half; i++)
            {
                var v = (double)i / (double)s;
                if (v < lowPass)
                {
                    temp[i] = new Complex(0, 0);
                    temp[temp.Count() - i - 1] = new Complex(0, 0);
                }

                if (v > highPass)
                {
                      //temp[i] = new Complex(0, 0);
                      //temp[half + i] = new Complex(0, 0);
                }
            }

            return temp;
            //FFTInv
            //Fourier.Inverse(temp, FourierOptions.Matlab);

            //codinate Inv
            //dst = temp.Select((v, i) => v / (float)(window[i])).Select(x => (float)x.Real).ToList();
            //return dst;
        }




	}
}
