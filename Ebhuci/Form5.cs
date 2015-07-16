using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ebhuci
{
    public partial class Form5 : Form
    {
        static protected int cx, cy, dx, dy;
        static protected long total = 0;
            
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            Timer t = new Timer();
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
            
            cx = Cursor.Position.X;
            cy = Cursor.Position.Y;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            total = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 dspform = new Form1();
            dspform.Show();
            this.Hide();
        }
    }
}
