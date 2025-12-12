using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTrain.IO
{
    public class Loader
    {
        public string BaseDirectory { get; set; } = "../../../../Data";

        public string WordFileAkdamy { get; set; } = "akademy.txt";
        public string WordFileAkdamyTarget { get; set; } = "akademy_target.txt";
        public string WordFileAkdamyAll { get; set; } = "akademy_all.txt";
        public Loader() { }
        public Loader(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
        }
        public void Dryrun()
        {
            FileLogger.WriteLine($"Trying base directory: Exists {Directory.Exists(BaseDirectory)}");
            FileLogger.WriteLine($"Trying File akademy: Exists {File.Exists(GetAkadamyFile())}");
            FileLogger.WriteLine($"Trying File akademy target: Exists {File.Exists(GetAkadamyTargetFile())}");
        }

        public string GetText(string akadamyFile)
        {
            return File.ReadAllText(akadamyFile);
        }
        public string SetText(string akadamyFile, string text)
        {
            File.WriteAllText(akadamyFile,text);
            return akadamyFile;
        }

        public string GetAkadamyFile()
        {
           return Path.Combine(BaseDirectory, WordFileAkdamy);
        }
        public string GetFile(string file)
        {
            return Path.Combine(BaseDirectory, file);
        }
        public string GetAkadamyTargetFile()
        {
            return Path.Combine(BaseDirectory, WordFileAkdamyTarget);
        }

        public string GetAkademyText()
        {
            return GetText(GetAkadamyFile());
        }
        public string GetAkademyTextText()
        {
            return GetText(GetAkadamyTargetFile());
        }
        public string SetAkademyTextText(string text)
        {
            return SetText(GetAkadamyTargetFile(), text);
        }
    }
}
