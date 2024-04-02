using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

    public enum TypeCheck
    {
        black = -1,
        white = 1,
        queen_black = -2,
        queen_white = 2,
        empty = 0
    }
    public class Cell
    {
        public TypeCheck typeCheck = TypeCheck.empty; 
    }
    
    public static class Play
    {
        public static int[,] way = { { 1, 1 }, { -1, 1 }, { 1, -1 }, { -1, -1 } };
        public static void Copy(Cell[,] b1, Cell[,] b2)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    b2[i, j].typeCheck = b1[i, j].typeCheck;
                }
        }
        public static void InitBoard(Cell[,] board)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Cell();
                }
        }
        public static void StartInitBoard(Cell[,] board)
        {
                InitBoard(board);
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 8; j += 2)
                    {
                        if (i != 1) board[i, j + 1].typeCheck = TypeCheck.black;
                        else board[i, j].typeCheck = TypeCheck.black;
                    }
            for (int i = 5; i < 8; i++)
                for (int j = 0; j < 8; j += 2)
                {
                    if (i != 6) board[i, j].typeCheck = TypeCheck.white;
                    else board[i, j + 1].typeCheck = TypeCheck.white;
                }
        }
        public static void ItEat(Cell[,] board, Tuple<int, int> id, Tuple<int, int> way0)
        {
            board[id.Item1 + 2 * way0.Item1, id.Item2 + 2 * way0.Item2].typeCheck = board[id.Item1, id.Item2].typeCheck;
            board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck = TypeCheck.empty;
            board[id.Item1, id.Item2].typeCheck = TypeCheck.empty;
            CanItBecomeQueen(board, Tuple.Create(id.Item1 + 2 * way0.Item1, id.Item2 + 2 * way0.Item2));
        }
        public static void ItMove(Cell[,] board, Tuple<int, int> id, Tuple<int, int> way0)
        {
            board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck = board[id.Item1, id.Item2].typeCheck;
            board[id.Item1, id.Item2].typeCheck = TypeCheck.empty;
            CanItBecomeQueen(board, Tuple.Create(id.Item1 + way0.Item1, id.Item2 + way0.Item2));
        }
        public static List<Tuple<int,int>> AllCheckEat(Cell[,] board, TypeCheck type1, TypeCheck type2)
            {
                List < Tuple<int, int> > a= new List<Tuple<int, int>>();
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                        {
                            if ((board[i, j].typeCheck == type1) || (board[i, j].typeCheck == type2)) 
                            {
                                for (int k = 0; k < 4; k++)
                                    if (CanItEat(board, Tuple.Create(i, j), Tuple.Create(way[k, 0], way[k, 1])))
                                    {
                                        a.Add(Tuple.Create(i, j));
                                        break;
                                    }
                            }
                        }
                    }
                return a;
            }
        public static bool CanItMoveWayEat(Tuple<int, int> id, Tuple<int, int> way)
        {
            if (((id.Item1 + way.Item1) >= 1) && ((id.Item2 + way.Item2) >= 1)&& ((id.Item1 + way.Item1) <= 6) && ((id.Item2 + way.Item2) <= 6))//есть смысл...
                return true;
            else 
                return false;
        }
        public static bool CanItMoveWay(Tuple<int, int> id, Tuple<int, int> way)
        {
            if (((id.Item1 + way.Item1) >= 0) && ((id.Item2 + way.Item2) >= 0) && ((id.Item1 + way.Item1) <= 7) && ((id.Item2 + way.Item2) <= 7))//есть смысл...
                return true;
            else
                return false;
        }
        public static bool CanItEat(Cell[,] board, Tuple<int, int> id, Tuple<int, int> way0)
        {
            if(CanItMoveWayEat(Tuple.Create(id.Item1, id.Item2), Tuple.Create(way0.Item1, way0.Item2)))
            {
                if ((board[id.Item1, id.Item2].typeCheck == TypeCheck.white) || (board[id.Item1, id.Item2].typeCheck == TypeCheck.black))
                    if ((((-1) * (int)board[id.Item1, id.Item2].typeCheck == (int)board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck) || 
                        ((-2) * (int)board[id.Item1, id.Item2].typeCheck == (int)board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck)) && 
                        (board[id.Item1 + 2 * way0.Item1, id.Item2 + 2 * way0.Item2].typeCheck == TypeCheck.empty))
                    {
                        return true;
                    }
                if ((board[id.Item1, id.Item2].typeCheck == TypeCheck.queen_white) || (board[id.Item1, id.Item2].typeCheck == TypeCheck.queen_black))
                    if ((((-1) * (int)board[id.Item1, id.Item2].typeCheck == (int)board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck) ||
                        ((-0.5) * (int)board[id.Item1, id.Item2].typeCheck == (int)board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck)) &&
                        (board[id.Item1 + 2 * way0.Item1, id.Item2 + 2 * way0.Item2].typeCheck == TypeCheck.empty))
                    {
                        return true;
                    } 
                
            }
            return false;
        }
   
        public static bool CanItMove(Cell[,] board, Tuple<int, int> id, Tuple<int, int> way0)
        {
            if (CanItMoveWay(id, way0))
            {
                if (board[id.Item1, id.Item2].typeCheck == TypeCheck.white)
                    if(way0.Item1==-1)
                        if ((board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck == TypeCheck.empty))
                        {
                            return true;
                        }
                if (board[id.Item1, id.Item2].typeCheck == TypeCheck.black)
                    if (way0.Item1 == 1)
                        if ((board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck == TypeCheck.empty))
                        {
                            return true;
                        }
                if ((board[id.Item1, id.Item2].typeCheck == TypeCheck.queen_white) || (board[id.Item1, id.Item2].typeCheck == TypeCheck.queen_black))
                    if ((board[id.Item1 + way0.Item1, id.Item2 + way0.Item2].typeCheck == TypeCheck.empty))
                    {
                        return true;
                    }
            }
            return false;
        }

        public static void CanItBecomeQueen(Cell[,] board, Tuple<int, int> id)
        {
            if (board[id.Item1, id.Item2].typeCheck == TypeCheck.white)
                if (id.Item1 == 0) board[id.Item1, id.Item2].typeCheck = TypeCheck.queen_white;
            if (board[id.Item1, id.Item2].typeCheck == TypeCheck.black)
                if (id.Item1 == 7) board[id.Item1, id.Item2].typeCheck = TypeCheck.queen_black;
        }
        public static int[] From64To32(Cell[,] board)
        {
            int[] board32 = new int[32];
            int k = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if ((i + j) % 2 == 1)
                    {
                        board32[k] = (int)board[i, j].typeCheck;
                        k++;
                    }
            return board32;
        }
    public static void BestNetworkMove(Cell[,] board, ref double res, PerceptronForCheckers per, TypeCheck type)//принимает простой тип шашки
    {
            List<Tuple<int, int>> a = new List<Tuple<int, int>>();
            List<Tuple<int, int>> b = new List<Tuple<int, int>>();
            double temp = -1;
            a = AllCheckEat(board, type, (TypeCheck)(2*(int)type));
            Tuple<int, int> id= Tuple.Create(0, 0);
            List<Tuple<int, int>> resEat = new List<Tuple<int, int>>();
            Tuple<int, int> resMoveTemp = Tuple.Create(0, 0);
            Tuple<int, int> resMove = Tuple.Create(0, 0);
            res = -1;
            if (a.Count != 0)
            {
                for (int k = 0; k < a.Count; k++)
                {
                    b=CalcCheckersMovesEat1(board, a[k], ref temp, per);
                    if(temp>res)
                    {
                        res = temp;
                        resEat.Clear();
                        for (int l = 0; l < b.Count; l++)
                        {
                            resEat.Add(b[l]);
                        }
                        id = Tuple.Create(a[k].Item1, a[k].Item2);
                    }
                }
                for(int k=0; k < resEat.Count; k++)
                {
                    ItEat(board, id, resEat[k]);
                    id = Tuple.Create(id.Item1+ 2*resEat[k].Item1, id.Item2+ 2*resEat[k].Item2);
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                            if ((board[i, j].typeCheck == type) || (board[i, j].typeCheck == (TypeCheck)(2 * (int)type)))//смотрим наличие клеток...
                            {
                                resMoveTemp = CalcCheckersMoves1(board, Tuple.Create(i, j), ref temp, per);
                                if (temp > res)
                                {
                                    res = temp;
                                    resMove = Tuple.Create(resMoveTemp.Item1, resMoveTemp.Item2); 
                                    id = Tuple.Create(i, j);
                                }
                            } 
                    }
                ItMove(board, id, resMove);
            }
    } 
    
    public static List<Tuple<int, int>> CalcCheckersMovesEat1(Cell[,] board, Tuple<int, int> id, ref double res, PerceptronForCheckers per)
    {
        Cell[,] board0 = new Cell[8, 8];
        InitBoard(board0);
        int[] board32 = new int[32];
        double resAll= -1;
        List<Tuple<int, int>> resWay = new List<Tuple<int, int>>();
        List<Tuple<int, int>> resWayEnd = new List<Tuple<int, int>>();
        res = -1;
        for (int k = 0; k < 4; k++)
        {
            Copy(board, board0);
            if (CanItEat(board0, id, Tuple.Create(way[k, 0], way[k, 1])))
            {
                ItEat(board0, id, Tuple.Create(way[k, 0], way[k, 1]));
                board32 = From64To32(board0);
                per.train(board32);
                double temp0 = per.res;
                double temp1=-1;
                if ( CanItEat(board0, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[0, 0], way[0, 1]))||
                CanItEat(board0, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[1, 0], way[1, 1])) ||
                CanItEat(board0, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[2, 0], way[2, 1])) ||
                CanItEat(board0, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[3, 0], way[3, 1])) )
                {
                    List<Tuple<int, int>> resWayTemp = CalcCheckersMovesEat1(board0, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), ref temp1, per);
                    resAll = temp1;
                    resWay.Clear();
                    resWay.Add(Tuple.Create(way[k, 0], way[k, 1]));
                    for (int l = 0; l < resWayTemp.Count; l++)
                        resWay.Add(resWayTemp[l]); 

                }
                else
                {
                    resAll = temp0;
                    resWay.Clear();
                    resWay.Add(Tuple.Create(way[k, 0], way[k, 1]));
                }
                if (resAll > res)
                {
                    res = resAll;
                    resWayEnd.Clear();
                    for (int l = 0; l < resWay.Count; l++)
                        resWayEnd.Add(resWay[l]);
                }
            }
        }
        return resWayEnd;
    }
    public static Tuple<int, int> CalcCheckersMoves1(Cell[,] board, Tuple<int, int> id, ref double res, PerceptronForCheckers per)
    {
        Cell[,] board0 = new Cell[8, 8];
        InitBoard(board0);
        int[] board32 = new int[32];
        Tuple<int, int> bestMove = Tuple.Create(0, 0);
        double temp = -1;
        res = -1;
        for (int k = 0; k < 4; k++)
        {
            Copy(board, board0);
            if(CanItMove(board0, id, Tuple.Create(way[k, 0], way[k, 1])))
            {
                ItMove(board0, id, Tuple.Create(way[k, 0], way[k, 1]));
                board32 = From64To32(board0);
                per.train(board32);
                double temp0 = per.res;
                if(temp0>temp)
                {
                    temp = temp0;
                    bestMove = Tuple.Create(way[k, 0], way[k, 1]);
                }
            }
        }
        res = temp;
        return bestMove;
    }
    public static List<Tuple<int, int>> CalcCheckersMovesEat(Cell[,] board, Tuple<int, int> id, ref double res, PerceptronForCheckers per)
    {
        Cell[,] board0 = new Cell[8, 8];
        InitBoard(board0);
        List<Tuple<int, int>> resWay = new List<Tuple<int, int>>();
        List<Tuple<int, int>> resWayEnd = new List<Tuple<int, int>>();
        res = -1;
        double res_temp = -1;
        double resAll;
        for (int k = 0; k < 4; k++)
        {
            Copy(board, board0);
            if (CanItEat(board0, Tuple.Create(id.Item1, id.Item2), Tuple.Create(way[k, 0], way[k, 1])))
            {
                TypeCheck tempType;
                if ((board0[id.Item1, id.Item2].typeCheck == TypeCheck.white) || (board0[id.Item1, id.Item2].typeCheck == TypeCheck.black))
                    tempType = board0[id.Item1, id.Item2].typeCheck;
                else tempType = (TypeCheck)(int)(0.5*(int)(board0[id.Item1, id.Item2].typeCheck));
                ItEat(board0, Tuple.Create(id.Item1, id.Item2), Tuple.Create(way[k, 0], way[k, 1]));
                Cell[,] board1 = new Cell[8, 8];
                InitBoard(board1);
                Copy(board0, board1);
                int s = 0;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                            if ((board0[i, j].typeCheck == (TypeCheck)((-1) * (int)tempType)) || (board0[i, j].typeCheck == (TypeCheck)((-2) * (int)tempType)))
                                s++;
                    }
                if(s == 0)
                {
                    res = 1;
                    resWayEnd.Clear();
                    resWayEnd.Add(Tuple.Create(way[k, 0], way[k, 1]));
                    break;

                }
                BestNetworkMove(board0, ref res_temp, per, (TypeCheck)((-1) * (int)tempType));
                s = 0;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                            if ((board0[i, j].typeCheck == tempType) || (board0[i, j].typeCheck == (TypeCheck)((2) * (int)tempType)))
                                s++;
                    }
                if (s == 0)
                {
                    res = 0;
                    resWayEnd.Clear();
                    resWayEnd.Add(Tuple.Create(way[k, 0], way[k, 1]));
                    continue;
                }
                BestNetworkMove(board0, ref res_temp, per, tempType);
                if (CanItEat(board1, Tuple.Create(id.Item1 + 2*way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[0, 0], way[0, 1])) ||
                CanItEat(board1, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[1, 0], way[1, 1])) ||
                CanItEat(board1, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[2, 0], way[2, 1])) ||
                CanItEat(board1, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), Tuple.Create(way[3, 0], way[3, 1])))
                {
                    double temp=-1;
                    List<Tuple<int, int>> resWayTemp = CalcCheckersMovesEat(board1, Tuple.Create(id.Item1 + 2 * way[k, 0], id.Item2 + 2 * way[k, 1]), ref temp, per);
                    resAll = temp;
                    resWay.Clear();
                    resWay.Add(Tuple.Create(way[k, 0], way[k, 1]));
                    for (int l = 0; l < resWayTemp.Count; l++)
                        resWay.Add(resWayTemp[l]);

                }
                else
                {
                    resAll = res_temp;
                    resWay.Clear();
                    resWay.Add(Tuple.Create(way[k, 0], way[k, 1]));
                }
                if (resAll > res)
                {
                    res = resAll;
                    resWayEnd.Clear();
                    for (int l = 0; l < resWay.Count; l++)
                        resWayEnd.Add(resWay[l]);
                }
            }
        }
        return resWayEnd;
    }
    public static Tuple<int, int> CalcCheckersMove(Cell[,] board, Tuple<int, int> id, ref double res, PerceptronForCheckers per)
    {
        Cell[,] board0 = new Cell[8, 8];
        InitBoard(board0);
        double resAll=-1;
        Tuple<int, int> bestMove = Tuple.Create(0, 0);
        res = -1;
        for (int k = 0; k < 4; k++)
        {
            Copy(board, board0);
            if (CanItMove(board0, id, Tuple.Create(way[k, 0], way[k, 1])))
            {
                TypeCheck tempType;
                if ((board0[id.Item1, id.Item2].typeCheck == TypeCheck.white) || (board0[id.Item1, id.Item2].typeCheck == TypeCheck.black))
                    tempType = board0[id.Item1, id.Item2].typeCheck;
                else tempType = (TypeCheck)(int)(0.5 * (int)(board0[id.Item1, id.Item2].typeCheck));
                ItMove(board0, id, Tuple.Create(way[k, 0], way[k, 1]));
                double res_temp = -1;
                BestNetworkMove(board0, ref res_temp, per, (TypeCheck)((-1) * (int)tempType));
                int s = 0;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                            if ((board0[i, j].typeCheck == tempType) || (board0[i, j].typeCheck == (TypeCheck)((2) * (int)tempType)))
                                s++;
                    }
                if (s == 0)
                {
                    resAll = 0;
                    bestMove = Tuple.Create(way[k, 0], way[k, 1]);
                    continue;
                }
                BestNetworkMove(board0, ref res_temp, per, tempType);
                if(res_temp>resAll) 
                {
                    resAll = res_temp;
                    bestMove = Tuple.Create(way[k, 0], way[k, 1]);
                }
            }
        }
        res = resAll;
        return bestMove;
    }
}


    

