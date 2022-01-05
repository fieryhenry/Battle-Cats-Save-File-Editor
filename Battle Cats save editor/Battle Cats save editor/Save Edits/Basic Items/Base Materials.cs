using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class BaseMaterials
    {
        public static void BaseMats(string path)
        {
            string[] types = { "Bricks", "Feathers", "Coal", "Sprockets", "Gold", "Meteorites", "Beast Bones", "Ammonites" };
            int[] baseMaterials = GetBaseMats(path);

            Editor.ColouredText($"&You have:\n&{Editor.CreateOptionsList(types, baseMaterials, false)}");

            Editor.ColouredText($"&What do you want to edit?{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(types)}&{types.Length+1}. &All at once&\n");
            string[] answer = Console.ReadLine().Split(' ');
            int[] baseMaterialsAmounts = baseMaterials;
            for (int i = 0; i < answer.Length; i++)
            {
                int choice = int.Parse(answer[i]);
                if (choice > 9)
                {
                    Console.WriteLine("Answer must be between 1 and 10");
                    BaseMats(path);
                }
                else if (choice == 9)
                {
                    Console.WriteLine("How many of each base material do you want?");
                    int amount = (int)Editor.Inputed();
                    baseMaterialsAmounts = Enumerable.Repeat(amount, types.Length).ToArray();
                    break;
                }
                else
                {
                    Editor.ColouredText($"&How many &{types[choice - 1]}& do you want?\n");
                    int amount = (int)Editor.Inputed();
                    baseMaterialsAmounts[choice - 1] = amount;
                }
            }
            SetBaseMats(path, baseMaterialsAmounts);
            Console.WriteLine("Successfuly gave base materials");
            
        }
        public static int[] GetBaseMats(string path)
        {
            int pos = Editor.GetOtotoPos(path) - 46;
            int types = 8;

            int[] materials = Editor.GetItemData(path, types, 4, pos);
            return materials;
        }
        public static void SetBaseMats(string path, int[] amounts)
        {
            int pos = Editor.GetOtotoPos(path) - 46;
            Editor.SetItemData(path, amounts, 4, pos);
        }
    }
}
