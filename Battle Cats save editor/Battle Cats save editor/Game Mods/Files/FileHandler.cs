using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.Game_Mods
{
    public class FileHandler
    {
        public static List<List<string>> ReadStringCSV(string path)
        {
            List<string> csvData = File.ReadAllLines(path).ToList();

            List<List<string>> AllData = new();
            foreach (string line in csvData)
            {
                List<string> split = line.Split(',').ToList();
                if (split.Count > 1)
                {
                    AllData.Add(split);
                }
            }
            return AllData;
        }
        public static List<List<int>> ReadIntCSV(string path)
        {
            List<string> csvData = File.ReadAllLines(path).ToList();

            List<List<int>> AllData = new();
            foreach (string line in csvData)
            {   
                string[] split = line.Split(',');
                if (split.Length > 1)
                {
                    List<int> temp = new();
                    foreach (string value in split)
                    {
                        bool success = int.TryParse(value, out int val);
                        if (success)
                        {
                            temp.Add(val);
                        }
                    }
                    AllData.Add(temp);
                }
            }
            return AllData;
        }
        public static void WriteCSV(List<List<int>> input, string output_path, bool carriage_return=false)
        {
            string output = "";
            foreach (List<int> row in input)
            {
                foreach (int value in row)
                {
                    output += $"{value},";
                }
                if (carriage_return)
                {
                    output += "\r";
                }
                output += "\n";
            }
            File.WriteAllText(output_path, output);
            AddExtraBytes(output_path);
        }
        public static byte[] AddExtraBytes(string path, bool overwrite = true, byte[] allBytes = null)
        {
            if (allBytes == null)
            {
                allBytes = File.ReadAllBytes(path);
            }
            // Make sure file length is divisible by 16, so it encrypts properly
            List<byte> ls = allBytes.ToList();
            int rem = (int)Math.Ceiling((decimal)ls.Count / 16);
            rem *= 16;
            rem -= ls.Count;
            for (int i = 0; i < rem && rem != 16; i++)
            {
                ls.Add((byte)rem);
            }
            if (overwrite)
            {
                File.WriteAllBytes(path, ls.ToArray());
            }
            return ls.ToArray();
        }
    }
}
