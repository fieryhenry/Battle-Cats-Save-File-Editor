using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class XP
    {
        public static void xp(string path)
        {

            Console.WriteLine("How much XP do you want?(max 99999999)");
            int XP = (int)Editor.Inputed();
            if (XP > 99999999) XP = 99999999;
            else if (XP < 0) XP = 0;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            Console.WriteLine("Set XP to " + XP);

            byte[] bytes = Editor.Endian(XP);
            int startPos = 76;

            // If using jp, xp is stored 1 offset less
            if (Editor.gameVer == "jp")
            {
                startPos = 75;
            }
            stream.Position = startPos;
            stream.Write(bytes, 0, 4);
        }
    }
}
