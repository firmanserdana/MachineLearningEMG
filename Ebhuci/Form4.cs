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
        double waktustart = 100;

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
                waktustart = Environment.TickCount;

                GraphPane grafikMagnitude = zedGraphControl1.GraphPane;
                grafikMagnitude.Title.Text = "Monitor EMG";
                grafikMagnitude.XAxis.Title.Text = "Time (Detik)";
                grafikMagnitude.YAxis.Title.Text = "Magnitude";

                RollingPointPairList listMagnitude = new RollingPointPairList(120);

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
                timer1.Start();
            }
            catch (Exception Gagal)
            { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                uno.Open();
               
                LineItem kurvaMagnitude = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
                IPointListEdit listMagnitude = kurvaMagnitude.Points as IPointListEdit;
                double waktu = (Environment.TickCount - waktustart) / 1000.0;
                double dataMagnitude = double.Parse(uno.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
                
             OnlineFilter bandpass = OnlineFilter.CreateBandpass(MathNet.Filtering.ImpulseResponse.Finite, 1000, 20, 500);
            OnlineFilter bandstop = OnlineFilter.CreateBandstop(MathNet.Filtering.ImpulseResponse.Finite, 1000, 48.5, 51.5);
            OnlineFilter denoise = OnlineFilter.CreateDenoise();
            double dataFilter = bandpass.ProcessSample(dataMagnitude);
            dataFilter = bandstop.ProcessSample(dataFilter);
            dataFilter = denoise.ProcessSample(dataFilter);
                listMagnitude.Add(waktu, dataFilter);
                label1.Text = Convert.ToString(dataFilter);
                uno.Close();
                
                Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;
                if (waktu > xScale.Max - xScale.MajorStep)
                {
                    xScale.Max = waktu + xScale.MajorStep;
                    xScale.Min = xScale.Max - 30.0;
                }

              // zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                

            }
            catch (Exception Gagal)
            {
                if (uno.IsOpen)
                { uno.Close(); }
            }
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
