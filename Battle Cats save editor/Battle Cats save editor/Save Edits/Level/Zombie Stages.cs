using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class ZombieStages
    {
        public static void Outbreaks(string path)
        {
            // Length of first outbreak chunk
            int len = 237;
            byte[] toSearch = new byte[len];
            byte[] choice = new byte[len];
            // Generate search terms
            for (int i = 0; i < 47; i++)
            {
                toSearch[(i * 5) + 1] = (byte)(i + 1);
                choice[i * 5] = 1;
            }
            toSearch[236] = 0x01;
            choice[235] = 0x01;
            int pos = Editor.GetPlatinumTicketPos(path)[0];

            bool found = false;
            int StartPos = 0;

            // Search for outbreak position
            int pos2 = Editor.Search(path, toSearch, false, pos, choice)[0];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            if (pos2 > 0)
            {
                found = true;
                StartPos = pos2;
            }
            if (!found || StartPos < 100)
            {
                Editor.Error();
            }
            for (int j = 0; j < length; j++)
            {
                stream.Position = StartPos + (j * 5);
                stream.WriteByte(1);
                // If it reaches the end of a chapter, skip forward to the next one and write some data
                if (allData[StartPos + (j * 5) + 10] == 0x30)
                {
                    StartPos += 5;
                    stream.Position = StartPos + (j * 5);
                    stream.WriteByte(1);
                    if (allData[StartPos + (j * 5) + 1] >= 0x07)
                    {
                        break;
                    }
                    StartPos += 8;
                }
                else if (allData[StartPos + (j * 5) + 13] >= 0x40)
                {
                    break;
                }
            }
            Console.WriteLine("Successfully cleared all zombie stages");
        }
    }
}