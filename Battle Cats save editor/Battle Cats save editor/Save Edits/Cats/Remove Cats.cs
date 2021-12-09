using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class RemoveCats
    {
        public static void RemCats(string path)
        {
            int[] occurrence = Editor.OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            for (int i = occurrence[0] + 4; i <= occurrence[1] - 4; i += 4)
            {
                stream.Position = i;
                stream.WriteByte(Convert.ToByte(0));
            }
            Console.WriteLine("Removed all cats");
        }
    }
}
