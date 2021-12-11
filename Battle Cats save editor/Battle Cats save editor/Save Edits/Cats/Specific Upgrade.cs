using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class SpecificUpgrade
    {
        public static void SpecifUpgrade(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            Console.WriteLine("Enter the cat ids for the cats you want to upgrade(you can enter multiple values separated by spaces to edit multiple at once):");
            int[] idInt = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);

            Console.WriteLine("Do you want to upgrade all of those cats to the same level?(yes/no)");
            string input = Console.ReadLine();

            int[] baseLevels = new int[idInt.Length];
            int[] plusLevels = new int[idInt.Length];
            if (input.ToLower() == "yes")
            {
                Editor.ColouredText($"&What level do you want: &enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\n");
                string level = Console.ReadLine();

                int baselevel = int.Parse(level.Split('+')[0]) - 1;
                int plusLevel = int.Parse(level.Split('+')[1]);

                baseLevels = Enumerable.Repeat(baselevel, baseLevels.Length).ToArray();
                plusLevels = Enumerable.Repeat(plusLevel, plusLevels.Length).ToArray();
            }
            else
            {
                for (int i = 0; i < idInt.Length; i++)
                {
                    Editor.ColouredText($"&What level do you want to upgrade cat {idInt[i]} to: &enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\n");
                    string level = Console.ReadLine();

                    int baselevel = int.Parse(level.Split('+')[0]) - 1;
                    int plusLevel = int.Parse(level.Split('+')[1]);

                    baseLevels[i] = baselevel;
                    plusLevels[i] = plusLevel;
                }
            }
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            bool repeat = true;

            for (int j = 9600; j <= 12000; j++)
            {
                if (allData[j] == 2 && repeat)
                {
                    for (int i = 0; i < idInt.Length; i++)
                    {
                        stream.Position = j + (idInt[i] * 4) + 3;
                        stream.WriteByte((byte)plusLevels[i]);
                        stream.Position++;
                        stream.WriteByte((byte)baseLevels[i]);
                    }
                    break;
                }
            }
            for (int i = 0; i < idInt.Length; i++)
            {
                Editor.ColouredText($"Upgraded cat &{idInt[i]}& to level &{baseLevels[i] + 1}& +&{plusLevels[i]}\n");
            }
            CloseBundle.Bundle(path);
        }
    }
}
