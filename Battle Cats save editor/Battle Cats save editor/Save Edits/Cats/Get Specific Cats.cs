using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class GetSpecificCats
    {
        public static int[] GetCats(string path)
        {
            int cat_amount = Editor.GetCatAmount(path);
            int pos = Editor.GetCatRelatedHackPositions(path)[0] + 4;
            int[] cat_data = Editor.GetItemData(path, cat_amount, 4, pos, false);
            return cat_data;
        }
        public static void SetCats(string path, int[] cat_data)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[0] + 4;
            Editor.SetItemData(path, cat_data, 4, pos);
        }
        public static void SpecifiCat(string path)
        {
            int[] cat_data = GetCats(path);
            Console.WriteLine($"What is the cat ID?, {Editor.multipleVals}");
            string[] input = Console.ReadLine().Split(' ');
            foreach (string cat in input)
            {
                int id = int.Parse(cat);
                cat_data[id] = 1;
            }
            SetCats(path, cat_data);
            Console.WriteLine("Successfully gave cats");
        }
    }
}
