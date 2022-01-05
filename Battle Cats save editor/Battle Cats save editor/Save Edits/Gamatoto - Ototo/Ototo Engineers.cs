using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class OtotoEngineers
    {
        public static void Engineers(string path)
        {
            int engineers = GetEngineers(path);

            Editor.ColouredText($"&You currently have &{engineers}& engineers\n");
            Console.WriteLine("How many engineers do you want? (max 5)");
            engineers = (int)Editor.Inputed();
            if (engineers > 5) engineers = 5;
            else if (engineers < 0) engineers = 0;

            SetEngineers(path, engineers);

            Editor.ColouredText($"&Set current amount of ototo engineers to &{engineers}&\n");
        }
        public static int GetEngineers(string path)
        {
            int pos = Editor.GetOtotoPos(path) - 1;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            stream.Position = pos;
            byte[] engineers = new byte[2];
            stream.Read(engineers, 0, 2);
            return BitConverter.ToInt16(engineers, 0);
        }
        public static void SetEngineers(string path, int engineers)
        {
            int pos = Editor.GetOtotoPos(path) - 1;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            stream.Position = pos;
            byte[] bytes = Editor.Endian(engineers);
            stream.Write(bytes, 0, 2);
        }
    }
}
