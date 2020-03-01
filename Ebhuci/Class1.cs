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
        public double S, Sig,Sigma, Sigma1, Sigma2, MAV, RMS, VAR, SD, WL, ZC, SSC, AvSig;

        public void SigmaData()
        {
            AvSig = 0;
            S = 0;
            for (int i = 0; i < dataMagnitude.Length; i++)
            {
                S = S + dataMagnitude[i];
            }
            AvSig = S / Convert.ToDouble(dataMagnitude.Length);
            Sig = 0;
            for (int i = 0; i < dataMagnitude.Length; i++)
            {
                Sig = Sig + Math.Abs(dataMagnitude[i]);
            }
            
            Sigma = 0;
            for (int i = 0; i < dataMagnitude.Length; i++)
			{
			  Sigma = Sigma + Math.Pow(dataMagnitude[i],2);
			}

           
            Sigma1 = 0;
            for (int i = 0; i < dataMagnitude.Length; i++)
            {
                Sigma1 = Sigma1 + Math.Pow((dataMagnitude[i]-AvSig), 2);
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
            VAR = (1 / Convert.ToDouble(dataMagnitude.Length)) * Sigma1;
        }

        public void StandarDeviate()
        {
            SigmaData();
            SD = Math.Sqrt(1 / Convert.ToDouble(dataMagnitude.Length) * Sigma1);
        }

        public void WaveLength()
        {
            SigmaData();
            WL = Sigma2;
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

        public void Normalization()
        {
            Feature();
            MAV = ((MAV - 14.58823529) * 2 / (51.74509804 - 14.58823529)) - 1;


            RMS = (((RMS * Sigma) - 19.3846513) * 2 / (89.68506991 - 19.3846513)) - 1;

            VAR = ( (VAR - 330.7958478) * 2 / (8036.300654 - 330.7958478)) - 1;

            SD = ((SD - 18.18779392) * 2 / (89.64541624 - 18.18779392)) - 1;


            WL = ((WL - 748) * 2 / (3232 - 748)) - 1;

            ZC = (ZC - 9) * 2 / (27 - 9) - 1;
            SSC = (SSC - 9) * 2 / (27 - 9) - 1;
        }
        
    }

}
