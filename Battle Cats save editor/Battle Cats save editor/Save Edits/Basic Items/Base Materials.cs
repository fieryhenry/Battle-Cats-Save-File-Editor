using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class BaseMaterials
    {
        public static void BaseMats(string path)
        {
            Console.WriteLine("How many Base Materials do you want(max 65535)");
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int pos = 0;
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 8 && allData[i + 4] == 0 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 2 && allData[i + 12] == 0 && allData[i + 13] == 0 && allData[i + 14] == 0 && allData[i + 15] == 3 && allData[i + 16] == 0 && allData[i + 17] == 0 && allData[i + 18] == 0)
                {
                    pos = i - 46;
                    break;
                }
            }
            if (pos < 200)
            {
                Console.WriteLine("Your base mats position couldn't be found, please report this on the discord");
                return;
            }
            string[] types = { "Brick", "Feather", "Coal", "Sprocket", "Gold", "Meteorite", "Beast Bone", "Ammonite" };
            Editor.ColouredText("&What base material type do you want to edit, you can enter multiple ids separated by spaces&\n&1.& " +
                "Bricks\n&2.& Feathers\n&3.& Coal\n&4.& Sprockets\n&5.& Gold\n&6.& Meteorite\n&7.& Beast Bones\n&8.& Ammonite\n&9.& All materials at once\n");
            string[] answer = Console.ReadLine().Split(' ');
            for (int i = 0; i < answer.Length; i++)
            {
                int id = int.Parse(answer[i]);
                if (id == 9)
                {
                    Console.WriteLine("How much of each material do you want?(max 65535)");
                    int platCatTickets = (int)Editor.Inputed();

                    if (platCatTickets > 65535) platCatTickets = 65535;
                    else if (platCatTickets < 0) platCatTickets = 0;

                    byte[] bytes = Editor.Endian(platCatTickets);

                    for (int j = 0; j < 8; j++)
                    {
                        stream.Position = pos + (j * 4);
                        stream.Write(bytes, 0, 4);
                        Editor.ColouredText($"&Set current amount of &{types[j]}& to &{platCatTickets}&\n");
                    }
                }
                else
                {
                    id -= 1;

                    Console.WriteLine($"How much {types[id]} do you want?(max 65535)");
                    int platCatTickets = (int)Editor.Inputed();

                    if (platCatTickets > 65535) platCatTickets = 65535;
                    else if (platCatTickets < 0) platCatTickets = 0;

                    byte[] bytes = Editor.Endian(platCatTickets);

                    stream.Position = pos + (id * 4);
                    stream.Write(bytes, 0, 4);

                    Editor.ColouredText($"&Set current amount of &{types[id]}& to &{platCatTickets}&\n");
                }
            }
        }
    }
}
