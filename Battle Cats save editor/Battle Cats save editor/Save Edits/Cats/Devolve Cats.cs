using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class DevolveCats
    {
        public static void DevolveAll(string path)
        {
            List<int> forms = new();
            forms.AddRange(Enumerable.Repeat(0, Editor.GetCatAmount(path)));

            EvolveCats.SetEvolveForms(path, forms.ToArray());
            Console.WriteLine("Successfully removed evolve forms");
        }
        public static void DevolveSpecific(string path)
        {
            List<int> forms = EvolveCats.GetEvolveForms(path).ToList();

            Editor.ColouredText($"&What cats do you want to edit?\nenter the cat release order of the cat:&https://battle-cats.fandom.com/wiki/Cat_Release_Order& {Editor.multipleVals}:\n");
            string[] cats = Console.ReadLine().Split(' ');
            foreach (string cat in cats)
            {
                int cat_id = int.Parse(cat);
                if (cat_id > forms.Count)
                {
                    Console.WriteLine($"Error, cat {cat_id} doesn't exist in your current game version");
                    continue;
                }
                forms[cat_id] = 0;
            }
            EvolveCats.SetEvolveForms(path, forms.ToArray());
            Console.WriteLine("Successfully removed evolve forms");
        }
    }
}
