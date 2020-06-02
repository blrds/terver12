using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class funсtions
    {

        private Random random = new Random();
        private double q, m;
        public double Q {
            get { return q; }
            set { q = value; }
        }

        public double M
        {
            get { return m; }
            set { m = value; }
        }

        public double fx(double x)
        {
            return Math.Exp(-Math.Pow(Math.Log(x) - m, 2) / (2 * q * q)) / (x * q * Math.Sqrt(2 * Math.PI));
        }

        public double aver(List<myPoint> list)
        {
            double aver = 0;
            for (int i = 0; i < list.Count; i++)
                aver += list[i].x;
            aver /= list.Count;
            return aver;
        }

        public double mode(List<myPoint> list)
        {
            double mode;
            myPoint m = new myPoint();
            list.Sort(m.ComparisonbyY);
            mode = list[list.Count - 1].x;
            list.Sort(m.ComparisonbyX);
            return mode;
        }

        public double median(List<myPoint> list)
        {
            double median;
            if (list.Count % 2 == 1)
                median = list[list.Count / 2].x;
            else
                median = 0.5 * (list[list.Count / 2 - 1].x + list[list.Count / 2].x);
            return median;
        }

        public double dispersion(List<myPoint> list)
        {
            double dispersion = 0, av = aver(list);
            for (int i = 0; i < list.Count; i++)
                dispersion += Math.Pow((list[i].x - av), 2);
            dispersion /= list.Count;
            return dispersion;
        }

        public double standart(List<myPoint> list)
        {
            double standart;
            standart = Math.Sqrt(dispersion(list));
            return standart;
        }

        public double expect(List<myPoint> list)
        {
            double expect = 0;
            for (int i = 0; i < list.Count; i++)
                expect += list[i].x * list[i].y;
            return expect;
        }

        public double excess(List<myPoint> list)
        {
            double excess = 0, ex = expect(list);
            for (int i = 0; i < list.Count; i++)
                excess += (Math.Pow((list[i].x - ex), 4) * list[i].y);
            excess = (excess / Math.Pow(dispersion(list), 2)) - 3;
            return excess;
        }

        public double asymmetry(List<myPoint> list)
        {
            double asymmetry = 0, ex = expect(list);
            for (int i = 0; i < list.Count; i++)
                asymmetry += Math.Pow((list[i].x - ex), 3) * list[i].y;
            asymmetry /= (Math.Pow(Math.Sqrt(dispersion(list)), 3));
            return asymmetry;
        }

        public double minimum(List<myPoint> list)
        {
            double minimum;
            minimum = list[0].x;
            return minimum;
        }

        public double maximum(List<myPoint> list)
        {
            double maximum;
            maximum = list[list.Count - 1].x;
            return maximum;
        }

        public void generate(double from, double to, out double K1, out double K2)
        {
            double k1, k2, x1, x2;
            do
            {

                k1 = random.NextDouble();
                k2 = random.NextDouble();
                x1 = from + (to - from) * k1;
                x2 = k2 * fx(Math.Exp(m - q * q));
            } while (x2 > fx(x1));
            K1 = x1;
            K2 = x2;
        }
    }
}
