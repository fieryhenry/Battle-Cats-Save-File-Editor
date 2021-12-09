using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class StoriesofLegend
    {
        public static void SoL(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            // Stars unlocked/unlock next chapter
            long unlock = 0;
            // Times beaten/graphical fix
            long levels = 0;
            // Levels beaten/unlock next chapter/levels
            long levsBeaten = 0;

            // Search for SoL positions
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 5 && allData[i + 1] == 0x2c && allData[i + 2] == 1 && allData[i + 3] == 4 && allData[i + 4] == 0x0c)
                {
                    levsBeaten = i + 6005;
                    levels = i + 12005;
                }
                else if (allData[i] == 0x2C && allData[i + 1] == 01 && allData[i + 2] == 0 && allData[i - 1] == 0 && allData[i + 3] == 0 && allData[i - 2] == 0 && allData[i - 3] == 0)
                {
                    unlock = i;
                    break;
                }
            }
            if (levels == 0 || unlock == 0 || stream.Position == 0)
            {
                Console.WriteLine("Sorry your SoL position couldn't be found, you are either using an old save or the editor is bugged - if that is the case please contact me on discord or in #bug-reports");
                return;
            }
            Editor.ColouredText("&What subchapter do you want to edit?, enter an id starting at &1& = &Legend Begins&, &2& = &Passion land& etc, you can enter multiple ids seperated by spaces, e.g &1 5 4 7&, or you can enter 2 ids seperated by a &-& to edit a range of" +
                " chapters, e.g &1&-&7&, or you can enter &all& to edit all subchapters at once\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string input = Console.ReadLine();
            int totalChapters = 49;
            List<int> chaptersToEdit = new();
            if (input.Contains("-"))
            {
                int start = int.Parse(input.Split('-')[0]);
                int end = int.Parse(input.Split('-')[1]);
                for (int i = start; i <= end; i++)
                {
                    chaptersToEdit.Add(i);
                }
            }
            else if (input.ToLower() == "all")
            {
                int start = 1;
                int end = totalChapters;
                for (int i = start; i < end; i++)
                {
                    chaptersToEdit.Add(i);
                }
            }
            else
            {
                string[] ids = input.Split(' ');
                int[] idsInt = Array.ConvertAll(ids, int.Parse);
                chaptersToEdit.AddRange(idsInt);
            }
            Editor.ColouredText("&Do you want to set all of the stars/crowns at the same time (&1&), or individually (&2&)?\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string sameOrIndividual = Console.ReadLine();
            int stars = 0;
            // Same
            if (sameOrIndividual == "1")
            {
                Editor.ColouredText("&How many stars/crowns do you want to complete for each chapter (&1&-&4&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                stars = (int)Editor.Inputed();
            }
            for (int i = 0; i < chaptersToEdit.Count; i++)
            {
                // Individual
                if (sameOrIndividual == "2")
                {
                    Editor.ColouredText($"&How many stars/crowns do you want to complete for chapter &{chaptersToEdit[i]}&? (&1&-&4&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    stars = (int)Editor.Inputed();
                }
                // Levels beaten, required for next chapter to unlock
                int id = chaptersToEdit[i] - 1;
                stream.Position = levsBeaten + (id * 4);
                for (int j = 0; j < stars; j++)
                {
                    stream.WriteByte(8);
                }
                // Stars/crowns unlocked and required for next chapter to unlock
                stream.Position = unlock - 6152 + ((id + 1) * 4);
                stream.WriteByte(3);
                // Times stage has been beaten, required to avoid graphical issues
                stream.Position = levels + (id * 97) - id;
                long startpos = stream.Position;
                for (int j = 0; j < stars; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        stream.WriteByte(1);
                        stream.Position += 7;
                    }
                    stream.Position = startpos + (j * 2) + 2;
                }
            }
        }
    }
}
