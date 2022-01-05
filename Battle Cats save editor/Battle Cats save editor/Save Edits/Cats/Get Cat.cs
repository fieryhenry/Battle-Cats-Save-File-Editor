using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class GetCat
    {
        public static void Cats(string path)
        {
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int ids = 0;
            for (int i = occurrence[0] + 4; i <= occurrence[1] - 4; i += 4)
            {
                if (ids != 542)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(01));
                }
                ids++;
            }
            Console.WriteLine("Gave all cats");

        }
    }
}
