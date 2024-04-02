using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using static Play;

namespace checkers
{
    public partial class Form1 : Form
    {
        private static int[] percepNumNeuronL = { 40, 10 };
        public PerceptronForCheckers per = new PerceptronForCheckers(32, percepNumNeuronL, 1);
        public Graphics gr;
        public Cell[,] board = new Cell[8, 8];
        public TypeCheck PlayerType;
        public int fromX=0;
        public int fromY = 0;
        public int toX = 0;
        public int toY = 0;
        public Form1()
        {
            InitializeComponent();
            //DrawBoard();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path= @"C:\Users\a_n_a\Documents\учеба\нейронки\checkers\checkers_train\bin\Debug\net6.0\weights.txt";
            using (StreamReader reader = new StreamReader(path))
            {
                for (int i = 0; i < 32; i++)
                    for (int j = 0; j < 40; j++)
                    {
                        per.w1[i,j] = double.Parse(reader.ReadLine());
                    }
                for (int i = 0; i < 40; i++)
                    for (int j = 0; j < 10; j++)
                    {
                        per.w2[i, j] = double.Parse(reader.ReadLine());
                    }
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 1; j++)
                    {
                        per.wout[i, j] = double.Parse(reader.ReadLine());
                    }
                for (int j = 0; j < 40; j++)
                {
                    per.b1[j] = double.Parse(reader.ReadLine());
                }
                for (int j = 0; j < 10; j++)
                {
                    per.b2[j] = double.Parse(reader.ReadLine());
                }
                for (int j = 0; j < 1; j++)
                {
                    per.b2[j] = double.Parse(reader.ReadLine());
                }
            }
            
        }
        private void DrawBoard(Graphics g,bool flagFrom = false, bool flagTo = false)
        {
            g.Clear(Color.Black);
            Pen pen = new Pen(Color.Black, 2);
            int cellSize = Math.Min(pictureBox1.Width / 8, pictureBox1.Height / 8);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0)
                        g.FillRectangle(Brushes.White, j * cellSize, i * cellSize, cellSize, cellSize);
                    else
                    {
                        g.FillRectangle(Brushes.Gray, j * cellSize, i * cellSize, cellSize, cellSize);
                        if (board[i, j].typeCheck!=TypeCheck.empty)
                        {
                            if (board[i, j].typeCheck == TypeCheck.white)
                                g.FillEllipse(Brushes.White, j * cellSize, i * cellSize , cellSize, cellSize );
                            if (board[i, j].typeCheck == TypeCheck.black)
                                g.FillEllipse(Brushes.Black, j * cellSize , i * cellSize , cellSize , cellSize);
                            if (board[i, j].typeCheck == TypeCheck.queen_white)
                            {
                                g.FillEllipse(Brushes.White, j * cellSize , i * cellSize, cellSize, cellSize);
                                g.FillEllipse(Brushes.Black, j * cellSize+ (float)cellSize/4, i * cellSize + (float)cellSize / 4, cellSize * (float)0.25, cellSize * (float)0.25);
                            }
                            if (board[i, j].typeCheck == TypeCheck.queen_black)
                            {
                                g.FillEllipse(Brushes.Black, j * cellSize, i * cellSize, cellSize, cellSize);
                                g.FillEllipse(Brushes.White, j * cellSize + (float)cellSize / 4, i * cellSize + (float)cellSize / 4, cellSize * (float)0.25, cellSize * (float)0.25);
                            }
                        }
                    }
                }
            }
            if (flagFrom)
            {
                Pen pen1 = new Pen(Color.Green, 4);
                g.DrawLine(pen1, fromX * cellSize, fromY * cellSize, fromX * cellSize + cellSize, fromY * cellSize);
                g.DrawLine(pen1, fromX * cellSize, fromY * cellSize, fromX * cellSize, fromY * cellSize + cellSize);
                g.DrawLine(pen1, fromX * cellSize, fromY * cellSize + cellSize, fromX * cellSize + cellSize, fromY * cellSize+ cellSize);
                g.DrawLine(pen1, fromX * cellSize + cellSize, fromY * cellSize + cellSize, fromX * cellSize + cellSize, fromY * cellSize);
            }
            if (flagTo)
            {
                Pen pen1 = new Pen(Color.Red, 4);
                g.DrawLine(pen1, toX * cellSize, toY * cellSize, toX * cellSize + cellSize, toY * cellSize);
                g.DrawLine(pen1, toX * cellSize, toY * cellSize, toX * cellSize, toY * cellSize + cellSize);
                g.DrawLine(pen1, toX * cellSize, toY * cellSize + cellSize, toX * cellSize + cellSize, toY * cellSize + cellSize);
                g.DrawLine(pen1, toX * cellSize + cellSize, toY * cellSize + cellSize, toX * cellSize + cellSize, toY * cellSize);
            }
        }
        //ход нейронки
        //мой ход
        private void button1_Click(object sender, EventArgs e)//пользователь за белых
        {
            gr = pictureBox1.CreateGraphics();
            StartInitBoard(board);
            DrawBoard(gr);
            PlayerType = TypeCheck.white;
        }

        private void button2_Click(object sender, EventArgs e)//пользователь за черных
        {
            gr = pictureBox1.CreateGraphics();
            StartInitBoard(board);
            DrawBoard(gr);
            PlayerType = TypeCheck.black;
            MoveNetwork();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)//выбор клетки, которой будем ходить
        {
            if (e.Button == MouseButtons.Left)
            {

                double x = e.X;
                double step = Math.Min(pictureBox1.Width / 8, pictureBox1.Height / 8);
                double t = 0;
                for (int i = 0; i < 8; i++)
                {
                    t += step;
                    if (x < t)
                    {
                        fromX = i;
                        break;
                    }
                }
                double y = e.Y;
                step = pictureBox1.Height / 8;
                t = 0;
                for (int i = 0; i < 8; i++)
                {
                    t += step;
                    if (y < t)
                    {
                        fromY = i;
                        break;
                    }
                }
                if ((board[fromY, fromX].typeCheck == PlayerType) || (board[fromY, fromX].typeCheck == (TypeCheck)(2*(int)PlayerType)))
                {
                    DrawBoard(gr, true, true);
                }
                else MessageBox.Show("С этой клетки нельзя сходить, выберете другую", "Внимание!");
            }
            if (e.Button == MouseButtons.Right)
            {
                double x = e.X;
                double step = Math.Min(pictureBox1.Width / 8, pictureBox1.Height / 8);
                double t = 0;
                for (int i = 0; i < 8; i++)
                {
                    t += step;
                    if (x < t)
                    {
                        toX = i;
                        break;
                    }
                }
                double y = e.Y;
                step = pictureBox1.Height / 8;
                t = 0;
                for (int i = 0; i < 8; i++)
                {
                    t += step;
                    if (y < t)
                    {
                        toY = i;
                        break;
                    }
                }
                if ((board[toY, toX].typeCheck ==TypeCheck.empty)&& ((toY + toX) % 2 == 1))
                {
                    DrawBoard(gr, true, true);
                }
                else MessageBox.Show("В эту клетку нельзя сходить, выберете другую", "Внимание!");
            }
            //и выделим цветом
        }
        public bool CanCheckEatYet(Tuple<int, int> id)
        {
            for (int k = 0; k < 4; k++)//все пути
                if (CanItEat(board, Tuple.Create(id.Item1, id.Item2), Tuple.Create(way[k, 0], way[k, 1])))
                    return true;
            return false;
        }
        private void button3_Click(object sender, EventArgs e)// ход пользователя
        {
            //есть ли у нас возможность съесть, если да, то проверить, можно ли так есть
            List<Tuple<int, int>> a = new List<Tuple<int, int>>();
            a = AllCheckEat(board, PlayerType, (TypeCheck)(2 * (int)PlayerType));
            if(a.Count > 0) 
            {
                int b = Math.Abs(fromY - toY);
                int c = Math.Abs(fromX - toX);
                int wayY=(toY- fromY)/2;
                int wayX = (toX - fromX) / 2;
                if ((b == 2) && (c == 2) && 
                    ( (board[wayY+ fromY, wayX+ fromX].typeCheck == (TypeCheck)(-1 * (int)PlayerType))|| 
                    (board[wayY + fromY, wayX+ fromX].typeCheck == (TypeCheck)(-2 * (int)PlayerType)) )&&
                    (board[toY, toX].typeCheck == TypeCheck.empty))
                {
                    //если да, сходить и посмотреть, можно ли съесть еще
                    ItEat(board, Tuple.Create(fromY,fromX), Tuple.Create(wayY, wayX));
                    DrawBoard(gr);
                    if (CanCheckEatYet(Tuple.Create(toY, toX)))
                    {
                        MessageBox.Show("Ешьте дальше", "Внимание!");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Вы должны есть!", "Внимание!");
                    return;
                }
            }
            else//иначе для выбранной клетки проверить может ли она так сходить
            {
                int b = Math.Abs(fromY - toY);
                int c = Math.Abs(fromX - toX);
                int wayY = (toY - fromY);
                int wayX = (toX - fromX);
                if ((b == 1) && (c == 1) &&
                (board[toY, toX].typeCheck == TypeCheck.empty)&&
                CanItMove(board,Tuple.Create(fromY, fromX), Tuple.Create(wayY, wayX)))
                {
                    
                    ItMove(board, Tuple.Create(fromY, fromX), Tuple.Create(wayY, wayX));
                    DrawBoard(gr);
                }
                else
                {
                    MessageBox.Show("Так сходить нельзя!", "Внимание!");
                    return;
                }

            }
            double count = 0;
            //проверка на результат, считаем шашки противника
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if ((board[i, j].typeCheck == PlayerType) || (board[i, j].typeCheck == (TypeCheck)(2 * (int)PlayerType)))
                        count++;
                }   
            if (count == 0)
            {
                MessageBox.Show("Вы выйграли!", "Внимание!");
                return;
            }
            // вызвать ход противника( в нем в конце тоже проверка на результат)
            MoveNetwork();

        }
        public void MoveNetwork()
        {
            double resNetwork = -1;
            bool BlockMove = false;
            List<Tuple<int, int>> a = new List<Tuple<int, int>>();
            a = AllCheckEat(board, (TypeCheck)(-1 * (int)PlayerType), (TypeCheck)(-2 * (int)PlayerType));
            List<Tuple<int, int>> resWayEnd = new List<Tuple<int, int>>();
            Tuple<int, int> bestMove = Tuple.Create(0, 0);
            Tuple<int, int> id = Tuple.Create(0, 0);
            if (a.Count != 0)//идем только по шашкам, которые могут съесть
            {
                double temp = -1;
                for (int k = 0; k < a.Count; k++)
                {
                    List<Tuple<int, int>> resWayTemp = CalcCheckersMovesEat(board, Tuple.Create(a[k].Item1, a[k].Item2), ref temp, per);
                    if (temp > resNetwork)
                    {
                        resNetwork = temp;//баллы
                        resWayEnd.Clear();
                        for (int i = 0; i < resWayTemp.Count(); i++)//ходы
                            resWayEnd.Add(resWayTemp[i]);
                        id = Tuple.Create(a[k].Item1, a[k].Item2);//id шашки
                    }
                }
                //делаем ход
                if (resWayEnd.Count != 0)
                {
                    for (int k = 0; k < resWayEnd.Count; k++)
                    {
                        ItEat(board, id, resWayEnd[k]);
                        id = Tuple.Create(id.Item1 + 2*resWayEnd[k].Item1, id.Item2 + 2*resWayEnd[k].Item2);
                    }
                    DrawBoard(gr);
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                        {
                            if ((board[i, j].typeCheck == (TypeCheck)(-1 * (int)PlayerType)) || (board[i, j].typeCheck == (TypeCheck)(-2* (int)PlayerType)))
                            {
                                double temp = -1;
                                Tuple<int, int> resWayTemp = CalcCheckersMove(board, Tuple.Create(i, j), ref temp, per);
                                if (temp > resNetwork)
                                {
                                    resNetwork = temp;//баллы
                                    bestMove = Tuple.Create(resWayTemp.Item1, resWayTemp.Item2);
                                    id = Tuple.Create(i, j);//id шашки
                                }
                            }
                        }

                    }
                if (bestMove.Item1 == 0) BlockMove = true;
                ItMove(board, id, bestMove);
                DrawBoard(gr);
            }

            double count = 0;
            //проверка на результат, считаем шашки противника
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if ((board[i, j].typeCheck == (TypeCheck)(-1 * (int)PlayerType)) || (board[i, j].typeCheck == (TypeCheck)(-2 * (int)PlayerType)))
                        count++;
                }
            if (count == 0)
            {
                MessageBox.Show("Вы проиграли!", "Внимание!");
                return;
            }
            if ((BlockMove))
            {
                MessageBox.Show("Ничья!", "Внимание!");
                return;
            }
        }
    }
}
