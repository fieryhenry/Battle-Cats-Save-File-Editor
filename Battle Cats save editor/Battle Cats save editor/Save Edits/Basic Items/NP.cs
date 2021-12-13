using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class NP
    {
        public static void np(string path)
        {
            Console.WriteLine("How much NP do you want(max 65535)");
            int CatFood = (int)Editor.Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            bool found = false;

            Console.WriteLine("Scan Complete");
            byte[] bytes = Editor.Endian(CatFood);

            for (int j = 0; j < length - 12; j++)
            {
                if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00))
                {
                    stream.Position = j - 5;
                    stream.WriteByte(bytes[0]);
                    stream.Position = j - 4;
                    stream.WriteByte(bytes[1]);
                    Console.WriteLine("Success");
                    found = true;
                }

            }
            if (!found) Editor.Error();
        }
    }
}
