using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static Play;
public class PercepScore
{
    public PerceptronForCheckers pers;
    public double score;
    public int coutCheckers = 12;
    public double countPlay = 0;
    public PercepScore(PerceptronForCheckers pers, double score)
    { 
        this.pers = pers;
        this.score = score;
    }
    public void CountCheckers(Cell[,] board,TypeCheck simple_type)
    {
        int temp = 0;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 1)
                    if ((board[i, j].typeCheck == simple_type) || (board[i, j].typeCheck == (TypeCheck)(2*(int)simple_type)))
                        temp++;
            }
        this.coutCheckers= temp;
    }
}
class GeneticAlg
{
    private int percepNumNeuronLin = 32;
    private int[] percepNumNeuronL = { 40, 10 };
    private int percepNumNeuronLout = 1;
    private int sizeP;
    List<PercepScore> population = new List<PercepScore>();
    double averageP = 0;
    public Random rnd = new Random();
    List<PercepScore> populationChild = new List<PercepScore>();


    public GeneticAlg(int sP)
    {
        this.sizeP = sP;
        for (int i = 0; i < sP; i++)
            population.Add(new PercepScore(new PerceptronForCheckers(percepNumNeuronLin, percepNumNeuronL, percepNumNeuronLout), 0));

    }
    public PerceptronForCheckers startAlgoritm()
    {
        int iter = 0;
        int maxInd1 = 0;
        do
        {
            iter++;
            playAllPercep();
            averageP = 0;
            for (int i = 0; i < sizeP; i++)
            {
                averageP += population[i].score;
            }
            averageP /= sizeP * sizeP;
            crossoverTournament(population);
            mutation(population);
            Console.WriteLine("баллы:{0} ", averageP);
            if (iter == 20) break;

        } while (true);
        Console.WriteLine("Количество итераций:{0} ", iter);
        Console.WriteLine("ошибка:{0} ", averageP);
        return population[maxInd1].pers;
    }
    private void crossoverTournament(List<PercepScore> population)
    {
        int p1 = 0;
        int p2 = 0;
        int sizeChild = (int)(sizeP / 2);
        populationChild.Clear();

        crossoverTournamentParents(sizeChild, population);

        for (int k = 0; k < sizeChild; k++)
        {
            p1 = rnd.Next(0, sizeChild - 1);

            p2 = rnd.Next(0, sizeChild - 1);

            populationChild.Add(new PercepScore(new PerceptronForCheckers(percepNumNeuronLin, percepNumNeuronL, percepNumNeuronLout), 0));

            double same = rnd.NextDouble();

            for (int i = 0; i < percepNumNeuronLin; i++)
                for (int j = 0; j < percepNumNeuronL[0]; j++)
                    populationChild[k].pers.w1[i, j] = same * population[p1].pers.w1[i, j] + (1 - same) * population[p2].pers.w1[i, j];

            for (int j = 0; j < percepNumNeuronL[0]; j++)
                populationChild[k].pers.b1[j] = same * population[p1].pers.b1[j] + (1 - same) * population[p2].pers.b1[j];

            for (int i = 0; i < percepNumNeuronL[0]; i++)
                for (int j = 0; j < percepNumNeuronL[1]; j++)
                    populationChild[k].pers.w2[i, j] = same * population[p1].pers.w2[i, j] + (1 - same) * population[p2].pers.w2[i, j];

            for (int j = 0; j < percepNumNeuronL[1]; j++)
                populationChild[k].pers.b2[j] = same * population[p1].pers.b2[j] + (1 - same) * population[p2].pers.b2[j];

            for (int i = 0; i < percepNumNeuronL[1]; i++)
                for (int j = 0; j < percepNumNeuronLout; j++)
                    populationChild[k].pers.wout[i, j] = same * population[p1].pers.wout[i, j] + (1 - same) * population[p2].pers.wout[i, j];

            for (int j = 0; j < percepNumNeuronLout; j++)
                populationChild[k].pers.bout[j] = same * population[p1].pers.bout[j] + (1 - same) * population[p2].pers.bout[j];

        }

        population.AddRange(populationChild);
        sizeP += sizeChild;
        for (int i = 0; i < 10; i++)
        {
            int rndId = rnd.Next(0, sizeP - 1);
            PercepScore temp = new PercepScore(new PerceptronForCheckers(percepNumNeuronLin, percepNumNeuronL, percepNumNeuronLout), 0);
            population[rndId] = temp;
        }

    }
    public void crossoverTournamentParents(int sizeChild, List<PercepScore> population)
    {
        for (int i = population.Count - 1; i >= 1; i--)
        {
            int j = rnd.Next(i + 1);
            var temp = population[j];
            population[j] = population[i];
            population[i] = temp;
        }
        for (int i = 0; i < sizeChild; i++)
        {
            if (population[i].score > population[i + 1].score)
                population.RemoveAt(i + 1);
            else
                population.RemoveAt(i);
            sizeP--;
        }
    }
    
    private void playAllPercep()
    {
        for (int i = 0; i < sizeP; i++)
        {
            population[i].score = 0;
            population[i].countPlay = 0;
        }
        for (int i=0; i<sizeP; i++)
            for(int j=0; j<5;  j++)
            {
                int id;
                do { id = rnd.Next(0, sizeP - 1); }
                while (id == i);
                playPercep(population[i], population[id]);
            }
        for (int i = 0; i < sizeP; i++)
        {
            population[i].score = population[i].score/(double)population[i].countPlay;
        }
    }

    private void playPercep(PercepScore percepScore1,  PercepScore percepScore2)
    {
        int CountQueenEmptyMove = 0;
        bool BlockMove = false;
        Cell[,] board = new Cell[8, 8];
        StartInitBoard(board);
        List<Tuple<int, int>> a = new List<Tuple<int, int>>();
        while (true)
        {
            double resNetwork=-1;
            a = AllCheckEat(board, TypeCheck.white, TypeCheck.queen_white);
            List<Tuple<int, int>> resWayEnd = new List<Tuple<int, int>>();
            Tuple<int, int> bestMove = Tuple.Create(0, 0);
            Tuple<int, int> id = Tuple.Create(0, 0);
            if (a.Count != 0)
            {
                double temp = -1;
                for(int k= 0; k<a.Count;k++)
                {
                    List<Tuple<int, int>> resWayTemp = CalcCheckersMovesEat(board, Tuple.Create(a[k].Item1, a[k].Item2), ref temp, percepScore1.pers);
                    if(temp>resNetwork)
                    {
                        resNetwork= temp;
                        resWayEnd.Clear();
                        for(int i=0;i< resWayTemp.Count();i++)
                            resWayEnd.Add(resWayTemp[i]);
                        id = Tuple.Create(a[k].Item1, a[k].Item2);
                    }
                }
                if(resWayEnd.Count != 0)
                {
                    for (int k = 0; k < resWayEnd.Count; k++)
                    {
                        ItEat(board, id, resWayEnd[k]);
                        id = Tuple.Create(id.Item1 + resWayEnd[k].Item1, id.Item2 + resWayEnd[k].Item2);
                    }
                }
                percepScore1.CountCheckers(board, TypeCheck.white);
                percepScore2.CountCheckers(board, TypeCheck.black);
                CountQueenEmptyMove = 0;
            }
            else
            {
                bool flag = true;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                        {
                            if ((board[i, j].typeCheck == TypeCheck.white) || (board[i, j].typeCheck == TypeCheck.queen_white))
                            {
                                double temp = -1;
                                Tuple<int, int> resWayTemp = CalcCheckersMove(board, Tuple.Create(i, j), ref temp, percepScore1.pers);
                                if (temp > resNetwork)
                                {
                                    flag = false;
                                    resNetwork = temp;
                                    bestMove = Tuple.Create(resWayTemp.Item1, resWayTemp.Item2);
                                    id = Tuple.Create(i, j);
                                }
                            }
                        }
                            
                    }
                if(flag)  BlockMove=true;

                if (board[id.Item1,id.Item2].typeCheck == TypeCheck.queen_white)
                {
                    CountQueenEmptyMove += 1;
                }
                else CountQueenEmptyMove = 0;
                ItMove(board, id, bestMove);
            }
            if(percepScore1.coutCheckers==0)
            {
                percepScore1.score += -2;
                percepScore2.score += 1;
                break;
            }
            if (percepScore2.coutCheckers == 0)
            {
                percepScore1.score += 1;
                percepScore2.score += -2;
                break;
            }
            if(CountQueenEmptyMove>=15) 
                break;
            if(BlockMove) 
                break; 
            resNetwork = -1;
            a = AllCheckEat(board, TypeCheck.black, TypeCheck.queen_black);
            resWayEnd.Clear();
            if (a.Count != 0)
            {
                double temp = -1;
                for (int k = 0; k < a.Count; k++)
                {
                    List<Tuple<int, int>> resWayTemp = CalcCheckersMovesEat(board, Tuple.Create(a[k].Item1, a[k].Item2), ref temp, percepScore2.pers);
                    if (temp > resNetwork)
                    {
                        resNetwork = temp;
                        resWayEnd.Clear();
                        for (int i = 0; i < resWayTemp.Count(); i++)
                            resWayEnd.Add(resWayTemp[i]);
                        id = Tuple.Create(a[k].Item1, a[k].Item2);
                    }
                }
                if (resWayEnd.Count != 0)
                {
                    for (int k = 0; k < resWayEnd.Count; k++)
                    {
                        ItEat(board, id, resWayEnd[k]);
                        id = Tuple.Create(id.Item1 + resWayEnd[k].Item1, id.Item2 + resWayEnd[k].Item2);
                    }
                }
                percepScore1.CountCheckers(board, TypeCheck.white);
                percepScore2.CountCheckers(board, TypeCheck.black);
                CountQueenEmptyMove = 0;
            }
            else
            {
                bool flag = true;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i + j) % 2 == 1)
                        {
                            if ((board[i, j].typeCheck == TypeCheck.black) || (board[i, j].typeCheck == TypeCheck.queen_black))
                            {
                                double temp = 0;
                                Tuple<int, int> resWayTemp = CalcCheckersMove(board, Tuple.Create(i, j), ref temp, percepScore2.pers);
                                if (temp > resNetwork)
                                {
                                    flag = false;
                                    resNetwork = temp;
                                    bestMove = Tuple.Create(resWayTemp.Item1, resWayTemp.Item2);
                                    id = Tuple.Create(i, j);
                                }
                            }
                        }

                    }
                if (board[id.Item1, id.Item2].typeCheck == TypeCheck.queen_black)
                {
                    CountQueenEmptyMove += 1;
                }
                else CountQueenEmptyMove =0;
                ItMove(board, id, bestMove);
            }
            if (percepScore1.coutCheckers == 0)
            {
                percepScore1.score += -2;
                percepScore2.score += 1;
                break;
            }
            if (percepScore2.coutCheckers == 0)
            {
                percepScore1.score += 1;
                percepScore2.score += -2;
                break;
            }
            if (CountQueenEmptyMove >= 15) break;
            if (BlockMove) break;

        }
        percepScore1.coutCheckers = 12;
        percepScore2.coutCheckers = 12;
        percepScore1.countPlay += 1;
        percepScore2.countPlay += 1;

    }
    public void mutation(List<PercepScore> population)
    {
        int sizeMutation = (int)(sizeP * 0.05);
        double n;
        int znak;
        for (int k = 0; k < sizeMutation; k++)
        {
            for (int i = 0; i < percepNumNeuronLin; i++)
                for (int j = 0; j < percepNumNeuronL[0]; j++)
                {
                    n = rnd.NextDouble() * 0.2/ 10;
                    znak = rnd.Next(0, 2);
                    if (znak == 0) n = 1 - n;
                    else n = 1 + n;
                    population[k].pers.w1[i, j] *= n;

                }
            for (int j = 0; j < percepNumNeuronL[0]; j++)
            {
                n = rnd.NextDouble() * 0.2 / 10;
                znak = rnd.Next(0, 2);
                if (znak == 0) n = 1 - n;
                else n = 1 + n;
                population[k].pers.b1[j] *= n;
            }
            for (int i = 0; i < percepNumNeuronL[0]; i++)
                for (int j = 0; j < percepNumNeuronL[1]; j++)
                {
                    n = rnd.NextDouble() * 0.2/ 10;
                    znak = rnd.Next(0, 2);
                    if (znak == 0) n = 1 - n;
                    else n = 1 + n;
                    population[k].pers.w2[i, j] *= n;
                }
            for (int j = 0; j < percepNumNeuronL[1]; j++)
            {
                n = rnd.NextDouble() * 0.2/ 10;
                znak = rnd.Next(0, 2);
                if (znak == 0) n = 1 - n;
                else n = 1 + n; ;
                population[k].pers.b2[j] *= n;
            }
            for (int i = 0; i < percepNumNeuronL[1]; i++)
                for (int j = 0; j < percepNumNeuronLout; j++)
                {
                    n = rnd.NextDouble() * 0.2 / 10;
                    znak = rnd.Next(0, 2);
                    if (znak == 0) n = 1 - n;
                    else n = 1 + n;
                    population[k].pers.wout[i, j] *= n;
                }
            for (int j = 0; j < percepNumNeuronLout; j++)
            {
                n = rnd.NextDouble() * 0.2/ 10;
                znak = rnd.Next(0, 2);
                if (znak == 0) n = 1 - n;
                else n = 1 + n; ;
                population[k].pers.bout[j] *= n;
            }
        }
    }
}

