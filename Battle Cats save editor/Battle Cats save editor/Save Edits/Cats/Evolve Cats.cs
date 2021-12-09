using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class EvolveCats
    {
        public static void Evolve(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            stream.Close();
            int[] occurrence = Editor.OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int backVal = 300;
            // Different game version correction
            if (allData[occurrence[5] - backVal - 4] == 0x2c && allData[occurrence[5] - backVal - 3] == 0x01)
            {
                stream2.Position = occurrence[5] + 40;
            }
            else if (allData[occurrence[4] - backVal - 4] == 0x2c && allData[occurrence[4] - backVal - 3] == 0x01)
            {
                stream2.Position = occurrence[4] + 40;
            }
            else
            {
                Console.WriteLine("Error, your evolved cat position couldn't be found, please report this to me on discord");
            }

            int[] form = Editor.EvolvedFormsGetter();
            bool stop = false;
            int t = 0;
            int pos = (int)stream2.Position;
            while (stream2.Position < pos + (Editor.catAmount * 4) - 37 && !stop)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (allData[stream2.Position + i] != 0x01 && allData[stream2.Position + i] != 0 && allData[stream2.Position + i] != 0x02 && allData[stream2.Position + i] != 0x03)
                    {
                        stop = true;
                        break;
                    }
                }
                try
                {
                    stream2.WriteByte((byte)form[t]);
                }
                catch
                {
                    stream2.WriteByte(0);
                }
                stream2.Position += 3;
                t++;
            }
        }
    }
}
