using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class BlueUpgrade
    {
        public static void Blue(string path)
        {
            int[] occurrence = Editor.OccurrenceB(path);
            Console.WriteLine("Do you want to upgrade all the blue upgrades at once? (yes/no)");
            string answer = Console.ReadLine();
            byte[] maxData = { 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x00, 0x00, 0x09, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13 };
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            // Individual upgrades
            if (answer == "no")
            {
                Editor.ColouredText("What do you want to upgrade?\n&1.& Power\n&2.& Range\n&3.& Charge\n&4.& Efficiency\n&5.& Wallet\n&6.& Health\n&7.& Research\n&8.& Accounting\n&9.& Study" +
                    "\n&10.& Energy\nInput more than 1 id to edit more than 1 at a time (separated by spaces)\n");
                string[] input = Console.ReadLine().Split(' ');
                int[] ids = Array.ConvertAll(input, int.Parse);

                Console.WriteLine("What base level do you want? (If you inputed more than 1 id before you must add more than 1 amount e.g if you said 1 4 5 before you need to input 3 numbers now)");
                string[] inputBase = Console.ReadLine().Split(' ');
                int[] idBase = Array.ConvertAll(inputBase, int.Parse);

                Console.WriteLine("What plus level do you want? (If you inputed more than 1 id before you must add more than 1 amount e.g if you said 1 4 5 before you need to input 3 numbers now)");
                string[] inputPlus = Console.ReadLine().Split(' ');
                int[] idPlus = Array.ConvertAll(inputPlus, int.Parse);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                int pos = occurrence[2] + (Editor.catAmount * 4);
                stream.Position = pos;

                // Loop through user entered ids
                for (int i = 0; i < ids.Length; i++)
                {
                    // For some reason there is a gap between the Cat cannon power and the rest of the upgrades
                    if (ids[i] == 1)
                    {
                        stream.Position = pos + (ids[i] * 4);
                    }
                    else
                    {
                        stream.Position = pos + ((ids[i] + 1) * 4);
                    }
                    stream.WriteByte((byte)idPlus[i]);
                    stream.Position++;
                    stream.WriteByte((byte)((byte)idBase[i] - 1));
                }

            }
            // All upgrades at once
            if (answer == "yes")
            {
                stream.Position = occurrence[2] + (Editor.catAmount * 4) + 4;
                stream.Write(maxData, 0, maxData.Length);
            }
            Console.WriteLine("Success");
        }
    }
}
