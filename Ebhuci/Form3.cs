using System;
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



namespace Ebhuci
{
    public partial class Form3 : Form
    {

        string[] data, dataSave;
        double[] dataMagnitude, MAVu, RMSu, VARu, SDu, WLu, ZCu, SSCu, MAVd, RMSd, VARd, SDd, WLd, ZCd, SSCd, MAVr, RMSr, VARr, SDr, WLr, ZCr, SSCr, MAVl, RMSl, VARl, SDl, WLl, ZCl, SSCl;

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


            LineItem kurvaError = grafikError.AddCurve("Magnitude", listError, Color.Red, SymbolType.None);


        }

        private void button1_Click(object sender, EventArgs e)
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
                MAVu = new double[openFileDialog1.FileNames.Length];
                RMSu = new double[openFileDialog1.FileNames.Length];
                VARu = new double[openFileDialog1.FileNames.Length];
                SDu = new double[openFileDialog1.FileNames.Length];
                WLu = new double[openFileDialog1.FileNames.Length];
                ZCu = new double[openFileDialog1.FileNames.Length];
                SSCu = new double[openFileDialog1.FileNames.Length];
                MAVd = new double[openFileDialog1.FileNames.Length];
                RMSd = new double[openFileDialog1.FileNames.Length];
                VARd = new double[openFileDialog1.FileNames.Length];
                SDd = new double[openFileDialog1.FileNames.Length];
                WLd = new double[openFileDialog1.FileNames.Length];
                ZCd = new double[openFileDialog1.FileNames.Length];
                SSCd = new double[openFileDialog1.FileNames.Length];
                MAVr = new double[openFileDialog1.FileNames.Length];
                RMSr = new double[openFileDialog1.FileNames.Length];
                VARr = new double[openFileDialog1.FileNames.Length];
                SDr = new double[openFileDialog1.FileNames.Length];
                WLr = new double[openFileDialog1.FileNames.Length];
                ZCr = new double[openFileDialog1.FileNames.Length];
                SSCr = new double[openFileDialog1.FileNames.Length];
                MAVl = new double[openFileDialog1.FileNames.Length];
                RMSl = new double[openFileDialog1.FileNames.Length];
                VARl = new double[openFileDialog1.FileNames.Length];
                SDl = new double[openFileDialog1.FileNames.Length];
                WLl = new double[openFileDialog1.FileNames.Length];
                ZCl = new double[openFileDialog1.FileNames.Length];
                SSCl = new double[openFileDialog1.FileNames.Length];
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    data = File.ReadAllLines(openFileDialog1.FileNames[i]);
                    dataMagnitude = data.Select(x => double.Parse(x)).ToArray();

                    FeatureExtract Inputs = new FeatureExtract();
                    Inputs.dataMagnitude = dataMagnitude;
                    Inputs.Feature();

                    string str = comboBox1.SelectedItem.ToString();
                    switch (str)
                    {
                        case "Up":

                            MAVu[i] = Inputs.MAV;
                            RMSu[i] = Inputs.RMS;
                            VARu[i] = Inputs.VAR;
                            SDu[i] = Inputs.SD;
                            WLu[i] = Inputs.WL;
                            ZCu[i] = Inputs.ZC;
                            SSCu[i] = Inputs.SSC;
                            break;
                        case "Down":
                            MAVd[i] = Inputs.MAV;
                            RMSd[i] = Inputs.RMS;
                            VARd[i] = Inputs.VAR;
                            SDd[i] = Inputs.SD;
                            WLd[i] = Inputs.WL;
                            ZCd[i] = Inputs.ZC;
                            SSCd[i] = Inputs.SSC;
                            break;
                        case "Right":
                            MAVr[i] = Inputs.MAV;
                            RMSr[i] = Inputs.RMS;
                            VARr[i] = Inputs.VAR;
                            SDr[i] = Inputs.SD;
                            WLr[i] = Inputs.WL;
                            ZCr[i] = Inputs.ZC;
                            SSCr[i] = Inputs.SSC;
                            break;
                        case "Left":
                            MAVl[i] = Inputs.MAV;
                            RMSl[i] = Inputs.RMS;
                            VARl[i] = Inputs.VAR;
                            SDl[i] = Inputs.SD;
                            WLl[i] = Inputs.WL;
                            ZCl[i] = Inputs.ZC;
                            SSCl[i] = Inputs.SSC;
                            break;
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

                            for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                            {
                                dataGridView1.Rows.Add(MAVu[i], RMSu[i], VARu[i], SDu[i], WLu[i], ZCu[i], SSCu[i], "up");
                            }

                            break;
                        case "Down":
                            for (int i = 0; i < MAVu.Length; i++)
                            {
                                file.WriteLine(MAVd[i] + ";" + RMSd[i] + ";" + VARd[i] + ";" + SDd[i] + ";" + WLd[i] + ";" + ZCd[i] + ";" + SSCd[i] + ";Down");
                            }
                            file.Close();

                            for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                            {
                                dataGridView1.Rows.Add(MAVd[i], RMSd[i], VARd[i], SDd[i], WLd[i], ZCd[i], SSCd[i], "down");
                            }

                            break;
                        case "Right":
                            for (int i = 0; i < MAVu.Length; i++)
                            {
                                file.WriteLine(MAVr[i] + ";" + RMSr[i] + ";" + VARr[i] + ";" + SDr[i] + ";" + WLr[i] + ";" + ZCr[i] + ";" + SSCr[i] + ";Right");
                            }
                            file.Close();

                            for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                            {
                                dataGridView1.Rows.Add(MAVr[i], RMSr[i], VARr[i], SDr[i], WLr[i], ZCr[i], SSCr[i], "right");
                            }
                            break;
                        case "Left":
                            for (int i = 0; i < MAVu.Length; i++)
                            {
                                file.WriteLine(MAVl[i] + ";" + RMSl[i] + ";" + VARl[i] + ";" + SDl[i] + ";" + WLl[i] + ";" + ZCl[i] + ";" + SSCl[i] + ";Left");
                            }
                            file.Close();

                            for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
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
                        for (int j = 0; j < 10; j++)
                        {
                            File1.Write(JST.GetWeight(0, i, j) + ",");

                        }
                        File1.Write("\n");
                    }
                    File1.Close();
                    for (int i = 0; i < 11; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            File2.Write(JST.GetWeight(1, i, j) + ",");

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
                InputJST.Normalize();

                //var methodFactory = new MLMethodFactory();
                //var method = methodFactory.Create(MLMethodFactory.TypeFeedforward, "?:B->SIGMOID->10:B->LINEAR->?", 7, 4);
                //var trainFactory = new MLTrainFactory();
                //var train = trainFactory.Create(method,InputJST,MLTrainFactory.TypeLma, "LR=0.7, MOM=0.3");


                //AnalystNormalizeCSV normal = new AnalystNormalizeCSV();

                model.HoldBackValidation(0.3, true, 1001);
                model.SelectTraining(InputJST, MLTrainFactory.TypeLma, "LR=0.7,MOM=0.3");
                model.SelectTrainingType(InputJST);

                var bestmethod = (IMLRegression)model.Crossvalidate(5, true);

                //var abc = InputJST.Data;
                //var norm = new NormalizeArray { NormalizedHigh = 1, NormalizedLow = -1 };
                //for (int i = 0; i < abc.Length; i++)
                //{
                //norm.Process(abc[i]);   
                //}
                //IMLDataSet trainingset = new BasicMLDataSet(abc, OutputIdeal);
                var m = new FileInfo(openFileDialog1.FileName + "_nor.csv");
                EncogUtility.SaveCSV(m, CSVFormat.DecimalComma, model.Dataset);

                CSVMLDataSet j = new CSVMLDataSet(openFileDialog1.FileName + "_nor.csv", 7, 4, false, CSVFormat.DecimalComma, false);
                IMLDataSet q = new BasicMLDataSet(j);

                LineItem kurvaError = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
                IPointListEdit listError = kurvaError.Points as IPointListEdit;
                listError.Clear();
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
                
                

                
                var t = 1;

                Stopwatch ticker = new Stopwatch();
                Encog.MathUtil.Error.ErrorCalculation.Mode = Encog.MathUtil.Error.ErrorCalculationMode.MSE;
                double error = 0;
                ticker.Start();
                do
                {
                    train.Iteration();
                    t++;
                    error = JST.CalculateError(q);
                    listError.Add(t, error);
                   
                    zedGraphControl1.AxisChange();
                    zedGraphControl1.Invalidate();
                    
                } while (error > 0.001 && t < 1000);
                ticker.Stop();


                textBox1.Text = "E= " + Convert.ToString(error);// +" & " + Convert.ToString(train.Error);
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
                    InputJST.Normalize();

                    //var methodFactory = new MLMethodFactory();
                    //var method = methodFactory.Create(MLMethodFactory.TypeFeedforward, "?:B->SIGMOID->10:B->LINEAR->?", 7, 4);
                    //var trainFactory = new MLTrainFactory();
                    //var train = trainFactory.Create(method,InputJST,MLTrainFactory.TypeLma, "LR=0.7, MOM=0.3");


                    //AnalystNormalizeCSV normal = new AnalystNormalizeCSV();

                    model.HoldBackValidation(0.3, true, 1001);
                    model.SelectTraining(InputJST, MLTrainFactory.TypeLma, "LR=0.7,MOM=0.3");
                    model.SelectTrainingType(InputJST);

                    var bestmethod = (IMLRegression)model.Crossvalidate(5, true);

                    //var abc = InputJST.Data;
                    //var norm = new NormalizeArray { NormalizedHigh = 1, NormalizedLow = -1 };
                    //for (int i = 0; i < abc.Length; i++)
                    //{
                    //norm.Process(abc[i]);   
                    //}
                    //IMLDataSet trainingset = new BasicMLDataSet(abc, OutputIdeal);
                    var m = new FileInfo(openFileDialog1.FileName + "_nor.csv");
                    EncogUtility.SaveCSV(m, CSVFormat.DecimalComma, model.Dataset);

                    CSVMLDataSet j = new CSVMLDataSet(openFileDialog1.FileName + "_nor.csv", 7, 4, false, CSVFormat.DecimalComma, false);
                    IMLDataSet q = new BasicMLDataSet(j);
                    switch (comboBox2.SelectedItem.ToString())
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
                    LineItem kurvaError = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
                    IPointListEdit listError = kurvaError.Points as IPointListEdit;
                    listError.Clear();
                    var t = 1;

                    Stopwatch ticker = new Stopwatch();

                    Encog.MathUtil.Error.ErrorCalculation.Mode = Encog.MathUtil.Error.ErrorCalculationMode.MSE;
                    double error = 0;
                    ticker.Start();
                    do
                    {
                        train.Iteration();
                        t++;
                        error = JST.CalculateError(q);
                        listError.Add(t, error);

                        zedGraphControl1.AxisChange();
                        zedGraphControl1.Invalidate();

                    } while (error > 0.001 && t < 1000);
                    ticker.Stop();


                    textBox1.Text = "E= " + Convert.ToString(error);// +" & " + Convert.ToString(train.Error);
                    textBox2.Text = "epoch= " + Convert.ToString(t) + " & time= " + Convert.ToString(ticker.ElapsedMilliseconds) + "ms";
                    ticker.Reset();
                    //var b = EncogUtility.CalculateRegressionError(bestmethod, model.TrainingDataset);
                    //var c = EncogUtility.CalculateRegressionError(bestmethod, model.ValidationDataset);
                    //textBox1.Text = textBox1.Text + Convert.ToString(b) +";"+Convert.ToString(c);
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
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
        }
    }
}
