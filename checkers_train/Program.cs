using System;

namespace checkers_train
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GeneticAlg a = new GeneticAlg(30);
            PerceptronForCheckers res = a.startAlgoritm();
            using (StreamWriter writer = new StreamWriter("weights.txt"))
            {
                for(int i = 0; i < 32; i++)
                    for (int j = 0; j < 40; j++)
                    {
                        writer.WriteLine(res.w1[i,j]);
                    }
                for (int i = 0; i < 40; i++)
                    for (int j = 0; j < 10; j++)
                    {
                        writer.WriteLine(res.w2[i, j]);
                    }
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 1; j++)
                    {
                        writer.WriteLine(res.wout[i, j]);
                    }
                    for (int j = 0; j < 40; j++)
                    {
                        writer.WriteLine(res.b1[j]);
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        writer.WriteLine(res.b2[j]);
                    }
                    for (int j = 0; j < 1; j++)
                    {
                        writer.WriteLine(res.bout[j]);
                    }
            }
        }
    }
}