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
        const double E = 1E-8; //Точность
        public double chis;
        List<string> strList = new List<string>();

        //Вывод вектора
        double f_14(double x)
        {
            return 2 * Math.Pow(Math.E,x) - 3 * x + 1;
        }
        double df_14(double x)
        {
            return -3 + 2 * Math.Pow(Math.E, x);
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
        public double f_Simpson(int a,int b,int n)
        {
            
            double res = 0,h=0;
            h = (double)(b - a) / n;

            res += f(a) + f(b);
            for (int i = 1; i <= n / 2; i++)
            {
                res += 4 * f(a + (2 * i - 1) * h);
                if (i < n / 2)
                    res += 2 * f(a + (2 * i) * h);
            }
            res *= h / 3;
            return res;
        }
        public double f_Trapec(int a,int b,int n)
        {
            double h = 0, res = 0;
            h = (double)(b - a) / n;

            res += 0.5 * (f(a) + f(b));           
            for (int i = 1; i < n; i++)
            {
                res += f(a + h * i);
            }
            res *= h;
            return res;
        }
        public double f_TrapecModif(int a, int b, int n)
        {
            double h = 0, res = 0;
            h = (double)(b - a) / n;
            res += (f(a) + f(b)) / 2;
           
            for (int i = 1; i < n; i++)
            {
                res += f(a + h * i);
            }
            res *= h;
            res += h * h / 12 * (f_derivative(a) - f_derivative(b));
            return res;
        }
        public double f_Gaus(int A, int B, int n)
        {
            double h = 0, res = 0,a=0,b=0;
            h = (double)(B - A) / n;          
            for (int i = 0; i < n; i++)
            {
                a = A + h * i;
                b = A + h * (i + 1);
                res += (b - a)/2 * 
                    (5.0 / 9 * f(0.5 * (a + b + (b - a) * - Math.Sqrt(3.0 / 5))) + 
                    8.0 / 9 * f(0.5 * (a + b)) +
                    5.0 / 9 * f(0.5 * (a + b + (b - a) * Math.Sqrt(3.0 / 5))));
            }            
            return res;
        }
        //трапеции
        public void KvadraturTrapec()
        {
            strList.Add("Квадратурное приближение методом трапеций");
            int N = 1;
            int kol_obr = 0;
            double res = 0,Exit,res_runge_1=0,res_runge_2=0,poradok=0;
            double h = (b - a) / (N * 2),h1=0;
            strList.Add("  N  " + "     Интеграл равен  " + "      Погрешность        " + "Порядок аппроксимации равен ");
            do
            {
                kol_obr = 0;
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;
                h = (double)(b - a) / N;
                kol_obr += (int)(Math.Pow(2, (int)Math.Log(N, 2))+1);
                res = f_Trapec(a, b, N);
                res_runge_1 = f_Trapec(a, b, 2 * N);
                res_runge_2 = f_Trapec(a, b, 4 * N); ;               
                Exit = Math.Abs(res_runge_1 - res) / res_runge_1;                
                poradok = Math.Log((res_runge_2 - res) / (res_runge_1 - res) - 1) / Math.Log(0.5);
                strList.Add(String.Format("|{0,4} |{1,21}|{2,21}|{3,21}|", N, res, Exit, poradok));
            } while (Exit > E);
            
            strList.Add(" Общее число обращений к подынтегральной функции = " + kol_obr);
        }
        public void KvadraturTrapecModif()
        {
            strList.Add("Квадратурное приближение модифицированным методом трапеций");
            N = 2;
            int kol_obr = 0;
            double res = 0, Exit, res_runge_1 = 0, res_runge_2 = 0, poradok = 0;
            double h = (b - a) / (N * 2),h1=0;
            strList.Add("  N  " + "     Интеграл равен  " + "      Погрешность        " + "Порядок аппроксимации равен ");
            do
            {
                kol_obr = 0;
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;
                h = (double)(b - a) / N;
                res = f_TrapecModif(a, b, N);
                res_runge_1 += f_TrapecModif(a, b, 2 * N);         
                res_runge_2 += f_TrapecModif(a, b, 4 * N); 
                poradok = Math.Log(Math.Abs((res_runge_2 - res) / (res_runge_1 - res) - 1)) / Math.Log(0.5);
                Exit = Math.Abs((res - res_runge_1)) / res_runge_1;
                kol_obr += (int)(Math.Pow(2, (int)Math.Log(N, 2)) + 1);
                strList.Add(String.Format("|{0,4} |{1,21}|{2,21}|{3,21}|", N, res, Exit, poradok));
            } while (Exit > E);
            strList.Add(" Общее число обращений к подынтегральной функции = " + kol_obr);

        }
        public void Simpson()
        {
            strList.Add("Квадратурное приближение методом Симпсона");
            N = 1;
            int kol_obr = 0;
            double res = 0, Exit, res_runge_1 = 0, res_runge_2 = 0, poradok = 0;
            double h = 0, h1 = 0;
            strList.Add("  N  " + "     Интеграл равен  " +  "      Погрешность        "  + "Порядок аппроксимации равен " );
            do
            {
                kol_obr = 0;
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;               
                res = f_Simpson(a,b,N);
                res_runge_1 = f_Simpson(a, b, 2 * N); ;
                Exit = Math.Abs((res - res_runge_1)) / res_runge_1;
                res_runge_2 = f_Simpson(a, b, 4 * N); 
                poradok = Math.Log(Math.Abs((res_runge_2 - res) / (res_runge_1 - res) - 1)) / Math.Log(0.5);
                strList.Add(String.Format("|{0,4} |{1,21}|{2,21}|{3,21}|", N, res, Exit, poradok));

            } while (Exit > E);
            kol_obr += (int)(Math.Pow(2, (int)Math.Log(N, 2)+1)) + 1;
            strList.Add(" Общее число обращений к подынтегральной функции = " + kol_obr);
        }
        public void Gaus()
        {
            strList.Add("Квадратурное приближение методом Гаусса");
            N = 1;
            int kol_obr = 0;
            double res = 0, Exit, res_runge_1 = 0, res_runge_2 = 0, poradok = 0;            
            strList.Add("  N  " + "     Интеграл равен  " + "      Погрешность        " + "Порядок аппроксимации равен ");
            do
            {
                res_runge_2 = res_runge_1 = res = 0;
                N *= 2;
                res = f_Gaus(a, b, N);
                res_runge_1 = f_Gaus(a, b, 2 * N); ;
                Exit = Math.Abs((res - res_runge_1)) / res_runge_1;
                res_runge_2 = f_Gaus(a, b, 4 * N);
                poradok = Math.Log(Math.Abs((res_runge_2 - res) / (res_runge_1 - res) - 1)) / Math.Log(0.5);
                strList.Add(String.Format("|{0,4} |{1,21}|{2,21}|{3,21}|", N, res, Exit, poradok));

            } while (Exit > E);
            kol_obr += N*3+3;
            strList.Add(" Общее число обращений к подынтегральной функции = " + kol_obr);
        }

        //Решение уравнения
        private void SolveButton_Click(object sender, EventArgs e)
        {
            KvadraturTrapec();
            KvadraturTrapecModif();
            Simpson();
            Gaus();
            SaveFile();
        }

       
    }
}
