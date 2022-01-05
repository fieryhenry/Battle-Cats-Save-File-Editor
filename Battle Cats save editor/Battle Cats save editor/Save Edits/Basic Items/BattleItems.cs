using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class BattleItems
    {
        public static void Items(string path)
        {
            int[] items = GetItems(path);
            string[] itemTypes = {"Speed Ups", "Treasure Radars", "Rich Cats", "Cat CPUs", "Cat Jobs", "Sniper the Cats" };
            Editor.ColouredText($"&You have:\n&{Editor.CreateOptionsList(itemTypes, items, false)}\n");
            Editor.ColouredText($"&What do you want to edit?{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(itemTypes)}&{itemTypes.Length+1}.& All at once\n");
            string[] answer = Console.ReadLine().Split(' ');
            for (int i = 0; i < answer.Length; i++)
            {
                int choice = int.Parse(answer[i]);
                if (choice > 7)
                {
                    Console.WriteLine("Error, id must not be above 7");
                    Items(path);
                }
                else if (choice == 7)
                {
                    Console.WriteLine("What do you want to set the value for all of your items to?(max 3999):");
                    int val = (int)Editor.Inputed();
                    if (val > 3999) val = 3999;
                    if (val < 0) val = 0;
                    items = Enumerable.Repeat(val, itemTypes.Length).ToArray();
                    break;
                }
                else
                {
                    Editor.ColouredText($"&What do you want to set the value for &{itemTypes[choice-1]}& to?(max 3999):");
                    int val = (int)Editor.Inputed();
                    if (val > 3999) val = 3999;
                    if (val < 0) val = 0;
                    items[choice - 1] = val;
                }
            }
            SetItems(path, items);
            Console.WriteLine("Successfully set battle items");
        }
        public static int GetItemPos(string path)
        {
            byte[] year = new byte[2];
            byte[] allData = File.ReadAllBytes(path);

            year[0] = allData[15];
            year[1] = allData[16];

            if (year[0] != 0x07)
            {
                year[0] = allData[19];
                year[1] = allData[20];
            }
            int[] occurrence = Editor.GetPositionsFromYear(path, year);

            if (occurrence[2] < 200)
            {
                Editor.Error();
            }
            int pos = occurrence[2] - 224;
            return pos;
        }
        public static int[] GetItems(string path)
        {
            int itemNum = 6;
            int pos = GetItemPos(path);
            int[] items = Editor.GetItemData(path, itemNum, 4, pos);
            return items;
        }
        public static void SetItems(string path, int[] items)
        {
            int pos = GetItemPos(path);
            Editor.SetItemData(path, items, 4, pos);
        }
    }
}
