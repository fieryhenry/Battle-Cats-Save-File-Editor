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
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);
            int[] allCats = Enumerable.Repeat(0, Editor.GetCatAmount(path)).ToArray();
            Editor.SetItemData(path, allCats, 4, occurrence[0] + 4);
            Console.WriteLine("Removed all cats");
        }
    }
}
