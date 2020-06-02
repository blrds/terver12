using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        List<myPoint> list = new List<myPoint> { };

        public sealed class myPointMap : ClassMap<myPoint>
        {
            public myPointMap()
            {
                Map(m => m.x).Index(0);
                Map(m => m.y).Index(1);
            }
        }

        funсtions f = new funсtions();

        public Form1()
        {
            InitializeComponent();
        }

        private String openFileName, saveFileName;

        public static void Log(string message)
        {
            File.AppendAllText("log.txt", message + "\n");
        }

        private void drawGraph(double from, double to)
        {
            const double MOVE = 0.05;
            double x = from;
            this.chart1.Series[0].Points.Clear();
            while (x <= to)
            {
                this.chart1.Series[0].Points.AddXY(x, f.fx(x));
                x += MOVE;
            }
            this.chart1.Update();
        }

        private void drawPoints(List<myPoint> list)
        {
            this.chart1.Series[1].Points.Clear();
            foreach (myPoint mp in list)
            {
                this.chart1.Series[1].Points.AddXY(mp.x, mp.y);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Log("bt1_cl " + DateTime.Now + "\n");
            list.Clear();
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[2].Points.Clear();
            double from=0, to=0;
            int count = 0;
            try
            {
                from = Convert.ToDouble(this.textBox1.Text);
                to = Convert.ToDouble(this.textBox2.Text);
                f.M = Convert.ToDouble(this.textBox4.Text);
                f.Q = Convert.ToDouble(this.textBox5.Text);
                count = Convert.ToInt16(this.textBox3.Text);
            }
            catch (Exception) {
                MessageBox.Show("Введите корректные данные","Ошибка данных",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            double x = from, X=0, Y=0;
            bool toFile = this.checkBox1.Checked;
            try
            {
                if (File.Exists(saveFileName)) File.Delete(saveFileName);
            }
            catch (IOException)
            {
                MessageBox.Show("Закройте файл для записи", "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Log("file cleared " + DateTime.Now + "\n");
            drawGraph(from, to);
            Log("graph " + DateTime.Now + "\n");
            this.chart1.Series[1].Points.Clear();
            for (int i = 0; i < count; i++)
            {
                f.generate(from,to,out X,out Y);
                Log("generated");
                list.Add(new myPoint() { x = X, y = Y });
            }
            drawPoints(list);
            Log("points " + DateTime.Now + "\n");
            if (toFile)
            {

                using (StreamWriter streamWriter = new StreamWriter(saveFileName))
                {

                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        try
                        {
                            csvWriter.Configuration.Delimiter = ";";
                            csvWriter.WriteRecords(list);
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Закройте файл для записи", "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }

            }
            Log("wroted " + DateTime.Now + "\n");
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

        private void labelmake(bool b)
        {
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
            label34.Visible = b;
            label35.Visible = b;
            label36.Visible = b;
            label37.Visible = b;
            label38.Visible = b;
            label39.Visible = b;
            label40.Visible = b;
            label41.Visible = b;
            label42.Visible = b;
            label43.Visible = b;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            comboBox1.SelectedItem = comboBox1.Items[0];
            try
            {
                File.Delete("log.txt");
                Log("start " + DateTime.Now + "\n");
            }
            catch (Exception err) { };
            labelmake(false);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Log("bt2_cl " + DateTime.Now + "\n");
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[2].Points.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                openFileName = openFileDialog.FileName;
                using (StreamReader streamReader = new StreamReader(openFileName))
                {
                    using (CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        try
                        {
                            csvReader.Configuration.Delimiter = ";";
                            csvReader.Configuration.RegisterClassMap<myPointMap>();
                            while (csvReader.Read())
                                list.Add(csvReader.GetRecord<myPoint>());
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Закройте считываемый файл", "Ошибка чтения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
            }

            drawPoints(list);
        }

        private void gist()
        {
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[2].Points.Clear();
            int groups = 0;
            if (comboBox1.SelectedItem == comboBox1.Items[0])
                groups = (int)Math.Floor(1 + 3.322*Math.Log(list.Count));
            else
                if (comboBox1.SelectedItem == comboBox1.Items[1])
                    groups =1+(int)Math.Floor(Math.Log(list.Count,2));
                else
                    groups =(int)Math.Floor(Math.Sqrt(list.Count));
            groups *= (int)numericUpDown1.Value;
            double move = (list[list.Count - 1].x - list[0].x) / groups;
            double cur = list[0].x;
            int count;
            for (int i = 0; i < groups; i++)
            {
                count = 0;
                for (int j = 0; j < list.Count; j++)
                    if (list[j].x >= cur && list[j].x < (cur + move)) count++;
                this.chart1.Series[2].Points.AddXY((cur + cur + move) / 2, count);
                cur += move;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Log("bt3_cl " + DateTime.Now + "\n");
            int i = 1;
            if (this.textBox4.Text == "")
            {
                MessageBox.Show("Введите мат.ожидание выборки", "Некоректные данные", MessageBoxButtons.OK, MessageBoxIcon.Error);
                i *= 0;
            }
            if (this.textBox5.Text == "")
            {
                MessageBox.Show("Введите среднеквадратическое отклонение выбоорки", "Некоректные данные", MessageBoxButtons.OK, MessageBoxIcon.Error);
                i *= 0;
            }
            if (list.Count != 0 && i != 0)
            {
                f.Q = Convert.ToDouble(textBox5.Text);
                f.M = Convert.ToDouble(textBox4.Text);
                double q2 = f.Q * f.Q, m2 = f.M * f.M;
                myPoint m = new myPoint();
                list.Sort(m.ComparisonbyX);
                label7.Text = Convert.ToString(Math.Round(f.aver(list), 4));
                label24.Text = textBox4.Text;
                label37.Text = Convert.ToString(Math.Round(Math.Exp((f.M + q2) / 2),4));
                label9.Text = Convert.ToString(Math.Round(f.mode(list), 4));
                label25.Text = Convert.ToString(f.M);
                label38.Text = Convert.ToString(Math.Round(Math.Exp(f.M - q2),4));
                label11.Text = Convert.ToString(Math.Round(f.median(list), 4));
                label26.Text = Convert.ToString(f.M);
                label39.Text = Convert.ToString(Math.Round(Math.Exp(f.M),4));
                label13.Text = Convert.ToString(Math.Round(f.dispersion(list), 4));
                label27.Text = Convert.ToString(Math.Round(q2, 4));
                label40.Text = Convert.ToString(Math.Round(((Math.Exp(q2) - 1) * Math.Exp(2 * f.M + q2)), 4));
                label15.Text = Convert.ToString(Math.Round(f.standart(list), 4));
                label29.Text = Convert.ToString(Math.Round(Math.Sqrt(q2), 4));
                label42.Text = Convert.ToString(Math.Round(Math.Sqrt(((Math.Exp(q2) - 1) * Math.Exp(2 * f.M + q2))), 4));
                label17.Text = Convert.ToString(Math.Round(f.excess(list), 4));
                label28.Text = Convert.ToString(0);
                label41.Text = Convert.ToString(Math.Round(Math.Exp(4 * q2) + 2 * Math.Exp(3 * q2) + 3 * Math.Exp(2 * q2) - 6, 4));
                label19.Text = Convert.ToString(Math.Round(f.asymmetry(list), 4));
                label30.Text = Convert.ToString(0);
                label43.Text = Convert.ToString(Math.Round((Math.Exp(q2) + 2) * Math.Sqrt(Math.Exp(q2) - 1), 4));
                label21.Text = Convert.ToString(Math.Round(f.minimum(list), 4));
                label23.Text = Convert.ToString(Math.Round(f.maximum(list), 4));
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
                else
                    checkBox1.Checked = false;
            }

        }
    }
}
