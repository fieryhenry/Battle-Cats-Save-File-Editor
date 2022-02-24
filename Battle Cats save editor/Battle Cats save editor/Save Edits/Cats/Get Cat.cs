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
            int[] allCats = Enumerable.Repeat(1, Editor.GetCatAmount(path)).ToArray();
            Editor.SetItemData(path, allCats, 4, occurrence[0] + 4);
            Console.WriteLine("Gave all cats");
        }
    }
}
