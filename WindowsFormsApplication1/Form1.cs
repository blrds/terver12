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

        List<myPoint> list = new List<myPoint> { };

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

        private double aver(List<myPoint> list) {
            double aver=0;
            for (int i = 0; i < list.Count; i++)
                aver += list[i].x;
            aver /= list.Count;
            return aver;
        }

        private double mode(List<myPoint> list) {
            double mode;
            list.Sort(ComparisonbyY);
            mode = list[list.Count - 1].x;
            list.Sort(ComparisonbyX);
            return mode;
        }

        private double median(List<myPoint> list) {
            double median;
            if (list.Count % 2 == 1)
                median = list[list.Count / 2].x;
            else
                median = 0.5 * (list[list.Count/2-1].x + list[list.Count/2].x);
            return median;
        }

        private double dispersion(List<myPoint> list) {
            double dispersion=0, av=aver(list);
            for (int i = 0; i < list.Count; i++)
                dispersion += Math.Pow((list[i].x-av), 2);
            dispersion /= list.Count;
            return dispersion;
        }

        private double standart(List<myPoint> list) {
            double standart;
            standart = Math.Sqrt(dispersion(list));
            return standart;
        }

        private double expect(List<myPoint> list) {
            double expect = 0;
            for (int i = 0; i < list.Count; i++)
                expect += list[i].x * list[i].y;
            return expect;
        }

        private double excess(List<myPoint> list) {
            double excess=0, ex = expect(list);
            for (int i = 0; i < list.Count; i++)
                excess += (Math.Pow((list[i].x-ex),4)*list[i].y);
            excess = (excess / (Math.Pow(dispersion(list), 2)*list.Count)) - 3;
            return excess;
        }

        private double asymmetry(List<myPoint> list) {
            double asymmetry=0, ex=expect(list);
            for (int i = 0; i < list.Count; i++)
                asymmetry += Math.Pow((list[i].x - ex), 3) * list[i].y;
            asymmetry /=( Math.Pow(dispersion(list), 3/2)*list.Count);
            return asymmetry;
        }

        private double minimum(List<myPoint> list) {
            double minimum;
            minimum = list[0].x;
            return minimum;
        }

        private double maximum(List<myPoint> list) {
            double maximum;
            maximum = list[list.Count - 1].x;
            return maximum;
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
            //log("cleared");
            foreach (myPoint mp in list) {
                this.chart1.Series[1].Points.AddXY(mp.x, mp.y);
               // Log("drawn " + mp.ToString());
               //this.chart1.Update();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            list.Clear();
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[2].Points.Clear();
            double from = Convert.ToDouble(this.textBox1.Text), to = Convert.ToDouble(this.textBox2.Text);
            double m = Convert.ToDouble(this.textBox4.Text), q = Convert.ToDouble(this.textBox5.Text);
            double x = from, X, Y;
            int count = Convert.ToInt16(this.textBox3.Text);
            bool toFile = this.checkBox1.Checked;
            if (File.Exists(saveFileName)) File.Delete(saveFileName);
            drawGraph(from, to, m, q);
            this.chart1.Series[1].Points.Clear();
            for (int i = 0; i < count; i++)
            {
                generate(from, to, m, q, out X, out Y);
                X=Math.Round(X, 6);
                Y=Math.Round(Y, 6);
                Log("postgenerated" + X.ToString() + " " + Y.ToString());
                list.Add(new myPoint() { x = X, y = Y });
                Log("added" + list[list.Count - 1].toString());
            }
            drawPoints(list);
            if (toFile) {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(saveFileName))
                    {

                        using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                        {
                            csvWriter.Configuration.Delimiter = ";";
                            csvWriter.WriteRecords(list);
                        }

                    }
                }
                catch (Exception err) {
                    MessageBox.Show("Закройте файл записи", "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    button1.Focus();
        }

        private void labelmake(bool b) {
            label6.Visible = b;
            label7.Visible = b;
            label8.Visible = b;
            label9.Visible = b;
            label10.Visible = b;
            label11.Visible = b;
            label12.Visible = b;
            label13.Visible = b;
            label14.Visible = b;
            label15.Visible = b;
            label16.Visible = b;
            label17.Visible = b;
            label18.Visible = b;
            label19.Visible = b;
            label20.Visible = b;
            label21.Visible = b;
            label22.Visible = b;
            label23.Visible = b;
            label24.Visible = b;
            label25.Visible = b;
            label26.Visible = b;
            label27.Visible = b;
            label28.Visible = b;
            label29.Visible = b;
            label30.Visible = b;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            try {
                File.Delete("log.txt");
            } catch (Exception err) { };
            labelmake(false);
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
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[2].Points.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                openFileName = openFileDialog.FileName;
                try
                {
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
                }
                catch (Exception err) {
                    MessageBox.Show("Закройте считываемый файл", "Ошибка чтения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                drawPoints(list);   
            }
        }

        private void gist() {
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            int groups = Convert.ToInt16(Math.Floor(1+3.322*Math.Log(list.Count)));
            double move = (list[list.Count - 1].x - list[0].x) / groups;
            double cur = list[0].x;
            int count;
            for (int i = 0; i < groups; i++) {
                count = 0;
                for (int j = 0; j < list.Count; j++)
                    if (list[j].x >= cur && list[j].x < (cur + move)) count++;
                this.chart1.Series[2].Points.AddXY((cur + cur + move) / 2, count);
                cur += move;
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = 1;
            if (this.textBox4.Text == "") {
                MessageBox.Show("Введите мат.ожидание выборки", "Некоректные данные", MessageBoxButtons.OK, MessageBoxIcon.Error);
                i *= 0;
            }
            if (this.textBox5.Text == "")
            {
                MessageBox.Show("Введите среднеквадратическое отклонение выбоорки", "Некоректные данные", MessageBoxButtons.OK, MessageBoxIcon.Error);
                i *= 0;
            }
            if (list.Count != 0 && i!=0)
            {
                double q = Convert.ToDouble(textBox5.Text), m = Convert.ToDouble(textBox4.Text), q2 = q * q, m2 = m * m;
                list.Sort(ComparisonbyX);
                label7.Text = Convert.ToString(Math.Round(aver(list),4));
                label24.Text = textBox4.Text;
                label9.Text = Convert.ToString(Math.Round(mode(list), 4));
                label25.Text = Convert.ToString(m);
                label11.Text = Convert.ToString(Math.Round(median(list), 4));
                label26.Text = Convert.ToString(m);
                label13.Text = Convert.ToString(Math.Round(dispersion(list), 4));
                label27.Text = Convert.ToString(Math.Round(q2, 4));
                label15.Text = Convert.ToString(Math.Round(standart(list), 4));
                label29.Text = Convert.ToString(Math.Round(Math.Sqrt(q2), 4));
                label17.Text = Convert.ToString(Math.Round(excess(list), 4));
                label28.Text = Convert.ToString(0);
                label19.Text = Convert.ToString(Math.Round(asymmetry(list), 4));
                label30.Text = Convert.ToString(0);
                label21.Text = Convert.ToString(Math.Round(minimum(list), 4));
                label23.Text = Convert.ToString(Math.Round(maximum(list), 4));
                gist();
                labelmake(true);
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
