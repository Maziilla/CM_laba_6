using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace SLAU
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SolveButton.Enabled = true;
        }
        public const int a = 1, b = 2;
        public const int  n=5; //Размерности
        public double[,] A = new double[n+1 , n + 2]; //Исходная, единичная       
        public double[] solution,RightPart; //Для решения
        public int[] kol;
        public double[,] MatrForM = new double[n - 1, n - 1], MateForSpline = new double[n + 1, n + 1];
        const double E = 1E-8; //Точность
        public double chis;
        List<string> strList = new List<string>(); 

        //Вывод вектора
        public void Write(double[] vec)
        {
            for (int i = 0; i < n-1; i++)
            {
                string str1 = "";
                str1 = String.Format("{0,-18}", vec[i]);
                strList.Add(str1);
            }
            strList.Add("");
        }

        //Выводим матрицу
        public void WriteMas(double[,] Mas)
        {
            strList.Add("");
            for (int i = 0; i <= n; i++)
            {
                string str1 = "";
                for (int j = 0; j < n+2; j++)
                    str1 += String.Format("{0,-15} ", Mas[i, j]);
                strList.Add(str1);
            }
            strList.Add("");
        }
        public void WriteMas_(double[,] Mas,int a,int b)
        {
            strList.Add("");
            for (int i = 0; i < a; i++)
            {
                string str1 = "";
                for (int j = 0; j < b ; j++)
                    str1 += String.Format("{0,-15} ", Mas[i, j]);
                strList.Add(str1);
            }
            strList.Add("");
        }
        //Сохраняем решение в файл
        public void SaveFile()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Текстовый(*.txt)|*.txt";
            saveFile.DefaultExt = "txt";
            saveFile.Title = "Сохранение решения";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveFile.FileName, FileMode.Create);
                StreamWriter st = new StreamWriter(fs);
                foreach (string str in strList)
                {
                        st.WriteLine(str);
                }
                st.Close();
            }
            SolveBox.Items.Add("Решение сохранено в " + saveFile.FileName);
            strList.Clear();
        }       

       

        public double [] Raund(double[] vec)
        {
            for (int i = 0; i < n; i++)
                vec[i] = Math.Round(vec[i], 4);
            return vec;
        }        

        //Нахождение нормы вектора (max)
        public double Norma(double[] vec)
        {
            double max = 0;
            for (int i = 0; i < n; i++)
                if (max < Math.Abs(vec[i]))
                    max = Math.Abs(vec[i]);
            return max;
        }

        //Для функции
        public double f(double x)
        {
            if(rb_13.Checked)
                return Math.Pow(3, x) + 2 * x - 5;            
            return Math.Atan(x) + 2 * x - 1;
        }

        public double f_derivative(double x)
        {
            if (rb_13.Checked)
                return (Math.Pow(3, x) * Math.Log(3) + 2);            
            return (1 / (x * x + 1) + 2);
        }
        public double f_derivative_2(double x)
        {
            if (rb_13.Checked)
                return (Math.Pow(3, x) * Math.Pow(Math.Log(3), 2));
            return (2*x/Math.Pow((x*x+1),2));
        }
        //d^4/dx^4 arccotx+2x-1 для вольфрама
        public double f_derivative_4(double x)
        {
            if (rb_13.Checked)
                return (Math.Pow(3, x) * Math.Pow(Math.Log(3), 4));            
            return (24 * x * (x * x - 1) / Math.Pow((x * x + 1), 4));
        }
        public double f_derivative_5(double x)
        {
            if (rb_13.Checked)
                return (Math.Pow(3, x) * Math.Pow(Math.Log(3), 5));
            return (-24 * (5 * x * x * x * x - 10 * x * x + 1) / Math.Pow((x * x + 1), 5));
        }
        public double f_derivative_6(double x)
        {
            if (rb_13.Checked)
                return (Math.Pow(3, x) * Math.Pow(Math.Log(3),6));
            return (240 * x * (3 * x * x * x * x - 10 * x * x + 3) / Math.Pow((x * x + 1), 6));
        } 
        public long Factorial(int n)
        {
            long temp = 1;
            for (int i = 1; i <= n; i++)
                temp *= i;
            return temp;
        }        
        public double[] ProgonkaSLAU(double [,] Matr,double[] Right)
        {
            double[] res = new double[n-1], a = new double[n - 1], B = new double[n - 1];
            int N1 = n - 2;
            double y;
            y = Matr[0,0];
            a[0] = -Matr[0,1] / y;
            B[0] = Right[0] / y;
            for (int i = 1; i < N1; i++)
            {
                y = Matr[i,i] + Matr[i,i -1] * a[i - 1];
                a[i] = -Matr[i,i + 1] / y;
                B[i] = (Right[i] - Matr[i,i-1] * B[i - 1]) / y;
            }
            B[N1] = (Right[N1] - Matr[N1, N1 - 1] * B[N1 - 1]) / (Matr[N1, N1] + Matr[N1, N1 - 1] * a[N1 - 1]);
            res[N1] = (Right[N1] - Matr[N1,N1 - 1] * B[N1 - 1]) / (Matr[N1,N1] + Matr[N1,N1 - 1] * a[N1 - 1]);
            for (int i = N1 - 1; i >= 0; i--)
            {
                res[i] = a[i] * res[i + 1] + B[i];
            }
            return res;
        }
        //трапеции
        public void KvadraturTrapec()
        {
            strList.Add("Квадратурное приближение методом трапеций");
            int N = 1;
            double res = 0,xi,M2,M2_temp,Exit;
            double h = (b - a) / (N * 2);            
            do
            {                
                N *= 2;
                h = (double)(b - a) / N;
                M2 = Math.Abs(f_derivative_2(a));
                res += f(a);
                for (int i = 1; i < N; i++)
                {
                    xi = a + h * i;
                    res += 2 * f(xi);
                    M2_temp = Math.Abs(f_derivative_2(xi));
                    if (M2_temp > M2)
                        M2 = M2_temp;
                }
                res *= (double)(b - a) / (2 * N);
                Exit = M2 * Math.Pow((b - a), 3) / (12 * N);
                strList.Add("N = "+ N +" Интеграл равен "+ res + " Погрешность = " + Exit);
            } while (Exit > E);
            

        }
        //Метод кубических сплайнов 1го дефекта
        public void CubSplain()
        {
            strList.Add("Метод кубических сплайнов 1го дефекта");
            double[,] splains = new double[n, n];
            double h = (double)(b - a) / n;
            double nu = h / (2 * h), lambda = nu;
            for (int i = 0; i <= n; i++)
                MateForSpline[i, 0] = a + (i * h);
            for (int i = 0; i <= n; i++)               
                MateForSpline[i, 1] = f(MateForSpline[i, 0]);                
            for (int i = 0; i < n - 1; i++)
            {
                MatrForM[i, i] = 4;
                if (i - 1 >= 0) MatrForM[i, i-1] = 1;
                if (i + 1 < n - 1) MatrForM[i, i+1] = 1;
            }
            strList.Add("Трёхдиагональная матрица");
            WriteMas_(MatrForM, 4, 4);
            RightPart = new double[n - 1];
            for (int i = 1; i < n; i++)
            {
                RightPart[i - 1] = 3 * (MateForSpline[i + 1, 1] - MateForSpline[i - 1, 1]) / h;
            }            
            RightPart[0] -= f_derivative(MateForSpline[0, 0]);
            RightPart[3] -= f_derivative(MateForSpline[n, 0]);            
            strList.Add("Правая часть:");
            Write(RightPart);
            var answer = ProgonkaSLAU(MatrForM, RightPart);
            //Write(answer);
            for (int i = 1; i < n; i++)
                MateForSpline[i,2] = answer[i - 1];
            double M5,M4;
            double temp;           
            MateForSpline[0, 2] = f_derivative(MateForSpline[0, 0]);
            MateForSpline[n, 2] = f_derivative(MateForSpline[n, 0]);
            M5 = Math.Abs(f_derivative_5(a));
            M4 = Math.Abs(f_derivative_4(a));
            for (double i = a + h; i <= b; i += h)
            {
                temp = Math.Abs(f_derivative_5(i));
                if (temp > M5)
                    M5 = temp;
                temp = Math.Abs(f_derivative_4(i));
                if (temp > M4)
                    M4 = temp;
            }
            for (int i = 0; i <= n; i++)
                    MateForSpline[i, 1] = f_derivative(MateForSpline[i, 0]);
            strList.Add(" M5 =  " + M5);
            strList.Add(" M4 =  " + M4);
            temp = M5 / 60 * h * h * h * h;
            for(int i=0;i<=n;i++)
            {
                MateForSpline[i, 3] = Math.Abs(MateForSpline[i, 1] - MateForSpline[i, 2]);
                MateForSpline[i, 4] = temp;
            }
            strList.Add("       х       |      f(x)      |      m[i]     |   Реальная   |       Оценка");
            WriteMas_(MateForSpline, n+1, n);

            for (int i = 0; i < n; i++)
            {
                splains[i, 0] = (a + 0.1) + h * i;
                splains[i, 4] = (M4 / 384 + M5 * h / 240) * h * h * h * h;
            }
            Func<double, double> z0 = x => (1 + 2 * x) * (1 - x) * (1 - x);
            Func<double, double> z1 = x => x * (1 - x) * (1 - x);           
            for (int i = 0; i < n; i++)
            {
                splains[i, 1] = f(splains[i, 0]);
                double t = (splains[i, 0] - MateForSpline[i, 0]) / h;
                splains[i, 2] = f(MateForSpline[i, 0]) * z0(t) + f(MateForSpline[i + 1, 0]) * z0(1 - t) + h * (z1(t) * MateForSpline[i, 3] + z1(1 - t) * MateForSpline[i + 1, 3]);
                splains[i, 3] = Math.Abs(splains[i, 1] - splains[i, 2]);
            }            
            strList.Add("       х       |      f(x)      |    S31(f;x)   |   Реальная   |       Оценка");
            WriteMas_(splains, n, n);
        }
        
        //Решение уравнения
        private void SolveButton_Click(object sender, EventArgs e)
        {

            KvadraturTrapec();
            
            SaveFile();
        }

       
    }
}
