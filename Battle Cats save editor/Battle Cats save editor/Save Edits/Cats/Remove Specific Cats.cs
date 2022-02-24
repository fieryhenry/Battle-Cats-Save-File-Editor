using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class RemoveSpecificCats
    {
        public static void RemSpecifiCat(string path)
        {
            int[] cat_data = GetSpecificCats.GetCats(path);
            Console.WriteLine($"What is the cat ID?, ({Editor.multipleVals})");
            string[] input = Console.ReadLine().Split(' ');
            foreach (string cat in input)
            {
                int id = int.Parse(cat);
                cat_data[id] = 0;
            }
            GetSpecificCats.SetCats(path, cat_data);
            Console.WriteLine("Successfully removed cats");
        }
    }
}
