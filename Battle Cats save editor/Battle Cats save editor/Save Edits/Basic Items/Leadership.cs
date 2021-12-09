using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class Leadership
    {
        public static void leadership(string path)
        {
            Console.WriteLine("How much leadership do you want(max 32767)");
            int Leadership = (int)Editor.Inputed();
            // Leadership is stored as a signed integer for some reason
            if (Leadership > 32767) Leadership = 32767;

            byte[] conditions = { 0x80, 0x38 };
            // Search for leadership position
            int pos = Editor.Search(path, conditions)[0];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");
            byte[] bytes = Editor.Endian(Leadership);

            stream.Position = pos + 5;
            stream.Write(bytes, 0, 2);
            Console.WriteLine("Success");
        }
    }
}
