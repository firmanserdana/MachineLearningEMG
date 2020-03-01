using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using ZedGraph;
using System.IO;
using System.Globalization;
using Encog.Util.Arrayutil;
using Encog.Persist;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Networks.Training.Lma;
using Encog.ML.Train;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Versatile.Sources;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Columns;
using Encog.Util.CSV;
using Encog.ML.Model;
using Encog.ML.Factory;
using Encog.ML.Data.Specific;
using Encog.ML;
using Encog.Neural.Data;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst;
using Encog.App.Analyst.Wizard;
using Encog.Util.Simple;
using Encog.Util.Error;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.MathUtil.Randomize.Generate;
using Encog.ML.Data.Versatile.Normalizers.Strategy;

namespace Ebhuci
{
    public partial class Form6 : Form
    {
        string[] data, dataSave;
        double[] dataN, dataMagnitude, feature, output;
        int[] N;
        double MAV, RMS, VAR, SD, WL, ZC, SSC;

        public Form6()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            GraphPane grafikMagnitude = zedGraphControl1.GraphPane;
            grafikMagnitude.Title.Text = "EMG Signal";
            grafikMagnitude.XAxis.Title.Text = "Data ke-n";
            grafikMagnitude.YAxis.Title.Text = "Magnitude";
            PointPairList listMagnitude = new PointPairList();
            
            LineItem kurvaMagintude = grafikMagnitude.AddCurve("Magnitude", listMagnitude, Color.Red, SymbolType.None);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {

                data = File.ReadAllLines(openFileDialog1.FileName);
                dataMagnitude = data.Select(x => double.Parse(x)).ToArray();

                N = Enumerable.Range(1, dataMagnitude.Length).ToArray();
                dataN = Array.ConvertAll<int, double>(N, Convert.ToDouble);

                LineItem kurvaMagnitude = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
                IPointListEdit listMagnitude = kurvaMagnitude.Points as IPointListEdit;
                listMagnitude.Clear();
                for (int i = 1; i < dataMagnitude.Length; i++)
                {
                    listMagnitude.Add(dataN[i], dataMagnitude[i]);
                }
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                
            }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
                FeatureExtract Inputs = new FeatureExtract();
                Inputs.dataMagnitude = dataMagnitude;
                Inputs.Feature();
                feature = new double[7];
                feature[0] = (Inputs.MAV - 18.231) * 2 / (131.22 - 18.231) - 1;
                feature[1] = (Inputs.RMS - 24.78176668) * 2 / (180.9976057 - 24.78176668) - 1;
                feature[2] = (Inputs.VAR - 575.07346) * 2 / (32723.06952 - 575.07346) - 1;
                feature[3] = (Inputs.SD - 23.98068931) * 2 / (180.8951893 - 23.98068931) - 1;
                feature[4] = (Inputs.WL - 411.76) * 2 / (1664.26 - 411.76) - 1;
                feature[5] = (Inputs.ZC - 1) * 2 / (12 - 1) - 1;
                feature[6] = (Inputs.SSC - 3) * 2 / (15 - 3) - 1;

                IMLData q = new BasicMLData(feature);


                BasicNetwork JST = new BasicNetwork();
            openFileDialog1.Title = "Open Network File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "EG (Encog Network)|*.eg|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                JST = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(openFileDialog1.FileName));
                
                    IMLData output = JST.Compute(q);
                    

                    if (output[0] > output[1] && output[0] > output[2] && output[0] > output[3])
                    {
                        label2.Text = "Atas";
                    }
                    else if (output[1] > output[0] && output[1] > output[2] && output[1] > output[3])
                    {
                        label2.Text = "Bawah";
                    }
                    else if (output[2] > output[1] && output[2] > output[0] && output[2] > output[3])
                    {
                        label2.Text = "Kanan" ;
                    }
                    else if (output[3] > output[1] && output[3] > output[2] && output[3] > output[0])
                    {
                        label2.Text = "Kiri";
                    }
                    else
                    {
                        label2.Text = "Tidak Terdefinisikan" ;
                    }
                    label3.Text = "output" + "= "+output[0] + " " + output[1] + " " + output[2] + " " + output[3];
            
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 dspform = new Form1();
            dspform.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            feature = new double[20];
            feature[0] = (dataMagnitude[0] + 70.51) * 2 / (65.24 + 70.51) - 1;
            feature[1] = (dataMagnitude[1] + 233.52) * 2 / (403.31 + 233.52) - 1;
            feature[2] = (dataMagnitude[2] + 259.22) * 2 / (402.28 + 259.22) - 1;
            feature[3] = (dataMagnitude[3] + 197.89) * 2 / (350.11 + 197.89) - 1;
            feature[4] = (dataMagnitude[4] + 197.55) * 2 / (339.16 + 197.55) - 1;
            feature[5] = (dataMagnitude[5] + 217.63) * 2 / (272.11 + 217.63) - 1;
            feature[6] = (dataMagnitude[6] + 258.16) * 2 / (263.24 + 258.16) - 1;
            feature[7] = (dataMagnitude[7] + 275.57) * 2 / (153.28 + 275.57) - 1;
            feature[8] = (dataMagnitude[8] + 247.04) * 2 / (261.35 + 247.04) - 1;
            feature[9] = (dataMagnitude[9] + 199.46) * 2 / (201.91 + 199.46) - 1;
            feature[10] = (dataMagnitude[10] + 196.31) * 2 / (110.42 + 196.31) - 1;
            feature[11] = (dataMagnitude[11] + 147.19) * 2 / (139.61 + 147.19) - 1;
            feature[12] = (dataMagnitude[12] + 180.49) * 2 / (157.4 + 180.49) - 1;
            feature[13] = (dataMagnitude[13] + 145.68) * 2 / (173.74 + 145.68) - 1;
            feature[14] = (dataMagnitude[14] + 162.67) * 2 / (90.23 + 162.67) - 1;
            feature[15] = (dataMagnitude[15] + 201) * 2 / (58.47 + 201) - 1;
            feature[16] = (dataMagnitude[16] + 115.06) * 2 / (67.74 + 115.06) - 1;
            feature[17] = (dataMagnitude[17] + 133.04) * 2 / (60.24 + 133.04) - 1;
            feature[18] = (dataMagnitude[18] + 121.36) * 2 / (111.62 + 121.36) - 1;
            feature[19] = (dataMagnitude[19] + 99.44) * 2 / (57.91 + 99.44) - 1;

            IMLData q = new BasicMLData(feature);

            EncogAnalyst analyst = new EncogAnalyst();
            Encog.App.Analyst.Script.Normalize.AnalystField field = new Encog.App.Analyst.Script.Normalize.AnalystField();
            
            BasicNetwork JST = new BasicNetwork();
            openFileDialog1.Title = "Open Network File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "EG (Encog Network)|*.eg|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                JST = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(openFileDialog1.FileName));

                IMLData output = JST.Compute(q);


                if (output[0] > output[1] && output[0] > output[2] && output[0] > output[3])
                {
                    label2.Text = "Atas";
                }
                else if (output[1] > output[0] && output[1] > output[2] && output[1] > output[3])
                {
                    label2.Text = "Bawah";
                }
                else if (output[2] > output[1] && output[2] > output[0] && output[2] > output[3])
                {
                    label2.Text = "Kanan";
                }
                else if (output[3] > output[1] && output[3] > output[2] && output[3] > output[0])
                {
                    label2.Text = "Kiri";
                }
                else
                {
                    label2.Text = "Tidak Terdefinisikan";
                }
                label3.Text = "output" + "= " + output[0] + " " + output[1] + " " + output[2] + " " + output[3];

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            FeatureExtract Inputs = new FeatureExtract();
            Inputs.dataMagnitude = dataMagnitude;
            Inputs.Feature();
            feature = new double[7];
            feature[0] = (Inputs.MAV - 18.231) * 2 / (131.22 - 18.231) - 1;
            feature[1] = (Inputs.RMS - 24.78176668) * 2 / (180.9976057 - 24.78176668) - 1;
            feature[2] = (Inputs.VAR - 575.07346) * 2 / (32723.06952 - 575.07346) - 1;
            feature[3] = (Inputs.SD - 23.98068931) * 2 / (180.8951893 - 23.98068931) - 1;
            feature[4] = (Inputs.WL - 411.76) * 2 / (1664.26 - 411.76) - 1;
            feature[5] = (Inputs.ZC - 1) * 2 / (12 - 1) - 1;
            feature[6] = (Inputs.SSC - 3) * 2 / (15 - 3) - 1;

            IMLData q = new BasicMLData(feature);


            BasicNetwork JST = new BasicNetwork();
            openFileDialog1.Title = "Open Network File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "EG (Encog Network)|*.eg|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                JST = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(openFileDialog1.FileName));

                IMLData output = JST.Compute(q);
                var outoff = new double[4];
                outoff[0] = Math.Round(output[0]);
                outoff[1] = Math.Round(output[1]);
                outoff[2] = Math.Round(output[2]);
                outoff[3] = Math.Round(output[3]);

                if (outoff[0] > outoff[1] && outoff[0] > outoff[2] && outoff[0] > outoff[3])
                {
                    label2.Text = "Atas";
                }
                else if (outoff[1] > outoff[0] && outoff[1] > outoff[2] && outoff[1] > outoff[3])
                {
                    label2.Text = "Bawah";
                }
                else if (outoff[2] > outoff[0] && outoff[2] > outoff[1] && outoff[2] > outoff[3])
                {
                    label2.Text = "Kanan";
                }
                else if (outoff[3] > outoff[0] && outoff[3] > outoff[1] && outoff[3] > outoff[2])
                {
                    label2.Text = "Kiri";
                }
                else
                {
                    label2.Text = "Tidak Terdefinisikan";
                }
                label3.Text = "output" + "= " + outoff[0] + " " + outoff[1] + " " + outoff[2] + " " + outoff[3];

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            feature = new double[20];
            feature[0] = (dataMagnitude[0] + 70.51) * 2 / (65.24 + 70.51) - 1;
            feature[1] = (dataMagnitude[1] + 233.52) * 2 / (403.31 + 233.52) - 1;
            feature[2] = (dataMagnitude[2] + 259.22) * 2 / (402.28 + 259.22) - 1;
            feature[3] = (dataMagnitude[3] + 197.89) * 2 / (350.11 + 197.89) - 1;
            feature[4] = (dataMagnitude[4] + 197.55) * 2 / (339.16 + 197.55) - 1;
            feature[5] = (dataMagnitude[5] + 217.63) * 2 / (272.11 + 217.63) - 1;
            feature[6] = (dataMagnitude[6] + 258.16) * 2 / (263.24 + 258.16) - 1;
            feature[7] = (dataMagnitude[7] + 275.57) * 2 / (153.28 + 275.57) - 1;
            feature[8] = (dataMagnitude[8] + 247.04) * 2 / (261.35 + 247.04) - 1;
            feature[9] = (dataMagnitude[9] + 199.46) * 2 / (201.91 + 199.46) - 1;
            feature[10] = (dataMagnitude[10] + 196.31) * 2 / (110.42 + 196.31) - 1;
            feature[11] = (dataMagnitude[11] + 147.19) * 2 / (139.61 + 147.19) - 1;
            feature[12] = (dataMagnitude[12] + 180.49) * 2 / (157.4 + 180.49) - 1;
            feature[13] = (dataMagnitude[13] + 145.68) * 2 / (173.74 + 145.68) - 1;
            feature[14] = (dataMagnitude[14] + 162.67) * 2 / (90.23 + 162.67) - 1;
            feature[15] = (dataMagnitude[15] + 201) * 2 / (58.47 + 201) - 1;
            feature[16] = (dataMagnitude[16] + 115.06) * 2 / (67.74 + 115.06) - 1;
            feature[17] = (dataMagnitude[17] + 133.04) * 2 / (60.24 + 133.04) - 1;
            feature[18] = (dataMagnitude[18] + 121.36) * 2 / (111.62 + 121.36) - 1;
            feature[19] = (dataMagnitude[19] + 99.44) * 2 / (57.91 + 99.44) - 1;

            IMLData q = new BasicMLData(feature);

            EncogAnalyst analyst = new EncogAnalyst();
            Encog.App.Analyst.Script.Normalize.AnalystField field = new Encog.App.Analyst.Script.Normalize.AnalystField();

            BasicNetwork JST = new BasicNetwork();
            openFileDialog1.Title = "Open Network File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "EG (Encog Network)|*.eg|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                JST = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(openFileDialog1.FileName));

                IMLData output = JST.Compute(q);
                var outoff = new double[4];
                outoff[0] = Math.Round(output[0]);
                outoff[1] = Math.Round(output[1]);
                outoff[2] = Math.Round(output[2]);
                outoff[3] = Math.Round(output[3]);

                if (outoff[0] > outoff[1] && outoff[0] > outoff[2] && outoff[0] > outoff[3])
                {
                    label2.Text = "Atas";
                }
                else if (outoff[1] > outoff[0] && outoff[1] > outoff[2] && outoff[1] > outoff[3])
                {
                    label2.Text = "Bawah";
                }
                else if (outoff[2] > outoff[0] && outoff[2] > outoff[1] && outoff[2] > outoff[3])
                {
                    label2.Text = "Kanan";
                }
                else if (outoff[3] > outoff[0] && outoff[3] > outoff[1] && outoff[3] > outoff[2])
                {
                    label2.Text = "Kiri";
                }
                else
                {
                    label2.Text = "Tidak Terdefinisikan";
                }
                label3.Text = "output" + "= " + outoff[0] + " " + outoff[1] + " " + outoff[2] + " " + outoff[3];

            }
        }
    }
}
