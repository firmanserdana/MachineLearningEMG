using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog;

namespace Ebhuci
{
    class FeatureExtract
    {
        public double[] dataMagnitude;
        public double S, Sig,Sigma, Sigma1, Sigma2, MAV, RMS, VAR, SD, WL, ZC, SSC;

        public void SigmaData()
        {
            Sig = 0;
            S = 0;
            for (int i = 0; i < dataMagnitude.Length; i++)
            {
                Sig = Sig + Math.Abs(dataMagnitude[i]);
                S = S + dataMagnitude[i];
            }
            Sigma = 0;
            for (int i = 0; i < dataMagnitude.Length; i++)
			{
			  Sigma = Sigma + Math.Pow(dataMagnitude[i],2);
			}

            Sigma1 = 0;
            for (int i = 0; i < dataMagnitude.Length; i++)
            {
                Sigma1 = Sigma1 + Math.Pow((dataMagnitude[i]-S/Convert.ToDouble(dataMagnitude.Length)), 2);
            }

            Sigma2 = 0;
            for (int i = 0; i < dataMagnitude.Length - 1; i++)
            {
                Sigma2 = Sigma2 + Math.Abs(dataMagnitude[i+1] - dataMagnitude[i]);
            }
        }

        public void MeanAverageValue()
        {
            SigmaData();
            MAV = (1 /Convert.ToDouble(dataMagnitude.Length)) * Sig;
        }

        public void RootMeanSquare()
        {
            SigmaData();
            RMS = (Math.Sqrt(1 / Convert.ToDouble(dataMagnitude.Length) * Sigma));
        }

        public void Variance()
        {
            SigmaData();
            VAR = (1 / Convert.ToDouble(dataMagnitude.Length - 1)) * Sigma1;
        }

        public void StandarDeviate()
        {
            SigmaData();
            SD = Math.Sqrt(1 / Convert.ToDouble(dataMagnitude.Length - 1) * Sigma1);
        }

        public void WaveLength()
        {
            SigmaData();
            WL = 1/Convert.ToDouble(dataMagnitude.Length)* Sigma2;
        }

        public void ZeroCrossing()
        {
            ZC = 0;
            for (int i = 0; i < dataMagnitude.Length-1; i++)
            {
                if (dataMagnitude[i] == 0 && dataMagnitude[i] != dataMagnitude[i+1])
                {
                    ZC++;
                }
                else if ((dataMagnitude[i] < 0 && dataMagnitude[i+1] >0) || (dataMagnitude[i] > 0 && dataMagnitude[i+1] < 0))
                {
                    ZC++;
                }
            }
        }

        public void SlopeSignChange()
        {
            SSC = 0;
            
            
            for (int i = 0; i < dataMagnitude.Length-2; i++)

            {
                
                if (dataMagnitude[i] > dataMagnitude[i + 1] && dataMagnitude[i+1] < dataMagnitude[i + 2])
                {
                    SSC++;
                }
                else
                    if (dataMagnitude[i] < dataMagnitude[i + 1] && dataMagnitude[i+1] > dataMagnitude[i + 2])
                    {
                        SSC++;
                    }
            }
        }

        public void Feature()
        {
            MeanAverageValue();
            RootMeanSquare();
            StandarDeviate();
            WaveLength();
            Variance();
            ZeroCrossing();
            SlopeSignChange();
        }
        
        
    }

}
