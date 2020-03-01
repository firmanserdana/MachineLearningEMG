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
using System.IO;
using System.Globalization;
using MathNet.Filtering.FIR;
using MathNet.Filtering;
using MathNet.Numerics.IntegralTransforms;

namespace Ebhuci
{




    public partial class Form2 : Form
    {
        string[] data, dataSave;
        double[] dataN, dataMagnitude, dataFilter;
        int[] N;

      
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {

                GraphPane grafikMagnitude = zedGraphControl1.GraphPane;
                grafikMagnitude.Title.Text = "Filtering EMG";
                grafikMagnitude.XAxis.Title.Text = "Data ke-n";
                grafikMagnitude.YAxis.Title.Text = "Magnitude";
                PointPairList listMagnitude = new PointPairList();
                PointPairList listFilter = new PointPairList();

                LineItem kurvaMagintude = grafikMagnitude.AddCurve("Magnitude", listMagnitude, Color.Red, SymbolType.None);
                LineItem kurvaMagnitude = grafikMagnitude.AddCurve("Filtered", listFilter, Color.Blue, SymbolType.None);
                


            }
            catch (Exception) { }
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
                button1.Enabled = false;
                button2.Enabled = true;
                button4.Enabled = false;
            }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {

         
            dataFilter = new double[dataMagnitude.Length];
            OnlineFilter bandpass = OnlineFilter.CreateBandpass(ImpulseResponse.Finite, 1000, 20, 500);
            OnlineFilter bandstop = OnlineFilter.CreateBandstop(MathNet.Filtering.ImpulseResponse.Finite, 1000, 49, 51);
           OnlineFilter denoise = OnlineFilter.CreateDenoise();
            dataFilter = bandpass.ProcessSamples(dataMagnitude);
            dataFilter = bandstop.ProcessSamples(dataFilter);
            dataFilter = denoise.ProcessSamples(dataFilter);
           
            LineItem kurvaMagnitude = zedGraphControl1.GraphPane.CurveList[1] as LineItem;
            IPointListEdit listFilter = kurvaMagnitude.Points as IPointListEdit;
            listFilter.Clear();
            for (int i = 1; i < dataFilter.Length; i++)
            {
                listFilter.Add(dataN[i], dataFilter[i]);
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            button2.Enabled = false;
            button3.Enabled = true;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save To...";
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                dataSave = new string[dataFilter.Length];
                dataSave = Array.ConvertAll<double, string>(dataFilter, Convert.ToString);
                File.WriteAllLines(saveFileDialog1.FileName, dataSave, Encoding.Default);
                MessageBox.Show("Data is Saved");
            }
            button3.Enabled = false;
            button1.Enabled = true;
            button4.Enabled = true;
            button6.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Open Files...";
            openFileDialog1.FileName = "";

            openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                folderBrowserDialog1.Description = "Choose Folder to Save...";
                if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Choice Cancelled");
                }
                else
                {
             
                var saveto = folderBrowserDialog1.SelectedPath;
                 
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    data = File.ReadAllLines(openFileDialog1.FileNames[i]);
                    dataMagnitude = data.Select(x => double.Parse(x)).ToArray();

                    dataFilter = new double[dataMagnitude.Length];
                    OnlineFilter bandstop = OnlineFilter.CreateBandstop(MathNet.Filtering.ImpulseResponse.Finite, 80, 48.5, 51.5);
                   // OnlineFilter denoise = OnlineFilter.CreateDenoise();
                    dataFilter = bandstop.ProcessSamples(dataMagnitude);
                    //dataFilter = denoise.ProcessSamples(dataFilter);
            
                    var path = Path.Combine(saveto, "Datafilter" + Convert.ToString(i) + ".csv");
                    File.WriteAllLines(path, Array.ConvertAll<double, string>(dataFilter, Convert.ToString), Encoding.Default);

                }
                MessageBox.Show("Done");
                }

                
            }

            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1 dspform = new Form1();
            dspform.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();
            zedGraphControl1.Refresh();
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            GraphPane grafikMagnitude = zedGraphControl1.GraphPane;
            grafikMagnitude.Title.Text = "Filtering EMG";
            grafikMagnitude.XAxis.Title.Text = "Data ke-n";
            grafikMagnitude.YAxis.Title.Text = "Magnitude";
            PointPairList listMagnitude = new PointPairList();
            PointPairList listFilter = new PointPairList();

            LineItem kurvaMagintude = grafikMagnitude.AddCurve("Magnitude", listMagnitude, Color.Red, SymbolType.None);
            LineItem kurvaMagnitude = grafikMagnitude.AddCurve("Filtered", listFilter, Color.Blue, SymbolType.None);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Open Files...";
            openFileDialog1.FileName = "";

            openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                folderBrowserDialog1.Description = "Choose Folder to Save...";
                if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Choice Cancelled");
                }
                else
                {

                    var saveto = folderBrowserDialog1.SelectedPath;

                    for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                    {
                        data = File.ReadAllLines(openFileDialog1.FileNames[i]);
                        dataMagnitude = data.Select(x => double.Parse(x)).ToArray();

                        dataFilter = new double[dataMagnitude.Length];
                        for (int j = 0; j < dataMagnitude.Length; j++)
                        {
                            dataFilter[j] = dataMagnitude[j]-511;
                        }

                        var path = Path.Combine(saveto, "Datafilter" + Convert.ToString(i) + ".csv");
                        File.WriteAllLines(path, Array.ConvertAll<double, string>(dataFilter, Convert.ToString), Encoding.Default);

                    }
                    MessageBox.Show("Done");
                }


            }


        }

        
       
    }
    
}