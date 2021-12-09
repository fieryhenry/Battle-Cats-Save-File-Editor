using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class BattleItems
    {
        public static void Items(string path)
        {
            Console.WriteLine("\nHow many of each item do you want?(max 3999)");
            int Items = (int)Editor.Inputed();
            if (Items > 3999) Items = 3999;
            if (Items < 0) Items = 0;

            byte[] year = new byte[2];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);


            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] bytes = Editor.Endian(Items);
            int[] occurrence = new int[100];

            try
            {
                year[0] = allData[15];
                year[1] = allData[16];

                if (year[0] != 0x07)
                {
                    year[0] = allData[19];
                    year[1] = allData[20];
                }
                stream.Close();
                occurrence = Editor.OccurrenceE(path, year);
            }
            catch
            {
                stream.Close();
            }
            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            try
            {
                stream2.Position = occurrence[2] - 224;
                for (int i = occurrence[2] - 224; i < occurrence[2] - 203; i += 4)
                {
                    stream2.Position = i;
                    stream2.WriteByte(bytes[0]);
                    stream2.WriteByte(bytes[1]);
                }
            }
            catch
            {
                Editor.ColouredText("You either haven't unlocked battle items yet, or the editor is bugged - if that's true please contact me on discord/the discord server in #bug-reports so I can fix it", ConsoleColor.Red, ConsoleColor.White);
            }

            stream2.Close();
        }
    }
}
