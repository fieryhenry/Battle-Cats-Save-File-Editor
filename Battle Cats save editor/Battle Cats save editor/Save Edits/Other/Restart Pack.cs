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
            byte[] conditions = { 0xf8, 0x88, 0x01 };
            int pos = Editor.Search(path, conditions)[0] + 4;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            stream.Position = pos;
            stream.WriteByte(1);
            Console.WriteLine("Successfully gave the restart pack");
        }
    }
}
