using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public class myPoint {
            public double x { get; set; }
            public double y{ get; set; }
            public string toString() {
                return x.ToString() + " " + y.ToString();
            }
           
        }

        public sealed class myPointMap : ClassMap<myPoint>
        {
            public myPointMap()
            {
                Map(m => m.x).Index(0);
                Map(m => m.y).Index(1);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private String openFileName, saveFileName;

        public static void Log(string message)
        {
            File.AppendAllText("log.txt", message+"\n");
        }

        private double fx(double x, double m, double q) {
            return Math.Exp(-Math.Pow(Math.Log(x) - m, 2) / (2 * q * q)) / (x * q * Math.Sqrt(2 * Math.PI));            
        }

        private void generate(double from, double to, double m, double q, out double K1, out double K2) { 
            Random random = new Random();
            double k1, k2;
            do
            {
                k1 = random.NextDouble();
                k2 = random.NextDouble();
                K1 = from + (to - from) * k1;
                K2 = k2 * fx(Math.Exp(m - q * q),m, q);
                Log("generated " + K1.ToString() + " " + K2.ToString());
            } while (K2 > fx(K1, m, q));
        }

        private void drawGraph(double from, double to, double m, double q) {
            const double MOVE = 0.05;
            double x = from;
            this.chart1.Series[0].Points.Clear();
            while (x <= to)
            {
                this.chart1.Series[0].Points.AddXY(x, fx(x, m, q));
                x += MOVE;
            }
        }
        
        private void drawPoints(List<myPoint> list)
        {
            this.chart1.Series[1].Points.Clear();
            Log("cleared");
            foreach (myPoint mp in list) {
                this.chart1.Series[1].Points.AddXY(mp.x, mp.y);
                Log("drawn " + mp.ToString());
                this.chart1.Update();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double from = Convert.ToDouble(this.textBox1.Text), to = Convert.ToDouble(this.textBox2.Text);
            double m = Convert.ToDouble(this.textBox4.Text), q = Convert.ToDouble(this.textBox5.Text);
            double x = from, X, Y;
            int count = Convert.ToInt16(this.textBox3.Text);
            bool toFile = this.checkBox1.Checked;
            if (File.Exists(saveFileName)) File.Delete(saveFileName);
            drawGraph(from, to, m, q);
            this.chart1.Series[1].Points.Clear();
            List<myPoint> list = new List<myPoint> {};
            for (int i = 0; i < count; i++)
            {
                generate(from, to, m, q, out X, out Y);
                Log("postgenerated" + X.ToString() + " " + Y.ToString());
                list.Add(new myPoint() { x = X, y = Y });
                Log("added" + list[list.Count - 1].toString());
            }
            drawPoints(list);
            if (toFile) {

                using (StreamWriter streamWriter = new StreamWriter(saveFileName))
                {

                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {

                        csvWriter.Configuration.Delimiter = ";";
                        csvWriter.WriteRecords(list);
                    }

                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
            if (Char.IsControl(e.KeyChar))
                if (e.KeyChar == (char)Keys.Enter)
                   textBox2.Focus();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
            if (Char.IsControl(e.KeyChar))
                if (e.KeyChar == (char)Keys.Enter)
                   textBox3.Focus();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (Char.IsControl(e.KeyChar))
                if (e.KeyChar == (char)Keys.Enter)
                    textBox4.Focus();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
            {
                e.Handled = true;
            }
            if (Char.IsControl(e.KeyChar))
                if (e.KeyChar == (char)Keys.Enter)
                    textBox5.Focus();
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
            if (Char.IsControl(e.KeyChar))
                if (e.KeyChar == (char)Keys.Enter)
                    button1.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try {
                File.Delete("log.txt");
            } catch (Exception err) { };
        }

        private int ComparisonbyX(myPoint a, myPoint b)
        {
            if (a.x == b.x) return 0;
            if (a.x > b.x) return 1;
            else return -1;
        }

        private int ComparisonbyY(myPoint a, myPoint b)
        {
            if (a.y == b.y) return 0;
            if (a.y > b.y) return 1;
            else return -1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                openFileName = openFileDialog.FileName;
                List<myPoint> list=new List<myPoint> { };
                using (StreamReader streamReader = new StreamReader(openFileName))
                {
                    using (CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        csvReader.Configuration.Delimiter = ";";
                        csvReader.Configuration.RegisterClassMap<myPointMap>();
                        while (csvReader.Read())
                            list.Add(csvReader.GetRecord<myPoint>());
                    }
                }
                drawPoints(list);   
                list.Sort(ComparisonbyX);
                label1.Text = Convert.ToString(list[0].x);
                label2.Text = Convert.ToString(list[list.Count-1].x);

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SaveFileDialog saveFileDiolog = new SaveFileDialog();
                saveFileDiolog.Filter = "csv files (*.csv)|*.csv";
                if (saveFileDiolog.ShowDialog() == DialogResult.OK)
                    saveFileName = saveFileDiolog.FileName;

            }
        }
    }
}
