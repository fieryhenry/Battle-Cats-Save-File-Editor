using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class ItFTimedScores
    {
        public static void TimedScore(string path)
        {
            Console.WriteLine("What timed score do you want? (max 9999)");
            int score = (int)Editor.Inputed();
            if (score > 9999) score = 9999;
            byte[] scoreByte = Editor.Endian(score);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            stream.Close();

            int[] occurance = Editor.OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            // Search for timed score position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x2D && allData[i + 1] == 0x0 && allData[i + 2] == 0x0 && allData[i + 3] == 0x0 && allData[i + 4] == 0x2E)
                {
                    for (int j = 1900; j < 2108; j++)
                    {
                        if (allData[i - j] == 09)
                        {
                            stream2.Position = i - j + 31;
                            found = true;
                        }
                    }
                }
            }
            if (!found)
            {
                Editor.Error();
            }
            // Set each of the 3 ItF chapter's timed scores
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 48; i++)
                {
                    stream2.WriteByte(scoreByte[0]);
                    stream2.WriteByte(scoreByte[1]);
                    stream2.Position += 2;
                }
                stream2.Position += 12;
            }
            Console.WriteLine("Set ItF timed score rewards to: " + score);

        }
    }
}
