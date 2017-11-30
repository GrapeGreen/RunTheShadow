using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Run_the_Shadow
{
    public partial class Form1 : Form
    {
        class PanelDB : Panel { public PanelDB() { this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true); } }

        int rball = 3; // радиус шарика
        int xball, yball;
        int v = 15; // v - скокрость (расстояние от центра шарика до курсора)
        int w = 20; // частота перерисовки
        int Formwidth, Formheight; //   размеры формы
        int gateX, gateY, gateWidht, gateHeight; // типа прямоугольничек, в который нужно попасть
        int[,] staticblocks = new int[0, 4]; //массив статичных блоков X, Y, Width, Height
        int[,] dynamicblocks = new int[0, 4]; // массив динамичных блоков X, Y, Width, Height
        int[,] opengate = new int[0, 3]; // массив шариков на которые надо нажать чтобы разблокировать ворота
        int[,] guards = new int[0, 10]; // стражи X, Y, D(const), D(again), x, y направления, число преломных точек, указатель в массиве path, v(скорость<>const), h(дальность<>const)
        int[,] path = new int[0, 100]; //записываем путь
        int[, ,] graghguard = new int[0, 0, 0];
        int win = -2; // количесто opengate на которые осталось нажать 
        bool keyleft = false; //нужно для управления клавишами
        bool keyright = false;
        bool keyup = false;
        bool keydown = false;
        bool touch = false; // касание шарика неподвижного блока
        int level;
        int numlevels = 15;
        int[] levelenable = new int[15];
        Button[] buttonlevel = new Button[15];

        public Form1()
        {
            InitializeComponent();
            //button1.Focus();
            button1.Visible = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            //buttonReturn.Visible = true;

            panel1.Location = new Point(buttonReturn.Location.X, label1.Location.Y + label1.Height + 5); // так чтобы панель не закрывала название
            panel1.Height = Height - panel1.Location.Y - 15*3; // множители вообще рандомные хочу панельку на весь экран
            panel1.Width = Width - 15 * 3;
            panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left));  //чтобы панель растягивалась всместе с формой


            //сотворю кнопочки уровней
            //int numlevels = 15; //число уровней
            //int[] levelenable = new int[numlevels]; // считываю массив активности уровней
            string line; //туда считываем
            int counter = 0; // типа сколько записали в массив чисел
            System.IO.StreamReader file = new System.IO.StreamReader("levelsOpen.kkk");
            while((line = file.ReadLine()) != null)  //вот тут представлен убогий способ заполнения числового массива, но это наиболее общий способ для заполнения массивов.
            {
                string[] split = line.Split(new Char[] { ' ' });
                foreach (string s in split)
                {
                    if (s.Trim() != "") //честно говоря тут проверка не нужна, но напишу для канона
                    {
                        if (counter >= numlevels)
                            break;
                        else
                        {
                            levelenable[counter] = int.Parse(s);
                            counter++;
                        }
                    }
                }
            }
            file.Close();
            int numrows = 5; // число строк в таблице уровней
            //Button[] buttonlevel = new Button[numlevels];
            for (int i = 0; i < buttonlevel.Length; i++)
            {
                buttonlevel[i] = new System.Windows.Forms.Button();
                buttonlevel[i].Location = new System.Drawing.Point(10 + 250*(i%numrows), 10 + 150*(i/numrows));
                buttonlevel[i].Name = "level" + (i+1).ToString();
                buttonlevel[i].Size = new System.Drawing.Size(250, 150);
                buttonlevel[i].TabIndex = i;
                buttonlevel[i].Text = "level " + (i+1).ToString();
                buttonlevel[i].Font = new Font(buttonlevel[i].Font.Name, 40, buttonlevel[i].Font.Style);
                buttonlevel[i].ForeColor = Color.Black;
                buttonlevel[i].BackColor = Color.WhiteSmoke;
                if (levelenable[i] == 1)
                    buttonlevel[i].Enabled = true;
                if (levelenable[i] == 0)
                    buttonlevel[i].Enabled = false;
                buttonlevel[i].Click += buttonlevel_Click;
                panel1.Controls.Add(buttonlevel[i]);
            }

            panel1.Visible = true;
        }

        private void buttonReturn_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            buttonReturn.Visible = false;
            button1.Visible = false;
            Finish();
        }

        private void buttonlevel_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            button1.Focus();

            //panellevel.Location = new Point(panel1.Location.X, panel1.Location.Y-40);
            //panellevel.Size = new Size(panel1.Size.Width, panel1.Size.Height+40);
            panellevel.Location = new Point(0, toolStrip1.Height);
            panellevel.Size = new Size(Width, Height);
            panellevel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left));
            panellevel.BackColor = Color.Black;
            Graphics graph = panellevel.CreateGraphics();
            //вывести panellevel на первый план
            panellevel.BringToFront();
            panellevel.Visible = true;
  
          
            // теперь пора прорисовывать сам уровень
            string pth;
            //pth = @System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath("Run the Shadow.sln")))))) + @"\Mpee\Mpe\bin\Debug\level1.sim";
            pth = @System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath("Run the Shadow.sln")))))) + @"\Mpee\Mpe\bin\Debug\" + (sender as Button).Name + ".sim";
            System.IO.StreamReader file = new System.IO.StreamReader(pth); // пока только один уровень надо прописать остальные и активироать следующую строчку взамен этой

            string[] split;

            string str = (sender as Button).Name;
            split = str.Split('l', 'e', 'v');
            level = int.Parse(split[5]);
            level--;
            //пока в файле только координаты шарика
            string line = file.ReadLine();
            if (line != null)
            {
                split = line.Split(' ');
                xball = int.Parse(split[0]);
                yball = int.Parse(split[1]);
                graph.DrawEllipse(Pens.White, xball - rball, yball - rball, 2 * rball, 2 * rball); //нет не буду пока с растягивание формы завязываться
            }

            //координаты шариков на которые надо нажать
            win = 0;
            line = file.ReadLine();
            if (line != null)
            {
                split = line.Split(' ');
                opengate = new int[split.Length / 2, 3];
                for (int i = 0; i < split.Length / 2; i++)
                {
                    opengate[i, 0] = int.Parse(split[2 * i]);
                    opengate[i, 1] = int.Parse(split[2 * i + 1]);
                    opengate[i, 2] = 10;
                    graph.FillEllipse(Brushes.Green, opengate[i, 0], opengate[i, 1], opengate[i, 2], opengate[i, 2]);
                    win++;
                }
            }

            //координаты калитки (выхода)
            line = file.ReadLine();
            if (line != null)
            {
                split = line.Split(' ');
                gateX = int.Parse(split[0]);
                gateY = int.Parse(split[1]);
                gateWidht = int.Parse(split[2]);
                gateHeight = int.Parse(split[3]);
                if (win == 0)
                    graph.FillRectangle(Brushes.White, gateX, gateY, gateWidht, gateHeight);
            }


            //теперь массив неподвижных блоков
            line = file.ReadLine();
            if (line != null)
            {
                split = line.Split(' ');
                staticblocks = new int[split.Length / 4, 4];
                for (int i = 0; i < split.Length / 4; i++)
                {
                    staticblocks[i, 0] = int.Parse(split[4 * i]);
                    staticblocks[i, 1] = int.Parse(split[4 * i + 1]);
                    staticblocks[i, 2] = int.Parse(split[4 * i + 2]);
                    staticblocks[i, 3] = int.Parse(split[4 * i + 3]);
                    graph.DrawRectangle(Pens.White, staticblocks[i, 0], staticblocks[i, 1], staticblocks[i, 2], staticblocks[i, 3]);
                }
            }


            //теперь массив ндинамичных блоков
            line = file.ReadLine();
            if (line != null)
            {
                split = line.Split(' ');
                dynamicblocks = new int[split.Length / 4, 4];
                for (int i = 0; i < split.Length / 4; i++)
                {
                    dynamicblocks[i, 0] = int.Parse(split[4 * i]);
                    dynamicblocks[i, 1] = int.Parse(split[4 * i + 1]);
                    dynamicblocks[i, 2] = int.Parse(split[4 * i + 2]);
                    dynamicblocks[i, 3] = int.Parse(split[4 * i + 3]);
                    graph.DrawRectangle(Pens.Blue, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);
                }
            }


            //старжи 
            line = file.ReadLine();
            if (line != null)
            {
                split = line.Split(' ');
                guards = new int[split.Length / 4, 10]; // пока ввожу только X и Y
                for (int i = 0; i < split.Length / 4; i++)
                {
                    guards[i, 0] = int.Parse(split[4 * i]);
                    guards[i, 1] = int.Parse(split[4 * i + 1]);
                    guards[i, 2] = 6;
                    guards[i, 3] = 6;
                    guards[i, 4] = guards[i, 0];
                    guards[i, 5] = guards[i, 1];
                    guards[i, 6] = int.Parse(split[4 * i + 2]); // с учетом начальной точки которая считывается
                    guards[i, 7] = 0;
                    guards[i, 8] = 7; // скорость
                    guards[i, 9] = int.Parse(split[4 * i + 3]);
                    graph.DrawEllipse(Pens.White, guards[i, 0] - guards[i, 9] + guards[i, 2] / 2, guards[i, 1] - guards[i, 9] + guards[i, 3] / 2, 2 * guards[i, 9], 2 * guards[i, 9]);
                    graph.DrawEllipse(Pens.Red, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                }
            }

            //считываю траектории
            path = new int[guards.GetLength(0), 100];
            for (int i = 0; i < path.GetLength(0); i++)
            {
                line = file.ReadLine();
                if (line != null)
                {
                    split = line.Split(' ');
                    for (int j = 0; j < split.Length / 2; j++)
                    {
                        path[i, 2 * j] = int.Parse(split[2 * j]);
                        path[i, 2 * j + 1] = int.Parse(split[2 * j + 1]);
                    }
                } 
            }

            graghguard = new int[guards.GetLength(0), panellevel.Width + 80, panellevel.Height + 80];
            for (int k = 0; k < guards.GetLength(0); k++)
                Fillmap(k, ref graghguard);

            file.Close();

            //включить таймер
            button1.Focus();
            timer1.Enabled = true;
            timer1.Interval = w;
            timer2.Interval = 40;
            timer2.Enabled = true;
        }

        private void Fillmap(int n, ref int[, ,] map)
        {
            //for (int i = 0; i < map.GetLength(1); i++)
              //  for (int j = 0; j < map.GetLength(2); j++)
                //    map[n, i, j] = 0;
            Array.Clear(map, map.GetLength(1) * map.GetLength(2) * n, map.GetLength(1) * map.GetLength(2));

            for (int g = 0; g < staticblocks.GetLength(0); g++)
                for (int i = staticblocks[g, 0] + 40 - guards[n, 2]; i < staticblocks[g, 0] + staticblocks[g, 2] + 40 + guards[n, 2]; i++)
                    for (int j = staticblocks[g, 1] + 40 - guards[n, 3]; j < staticblocks[g, 1] + staticblocks[g, 3] + 40 + guards[n, 3]; j++)
                        map[n, i, j] = -1;
            for (int g = 0; g < dynamicblocks.GetLength(0); g++)
                for (int i = dynamicblocks[g, 0] + 40 - guards[n, 2]; i < dynamicblocks[g, 0] + dynamicblocks[g, 2] + 40 + guards[n, 2]; i++)
                    for (int j = dynamicblocks[g, 1] + 40 - guards[n, 3]; j < dynamicblocks[g, 1] + dynamicblocks[g, 3] + 40 + guards[n, 3]; j++)
                        map[n, i, j] = -1;
            if (win == 0)
                for (int i = gateX + 40 - guards[n, 2]; i < gateX + gateWidht + 40 + guards[n, 2]; i++)
                    for (int j = gateY + 40 - guards[n, 3]; j < gateY + gateHeight + 40 + guards[n, 3]; j++)
                        map[n, i, j] = -1;
            for (int j = guards[n, 0] + 40 - 3; j < guards[n, 0] + 40 + guards[n, 2] + 3; j++)
                for (int g = guards[n, 1] + 40 - 3; g < guards[n, 1] + 40 + guards[n, 3] + 3; g++)
                    graghguard[n, j, g] = 0;
            for (int j = xball - rball + 40 - 3; j < xball + rball + 40 + 3; j++)
                for (int g = yball - rball + 40 - 3; g < yball + rball + 40 + 3; g++)
                    graghguard[n, j, g] = 0;
        }


        private void Fillmappart(int n, ref int[, ,] map)
        {
            for (int i = 0; i < map.GetLength(1); i++)
                for (int j = 0; j < map.GetLength(2); j++)
                    map[n, i, j] = 0;
            for (int g = 0; g < staticblocks.GetLength(0); g++)
                for (int i = staticblocks[g, 0] + 40 - guards[n, 2]; i < staticblocks[g, 0] + staticblocks[g, 2] + 40 + guards[n, 2]; i++)
                    for (int j = staticblocks[g, 1] + 40 - guards[n, 3]; j < staticblocks[g, 1] + staticblocks[g, 3] + 40 + guards[n, 3]; j++)
                        map[n, i, j] = -1;
            for (int g = 0; g < dynamicblocks.GetLength(0); g++)
                for (int i = dynamicblocks[g, 0] + 40 - guards[n, 2]; i < dynamicblocks[g, 0] + dynamicblocks[g, 2] + 40 + guards[n, 2]; i++)
                    for (int j = dynamicblocks[g, 1] + 40 - guards[n, 3]; j < dynamicblocks[g, 1] + dynamicblocks[g, 3] + 40 + guards[n, 3]; j++)
                        map[n, i, j] = -1;
            if (win == 0)
                for (int i = gateX + 40 - guards[n, 2]; i < gateX + gateWidht + 40 + guards[n, 2]; i++)
                    for (int j = gateY + 40 - guards[n, 3]; j < gateY + gateHeight + 40 + guards[n, 3]; j++)
                        map[n, i, j] = -1;
            for (int j = guards[n, 0] + 40 - 3; j < guards[n, 0] + 40 + guards[n, 2] + 3; j++)
                for (int g = guards[n, 1] + 40 - 3; g < guards[n, 1] + 40 + guards[n, 3] + 3; g++)
                    graghguard[n, j, g] = 0;
            for (int j = xball - rball + 40 - 3; j < xball + rball + 40 + 3; j++)
                for (int g = yball - rball + 40 - 3; g < yball + rball + 40 + 3; g++)
                    graghguard[n, j, g] = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics graph = panellevel.CreateGraphics();

            //по идее сдесь буду перерисовывать поле при изменении размеров формы


            //управление шариком с помощью мыши слегка недописанное тут можно еще пару строк написать и будет с помощью клавиш потом напишу
            int xMouse = MousePosition.X; // координаты мыши в системе отсчета panellevel
            int yMouse = MousePosition.Y - toolStrip1.Height;


            //клавишами
            if (keyleft == true && keyright == false)
            {
                xMouse = xball - v / 3 + 1;
                if (Math.Abs(yMouse - yball) > v)
                    yMouse = yball;
            }
            if (keyleft == false && keyright == true)
            {
                xMouse = xball + v / 3 - 1;
                if (Math.Abs(yMouse - yball) > v)
                    yMouse = yball;
            }
            if (keyup == true && keydown == false)
            {
                yMouse = yball - v / 3 + 1;
                if (Math.Abs(xMouse - xball) > v)
                    xMouse = xball;
            }
            if (keyup == false && keydown == true)
            {
                yMouse = yball + v / 3 - 1;
                if (Math.Abs(xMouse - xball) > v)
                    xMouse = xball;
            }

       
            
            if (Math.Abs(xMouse - xball) <= v && Math.Abs(yMouse - yball) <= v && (xMouse - rball)*(panellevel.Width+1-rball-xMouse) >= 0 && (yMouse-rball)*(panellevel.Height + 1-rball-yMouse) >= 0)
            {
                graph.DrawEllipse(Pens.Black, xball - rball, yball - rball, 2 * rball, 2 * rball);
                xball = xball - rball; // тут просто хочу чтобы шарик "выглядел" так же как и dynamicblock
                yball = yball - rball;
                xMouse = xMouse - rball;
                yMouse = yMouse - rball;
                bool flagtouch = true;
                
                CheckMove(xMouse, yMouse, ref xball, ref yball, 2*rball, 2*rball, ref flagtouch);

                xball = xball + rball;
                yball = yball + rball;
                xMouse = xMouse + rball;
                yMouse = yMouse + rball;
                graph.DrawEllipse(Pens.White, xball - rball, yball - rball, 2 * rball, 2 * rball);

                //теперь смотрю сколько opengate нажато
                win = opengate.GetLength(0);
                for (int i = 0; i < opengate.GetLength(0); i++)
                    for (int j = 0; j < dynamicblocks.GetLength(0); j++)
                        if ((dynamicblocks[j, 0] - opengate[i, 0]) * (dynamicblocks[j, 0] + dynamicblocks[j, 2] - opengate[i, 0] - opengate[i, 2]) <= 0 && (dynamicblocks[j, 1] - opengate[i, 1]) * (dynamicblocks[j, 1] + dynamicblocks[j, 3] - opengate[i, 1] - opengate[i, 2]) <= 0)
                        {
                            win--;
                            break;
                        }
           
                if (win == 0)
                    graph.FillRectangle(Brushes.White, gateX, gateY, gateWidht, gateHeight);
                else
                    graph.FillRectangle(Brushes.Black, gateX, gateY, gateWidht, gateHeight);


                //победа
                if ((gateX - xball - rball) * (gateX + gateWidht - xball + rball) < 0 && (gateY - yball - rball) * (gateY + gateHeight - yball + rball) < 0 && win == 0)
                {
                    level++;
                    level = level % numlevels;
                    buttonlevel[level].ForeColor = Color.Black;
                    buttonlevel[level].Enabled = true;
                    Finish();
                }
            }

            // сохраняю положение формы
        }

        public void Finish()
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            Graphics graph = panellevel.CreateGraphics();
            graph.FillRectangle(Brushes.Black, 0, 0, panellevel.Width, panellevel.Height);
            panellevel.Visible = false;

            levelenable[level] = 1;
            //записать!!!
            StreamWriter sw = new StreamWriter("levelsOpen.kkk");
            for (int i = 0; i < levelenable.GetLength(0); i++)
            {
                sw.Write(levelenable[i].ToString());
                sw.Write(" ");
            }
            sw.Close();

            win = -2; // количесто opengate Oна которые осталось нажать 
            keyleft = false; //нужно для управления клавишами
            keyright = false;
            keyup = false;
            keydown = false;
            touch = false; // касание шарика неподвижного блока

            
        }


        public void WaveSearch(int n, ref int Xnew, ref int Ynew) // n - номер стража
        {
            int Xstart = guards[n, 0] + guards[n, 2] / 2 + 40;
            int Ystart = guards[n, 1] + guards[n, 3] / 2 + 40;
            int Xfinish = guards[n, 4] + guards[n, 2] / 2 + 40;
            int Yfinish = guards[n, 5] + guards[n, 2] / 2 + 40; //это я считаю относительно центра стража и с запасом (чтобы не вылететь за пределы массива)

            int step;
            Xnew = Xstart;
            Ynew = Ystart;
            if (graghguard[n, Xstart, Ystart] > 1)
            {
                step = graghguard[n, Xstart, Ystart];
                FindWay(n, Xstart, Ystart, ref Xnew, ref Ynew, ref step);
            }
            if (Xnew == Xstart && Ystart == Ynew)
            {
                Fillmap(n, ref graghguard);

                bool add = false;
                step = 2;
                graghguard[n, Xfinish, Yfinish] = 1;
                int[,] queue = new int[5000, 2];
                int[,] queue2 = new int[5000, 2];
                int cur = 1;
                int cur2 = 0;
                queue[0, 0] = Xfinish;
                queue[0, 1] = Yfinish;
                while (add == false)
                {
                    for (int i = 0; i < cur; i++)
                    {
                        if (queue[i, 0] - 1 >= 40 && graghguard[n, queue[i, 0] - 1, queue[i, 1]] == 0)
                        {
                            graghguard[n, queue[i, 0] - 1, queue[i, 1]] = step;
                            queue2[cur2, 0] = queue[i, 0] - 1;
                            queue2[cur2, 1] = queue[i, 1];
                            cur2++;
                        }
                        if (queue[i, 0] + 1 <= graghguard.GetLength(1) - 40 && graghguard[n, queue[i, 0] + 1, queue[i, 1]] == 0)
                        {
                            graghguard[n, queue[i, 0] + 1, queue[i, 1]] = step;
                            queue2[cur2, 0] = queue[i, 0] + 1;
                            queue2[cur2, 1] = queue[i, 1];
                            cur2++;
                        }
                        if (queue[i, 1] - 1 >= 40 && graghguard[n, queue[i, 0], queue[i, 1] - 1] == 0)
                        {
                            graghguard[n, queue[i, 0], queue[i, 1] - 1] = step;
                            queue2[cur2, 0] = queue[i, 0];
                            queue2[cur2, 1] = queue[i, 1] - 1;
                            cur2++;
                        }
                        if (queue[i, 1] + 1 <= graghguard.GetLength(2) - 40 && graghguard[n, queue[i, 0], queue[i, 1] + 1] == 0)
                        {
                            graghguard[n, queue[i, 0], queue[i, 1] + 1] = step;
                            queue2[cur2, 0] = queue[i, 0];
                            queue2[cur2, 1] = queue[i, 1] + 1;
                            cur2++;
                        }
                    }
                    step++;
                    if (step > graghguard.GetLength(1) + graghguard.GetLength(2))
                    {
                        Xnew = Xstart;
                        Ynew = Ystart;
                        break;
                    }
                    if (graghguard[n, Xstart, Ystart] > 0 || graghguard[n, Xstart, Ystart] == -1 || graghguard[n, Xfinish, Yfinish] == -1)
                        add = true;
                    for (int i = 0; i < cur2; i++)
                    {
                        queue[i, 0] = queue2[i, 0];
                        queue[i, 1] = queue2[i, 1];
                    }
                    cur = cur2;
                    cur2 = 0;
                }
                if (add == true)
                {
                    step = graghguard[n, Xstart, Ystart] - 1;
                    Xnew = Xstart;
                    Ynew = Ystart;
                    FindWay(n, Xstart, Ystart, ref Xnew, ref Ynew, ref step);
                }
            }
        }

        public void FindWay(int n, int Xstart, int Ystart, ref int Xnew, ref int Ynew, ref int step)
        {
            step = graghguard[n, Xstart, Ystart] - 1;
            Xnew = Xstart;
            Ynew = Ystart;
            while (step + guards[n, 8] > graghguard[n, Xstart, Ystart] - 1 && step > 0)
            {
                int step2 = step;
                if (Xnew - 1 >= 40 && graghguard[n, Xnew - 1, Ynew] == step)
                {
                    Xnew = Xnew - 1;
                    step--;
                }
                if (Xnew + 1 <= graghguard.GetLength(1) - 40 && graghguard[n, Xnew + 1, Ynew] == step)
                {
                    Xnew = Xnew + 1;
                    step--;
                }
                if (Ynew - 1 >= 40 && graghguard[n, Xnew, Ynew - 1] == step)
                {
                    Ynew = Ynew - 1;
                    step--;
                }
                if (Ynew + 1 <= graghguard.GetLength(1) - 40 && graghguard[n, Xnew, Ynew + 1] == step)
                {
                    Ynew = Ynew + 1;
                    step--;
                }
                if (step == step2)
                    break;
            }
        }

        public void CheckMove(int Xnew, int Ynew, ref int Xold, ref int Yold, int objWidth, int objHeight, ref bool flag) // можно мячку двигаться влево xnew, ynew - новые координаты xold, yold - cтарые
        {
            Graphics graph = panellevel.CreateGraphics();

            //перерисовка opengate
            for (int i = 0; i < opengate.GetLength(0); i++)
                graph.FillEllipse(Brushes.Green, opengate[i, 0], opengate[i, 1], opengate[i, 2], opengate[i, 2]);

            //статичный блок
            for (int i = 0; i < staticblocks.GetLength(0); i++)
            {
                if ((staticblocks[i, 0] - Xnew - objWidth - 1) * (staticblocks[i, 0] + staticblocks[i, 2] + 1 - Xnew) <= 0 && (staticblocks[i, 1] - Ynew - objHeight - 1) * (staticblocks[i, 1] + staticblocks[i, 3] + 1 - Ynew) <= 0) // условие того что объект войдет внутрь staticblock
                {
                    if (objWidth == 2 * rball && objHeight == 2 * rball) //тут я узнаю шарик это или нет... читерско... и можно перепусть, вероятность оч маленькая
                        touch = true;

                    flag = false;
                    while (staticblocks[i, 0] - Xold - objWidth - 3 >= 0) //слева
                        Xold++;
                    while (staticblocks[i, 0] + staticblocks[i, 2] - Xold + 3 <= 0)//справа
                        Xold--;
                    while (staticblocks[i, 1] - Yold - objHeight - 3 >= 0)//сверху
                        Yold++;
                    while (staticblocks[i, 1] + staticblocks[i, 3] - Yold + 3 <= 0)//снизу
                        Yold--;        
                }
            }
            //подвижный блок
            if (flag == true)
            {
                for (int i = 0; i < dynamicblocks.GetLength(0); i++)
                {
                    if ((dynamicblocks[i, 0] - Xnew - objWidth - 1) * (dynamicblocks[i, 0] + dynamicblocks[i, 2] + 1 - Xnew) <= 0 && (dynamicblocks[i, 1] - Ynew - objHeight - 1) * (dynamicblocks[i, 1] + dynamicblocks[i, 3] + 1 - Ynew) <= 0) // условие того что объект войдет внутрь staticblock
                    {
                        int newcoordinateX, newcoordinateY;
                                               
                        if (dynamicblocks[i, 0] - Xold - objWidth >= 0)
                        {
                            newcoordinateX = Xnew + objWidth + 2;
                            newcoordinateY = dynamicblocks[i, 1];
                            graph.DrawRectangle(Pens.Black, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = 0;

                            CheckMove(newcoordinateX, newcoordinateY, ref dynamicblocks[i, 0], ref dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3], ref flag);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = -1;

                            graph.DrawRectangle(Pens.Blue, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);
                            Xold = dynamicblocks[i, 0] - objWidth - 2;
                            Yold = Ynew;

                        }
                        if (dynamicblocks[i, 0] + dynamicblocks[i, 2] - Xold <= 0)
                        {
                            newcoordinateX = Xnew - 3 - dynamicblocks[i, 2];
                            newcoordinateY = dynamicblocks[i, 1];
                            graph.DrawRectangle(Pens.Black, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = 0;

                            CheckMove(newcoordinateX, newcoordinateY, ref dynamicblocks[i, 0], ref dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3], ref flag);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = -1;

                            graph.DrawRectangle(Pens.Blue, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);
                            Xold = dynamicblocks[i, 0] + 3 + dynamicblocks[i, 2];
                            Yold = Ynew;
                        }
                        if (dynamicblocks[i, 1] - Yold - objHeight >= 0)
                        {
                            newcoordinateX = dynamicblocks[i, 0];
                            newcoordinateY = Ynew + objHeight + 2;
                            graph.DrawRectangle(Pens.Black, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = 0;

                            CheckMove(newcoordinateX, newcoordinateY, ref dynamicblocks[i, 0], ref dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3], ref flag);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = -1;

                            graph.DrawRectangle(Pens.Blue, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);
                            Xold = Xnew;
                            Yold = dynamicblocks[i, 1] - objHeight - 2;
                        }
                        if (dynamicblocks[i, 1] + dynamicblocks[i, 3] - Yold <= 0)
                        {
                            newcoordinateX = dynamicblocks[i, 0];
                            newcoordinateY = Ynew - 3 - dynamicblocks[i, 3];
                            graph.DrawRectangle(Pens.Black, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = 0;

                            CheckMove(newcoordinateX, newcoordinateY, ref dynamicblocks[i, 0], ref dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3], ref flag);

                            for (int j = 0; j < graghguard.GetLength(0); j++)
                                for (int k = dynamicblocks[i, 0] + 40 - guards[j, 2]; k < dynamicblocks[i, 0] + dynamicblocks[i, 2] + 40 + guards[j, 2]; k++)
                                    for (int g = dynamicblocks[i, 1] + 40 - guards[j, 3]; g < dynamicblocks[i, 1] + dynamicblocks[i, 3] + 40 + guards[j, 3]; g++)
                                        graghguard[j, k, g] = -1;

                            graph.DrawRectangle(Pens.Blue, dynamicblocks[i, 0], dynamicblocks[i, 1], dynamicblocks[i, 2], dynamicblocks[i, 3]);
                            Xold = Xnew;
                            Yold = dynamicblocks[i, 1] + 3 + dynamicblocks[i, 3];
                        }
                    }
                }

                //со стражем
                for (int i = 0; i < guards.GetLength(0); i++)
                {
                    if ((guards[i, 0] - Xnew - objWidth - 1) * (guards[i, 0] + guards[i, 2] + 1 - Xnew) <= 0 && (guards[i, 1] - Ynew - objHeight - 1) * (guards[i, 1] + guards[i, 3] + 1 - Ynew) <= 0) // условие того что объект войдет внутрь staticblock
                    {
                        int newcoordinateX, newcoordinateY;

                        if (guards[i, 0] - Xold - objWidth >= 0)
                        {
                            newcoordinateX = Xnew + objWidth + 2;
                            newcoordinateY = guards[i, 1];
                            graph.DrawEllipse(Pens.Black, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            CheckMove(newcoordinateX, newcoordinateY, ref guards[i, 0], ref guards[i, 1], guards[i, 2], guards[i, 3], ref flag);
                            graph.DrawEllipse(Pens.Red, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            Xold = guards[i, 0] - objWidth - 2;
                            Yold = Ynew;
                        }
                        if (guards[i, 0] + guards[i, 2] - Xold <= 0)
                        {
                            newcoordinateX = Xnew - 3 - guards[i, 2];
                            newcoordinateY = guards[i, 1];
                            graph.DrawEllipse(Pens.Black, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            CheckMove(newcoordinateX, newcoordinateY, ref guards[i, 0], ref guards[i, 1], guards[i, 2], guards[i, 3], ref flag);
                            graph.DrawEllipse(Pens.Red, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            Xold = guards[i, 0] + 3 + guards[i, 2];
                            Yold = Ynew;
                        }
                        if (guards[i, 1] - Yold - objHeight >= 0)
                        {
                            newcoordinateX = guards[i, 0];
                            newcoordinateY = Ynew + objHeight + 2;
                            graph.DrawEllipse(Pens.Black, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            CheckMove(newcoordinateX, newcoordinateY, ref guards[i, 0], ref guards[i, 1], guards[i, 2], guards[i, 3], ref flag);
                            graph.DrawEllipse(Pens.Red, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            Xold = Xnew;
                            Yold = guards[i, 1] - objHeight - 2;
                        }
                        if (guards[i, 1] + guards[i, 3] - Yold <= 0)
                        {
                            newcoordinateX = guards[i, 0];
                            newcoordinateY = Ynew - 3 - guards[i, 3];
                            graph.DrawEllipse(Pens.Black, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            CheckMove(newcoordinateX, newcoordinateY, ref guards[i, 0], ref guards[i, 1], guards[i, 2], guards[i, 3], ref flag);
                            graph.DrawEllipse(Pens.Red, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                            Xold = Xnew;
                            Yold = guards[i, 1] + 3 + guards[i, 3];
                        }
                    }
                }
                // если встречи с неподвижным не произошло
                if (flag == true)
                {
                Xold = Xnew;
                Yold = Ynew;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // движение стража
            Graphics graph = panellevel.CreateGraphics();


            for (int i = 0; i < guards.GetLength(0); i++)
            {
                bool newtarget = false; // появилась ли новая цель

                // следующая точка path
                if (guards[i, 0] == guards[i, 4] && guards[i, 1] == guards[i, 5])
                {
                    guards[i, 7]++;
                    guards[i, 7] = guards[i, 7] % guards[i, 6];
                    guards[i, 4] = path[i, 2 * guards[i, 7]];
                    guards[i, 5] = path[i, 2 * guards[i, 7] + 1];
                    guards[i, 8] = 5;
                    newtarget = true;
                }

                // шарик по звуку
                if (touch == true)
                {
                    guards[i, 4] = xball - rball;
                    guards[i, 5] = yball - rball;
                    guards[i, 8] = 15;
                    newtarget = true;
                    if (i == guards.GetLength(0) - 1)
                        touch = false;
                }

                int l = (xball - (guards[i, 0] + guards[i, 2] / 2)) * (xball - (guards[i, 0] + guards[i, 2] / 2)) + (yball - (guards[i, 1] + guards[i, 3] / 2)) * (yball - (guards[i, 1] + guards[i, 3] / 2));
                bool see = false;
                if (l + 3 <= guards[i, 9] * guards[i, 9])
                {
                    see = true;
                    for (int j = 0; j < staticblocks.GetLength(0); j++)
                    {
                        
                        if ((staticblocks[j, 1] - yball) * (staticblocks[j, 1] - guards[i, 1]) <= 0)
                        {
                            int dleft = (staticblocks[j, 0] - guards[i, 0] - guards[i, 2] / 2) * (yball - guards[i, 1] - guards[i, 3] / 2) / (staticblocks[j, 1] + staticblocks[j, 3] / 2 - guards[i, 1] - guards[i, 3] / 2);
                            int dright = (staticblocks[j, 0] + staticblocks[j, 2] - guards[i, 0] - guards[i, 2] / 2) * (yball - guards[i, 1] - guards[i, 3] / 2) / (staticblocks[j, 1] + staticblocks[j, 3] / 2 - guards[i, 1] - guards[i, 3] / 2);
                            if ((guards[i, 0] + guards[i, 2] / 2 + dleft - xball) * (guards[i, 0] + guards[i, 2] / 2 + dright - xball) <= 0)
                                see = false;
                        }
                        if ((staticblocks[j, 0] - xball) * (staticblocks[j, 0] - guards[i, 0]) <= 0)
                        {
                            int dleft = (staticblocks[j, 1] - guards[i, 1] - guards[i, 3] / 2) * (xball - guards[i, 0] - guards[i, 2] / 2) / (staticblocks[j, 0] + staticblocks[j, 2] / 2 - guards[i, 0] - guards[i, 2] / 2);
                            int dright = (staticblocks[j, 1] + staticblocks[j, 3] - guards[i, 1] - guards[i, 3] / 2) * (xball - guards[i, 0] - guards[i, 2] / 2) / (staticblocks[j, 0] + staticblocks[j, 2] / 2 - guards[i, 0] - guards[i, 2] / 2);
                            if ((guards[i, 1] + guards[i, 3] / 2 + dleft - yball) * (guards[i, 1] + guards[i, 3] / 2 + dright - yball) <= 0)
                                see = false;
                        }
                        if (see == false)
                            break;
                    }
                    for (int j = 0; j < dynamicblocks.GetLength(0); j++)
                    {

                        if ((dynamicblocks[j, 1] - yball) * (dynamicblocks[j, 1] - guards[i, 1]) <= 0)
                        {
                            int dleft = (dynamicblocks[j, 0] - guards[i, 0] - guards[i, 2] / 2) * (yball - guards[i, 1] - guards[i, 3] / 2) / (dynamicblocks[j, 1] + dynamicblocks[j, 3] / 2 - guards[i, 1] - guards[i, 3] / 2);
                            int dright = (dynamicblocks[j, 0] + dynamicblocks[j, 2] - guards[i, 0] - guards[i, 2] / 2) * (yball - guards[i, 1] - guards[i, 3] / 2) / (dynamicblocks[j, 1] + dynamicblocks[j, 3] / 2 - guards[i, 1] - guards[i, 3] / 2);
                            if ((guards[i, 0] + guards[i, 2] / 2 + dleft - xball) * (guards[i, 0] + guards[i, 2] / 2 + dright - xball) <= 0)
                                see = false;
                        }
                        if ((dynamicblocks[j, 0] - xball) * (dynamicblocks[j, 0] - guards[i, 0]) <= 0)
                        {
                            int dleft = (dynamicblocks[j, 1] - guards[i, 1] - guards[i, 3] / 2) * (xball - guards[i, 0] - guards[i, 2] / 2) / (dynamicblocks[j, 0] + dynamicblocks[j, 2] / 2 - guards[i, 0] - guards[i, 2] / 2);
                            int dright = (dynamicblocks[j, 1] + dynamicblocks[j, 3] - guards[i, 1] - guards[i, 3] / 2) * (xball - guards[i, 0] - guards[i, 2] / 2) / (dynamicblocks[j, 0] + dynamicblocks[j, 2] / 2 - guards[i, 0] - guards[i, 2] / 2);
                            if ((guards[i, 1] + guards[i, 3] / 2 + dleft - yball) * (guards[i, 1] + guards[i, 3] / 2 + dright - yball) <= 0)
                                see = false;
                        }
                        if (see == false)
                            break;
                    }
                }
                if (see == true)
                {
                    guards[i, 4] = xball - rball;
                    guards[i, 5] = yball - rball;
                    guards[i, 8] = 15;
                    newtarget = true;
                }
                if (l < 64)
                {
                    Finish();
                    MessageBox.Show("YOU ARE TOO NOISY");
                }

                if (newtarget == true)
                    Fillmap(i, ref graghguard);


                int Xstep, Ystep;
                Xstep = guards[i, 0];
                Ystep = guards[i, 1];
                WaveSearch(i, ref Xstep, ref Ystep);
                Xstep = Xstep - 40 - guards[i, 2] / 2;
                Ystep = Ystep - 40 - guards[i, 3] / 2;
                
                if (Xstep == guards[i, 0] && Ystep == guards[i, 1])
                {
                    guards[i, 7]++;
                    guards[i, 7] = guards[i, 7] % guards[i, 6];
                    guards[i, 4] = path[i, 2 * guards[i, 7]];
                    guards[i, 5] = path[i, 2 * guards[i, 7] + 1];
                    guards[i, 8] = 5;
                    Fillmap(i, ref graghguard);
                }
                else
                {
                    graph.DrawEllipse(Pens.Black, guards[i, 0] - guards[i, 9] + guards[i, 2] / 2, guards[i, 1] - guards[i, 9] + guards[i, 3] / 2, 2 * guards[i, 9], 2 * guards[i, 9]);
                    graph.DrawEllipse(Pens.Black, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);
                    guards[i, 0] = Xstep;
                    guards[i, 1] = Ystep;
                    graph.DrawEllipse(Pens.White, guards[i, 0] - guards[i, 9] + guards[i, 2] / 2, guards[i, 1] - guards[i, 9] + guards[i, 3] / 2, 2 * guards[i, 9], 2 * guards[i, 9]);
                    graph.DrawEllipse(Pens.Red, guards[i, 0], guards[i, 1], guards[i, 2], guards[i, 3]);

                    graph.DrawEllipse(Pens.White, xball - rball, yball - rball, 2 * rball, 2 * rball);
                    for (int j = 0; j < opengate.GetLength(0); j++)
                        graph.FillEllipse(Brushes.Green, opengate[j, 0], opengate[j, 1], opengate[j, 2], opengate[j, 2]);
                    for (int j = 0; j < dynamicblocks.GetLength(0); j++)
                        graph.DrawRectangle(Pens.Blue, dynamicblocks[j, 0], dynamicblocks[j, 1], dynamicblocks[j, 2], dynamicblocks[j, 3]);
                    for (int j = 0; j < staticblocks.GetLength(0); j++)
                        graph.DrawRectangle(Pens.White, staticblocks[j, 0], staticblocks[j, 1], staticblocks[j, 2], staticblocks[j, 3]);
                    if (win == 0)
                        graph.FillRectangle(Brushes.White, gateX, gateY, gateWidht, gateHeight);
                }
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (timer1.Enabled == true)
            switch (e.KeyCode)
            {
                case Keys.Left:
                    keyleft = true;
                    break;
                case Keys.Right:
                    keyright = true;
                    break;
                case Keys.Up:
                    keyup = true;
                    break;
                case Keys.Down:
                    keydown = true;
                    break;
                case Keys.A:
                    keyleft = true;
                    break;
                case Keys.D:
                    keyright = true;
                    break;
                case Keys.W:
                    keyup = true;
                    break;
                case Keys.S:
                    keydown = true;
                    break;
            }
        }

        private void button1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            if (timer1.Enabled == true)
            switch (e.KeyCode)
            {
                case Keys.Left:
                    keyleft = false;
                    break;
                case Keys.Right:
                    keyright = false;
                    break;
                case Keys.Up:
                    keyup = false;
                    break;
                case Keys.Down:
                    keydown = false;
                    break;
                case Keys.A:
                    keyleft = false;
                    break;
                case Keys.D:
                    keyright = false;
                    break;
                case Keys.W:
                    keyup = false;
                    break;
                case Keys.S:
                    keydown = false;
                    break;
            }
        }
    }
}