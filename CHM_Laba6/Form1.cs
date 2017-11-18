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
        public  int  N=5; //Размерности
        public double[] solution,RightPart; //Для решения
        public int[] kol;
        public double[,] MatrForM, MateForSpline;
        const double E = 1E-27; //Точность
        public double chis;
        List<string> strList = new List<string>(); 

        //Вывод вектора
      
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
        public double[] ProgonkaSLAU(/*double [,] Matr,*/double[] Right)
        {
            double[] res = new double[N-1], a = new double[N - 1], B = new double[N - 1];
            int N1 = N - 2;
            double y;
            y = 4;
            a[0] = -1 / y;
            B[0] = Right[0] / y;
            for (int i = 1; i < N1; i++)
            {
                y =4 + 1 * a[i - 1];
                a[i] = -1 / y;
                B[i] = (Right[i] - 1 * B[i - 1]) / y;
            }
            B[N1] = (Right[N1] - 1 * B[N1 - 1]) / (4 + 1 * a[N1 - 1]);
            res[N1] = (Right[N1] - 1 * B[N1 - 1]) / (4 +1 * a[N1 - 1]);
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
            double res = 0,xi,M2,M2_temp,Exit,res_runge_1=0,res_runge_2=0,poradok=0;
            double h = (b - a) / (N * 2);            
            do
            {
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;
                h = (double)(b - a) / N;
                M2 = Math.Abs(f_derivative_2(a));
               
                res += f(a) + f(b);
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
            //Для Ранге
            res_runge_1 += f(a) + f(b);
            res_runge_2 += f(a) + f(b);
            for (int i = 1; i < N; i++)
            {
                xi = a + h * i;
                res_runge_1 +=2* f(a + 0.5 * h * i);
                res_runge_2 +=2* f(a + 0.25 * h * i);                
            }
            res_runge_1 *= (double)(b - a) / (2 * N);
            res_runge_2 *= (double)(b - a) / (2 * N);
            poradok = Math.Log((res_runge_2 - res) / (res_runge_1 - res) - 1) / Math.Log(0.5);
            strList.Add("Порядок аппроксимации равен = " + poradok);
        }
        public void KvadraturTrapecModif()
        {
            strList.Add("Квадратурное приближение модифицированным методом трапеций");
            N = 2;
            double res = 0, xi, M4, M4_temp, Exit, R, res_runge_1 = 0, res_runge_2 = 0, poradok = 0;
            double h = (b - a) / (N * 2);
            do
            {
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;
                h = (double)(b - a) / N;              
                //сплайны начало
                MateForSpline = new double[N + 1, 3];
                //MatrForM = new double[N - 1, N - 1];
                double nu = h / (2 * h), lambda = nu;
                for (int i = 0; i <= N; i++)
                {
                    MateForSpline[i, 0] = a + (i * h);
                    MateForSpline[i, 1] = f(MateForSpline[i, 0]);
                }
                //for (int i = 0; i < N - 1; i++)
                //{
                //    MatrForM[i, i] = 4;
                //    if (i - 1 >= 0) MatrForM[i, i - 1] = 1;
                //    if (i + 1 < N - 1) MatrForM[i, i + 1] = 1;
                //}
                RightPart = new double[N - 1];
                for (int i = 1; i < N; i++)
                {
                    RightPart[i - 1] = 3 * (MateForSpline[i + 1, 1] - MateForSpline[i - 1, 1]) / h;
                }
                RightPart[0] -= f_derivative(MateForSpline[0, 0]);
                RightPart[N-2] -= f_derivative(MateForSpline[N, 0]);
                var answer = ProgonkaSLAU(/*MatrForM, */RightPart);
                for (int i = 1; i < N; i++)
                    MateForSpline[i, 2] = answer[i - 1];
                MateForSpline[0, 2] = f_derivative(MateForSpline[0, 0]);
                MateForSpline[N, 2] = f_derivative(MateForSpline[N, 0]);
                Func<double, double> z0 = x => (1 + 2 * x) * (1 - x) * (1 - x);
                Func<double, double> z1 = x => x * (1 - x) * (1 - x);
               
                //сплайны конец
                res += (f(a) + f(b))/2;
                R = 0;
                M4 = Math.Abs(f_derivative_4(a));
                for (int i = 1; i < N-1; i++)
                {
                    res += MateForSpline[i, 1];
                    R += MateForSpline[i + 1, 2];
                    xi = a + h * i;                    
                    M4_temp = Math.Abs(f_derivative_4(xi));
                    if (M4_temp > M4)
                        M4 = M4_temp;
                }
                res = (res - R * h * h / 6.0) * h;
                Exit = M4 * Math.Pow(h, 4) * (b - a) / 2880;
                strList.Add("N = " + N + " Интеграл равен " + res + " Погрешность = " + Exit);
            } while (Exit > E);
            //Для Ранге
            res_runge_1 += (f(a) + f(b))/2;
            R = 0;
            res_runge_2 += (f(a) + f(b))/2;
            for (int i = 1; i < N-1; i++)
            {
               
                res_runge_1 += f(a + 0.5 * h * i);
                R += MateForSpline[i + 1, 2];
                res_runge_2 += f(a + 0.25 * h * i);
            }
            res_runge_1 *= (res_runge_1 - R * h * h / 6.0) * h; ;
            res_runge_2 *= (res_runge_2 - R * h * h / 6.0) * h; ;
            poradok = Math.Log((res_runge_2 - res) / (res_runge_1 - res) - 1) / Math.Log(0.5);
            strList.Add("Порядок аппроксимации равен = " + poradok);
        }
        //Метод кубических сплайнов 1го дефекта
       /* public void CubSplain()
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
           
            for (int i = 0; i <= n; i++)
                    MateForSpline[i, 1] = f_derivative(MateForSpline[i, 0]);
          

                 
            strList.Add("       х       |      f(x)      |    S31(f;x)   |   Реальная   |       Оценка");
            WriteMas_(splains, n, n);
        }
        */
        //Решение уравнения
        private void SolveButton_Click(object sender, EventArgs e)
        {

           // KvadraturTrapec();
            KvadraturTrapecModif();
            SaveFile();
        }

       
    }
}
