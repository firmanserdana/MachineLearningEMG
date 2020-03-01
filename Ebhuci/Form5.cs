using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Encog.Util.Arrayutil;
using Encog.Persist;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Networks.Training.Lma;
using Encog.ML.Train;
using Encog.ML.Data.Basic;
using System.IO;
using System.IO.Ports;
using System.Globalization;
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
using MathNet.Filtering;


namespace Ebhuci
{
    public partial class Form5 : Form
    {
        static protected int cx, cy, dx, dy;
        static protected long total = 0;
        double[] dataMagnitude = new double[1000];
        double[] dataFilter = new double[1000];
        double[] feature = new double[7];
        double[] output = new double[4];
        Stopwatch m = new Stopwatch();
        BasicNetwork JST = new BasicNetwork();
        SerialPort uno = new SerialPort("COM6", 9600);
        Timer t = new Timer();
    
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            Cursor.Position = new Point(300, 300);
            t.Interval = 10;
            t.Tick += new EventHandler(timer_tick);

            t.Enabled = true;

            
        }

        private void timer_tick(object sender, EventArgs e)
        {


            label1.Text = "X = " + Cursor.Position.X.ToString();
            label2.Text = "Y = " + Cursor.Position.Y.ToString();
            
            if (dx != Cursor.Position.X | dy != Cursor.Position.Y)
            {
                dx = Cursor.Position.X - cx;
                dy = Cursor.Position.Y - cy;

                if (dx < 0)
                {
                    dx *= -1;
                }
                if (dy < 0)
                {
                    dy *= -1;
                }

                total += dx + dy;
            }
            
            label3.Text = "Pixels traveled = " + total;
          

            if (total - 100 > 0 )
            {
                m.Stop();
                label5.Text = "Current Position = " + Cursor.Position.X.ToString() + "," + Cursor.Position.Y.ToString();
                label6.Text = "Time Elapsed = " + m.ElapsedMilliseconds * 0.001+ " second(s)";
                
            }
            cx = Cursor.Position.X;
            cy = Cursor.Position.Y;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            total = 0;
            m.Reset();
            label6.Text = "Time Elapsed = ";
            m.Start();;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 dspform = new Form1();
            dspform.Show();
            this.Hide();
        }

        
    }
}
