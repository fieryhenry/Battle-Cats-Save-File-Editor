using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class CompleteTutorial
    {
        public static void Complete_Tutorial(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int pos = 80;
            if (Editor.gameVer == "jp")
            {
                pos = 79;
            }
            stream.Position = pos;
            stream.WriteByte(1);

            Console.WriteLine("Cleared the tutorial");
        }
    }
}
