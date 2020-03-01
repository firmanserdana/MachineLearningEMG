﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
using ZedGraph;
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
using CsvFile;
using System.Globalization;



namespace Ebhuci
{
    public partial class Form3 : Form
    {

        List<double> data = new List<double>();
        double[] dataMagnitude;
        double[] MAVu, RMSu, VARu, SDu, WLu, ZCu, SSCu, MAVd, RMSd, VARd, SDd, WLd, ZCd, SSCd, MAVr, RMSr, VARr, SDr, WLr, ZCr, SSCr, MAVl, RMSl, VARl, SDl, WLl, ZCl, SSCl;
        string[] lines, w1, w2;
        string[] minmax = new string[20];
        IMLTrain train;

        public Form3()
        {
            InitializeComponent();
        }

        public static double[][] OutputIdeal =
        {
            new[] {1.0, 0.0, 0.0, 0.0},
            new[] {0.0, 1.0, 0.0, 0.0},
            new[] {0.0, 0.0, 1.0, 0.0},
            new[] {0.0, 0.0, 0.0, 1.0}
        };

        private void Form3_Load(object sender, EventArgs e)
        {
            GraphPane grafikError = zedGraphControl1.GraphPane;
            grafikError.Title.Text = "Grafik Error Training JST";
            grafikError.XAxis.Title.Text = "Epoch";
            grafikError.YAxis.Title.Text = "MSE";
            PointPairList listError = new PointPairList();
            PointPairList listValid = new PointPairList();
            zedGraphControl1.IsShowPointValues = true;



            LineItem kurvaError = grafikError.AddCurve("Training", listError, Color.Red, SymbolType.None);
            LineItem kurvaValidasi = grafikError.AddCurve("Validasi", listValid, Color.Blue, SymbolType.None);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open Raw EMG File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                int j = 0;
                List<string> columns = new List<string>();
                FeatureExtract Inputs = new FeatureExtract();
                var line = File.ReadLines(openFileDialog1.FileName).Count();
                MAVu = new double[line];
                RMSu = new double[line];
                VARu = new double[line];
                SDu = new double[line];
                WLu = new double[line];
                ZCu = new double[line];
                SSCu = new double[line];
                MAVd = new double[line];
                RMSd = new double[line];
                VARd = new double[line];
                SDd = new double[line];
                WLd = new double[line];
                ZCd = new double[line];
                SSCd = new double[line];
                MAVr = new double[line];
                RMSr = new double[line];
                VARr = new double[line];
                SDr = new double[line];
                WLr = new double[line];
                ZCr = new double[line];
                SSCr = new double[line];
                MAVl = new double[line];
                RMSl = new double[line];
                VARl = new double[line];
                SDl = new double[line];
                WLl = new double[line];
                ZCl = new double[line];
                SSCl = new double[line];

                using (var reader = new CsvFileReader(openFileDialog1.FileName))
                {
                    while (reader.ReadRow(columns))
                    {
                        //data = columns.Select(x => double.Parse(x, NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture)).ToList();
                        for (int i = 0; i < 20; i++)
                        {
                            data.Add(double.Parse(columns[i]));
                        }

                        dataMagnitude = data.ToArray();
                        Inputs.dataMagnitude = dataMagnitude;
                        Inputs.Feature();
                        //dataGridView1.Rows.Add(columns.ToArray());
                        data.Clear();
                        string str = comboBox1.SelectedItem.ToString();
                        switch (str)
                        {
                            case "Up":

                                MAVu[j] = Inputs.MAV;
                                RMSu[j] = Inputs.RMS;
                                VARu[j] = Inputs.VAR;
                                SDu[j] = Inputs.SD;
                                WLu[j] = Inputs.WL;
                                ZCu[j] = Inputs.ZC;
                                SSCu[j] = Inputs.SSC;
                                break;
                            case "Down":
                                MAVd[j] = Inputs.MAV;
                                RMSd[j] = Inputs.RMS;
                                VARd[j] = Inputs.VAR;
                                SDd[j] = Inputs.SD;
                                WLd[j] = Inputs.WL;
                                ZCd[j] = Inputs.ZC;
                                SSCd[j] = Inputs.SSC;
                                break;
                            case "Right":
                                MAVr[j] = Inputs.MAV;
                                RMSr[j] = Inputs.RMS;
                                VARr[j] = Inputs.VAR;
                                SDr[j] = Inputs.SD;
                                WLr[j] = Inputs.WL;
                                ZCr[j] = Inputs.ZC;
                                SSCr[j] = Inputs.SSC;
                                break;
                            case "Left":
                                MAVl[j] = Inputs.MAV;
                                RMSl[j] = Inputs.RMS;
                                VARl[j] = Inputs.VAR;
                                SDl[j] = Inputs.SD;
                                WLl[j] = Inputs.WL;
                                ZCl[j] = Inputs.ZC;
                                SSCl[j] = Inputs.SSC;
                                break;
                        }
                        j++;
                    }
                }



                saveFileDialog1.Title = "Save Extracted Features to..";
                saveFileDialog1.FileName = "";
                saveFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Please Choose");
                    saveFileDialog1.ShowDialog();
                }
                else
                {

                    string str = comboBox1.SelectedItem.ToString();
                    StreamWriter file = new StreamWriter(saveFileDialog1.FileName, true);
                    switch (str)
                    {
                        case "Up":
                            for (int i = 0; i < MAVu.Length; i++)
                            {
                                file.WriteLine(MAVu[i] + ";" + RMSu[i] + ";" + VARu[i] + ";" + SDu[i] + ";" + WLu[i] + ";" + ZCu[i] + ";" + SSCu[i] + ";Up");

                            }
                            file.Close();

                            for (int i = 0; i < MAVu.Length; i++)
                            {
                                dataGridView1.Rows.Add(MAVu[i], RMSu[i], VARu[i], SDu[i], WLu[i], ZCu[i], SSCu[i], "up");
                            }

                            break;
                        case "Down":
                            for (int i = 0; i < MAVd.Length; i++)
                            {
                                file.WriteLine(MAVd[i] + ";" + RMSd[i] + ";" + VARd[i] + ";" + SDd[i] + ";" + WLd[i] + ";" + ZCd[i] + ";" + SSCd[i] + ";Down");
                            }
                            file.Close();

                            for (int i = 0; i < MAVd.Length; i++)
                            {
                                dataGridView1.Rows.Add(MAVd[i], RMSd[i], VARd[i], SDd[i], WLd[i], ZCd[i], SSCd[i], "down");
                            }

                            break;
                        case "Right":
                            for (int i = 0; i < MAVr.Length; i++)
                            {
                                file.WriteLine(MAVr[i] + ";" + RMSr[i] + ";" + VARr[i] + ";" + SDr[i] + ";" + WLr[i] + ";" + ZCr[i] + ";" + SSCr[i] + ";Right");
                            }
                            file.Close();

                            for (int i = 0; i < MAVr.Length; i++)
                            {
                                dataGridView1.Rows.Add(MAVr[i], RMSr[i], VARr[i], SDr[i], WLr[i], ZCr[i], SSCr[i], "right");
                            }
                            break;
                        case "Left":
                            for (int i = 0; i < MAVl.Length; i++)
                            {
                                file.WriteLine(MAVl[i] + ";" + RMSl[i] + ";" + VARl[i] + ";" + SDl[i] + ";" + WLl[i] + ";" + ZCl[i] + ";" + SSCl[i] + ";Left");
                            }
                            file.Close();

                            for (int i = 0; i < MAVl.Length; i++)
                            {
                                dataGridView1.Rows.Add(MAVl[i], RMSl[i], VARl[i], SDl[i], WLl[i], ZCl[i], SSCl[i], "left");
                            }
                            break;
                    }


                }
            }


        }



        private void button5_Click(object sender, EventArgs e)
        {
            Form1 dspform = new Form1();
            dspform.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
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



                saveFileDialog1.Title = "Save Extracted Weights to..";
                saveFileDialog1.FileName = "";
                saveFileDialog1.Filter = "All Files|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Please Choose");
                    saveFileDialog1.ShowDialog();
                }
                else
                {
                    StreamWriter File1 = new StreamWriter(saveFileDialog1.FileName + "_Hid.csv");
                    StreamWriter File2 = new StreamWriter(saveFileDialog1.FileName + "_Out.csv");

                    for (int i = 0; i < 8; i++)
                    {
                        File1.Write("{");
                        for (int j = 0; j < 10; j++)
                        {
                            File1.Write(JST.GetWeight(0, i, j));
                            if (j < 9)
                            {
                                File1.Write(",");
                            }

                        }
                        File1.Write("}");
                        if (i < 7)
                        {
                            File1.Write(",");
                        }
                        File1.Write("\n");
                    }
                    File1.Close();
                    for (int i = 0; i < 11; i++)
                    {
                        File2.Write("{");
                        for (int j = 0; j < 4; j++)
                        {
                            File2.Write(JST.GetWeight(1, i, j));
                            if (j < 3)
                            {
                                File2.Write(",");
                            }

                        }
                        File2.Write("}");
                        if (i < 10)
                        {
                            File2.Write(",");
                        }
                        File2.Write("\n");
                    }
                    File2.Close();
                }
            }
        }



        private void button2_Click_1(object sender, EventArgs e)
        {
            BasicNetwork JST = new BasicNetwork();
            JST.AddLayer(new BasicLayer(null, true, 7));
            JST.AddLayer(new BasicLayer(new ActivationTANH(), true, 10));
            JST.AddLayer(new BasicLayer(new ActivationLinear(), false, 4));
            JST.Structure.FinalizeStructure();
            JST.Reset();

            
            openFileDialog1.Title = "Open Feature File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                IVersatileDataSource data = new CSVDataSource(openFileDialog1.FileName, false, CSVFormat.DecimalComma);
                var InputJST = new VersatileMLDataSet(data);
                InputJST.DefineSourceColumn("MAV", 0, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("RMS", 1, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("VAR", 2, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("SD", 3, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("WL", 4, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("ZC", 5, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("SSC", 6, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                ColumnDefinition outputColumn = InputJST.DefineSourceColumn("Arrow", 7, ColumnType.Nominal);
                InputJST.DefineSingleOutputOthersInput(outputColumn);
                InputJST.Analyze();
                var model = new EncogModel(InputJST);

                model.SelectMethod(InputJST, MLMethodFactory.TypeFeedforward);//, "?:B->SIGMOID->10:B->SIGMOID->?", MLTrainFactory.TypeLma, "LR=0.7, MOM=0.3");
                InputJST.NormHelper.NormStrategy = new BasicNormalizationStrategy(-1, 1, 0, 1);
                InputJST.Normalize();


                List<Encog.ML.Data.Versatile.Division.DataDivision> dataDivisionList = new List<Encog.ML.Data.Versatile.Division.DataDivision>();
                dataDivisionList.Add(new Encog.ML.Data.Versatile.Division.DataDivision(0.2));
                dataDivisionList.Add(new Encog.ML.Data.Versatile.Division.DataDivision(0.8));

                InputJST.Divide(dataDivisionList, true, new MersenneTwisterGenerateRandom());
                IMLDataSet validation = dataDivisionList[0].Dataset;
                IMLDataSet training = dataDivisionList[1].Dataset;

                var m = new FileInfo(openFileDialog1.FileName + "_nor_train.csv");
                var n = new FileInfo(openFileDialog1.FileName + "_nor_valid.csv");
                EncogUtility.SaveCSV(m, CSVFormat.DecimalComma, training);
                EncogUtility.SaveCSV(n, CSVFormat.DecimalComma, validation);

                CSVMLDataSet j = new CSVMLDataSet(openFileDialog1.FileName + "_nor_train.csv", 7, 4, false, CSVFormat.DecimalComma, false);
                CSVMLDataSet k = new CSVMLDataSet(openFileDialog1.FileName + "_nor_valid.csv", 7, 4, false, CSVFormat.DecimalComma, false);
                IMLDataSet q = new BasicMLDataSet(j);
                IMLDataSet r = new BasicMLDataSet(k);

                LineItem kurvaTrain = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
                LineItem kurvaValid = zedGraphControl1.GraphPane.CurveList[1] as LineItem;
                IPointListEdit listError = kurvaTrain.Points as IPointListEdit;
                IPointListEdit listValid = kurvaValid.Points as IPointListEdit;
                listError.Clear();
                listValid.Clear();
                var str = comboBox2.SelectedItem.ToString();

                switch (str)
                {
                    case "TrainSCG":
                        train = new ScaledConjugateGradient(JST, q);
                        break;
                    case "NormBackpro":
                        train = new Backpropagation(JST, q);
                        break;
                    case "LMATrain":
                        train = new LevenbergMarquardtTraining(JST, q);
                        break;
                }


                

                var t = 0;
                var i = 0;

                Stopwatch ticker = new Stopwatch();
                Encog.MathUtil.Error.ErrorCalculation.Mode = Encog.MathUtil.Error.ErrorCalculationMode.MSE;
                double error = 0;
                List<double> valid = new List<double>();
                
                ticker.Start();
                do
                {
                    train.Iteration();

                    error = JST.CalculateError(q);
                    valid.Add(JST.CalculateError(r));


                    listError.Add(t, error);

                    listValid.Add(t, valid[t]);


                    zedGraphControl1.AxisChange();
                    zedGraphControl1.Invalidate();
                    if (t>0)
                    {
                        if (valid[t]>valid[t-1])
                        {
                            i++;  
                        }
                    }
                    if (i>10)
                    {
                        
            //            EncogDirectoryPersistence.SaveObject(new FileInfo(openFileDialog1.FileName + "_valid_network.eg"), JST);
                        textBox1.Text = "V= " + Convert.ToString(valid[t]) + " best epoch=" + Convert.ToString(t);
                        if (valid[t] > 0.1)
                        {
                            textBox1.Text = "Validation not satisfied, stopped at E= " + Convert.ToString(error);

                        }
                        break;
                    }
                    
                    t++;

                } while (error > 0.001 && t < 1000);
                ticker.Stop();

                
                        
                //textBox1.Text += "E= " + Convert.ToString(error);
                textBox2.Text = "epoch= " + Convert.ToString(t) + " & time= " + Convert.ToString(ticker.ElapsedMilliseconds) + "ms";
                ticker.Reset();
                saveFileDialog1.Title = "Save Network to..";
                saveFileDialog1.FileName = "";
                saveFileDialog1.Filter = "EG (Encog Network)|*.eg|All Files|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Please Choose");
                    saveFileDialog1.ShowDialog();
                }
                else
                {
                    EncogDirectoryPersistence.SaveObject(new FileInfo(saveFileDialog1.FileName), JST);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BasicNetwork JST = new BasicNetwork();
            JST.AddLayer(new BasicLayer(null, true, 20));
            JST.AddLayer(new BasicLayer(new ActivationTANH(), true, 10));
            JST.AddLayer(new BasicLayer(new ActivationLinear(), false, 4));
            JST.Structure.FinalizeStructure();
            JST.Reset();


            openFileDialog1.Title = "Open Feature File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                IVersatileDataSource data = new CSVDataSource(openFileDialog1.FileName, false, CSVFormat.DecimalComma);
                var InputJST = new VersatileMLDataSet(data);
                InputJST.DefineSourceColumn("1", 0, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("2", 1, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("3", 2, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("4", 3, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("5", 4, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("6", 5, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("7", 6, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("8", 7, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("9", 8, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("10", 9, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("11", 10, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("12", 11, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("13", 12, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("14", 13, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("15", 14, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("16", 15, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("17", 16, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("18", 17, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("19", 18, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                InputJST.DefineSourceColumn("20", 19, Encog.ML.Data.Versatile.Columns.ColumnType.Continuous);
                ColumnDefinition outputColumn = InputJST.DefineSourceColumn("Arrow", 20, ColumnType.Nominal);
                InputJST.DefineSingleOutputOthersInput(outputColumn);
                InputJST.Analyze();


                var model = new EncogModel(InputJST);

                model.SelectMethod(InputJST, MLMethodFactory.TypeFeedforward);//, "?:B->SIGMOID->10:B->SIGMOID->?", MLTrainFactory.TypeLma, "LR=0.7, MOM=0.3");
                InputJST.NormHelper.NormStrategy = new BasicNormalizationStrategy(-1, 1, 0, 1);
                InputJST.Normalize();


                List<Encog.ML.Data.Versatile.Division.DataDivision> dataDivisionList = new List<Encog.ML.Data.Versatile.Division.DataDivision>();
                dataDivisionList.Add(new Encog.ML.Data.Versatile.Division.DataDivision(0.2));
                dataDivisionList.Add(new Encog.ML.Data.Versatile.Division.DataDivision(0.8));

                InputJST.Divide(dataDivisionList, true, new MersenneTwisterGenerateRandom());
                IMLDataSet validation = dataDivisionList[0].Dataset;
                IMLDataSet training = dataDivisionList[1].Dataset;

                var m = new FileInfo(openFileDialog1.FileName + "_nor_train.csv");
                var n = new FileInfo(openFileDialog1.FileName + "_nor_valid.csv");
                EncogUtility.SaveCSV(m, CSVFormat.DecimalComma, training);
                EncogUtility.SaveCSV(n, CSVFormat.DecimalComma, validation);

                CSVMLDataSet j = new CSVMLDataSet(openFileDialog1.FileName + "_nor_train.csv", 20, 4, false, CSVFormat.DecimalComma, false);
                CSVMLDataSet k = new CSVMLDataSet(openFileDialog1.FileName + "_nor_valid.csv", 20, 4, false, CSVFormat.DecimalComma, false);
                IMLDataSet q = new BasicMLDataSet(j);
                IMLDataSet r = new BasicMLDataSet(k);

                LineItem kurvaTrain = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
                LineItem kurvaValid = zedGraphControl1.GraphPane.CurveList[1] as LineItem;
                IPointListEdit listError = kurvaTrain.Points as IPointListEdit;
                IPointListEdit listValid = kurvaValid.Points as IPointListEdit;
                listError.Clear();
                listValid.Clear();
                var str = comboBox2.SelectedItem.ToString();

                switch (str)
                {
                    case "TrainSCG":
                        train = new ScaledConjugateGradient(JST, q);
                        break;
                    case "NormBackpro":
                        train = new Backpropagation(JST, q);
                        break;
                    case "LMATrain":
                        train = new LevenbergMarquardtTraining(JST, q);
                        break;
                }




                var t = 0;
                var i = 0;

                Stopwatch ticker = new Stopwatch();
                Encog.MathUtil.Error.ErrorCalculation.Mode = Encog.MathUtil.Error.ErrorCalculationMode.MSE;
                double error = 0;
                List<double> valid = new List<double>();

                ticker.Start();
                do
                {
                    train.Iteration();

                    error = JST.CalculateError(q);
                    valid.Add(JST.CalculateError(r));


                    listError.Add(t, error);

                    listValid.Add(t, valid[t]);


                    zedGraphControl1.AxisChange();
                    zedGraphControl1.Invalidate();
                    if (t > 0)
                    {
                        if (valid[t] > valid[t - 1])
                        {
                            i++;
                        }
                    }

                    if (i > 10)
                    {

                        //EncogDirectoryPersistence.SaveObject(new FileInfo(openFileDialog1.FileName + "_valid_network.eg"), JST);
                        textBox1.Text = "V= " + Convert.ToString(valid[t]) + " best epoch=" + Convert.ToString(t);
                        if (valid[t] > 0.1)
                        {
                            textBox1.Text = "Validation not satisfied, stopped at E= " + Convert.ToString(error);

                        }
                        break;
                    }

                    t++;

                } while (error > 0.001 && t < 1000);
                ticker.Stop();


                textBox1.Text = "V= " + Convert.ToString(valid[t-1]) + " best epoch=" + Convert.ToString(t);
                //textBox1.Text += "E= " + Convert.ToString(error);
                textBox2.Text = "epoch= " + Convert.ToString(t) + " & time= " + Convert.ToString(ticker.ElapsedMilliseconds) + "ms";
                ticker.Reset();
                saveFileDialog1.Title = "Save Network to..";
                saveFileDialog1.FileName = "";
                saveFileDialog1.Filter = "EG (Encog Network)|*.eg|All Files|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Please Choose");
                    saveFileDialog1.ShowDialog();
                }
                else
                {
                    EncogDirectoryPersistence.SaveObject(new FileInfo(saveFileDialog1.FileName), JST);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button9.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView4.Rows.Clear();
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
                openFileDialog1.Title = "Open Normalized Feature File...";
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Choice Cancelled");
                }
                else
                {
                    CSVMLDataSet j = new CSVMLDataSet(openFileDialog1.FileName, 7, 4, false, CSVFormat.DecimalComma, false);
                    
                    IMLDataSet q = new BasicMLDataSet(j);
                    
                    
                    saveFileDialog1.Title = "Save computed output..";
                    saveFileDialog1.FileName = "";
                    saveFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";

                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    {
                        MessageBox.Show("Choice Cancelled");
                    }
                    else
                    {
                        StreamWriter m = new StreamWriter(saveFileDialog1.FileName, true);

                        foreach (IMLDataPair pair in q)
                        {
                            IMLData output = JST.Compute(pair.Input);
                            
                            dataGridView4.Rows.Add(output[0], output[1], output[2], output[3]);

                            m.WriteLine(output[0] + ";" + output[1] + ";" + output[2] + ";" + output[3]);
                        }
                        m.Close();
                        dataGridView4.Rows.Add(JST.CalculateError(q));
                    }
                }
            }




        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button4.Enabled = true;
            //button7.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
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



                saveFileDialog1.Title = "Save Extracted Weights to..";
                saveFileDialog1.FileName = "";
                saveFileDialog1.Filter = "All Files|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Please Choose");
                    saveFileDialog1.ShowDialog();
                }
                else
                {
                    StreamWriter File1 = new StreamWriter(saveFileDialog1.FileName + "_Hid.csv");
                    StreamWriter File2 = new StreamWriter(saveFileDialog1.FileName + "_Out.csv");

                    for (int i = 0; i < 21; i++)
                    {
                        File1.Write("{");
                        for (int j = 0; j < 10; j++)
                        {
                            File1.Write(JST.GetWeight(0, i, j));
                            if (j < 9)
                            {
                                File1.Write(",");
                            }

                        }
                        File1.Write("}");
                        if (i < 20)
                        {
                            File1.Write(",");
                        }
                        File1.Write("\n");
                    }
                    File1.Close();
                    for (int i = 0; i < 11; i++)
                    {
                        File2.Write("{");
                        for (int j = 0; j < 4; j++)
                        {
                            File2.Write(JST.GetWeight(1, i, j));
                            if (j < 3)
                            {
                                File2.Write(",");
                            }

                        }
                        File2.Write("}");
                        if (i < 10)
                        {
                            File2.Write(",");
                        }
                        File2.Write("\n");
                    }
                    File2.Close();
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView4.Rows.Clear();
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
                openFileDialog1.Title = "Open Normalized Feature File...";
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Choice Cancelled");
                }
                else
                {
                    CSVMLDataSet j = new CSVMLDataSet(openFileDialog1.FileName, 20, 4, false, CSVFormat.DecimalComma, false);
                    IMLDataSet q = new BasicMLDataSet(j);
                    saveFileDialog1.Title = "Save computed output..";
                    saveFileDialog1.FileName = "";
                    saveFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";

                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    {
                        MessageBox.Show("Choice Cancelled");
                    }
                    else
                    {
                        StreamWriter m = new StreamWriter(saveFileDialog1.FileName, true);

                        foreach (IMLDataPair pair in q)
                        {
                            IMLData output = JST.Compute(pair.Input);

                            dataGridView4.Rows.Add(output[0], output[1], output[2], output[3]);

                            m.WriteLine(output[0] + ";" + output[1] + ";" + output[2] + ";" + output[3]);
                        }
                        m.Close();
                        dataGridView4.Rows.Add(JST.CalculateError(q));
                    }
                }
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open Raw EMG File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {

                List<string> columns = new List<string>();
                using (var reader = new CsvFileReader(openFileDialog1.FileName))
                {
                    while (reader.ReadRow(columns))
                    {
                        columns.RemoveRange(21, 29);
                        columns[20] = comboBox1.SelectedItem.ToString();
                        dataGridView2.Rows.Add(columns.ToArray());


                    }
                }


            }





        }

        private void button10_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save Feauture File..";
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "CSV (comma delimited)|*.csv|All Files|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                int numColumns = 21;
                using (var writer = new CsvFileWriter(saveFileDialog1.FileName))
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            List<string> columns = new List<string>();
                            for (int col = 0; col < numColumns; col++)
                                columns.Add((string)row.Cells[col].Value ?? String.Empty);

                            writer.WriteRow(columns);
                        }
                    }
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select First Layer Weight File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                w1 = File.ReadAllLines(openFileDialog1.FileName);

                openFileDialog1.Title = "Select Second Layer Weight File...";
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";

                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Choice Cancelled");
                }
                else
                {
                    w2 = File.ReadAllLines(openFileDialog1.FileName);

             openFileDialog1.Title = "Select Database File...";
                    openFileDialog1.FileName = "";
                    openFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";

                    if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    {
                        MessageBox.Show("Choice Cancelled");
                    }
                    else
                    {
                        var m = File.ReadAllLines(openFileDialog1.FileName);
                        List<double> mini = new List<double>();
                        List<double> maxi = new List<double>();
                        double[] d1 =  new double[m.Length];
                        double[] d2 = new double[m.Length];
                        double[] d3 = new double[m.Length];
                        double[] d4 = new double[m.Length];
                        double[] d5 = new double[m.Length];
                        double[] d6 = new double[m.Length];
                        double[] d7 = new double[m.Length];
                        double[] d8 = new double[m.Length];
                        double[] d9 = new double[m.Length];
                        double[] d10 = new double[m.Length];
                        double[] d11 = new double[m.Length];
                        double[] d12 = new double[m.Length];
                        double[] d13 = new double[m.Length];
                        double[] d14 = new double[m.Length];
                        double[] d15 = new double[m.Length];
                        double[] d16 = new double[m.Length];
                        double[] d17 = new double[m.Length];
                        double[] d18 = new double[m.Length];
                        double[] d19 = new double[m.Length];
                        double[] d20 = new double[m.Length];

                        for (int i = 0; i < m.Length; i++)
                        {
                            var l = m[i];
                            List<string> l1 = l.Split(';').ToList();
                            l1.RemoveAt(20);
                            var l3 = l1.ToArray();
                            
                            var l2 = Array.ConvertAll<string, double>(l3,new Converter<string,double>(Double.Parse));

                            d1[i] = l2[0];
                            d2[i] = l2[1];
                            d3[i] = l2[2];
                            d4[i] = l2[3];
                            d5[i] = l2[4];
                            d6[i] = l2[5];
                            d7[i] = l2[6];
                            d8[i] = l2[7];
                            d9[i] = l2[8];
                            d10[i] = l2[9];
                            d11[i] = l2[10];
                            d12[i] = l2[11];
                            d13[i] = l2[12];
                            d14[i] = l2[13];
                            d15[i] = l2[14];
                            d16[i] = l2[15];
                            d17[i] = l2[16];
                            d18[i] = l2[17];
                            d19[i] = l2[18];
                            d20[i] = l2[19];

                            
                        }

                        mini.Add(d1.Min());
                        mini.Add(d2.Min());
                        mini.Add(d3.Min());
                        mini.Add(d4.Min());
                        mini.Add(d5.Min());
                        mini.Add(d6.Min());
                        mini.Add(d7.Min());
                        mini.Add(d8.Min());
                        mini.Add(d9.Min());
                        mini.Add(d10.Min());
                        mini.Add(d11.Min());
                        mini.Add(d12.Min());
                        mini.Add(d13.Min());
                        mini.Add(d14.Min());
                        mini.Add(d15.Min());
                        mini.Add(d16.Min());
                        mini.Add(d17.Min());
                        mini.Add(d18.Min());
                        mini.Add(d19.Min());
                        mini.Add(d20.Min());
                        maxi.Add(d1.Max());
                        maxi.Add(d2.Max());
                        maxi.Add(d3.Max());
                        maxi.Add(d4.Max());
                        maxi.Add(d5.Max());
                        maxi.Add(d6.Max());
                        maxi.Add(d7.Max());
                        maxi.Add(d8.Max());
                        maxi.Add(d9.Max());
                        maxi.Add(d10.Max());
                        maxi.Add(d11.Max());
                        maxi.Add(d12.Max());
                        maxi.Add(d13.Max());
                        maxi.Add(d14.Max());
                        maxi.Add(d15.Max());
                        maxi.Add(d16.Max());
                        maxi.Add(d17.Max());
                        maxi.Add(d18.Max());
                        maxi.Add(d19.Max());
                        maxi.Add(d20.Max());

                        for (int i = 0; i < 20; i++)
                        {
                         minmax[i] = "{"+mini[i].ToString()+","+maxi[i].ToString()+"}";
                            if (i<19)
                            {
                                minmax[i] += ",";
                            }   
                        }

                        openFileDialog1.Title = "Select Uno-ModeKlasifikasi.ino File...";
                        openFileDialog1.FileName = "";
                        openFileDialog1.Filter = "Arduino File|*.ino|All Files|*.*";

                        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                        {
                            MessageBox.Show("Choice Cancelled");
                        }
                        else
                        {
                            lines = File.ReadAllLines(openFileDialog1.FileName);
                            for (int i = 9; i < 30; i++)
                            {
                                lines[i] = w1[i - 9];
                            }
                            for (int i = 32; i < 43; i++)
                            {
                                lines[i] = w2[i - 32];
                            }
                            for (int i = 45; i < 65; i++)
                            {
                                lines[i] = minmax[i - 45];
                            }

                            File.WriteAllLines(openFileDialog1.FileName, lines);
                        }

                    }

                }

            }
        }
    
        private void button12_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select First Layer Weight File...";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Choice Cancelled");
            }
            else
            {
                w1 = File.ReadAllLines(openFileDialog1.FileName);

                openFileDialog1.Title = "Select Second Layer Weight File...";
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";

                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Choice Cancelled");
                }
                else
                {
                    w2 = File.ReadAllLines(openFileDialog1.FileName);

                    openFileDialog1.Title = "Select Database File...";
                    openFileDialog1.FileName = "";
                    openFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";

                    if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    {
                        MessageBox.Show("Choice Cancelled");
                    }
                    else
                    {
                        var m = File.ReadAllLines(openFileDialog1.FileName);
                        List<double> mini = new List<double>();
                        List<double> maxi = new List<double>();
                        double[] d1 = new double[m.Length];
                        double[] d2 = new double[m.Length];
                        double[] d3 = new double[m.Length];
                        double[] d4 = new double[m.Length];
                        double[] d5 = new double[m.Length];
                        double[] d6 = new double[m.Length];
                        double[] d7 = new double[m.Length];
                        
                        for (int i = 0; i < m.Length; i++)
                        {
                            var l = m[i];
                            List<string> l1 = l.Split(';').ToList();
                            l1.RemoveAt(7);
                            var l3 = l1.ToArray();

                            var l2 = Array.ConvertAll<string, double>(l3, new Converter<string, double>(Double.Parse));

                            d1[i] = l2[0];
                            d2[i] = l2[1];
                            d3[i] = l2[2];
                            d4[i] = l2[3];
                            d5[i] = l2[4];
                            d6[i] = l2[5];
                            d7[i] = l2[6];
                            

                        }

                        mini.Add(d1.Min());
                        mini.Add(d2.Min());
                        mini.Add(d3.Min());
                        mini.Add(d4.Min());
                        mini.Add(d5.Min());
                        mini.Add(d6.Min());
                        mini.Add(d7.Min());
                        maxi.Add(d1.Max());
                        maxi.Add(d2.Max());
                        maxi.Add(d3.Max());
                        maxi.Add(d4.Max());
                        maxi.Add(d5.Max());
                        maxi.Add(d6.Max());
                        maxi.Add(d7.Max());
                        
                        for (int i = 0; i < 7; i++)
                        {
                            minmax[i] = "{" + mini[i].ToString() + "," + maxi[i].ToString() + "}";
                            if (i < 6)
                            {
                                minmax[i] += ",";
                            }
                        }
                        openFileDialog1.Title = "Select Uno-ModeKlasifikasi.ino File...";
                        openFileDialog1.FileName = "";
                        openFileDialog1.Filter = "Arduino File|*.ino|All Files|*.*";

                        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                        {
                            MessageBox.Show("Choice Cancelled");
                        }
                        else
                        {
                            lines = File.ReadAllLines(openFileDialog1.FileName);
                            for (int i = 10; i < 18; i++)
                            {
                                lines[i] = w1[i - 10];
                            }
                            for (int i = 20; i < 31; i++)
                            {
                                lines[i] = w2[i - 20];
                            }
                            for (int i = 33; i < 40; i++)
                            {
                                lines[i] = minmax[i - 33];
                            }

                            File.WriteAllLines(openFileDialog1.FileName, lines);
                        }

                    }

                }

            }
        }

        
    }
}
