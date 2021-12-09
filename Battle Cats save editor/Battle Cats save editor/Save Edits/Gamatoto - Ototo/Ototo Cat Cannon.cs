using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class OtotoCatCannon
    {
        public static void CatCannon(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int pos = 0;

            // Search for cat cannon position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 8 && allData[i + 4] == 0 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 2 && allData[i + 12] == 0 && allData[i + 13] == 0 && allData[i + 14] == 0 && allData[i + 15] == 3 && allData[i + 16] == 0 && allData[i + 17] == 0 && allData[i + 18] == 0)
                {
                    pos = i + 15;
                    break;
                }
            }
            Editor.ColouredText("What cat cannon type do you want to edit?:\n&1.& Imporve Base\n&2.& Develop Slow Beam\n&3.& Develop Iron Wall\n&4.& Develop Thunderbolt\n&5.& Develop Waterblast\n&6.& Develop Holy Blast\n&7.& Develop Breakerblast\n&8.& Develop Curseblast\n&You can edit multiple at once by entering multiple numbers separated by spaces\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] answer = Console.ReadLine().Split(' ');
            for (int i = 0; i < answer.Length; i++)
            {
                int choice = int.Parse(answer[i]);
                int max = 0;

                // Set max upgrade amount
                switch (choice)
                {
                    case 1:
                        max = 20;
                        break;
                    case 2:
                        max = 30;
                        break;
                    case 3:
                        max = 30;
                        break;
                    case 4:
                        max = 30;
                        break;
                    case 5:
                        max = 20;
                        break;
                    case 6:
                        max = 25;
                        break;
                    case 7:
                        max = 30;
                        break;
                    case 8:
                        max = 30;
                        break;
                }
                // Position for the cat cannon is unlocked flag
                int unlockPos = pos + (16 * (choice - 1));
                stream.Position = unlockPos;
                stream.WriteByte(3);

                Console.WriteLine($"What level do you want for {choice}? (max {max})");
                int level = (int)Editor.Inputed();
                if (level == 0)
                {
                    stream.Position = unlockPos;
                    stream.WriteByte(0);
                }
                else if (level > max) level = max;
                else if (level < 1) level = 1;
                level -= 1;

                int levelPos = unlockPos + 4;
                stream.Position = levelPos;
                stream.WriteByte((byte)level);
            }
        }
    }
}
