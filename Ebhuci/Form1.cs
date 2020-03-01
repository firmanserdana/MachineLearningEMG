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

namespace Ebhuci
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            e.Cancel = false;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process open = new Process();
            folderBrowserDialog1.Description = "Select Arduino Directory...";
            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                
                string winpath = folderBrowserDialog1.SelectedPath;
                open.StartInfo.FileName = winpath + "\\arduino.exe";
                open.Start();
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            Form2 dspform = new Form2();
            dspform.Show();
            this.Hide();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 dspform = new Form4();
            dspform.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 dspform = new Form3();
            dspform.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form5 dspform = new Form5();
            dspform.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form6 dspform = new Form6();
            dspform.Show();
            this.Hide();
        }

      
    }
}
