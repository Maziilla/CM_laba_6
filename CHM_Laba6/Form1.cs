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
        const double E = 1E-8; //Точность
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
            int kol_obr = 0;
            double res = 0,xi,M2,M2_temp,Exit,res_runge_1=0,res_runge_2=0,poradok=0;
            double h = (b - a) / (N * 2);            
            do
            {
                kol_obr = 0;
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;
                h = (double)(b - a) / N;
                M2 = Math.Abs(f_derivative_2(a));
               
                res += f(a) + f(b);
                kol_obr += 2;
                for (int i = 1; i < N; i++)
                {
                    xi = a + h * i;
                    res += 2 * f(xi);
                    kol_obr++;
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
            strList.Add(" Общее число обращений к подынтергатльной функции = " + kol_obr);
        }
        public void KvadraturTrapecModif()
        {
            strList.Add("Квадратурное приближение модифицированным методом трапеций");
            N = 2;
            int kol_obr = 0;
            double res = 0, xi, M4, M4_temp, Exit, res_runge_1 = 0, res_runge_2 = 0, poradok = 0;
            double h = (b - a) / (N * 2);
            do
            {
                kol_obr = 0;
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;
                h = (double)(b - a) / N;  
                res += (f(a) + f(b))/2;
                kol_obr += 2;
                M4 = Math.Abs(f_derivative_4(a));
                for (int i = 1; i < N; i++)
                {
                    xi = a + h * i;
                    res += f(xi);
                    kol_obr++;
                    M4_temp = Math.Abs(f_derivative_4(xi));
                    if (M4_temp > M4)
                        M4 = M4_temp;
                }
                res *= h;
                res += h * h / 12 * (f_derivative(a) - f_derivative(b));
                kol_obr += 2;
                Exit = M4 * Math.Pow(h, 4) * (b - a) / 2880;
                strList.Add("N = " + N + " Интеграл равен " + res + " Погрешность = " + Exit);
            } while (Exit > E);
            //Для Ранге
            res_runge_1 += (f(a) + f(b)) / 2;
            res_runge_2 += (f(a) + f(b)) / 2;
            for (int i = 1; i < N - 1; i++)
            {

                res_runge_1 += f(a + 0.5 * h * i);
                res_runge_2 += f(a + 0.25 * h * i);
            }
            res_runge_1 *= h;
            res_runge_2 *= h;
            res_runge_1 += h * h / 12 * (f_derivative(a) - f_derivative(b));
            res_runge_2 += h * h / 12 * (f_derivative(a) - f_derivative(b));
            poradok = Math.Log((res_runge_2 - res) / (res_runge_1 - res) - 1) / Math.Log(0.5);
            strList.Add("Порядок аппроксимации равен = " + poradok);
            strList.Add(" Общее число обращений к подынтергатльной функции = " + kol_obr);

        }
        
        //Решение уравнения
        private void SolveButton_Click(object sender, EventArgs e)
        {

            //KvadraturTrapec();
            KvadraturTrapecModif();
            SaveFile();
        }

       
    }
}
