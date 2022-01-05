using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class CloseBundle
    {
        public static void Bundle(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            bool found = false;

            // Search for bundle counter position
            for (int i = 0; i < length - 32; i++)
            {
                if (allData[i] == 0x31 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 0 && allData[i + 4] == 0x32 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0x33 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 0)
                {
                    stream.Position = i - 4;
                    // Set total counter for bundle menus seen to 65535, stopping the game from opening any more
                    stream.WriteByte(0xff);
                    stream.WriteByte(0xff);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Editor.Error();
            }
            Console.WriteLine("Closed all bundle menus");

        }
    }
}
