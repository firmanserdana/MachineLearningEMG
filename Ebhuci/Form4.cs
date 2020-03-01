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
using System.IO.Ports;
using System.Globalization;
using MathNet.Filtering;
namespace Ebhuci
{
    public partial class Form4 : Form
    {
        SerialPort uno = new SerialPort("COM6", 9600);
        //double waktustart = 100;
        double[] dataMagnitude, dataFilter,waktu;
        

        public Form4()
        {
            InitializeComponent();
            
        }


        private void Form4_Load(object sender, EventArgs e)
        {
            try
            {
                var ports = SerialPort.GetPortNames();
                comboBox1.DataSource = ports;  
                //waktustart = Environment.TickCount;

                GraphPane grafikMagnitude = zedGraphControl1.GraphPane;
                grafikMagnitude.Title.Text = "Monitor EMG";
                grafikMagnitude.XAxis.Title.Text = "Data ke-";
                grafikMagnitude.YAxis.Title.Text = "Magnitude";
                grafikMagnitude.YAxis.Scale.Min = -600;
                grafikMagnitude.YAxis.Scale.Max = 600;
                grafikMagnitude.XAxis.Scale.Min = 0;
                grafikMagnitude.XAxis.Scale.Max = 1000;
                //grafikMagnitude.YAxis.Scale.MagAuto = true;

                PointPairList listMagnitude = new PointPairList();

                LineItem kurvaMagintude = grafikMagnitude.AddCurve("Magnitude", listMagnitude, Color.Red, SymbolType.None);

            }
            catch (Exception Gagal) { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                button2.Enabled = true;
                uno.ReadTimeout = 1000;
                uno.WriteTimeout = 1000;
            
                timer1.Enabled = true;
                timer1.Interval = 1;
                timer1.Start();
                uno.Open();
            }
            catch (Exception Gagal)
            { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            LineItem kurvaMagnitude = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            IPointListEdit listMagnitude = kurvaMagnitude.Points as IPointListEdit;
            
            dataMagnitude = new double[1000];

            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    dataMagnitude[i] = double.Parse(uno.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);

                }
            }
            catch (Exception)
            {
                
                //uno.Close();
                return;
            }
            

            OnlineFilter bandpass = OnlineFilter.CreateBandpass(MathNet.Filtering.ImpulseResponse.Finite, 1000, 20, 500);
            OnlineFilter bandstop = OnlineFilter.CreateBandstop(MathNet.Filtering.ImpulseResponse.Finite, 1000, 48.5, 51.5);
            OnlineFilter denoise = OnlineFilter.CreateDenoise();
            dataFilter = new double[1000];
            dataFilter = bandpass.ProcessSamples(dataMagnitude);
            dataFilter = bandstop.ProcessSamples(dataFilter);
            dataFilter = denoise.ProcessSamples(dataFilter);
            var N = Enumerable.Range(1, dataMagnitude.Length).ToArray();
            waktu = Array.ConvertAll<int, double>(N, Convert.ToDouble);
                
                
               
               
               // double waktu = (Environment.TickCount - waktustart) / 1000.0;
                listMagnitude.Clear();
               
            for (int i = 0; i < 1000; i++)
            {
                listMagnitude.Add(waktu[i], dataFilter[i]);                
            }
            
                label1.Text = Convert.ToString(dataFilter[500]);
                //uno.Close();
                
               

              zedGraphControl1.AxisChange();
              zedGraphControl1.Refresh();
                

            
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SerialPort uno = new SerialPort(Convert.ToString(comboBox1.SelectedItem), 9600);
            button1.Enabled = true;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = true;
                button2.Enabled = false;
                timer1.Stop();
                timer1.Enabled = false;
                if (uno.IsOpen)
                { uno.Close(); }
            }
            catch (Exception Gagal)
            { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1 dspform = new Form1();
            dspform.Show();
            this.Hide();
        }

        

        

    }
}
