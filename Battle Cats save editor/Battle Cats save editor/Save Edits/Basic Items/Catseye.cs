using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class Catseye
    {
        public static void Catseyes(string path)
        {
            int[] catseyes = GetCatseyes(path);
            string[] CatseyeTypes =
            {
                "Special Catseyes", "Rare Catseyes", "Super Rare Catseyes", "Uber Super Rare Catseyes", "Legend Catseyes"
            };
            Editor.ColouredText($"&You have:\n&{Editor.CreateOptionsList(CatseyeTypes, catseyes, false)}");
            Editor.ColouredText($"&What do you want to edit?{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(CatseyeTypes)}&{CatseyeTypes.Length+1}.& All at once\n");
            string[] answer = Console.ReadLine().Split(' ');
            foreach (string choice in answer)
            {
                int catseyeID = int.Parse(choice) -1;
                if (catseyeID == CatseyeTypes.Length)
                {
                    Editor.ColouredText($"&What do you want to set your catseyes to?:\n");
                    int val = (int)Editor.Inputed();
                    catseyes = Enumerable.Repeat(val, CatseyeTypes.Length).ToArray();
                }
                else
                {
                    Editor.ColouredText($"&What do you want to set your &{CatseyeTypes[catseyeID]}& to?:\n");
                    int val = (int)Editor.Inputed();
                    catseyes[catseyeID] = val;
                }
            }
            SetCatseyes(path, catseyes);
            Editor.ColouredText($"&Successfully set catseyes to:\n&{Editor.CreateOptionsList(CatseyeTypes, catseyes, false)}");
        }
        public static int[] GetCatseyes(string path)
        {
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);

            int pos = occurrence[8] + (Editor.catAmount * 4) + 8;
            if (pos < 100)
            {
                Editor.Error();
            }
            int catseyeAmount = 5;
            int[] catseyes = Editor.GetItemData(path, catseyeAmount, 4, pos);

            return catseyes;
        }
        public static void SetCatseyes(string path, int[] catseyes)
        {
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);

            int pos = occurrence[8] + (Editor.catAmount * 4) + 8;
            Editor.SetItemData(path, catseyes, 4, pos);
        }
    }
}
