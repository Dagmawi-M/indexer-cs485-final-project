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

         
            // string filename = "C:/c#/Dani.txt";
            string content = File.ReadAllText(filename);
            //string[,] index = Tokenize(content);

            Dictionary < string, int> Tokenizedindex = Tokenize(content);
            Dictionary<string, int> StopwordRmvIndex = RemoveStopwords(content);

            // string[,] findex = GetRank(index);
            Console.WriteLine("************************************************************");
            Console.WriteLine("AFTER TOKENIZATION");
            Console.WriteLine("Number of Index Terms : " + Tokenizedindex.Count());
            Console.WriteLine("************************************************************");
            Console.WriteLine("WORD\t\tFREQUENCY\t\tRANK");

            Dictionary<string, int> SortedTokenizedindex = Tokenizedindex.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            int[] ranks = GetRank(SortedTokenizedindex);
            int idx = 0;
            foreach (var word in SortedTokenizedindex)
            {
                Console.WriteLine(word.Key + "\t\t" + word.Value + "\t\t" + ranks[idx]);
                idx++;
            }


            // for (int i = 0; i < findex.GetLength(0); i++)
            // {
            //     for (int j = 0; j < findex.GetLength(1); j++)
            //         Console.Write(findex[i, j] + "\t\t");
            //     Console.WriteLine();
            // }

            //// new TestClass();

            Console.WriteLine();
            Console.WriteLine("************************************************************");
            Console.WriteLine("AFTER STOPWORD REMOVAL");
            Console.WriteLine("Number of Index Terms : " + StopwordRmvIndex.Count());
            Console.WriteLine("************************************************************");
            Console.WriteLine("WORD\t\tFREQUENCY\t\tRANK");
            Dictionary<string, int> SortedStopwordRmvIndex = StopwordRmvIndex.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            int[] ranksIndex = RankIndex(SortedTokenizedindex, SortedStopwordRmvIndex);
            int idxR = 0;
            foreach (var word in SortedStopwordRmvIndex)
            {
                Console.WriteLine(word.Key +"\t\t" +ranksIndex[idxR]);
                idxR++;
            } 

            Console.ReadKey();
        }

        static Dictionary<string, int> Tokenize(string content)
        {
            string[] delimitedIndex = RemoveDelimiters(content);
            string[] lowercaseIndex = SameCase(delimitedIndex);

            //USing Dictionary (Trial)
            Dictionary<string, int> dictionaryUse = new Dictionary<string, int>();

            for (int i = 0; i < lowercaseIndex.Length; i++)
            {
                if (dictionaryUse.ContainsKey(lowercaseIndex[i]))
                {
                    dictionaryUse[lowercaseIndex[i]] = dictionaryUse[lowercaseIndex[i]] + 1;
                }
                else
                {
                    dictionaryUse.Add(lowercaseIndex[i], 1);
                }
            }
            Dictionary<string, int> ordered = new Dictionary<string, int>();
         
        
            return dictionaryUse;// (x=>x.Value);
        }

        //static string[,] Tokenize(string content)
        //{
        //    string[] delimitedIndex = RemoveDelimiters(content);
        //    string[] lowercaseIndex = SameCase(delimitedIndex);

        //    //USing Dictionary (Trial)
        //    Dictionary<string, int> dictionaryUse = new Dictionary<string, int>();

        //    for (int i = 0; i < lowercaseIndex.Length; i++)
        //    {
        //        if (dictionaryUse.ContainsKey(lowercaseIndex[i]))
        //        {
        //            dictionaryUse[lowercaseIndex[i]] = dictionaryUse[lowercaseIndex[i]] + 1;
        //            //  Console.WriteLine(dictionaryUse["Abebe"]);
        //        }
        //        else
        //        {
        //            dictionaryUse.Add(lowercaseIndex[i], 1);
        //        }
        //    }

        //  //  int[] freq = FrequencyOfWord(lowercaseIndex);
        //   // string[] distinctlIndex = Distinctize(lowercaseIndex);



        //    //string[,] finalIndex = new string[freq.Length, 2];

        //    //for (int i = 0; i < freq.Length; i++)
        //    //{
        //    //    finalIndex[i, 0] = distinctlIndex[i];
        //    //    finalIndex[i, 1] = freq[i].ToString();

        //    //}
        //    return finalIndex;
        //}

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

        static string[] RemoveDelimiters(string content)
        {
            char[] delims = { '.', '!', '?', ',', '(', ')', '\t', '\n', '\r', ' ' };
            string[] words = content.Split(delims, StringSplitOptions.RemoveEmptyEntries);
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

        static int[] GetRank(Dictionary<string,int> index)
        {
            int rank = 1;
            int[] rankArray=new int[index.Count];
            int idx = 0;
            int prev = 0;
           foreach(KeyValuePair<string,int> val in index)
            {
                if(val.Value == prev)
                {
                    rankArray[idx] = rank;
                    idx++;
                }
                else
                {
                    prev = val.Value;
                    rankArray[idx] = rank;
                    rank++;
                    idx++;
                }
            }

            return rankArray;

        }


        //static string RemoveStopwords(string content)
        //{
        //    string[] delimitedIndex = RemoveDelimiters(content);      
        //    string[] lowercaseIndex = SameCase(delimitedIndex); 
        //    string[] distinctlIndex = Distinctize(lowercaseIndex);
  
        //    var found = new Dictionary<string, bool>();

        //    StringBuilder builder = new StringBuilder();

        //    foreach (string currentWord in distinctlIndex)
        //    {

        //        if (!_stops.ContainsKey(currentWord) &&  !found.ContainsKey(currentWord))
        //        {
        //            builder.Append(currentWord).Append(' ');
        //            found.Add(currentWord, true);
        //        }
        //    }
        //    return builder.ToString().Trim();
        //    //  return builder;
        //}

        static  Dictionary<string, int> RemoveStopwords(string content)
        {
            string[] delimitedIndex = RemoveDelimiters(content);
            string[] lowercaseIndex = SameCase(delimitedIndex);
            string[] distinctlIndex = Distinctize(lowercaseIndex);

            Dictionary<string, int> found = new Dictionary<string, int>();

            //     StringBuilder builder = new StringBuilder();
        
            foreach (string currentWord in distinctlIndex)
            {

                if (!_stops.ContainsKey(currentWord) && !found.ContainsKey(currentWord))
                {
                    found.Add(currentWord, 1);
                }
               else if (!_stops.ContainsKey(currentWord) && found.ContainsKey(currentWord))
                {
                     //   found.(currentWord) = found(currentWord) + 1;
                }

            }


            return found;
            //return builder.ToString().Trim();
            //  return builder;
        }




        static Dictionary<string, bool> _stops = new Dictionary<string, bool>
    {
        { "a", true },
        { "about", true },
        { "above", true },
        { "across", true },
        { "after", true },
        { "afterwards", true },
        { "again", true },
        { "against", true },
        { "all", true },
        { "almost", true },
        { "alone", true },
        { "along", true },
        { "already", true },
        { "also", true },
        { "although", true },
        { "always", true },
        { "am", true },
        { "among", true },
        { "amongst", true },
        { "amount", true },
        { "an", true },
        { "and", true },
        { "another", true },
        { "any", true },
        { "anyhow", true },
        { "anyone", true },
        { "anything", true },
        { "anyway", true },
        { "anywhere", true },
        { "are", true },
        { "around", true },
        { "as", true },
        { "at", true },
        { "back", true },
        { "be", true },
        { "became", true },
        { "because", true },
        { "become", true },
        { "becomes", true },
        { "becoming", true },
        { "been", true },
        { "before", true },
        { "beforehand", true },
        { "behind", true },
        { "being", true },
        { "below", true },
        { "beside", true },
        { "besides", true },
        { "between", true },
        { "beyond", true },
        { "bill", true },
        { "both", true },
        { "bottom", true },
        { "but", true },
        { "by", true },
        { "call", true },
        { "can", true },
        { "cannot", true },
        { "cant", true },
        { "co", true },
        { "computer", true },
        { "con", true },
        { "could", true },
        { "couldnt", true },
        { "cry", true },
        { "de", true },
        { "describe", true },
        { "detail", true },
        { "do", true },
        { "done", true },
        { "down", true },
        { "due", true },
        { "during", true },
        { "each", true },
        { "eg", true },
        { "eight", true },
        { "either", true },
        { "eleven", true },
        { "else", true },
        { "elsewhere", true },
        { "empty", true },
        { "enough", true },
        { "etc", true },
        { "even", true },
        { "ever", true },
        { "every", true },
        { "everyone", true },
        { "everything", true },
        { "everywhere", true },
        { "except", true },
        { "few", true },
        { "fifteen", true },
        { "fify", true },
        { "fill", true },
        { "find", true },
        { "fire", true },
        { "first", true },
        { "five", true },
        { "for", true },
        { "former", true },
        { "formerly", true },
        { "forty", true },
        { "found", true },
        { "four", true },
        { "from", true },
        { "front", true },
        { "full", true },
        { "further", true },
        { "get", true },
        { "give", true },
        { "go", true },
        { "had", true },
        { "has", true },
        { "have", true },
        { "he", true },
        { "hence", true },
        { "her", true },
        { "here", true },
        { "hereafter", true },
        { "hereby", true },
        { "herein", true },
        { "hereupon", true },
        { "hers", true },
        { "herself", true },
        { "him", true },
        { "himself", true },
        { "his", true },
        { "how", true },
        { "however", true },
        { "hundred", true },
        { "i", true },
        { "ie", true },
        { "if", true },
        { "in", true },
        { "inc", true },
        { "indeed", true },
        { "interest", true },
        { "into", true },
        { "is", true },
        { "it", true },
        { "its", true },
        { "itself", true },
        { "keep", true },
        { "last", true },
        { "latter", true },
        { "latterly", true },
        { "least", true },
        { "less", true },
        { "ltd", true },
        { "made", true },
        { "many", true },
        { "may", true },
        { "me", true },
        { "meanwhile", true },
        { "might", true },
        { "mill", true },
        { "mine", true },
        { "more", true },
        { "moreover", true },
        { "most", true },
        { "mostly", true },
        { "move", true },
        { "much", true },
        { "must", true },
        { "my", true },
        { "myself", true },
        { "name", true },
        { "namely", true },
        { "neither", true },
        { "never", true },
        { "nevertheless", true },
        { "next", true },
        { "nine", true },
        { "no", true },
        { "nobody", true },
        { "none", true },
        { "nor", true },
        { "not", true },
        { "nothing", true },
        { "now", true },
        { "nowhere", true },
        { "of", true },
        { "off", true },
        { "often", true },
        { "on", true },
        { "once", true },
        { "one", true },
        { "only", true },
        { "onto", true },
        { "or", true },
        { "other", true },
        { "others", true },
        { "otherwise", true },
        { "our", true },
        { "ours", true },
        { "ourselves", true },
        { "out", true },
        { "over", true },
        { "own", true },
        { "part", true },
        { "per", true },
        { "perhaps", true },
        { "please", true },
        { "put", true },
        { "rather", true },
        { "re", true },
        { "same", true },
        { "see", true },
        { "seem", true },
        { "seemed", true },
        { "seeming", true },
        { "seems", true },
        { "serious", true },
        { "several", true },
        { "she", true },
        { "should", true },
        { "show", true },
        { "side", true },
        { "since", true },
        { "sincere", true },
        { "six", true },
        { "sixty", true },
        { "so", true },
        { "some", true },
        { "somehow", true },
        { "someone", true },
        { "something", true },
        { "sometime", true },
        { "sometimes", true },
        { "somewhere", true },
        { "still", true },
        { "such", true },
        { "system", true },
        { "take", true },
        { "ten", true },
        { "than", true },
        { "that", true },
        { "the", true },
        { "their", true },
        { "them", true },
        { "themselves", true },
        { "then", true },
        { "thence", true },
        { "there", true },
        { "thereafter", true },
        { "thereby", true },
        { "therefore", true },
        { "therein", true },
        { "thereupon", true },
        { "these", true },
        { "they", true },
        { "thick", true },
        { "thin", true },
        { "third", true },
        { "this", true },
        { "those", true },
        { "though", true },
        { "three", true },
        { "through", true },
        { "throughout", true },
        { "thru", true },
        { "thus", true },
        { "to", true },
        { "together", true },
        { "too", true },
        { "top", true },
        { "toward", true },
        { "towards", true },
        { "twelve", true },
        { "twenty", true },
        { "two", true },
        { "un", true },
        { "under", true },
        { "until", true },
        { "up", true },
        { "upon", true },
        { "us", true },
        { "very", true },
        { "via", true },
        { "was", true },
        { "we", true },
        { "well", true },
        { "were", true },
        { "what", true },
        { "whatever", true },
        { "when", true },
        { "whence", true },
        { "whenever", true },
        { "where", true },
        { "whereafter", true },
        { "whereas", true },
        { "whereby", true },
        { "wherein", true },
        { "whereupon", true },
        { "wherever", true },
        { "whether", true },
        { "which", true },
        { "while", true },
        { "whither", true },
        { "who", true },
        { "whoever", true },
        { "whole", true },
        { "whom", true },
        { "whose", true },
        { "why", true },
        { "will", true },
        { "with", true },
        { "within", true },
        { "without", true },
        { "would", true },
        { "yet", true },
        { "you", true },
        { "your", true },
        { "yours", true },
        { "yourself", true },
        { "yourselves", true }
    };
        static int[] RankIndex(Dictionary<string, int> origDictionary, Dictionary<string, int> indexedDictionary)
        {
            int[] ranks = new int[indexedDictionary.Count];
            int[] origRank = GetRank(origDictionary);
            int idx = 0;
            int cnt = 0;
            foreach(KeyValuePair<string,int> val in origDictionary)
            {
                if (indexedDictionary.ContainsKey(val.Key))
                {
                    ranks[idx] = origRank[cnt];
                    idx++;
                }
                cnt++;
            }
            return ranks;
        }
    }

 
}
