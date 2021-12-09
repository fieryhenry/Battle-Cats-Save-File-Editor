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

            Editor.ColouredText("What is the cat id? (you can input more than 1 to upgrade more than 1 e.g 15 200 78 will select those cats)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] ids = Console.ReadLine().Split(' ');
            int[] idInt = Array.ConvertAll(ids, int.Parse);

            Editor.ColouredText("What base level do you want? (max 50) - (If you inputed more than 1 id before, then input the base upgrades that amount of times e.g if you inputed id 12 56, then you need to input 2 numbers)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] baselevel = Console.ReadLine().Split(' ');
            int[] baseID = Array.ConvertAll(baselevel, int.Parse);

            Editor.ColouredText("What plus level do you want? (max +90) - (If you inputed more than 1 id before, then input the plus levels that amount of times e.g if you inputed id 12 56, then you need to input 2 numbers)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] plusLevel = Console.ReadLine().Split(' ');
            int[] plusID = Array.ConvertAll(plusLevel, int.Parse);

            if (plusID.Length < ids.Length || baselevel.Length < ids.Length)
            {
                Editor.ColouredText("Error: not enough inputs were given", ConsoleColor.White, ConsoleColor.Red);
                SpecifUpgrade(path);
            }
            else if (plusID.Length > ids.Length || baselevel.Length > ids.Length)
            {
                Editor.ColouredText("Error: too many inputs were given", ConsoleColor.White, ConsoleColor.Red);
                SpecifUpgrade(path);
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
                        stream.WriteByte((byte)plusID[i]);
                        stream.Position++;
                        stream.WriteByte((byte)((byte)baseID[i] - 1));
                    }
                    break;
                }
            }
            for (int i = 0; i < ids.Length; i++)
            {
                Editor.ColouredText("Upgraded cat &" + ids[i] + "& to level &" + baseID[i] + "& +&" + plusID[i] + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            }
        }
    }
}
