using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class GamatotoXP
    {
        public static void GamXP(string path)
        {
            Console.WriteLine("How much gamatoto xp do you want?\nLevel bounderies: https://battle-cats.fandom.com/wiki/Gamatoto_Expedition#Level-up_Requirements");

            long amount = Editor.Inputed();
            byte[] bytes = Editor.Endian(amount);
            int[] occurrence = Editor.OccurrenceB(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            int pos = occurrence[8] + (Editor.catAmount * 4) + 53;
            if (pos > 0)
            {
                found = true;
                stream.Position = pos;
            }
            stream.Write(bytes, 0, 4);
            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Editor.Error();
        }
    }
}
