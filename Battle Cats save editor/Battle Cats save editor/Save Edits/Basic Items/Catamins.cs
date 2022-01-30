using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class Catamins
    {
        public static void Catamin(string path)
        {
            int[] catamins = GetCatamins(path);

            string[] catamin_types =
            {
                "Catamin A", "Catamin B", "Catamin C"
            };
            Editor.ColouredText($"&You have:\n&{Editor.CreateOptionsList(catamin_types, catamins, false)}");
            Editor.ColouredText($"&What do you want to edit?{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(catamin_types)}");
            string[] answer = Console.ReadLine().Split(' ');
            foreach (string choice in answer)
            {
                int choice_id = int.Parse(choice) -1;
                Editor.ColouredText($"&How many &{catamin_types[choice_id]}s& do you want?:\n");
                int val = (int)Editor.Inputed();
                catamins[choice_id] = val;
                SetCatamins(path, catamins);
            }
            Editor.ColouredText($"&Successfully set catamins to:\n&{Editor.CreateOptionsList(catamin_types, catamins, false)}");
        }
        public static int[] GetCatamins(string path)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[8];
            pos += (Editor.GetCatAmount(path) * 4) + 32;

            int catamin_types = 3;
            int[] catamins = Editor.GetItemData(path, catamin_types, 4, pos);

            if (pos < 100)
            {
                Editor.Error();
            }

            return catamins;
        }
        public static void SetCatamins(string path, int[] catamins)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[8];
            pos += (Editor.GetCatAmount(path) * 4) + 32;

            Editor.SetItemData(path, catamins, 4, pos);
        }
    }
}
