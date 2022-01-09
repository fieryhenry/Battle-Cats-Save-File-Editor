using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class RestartPack
    {
        public static void GetRestartPack(string path)
        {
            byte[] conditions = { 0xf8, 0x88, 1, 0, 0, 0x68, 0x3c, 0x01 };
            byte[] mult = { 0, 0, 0, 1, 1, 0, 0, 0 };
            int pos = Editor.Search(path, conditions, mult: mult)[0] + 4;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            stream.Position = pos;
            stream.WriteByte(1);
            Console.WriteLine("Successfully gave the restart pack");
        }
    }
}
