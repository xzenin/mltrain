using NetTrain.IO;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace NetTrain
{
    public class PrepareWordDataSet
    {
        Loader loader = new Loader();
        string textfile;
        public List<string> words;
        public Dictionary<string, string> WordDictionary = new Dictionary<string, string>();
        public Dictionary<char, string> subwordIndex = new Dictionary<char, string>();
        public Dictionary<string, string> AdditionalDictionary = new Dictionary<string, string>();
        public PrepareWordDataSet(string file)
        {
            textfile = file;
        }
        public PrepareWordDataSet()
        {
            textfile = loader.WordFileAkdamy;
        }
        public void RunThread()
        {
            FileLogger.WriteLine("Loading..");
            Load(textfile);
            FileLogger.WriteLine("Loading..");
            FileLogger.WriteLine("Creating Letter index..");
            CreateIndex();
            FileLogger.WriteLine("Created Letter index..");
            DropSimilar();
            FileLogger.WriteLine("Dropped words by Letter index..");
            AddVariant();
            FileLogger.WriteLine("Writing file Letter index..");

            FileLogger.WriteLine("Done..");
        }
        public Dictionary<string,string> GetMegredWords()
        {
            foreach (var word in AdditionalDictionary)
            {
                if (!WordDictionary.ContainsKey(word.Key))
                {
                    WordDictionary[word.Key] = word.Value;
                }
            }
            return WordDictionary;
        }
        public void DropSimilar()
        {
            WordDictionary = words.Distinct().ToDictionary(x => x, y => y);
            int dsf = words.Where(x => x.StartsWith("অংশ")).Count();
            int maxchars = words.Max(x => x.Length);
            FileLogger.WriteLine($"Total staring words {words.Count()}");
           
         
            var relatedWords = words.Where(x => x.Length < 7 ).OrderBy(x => x.Length).ToList();
            FileLogger.WriteLine($"Total words on for {relatedWords.Count()}");

            foreach (var word in relatedWords)
            {
                var keys = WordDictionary.Keys.Where(x => x.StartsWith(word));
                FileLogger.WriteLine($"Similar keys Seed word is {word} key count {keys.Count()}");
                foreach (var key in keys)
                {
                    if (key != word)
                    {
                        WordDictionary.Remove(key);
                    }
                }
            }
            
            FileLogger.WriteLine($"Total remaining words {WordDictionary.Count()}");
        }
        public void AddVariant()
        {
            string[] ikar = new string[] { "কি".Substring(1), "কী".Substring(1)};
            string[] ukar = new string[] { "কু".Substring(1), "কূ".Substring(1) };
            string[] kkk = new string[] { "ক","ক্ষ", "খ" };
            string[] ghh = new string[] { "গ", "ঘ" };
            string[] chh = new string[] { "চ", "ছ" };
            string[] jhh = new string[] { "জ", "ঝ", "য়", "য" };
            string[] thh = new string[] { "ট", "ঠ", "ত", "থ" };
            string[] dhh = new string[] { "ড", "ঢ" };
            string[] nn = new string[] { "ন", "ণ", "ঙ", "ঞ", "ং", "ঙ্গ" };

            string[] phh = new string[] { "প", "ফ" };
            string[] bhh = new string[] { "ব","ভ" };
            string[] mng = new string[] { "ম", "ঙ", "ঞ", "ং" };

            string[] rrh = new string[] { "র", "ড়", "ঢ়" };
            string[] ssh = new string[] { "স", "শ", "ষ" };

            List<string[]> equivalents = new List<string[]> { ikar , ukar,  kkk , ghh ,  chh , jhh ,  thh ,  dhh ,  nn ,  phh,  bhh, mng ,  rrh , ssh };


           
            FileLogger.WriteLine($"Total staring words {WordDictionary.Count()}");
            foreach (var word in WordDictionary)
            {
                foreach (var equiSet in equivalents)
                {
                    HashSet<string> dones = new HashSet<string>();    
                    foreach (var eq in equiSet)
                    {
                        dones.Add(eq);
                        var others = equiSet.Where(x=> x!=eq).ToList();
                        foreach (var other in others)
                        {
                            if (!dones.Contains(other))
                            {
                                var key = word.Key.Replace(eq, other);
                                if (key != word.Key)
                                {
                                    AdditionalDictionary[key] = word.Value;
                                }
                            }
                        }
                    }

                }
            }            
            FileLogger.WriteLine($"Total additional words {AdditionalDictionary.Count()}");
        }
        public void Load(string file)
        {  
            var moretexts = loader.GetText(loader.GetFile(file));
            words = moretexts.Split(",").Where(x => x.Length > 1).Select(x => x.Replace("\"","")
                    .Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\\", "").Replace("r","")
                    .Trim()).ToList();
            words = words.Where(x=>x.Length > 0).ToList();
        }
        public void CreateIndex()
        {
            foreach (var word in words)
            {
                char[] chars = word.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] > 1000)
                    {
                        subwordIndex[chars[i]] = word;
                    }
                }
            }
        }
        public void WriteLine(string s)
        {
            FileLogger.WriteLine(s);
            Debug.WriteLine(s);
        }
    }
}
