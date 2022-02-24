using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class EvolveCats
    {
        public static void EvolveAll(string path)
        {
            List<int> forms = new();
            forms.AddRange(Enumerable.Repeat(0, 9));
            forms.AddRange(Editor.EvolvedFormsGetter().ToList());

            SetEvolveForms(path, forms.ToArray());
            Console.WriteLine("Successfully set evolve forms");
        }
        public static void EvolveSpecific(string path)
        {
            List<int> forms = GetEvolveForms(path).ToList();

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
                forms[cat_id] = 2;
            }
            SetEvolveForms(path, forms.ToArray());
            Console.WriteLine("Successfully set evolve forms");
        }
        public static int[] GetEvolveForms(string path)
        {
            int pos = Editor.GetEvolvePos(path) - 36;
            int[] evolve_forms = Editor.GetItemData(path, Editor.GetCatAmount(path), 4, pos, false);
            return evolve_forms;
        }
        public static void SetEvolveForms(string path, int[] forms)
        {
            int pos = Editor.GetEvolvePos(path) - 36;
            Editor.SetItemData(path, forms, 4, pos, Editor.GetCatAmount(path));
        }
    }
}
