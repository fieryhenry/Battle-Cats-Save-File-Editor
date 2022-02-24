using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class EvolveCurrentCats
    {
        public static void EvolveCurrent(string path)
        {
            int[] curr_cats = CatHandler.GetCurrentCats(path);
            List<int> forms = EvolveCats.GetEvolveForms(path).ToList();
            List<int> max_forms = Enumerable.Repeat(0, 9).ToList();

            max_forms.AddRange(Editor.EvolvedFormsGetter().ToList());
            for (int i = 0; i < curr_cats.Length; i++)
            {
                if (curr_cats[i] == 1 && max_forms[i] == 2)
                {
                    forms[i] = 2;
                }
            }
            EvolveCats.SetEvolveForms(path, forms.ToArray());
            Console.WriteLine("Successfully set evolve forms");
        }
    }
}
