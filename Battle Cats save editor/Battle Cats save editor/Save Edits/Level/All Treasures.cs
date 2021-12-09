using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class AllTreasures
    {
        public static void MaxTreasures(string path)
        {
            Editor.ColouredText(
                "&What chapter do you want to edit?(You can enter multiple numbers separated by spaces to edit multiple at once):&\n" +
                "&1.& EoC 1\n" +
                "&2.& EoC 2\n" +
                "&3.& EoC 3\n" +
                "&4.& ItF 1\n" +
                "&5.& ItF 2\n" +
                "&6.& ItF 3\n" +
                "&7.& CotC 1\n" +
                "&8.& CotC 2\n" +
                "&9.& CotC 3\n" +
                "&10.& All\n",
                ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] choice = Console.ReadLine().Split(' ');
            Console.WriteLine("What level of treasures of you want?: 0=none, 1=inferior, 2=normal, 3=superior:");
            int level = (int)Editor.Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            if (level > 255) level = 255;

            for (int k = 0; k < choice.Length; k++)
            {
                int choiceInt = int.Parse(choice[k]);
                if (choiceInt <= 3)
                {
                    choiceInt--;
                }
                int j = 0;
                int startPos = 2986;
                int endPos = 4942;
                int ChapterID = 0;
                for (int i = startPos; i <= endPos; i += 4)
                {
                    j++;
                    // If it's not end of the chapter treasure data write data
                    if (j % 49 == 0)
                    {
                        ChapterID++;
                    }
                    else
                    {
                        if (ChapterID == choiceInt || choiceInt == 10)
                        {
                            stream.Position = i;
                            stream.WriteByte((byte)level);
                        }
                    }

                }
                Console.WriteLine("All treasures level {0}", level);
            }
        }
    }
}
