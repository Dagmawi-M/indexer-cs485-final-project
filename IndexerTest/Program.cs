using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace IndexerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"../../Data/test.txt");
            string[,] index = Tokenize(filename);
            string[,] findex = GetRank(index);
            Console.WriteLine("WORD\t\tFREQUENCY\t\tRANK");
            for (int i = 0; i < findex.GetLength(0); i++)
            {
                for (int j = 0; j < findex.GetLength(1); j++)
                    Console.Write(findex[i, j] + "\t\t");
                Console.WriteLine();
            }
            new TestClass();

        }

        static string[,] Tokenize(string filename)
        {
            string[] delimitedIndex = RemoveDelimiters(filename);
            string[] lowercaseIndex = SameCase(delimitedIndex);
            int[] freq = FrequencyOfWord(lowercaseIndex);
            string[] distinctlIndex = Distinctize(lowercaseIndex);
            string[,] finalIndex = new string[freq.Length, 2];

            for (int i = 0; i < freq.Length; i++)
            {
                finalIndex[i, 0] = distinctlIndex[i];
                finalIndex[i, 1] = freq[i].ToString();

            }
            return finalIndex;
        }

        static int[] FrequencyOfWord(string[] index)
        {
            int[] tfreq = new int[index.Length];
            int[] rep = new int[index.Length];
            int nf = 0, nr = 0;
            bool ignore = false;

            foreach (string idx in index)
            {
                int count = 0;
                for (int i = 0; i < index.Length; i++)
                {
                    for (int j = 0; j < nr; j++)
                    {
                        if (i == rep[j])
                        {
                            ignore = true;
                            break;
                        }
                    }
                    if (!ignore)
                    {
                        if (String.Equals(idx, index[i]))
                        {
                            rep[nr] = i;
                            nr++;
                            count++;
                        }
                    }
                    ignore = false;
                }
                tfreq[nf] = count;
                nf++;

            }
            int cnt = 0;
            foreach (int f in tfreq)
            {
                if (f != 0)
                    cnt++;
            }
            int[] freq = new int[cnt];
            int c = 0;
            foreach (int f in tfreq)
            {
                if (f != 0)
                {
                    freq[c] = f;
                    c++;
                }

            }

            return freq;
        }

        static string[] Distinctize(string[] index)
        {
            return index.Distinct().ToArray();
        }

        static string[] RemoveDelimiters(string filename)
        {
            char[] delims = { '.', '!', '?', ',', '(', ')', '\t', '\n', '\r', ' ' };
            string[] words = File.ReadAllText(filename).Split(delims, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            foreach (string word in words)
            {
                list.Add(word);
            }
            return list.ToArray();
        }

        static string[] SameCase(string[] index)
        {
            List<string> list = new List<string>();
            foreach (string idx in index)
            {
                list.Add(idx.ToLower());
            }
            return list.ToArray();
        }

        static string[,] GetRank(string[,] index)
        {
            List<string[]> list = new List<string[]>();
            for (int i = 0; i < index.GetLength(0); i++)
            {
                string[] temp = new string[index.GetLength(1)];
                for (int n = 0; n < temp.Length; n++)
                {
                    temp[n] = index[i, n];
                }
                list.Add(temp);
            }

            list = list.OrderBy(x => double.Parse(x[1])).ToList<string[]>();
      //      list = list.SkipWhile(c => c is null).ToList<string[]>();

            string[,] finalIndex = new string[index.GetLength(0), 2];
            int tmpi = index.GetLength(0) - 1;
            // int tmpi = 0;
            int rank = 1;
            int prev = 0;
            //Console.WriteLine("\n" + "here");
            foreach (string[] str in list)
            {
                //Console.WriteLine(str[0] + "\t" + str[1]);
                if (str != null)
                {
                    finalIndex[tmpi, 0] = str[0];
                    finalIndex[tmpi, 1] = str[1];
                    tmpi--;
                    //tmpi++;
                }
            }
            string[,] finalRankedIndex = new string[index.GetLength(0), 3];

            for (int i = 0; i < index.GetLength(0); i++)
            {
                finalRankedIndex[i, 0] = finalIndex[i, 0];
                finalRankedIndex[i, 1] = finalIndex[i, 1];
                if (prev == 0)
                {
                    prev = Int32.Parse(finalRankedIndex[i, 1]);
                    finalRankedIndex[i, 2] = rank.ToString();
                }
                else if (prev == Int32.Parse(finalRankedIndex[i, 1]))
                {
                    finalRankedIndex[i, 2] = rank.ToString();
                }
                else
                {
                    prev = Int32.Parse(finalRankedIndex[i, 1]);
                    rank++;
                    finalRankedIndex[i, 2] = rank.ToString();
                }
            }

            return finalRankedIndex;

        }

    }
}
