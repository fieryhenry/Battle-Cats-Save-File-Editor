using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class CatFruits
    {
        public static void CatFruit(string path)
        {
            List<string> fruit_types = new() { "Purple Seeds", "Red Seeds", "Blue Seeds", "Green Seeds", "Yellow Seeds", "Purple Fruit", "Red Fruit", "Blue Fruit", "Green Fruit", "Yellow Fruit", "Epic Fruit", "Elder Seeds", "Elder Fruit", "Epic Seeds", "Gold Fruit", "Aku Seeds", "Aku Fruit", "Gold Seeds", "Pure Seeds", "Pure Fruit" };
            int[] current_fruits = GetCatFruit(path);
            if (current_fruits.Length < fruit_types.Count)
            {
                fruit_types.RemoveRange(current_fruits.Length, fruit_types.Count - current_fruits.Length);
            }
            
            Editor.ColouredText($"&You have:\n&{Editor.CreateOptionsList(fruit_types.ToArray(), current_fruits, false)}Total &:& {current_fruits.Sum()}\n");
            Editor.ColouredText($"&What do you want to edit?{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(fruit_types.ToArray())}&{fruit_types.Count + 1}.& All at once\n");
            string[] answer = Console.ReadLine().Split(' ');
            foreach (string choice in answer)
            {
                int catfruit_id = Convert.ToInt32(choice) -1;
                if (catfruit_id == fruit_types.Count)
                {
                    Editor.ColouredText($"&What do you want to set all of your catfruits / catfruit seeds to?:\n");
                    int val = (int)Editor.Inputed();
                    current_fruits = Enumerable.Repeat(val, current_fruits.Length).ToArray();
                }
                else
                {
                    Editor.ColouredText($"&What do you want to set &{fruit_types[catfruit_id]}& to?:\n");
                    int val = (int)Editor.Inputed();
                    current_fruits[catfruit_id] = val;
                }
            }
            SetCatFruit(path, current_fruits);
            Editor.ColouredText($"&Successfully set catfruits to:\n&{Editor.CreateOptionsList(fruit_types.ToArray(), current_fruits, false)}Total &:& {current_fruits.Sum()}\n");
        }
        public static Tuple<int, int> GetCatFruitPos(string path)
        {
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);

            byte[] allData = File.ReadAllBytes(path);


            int pos = occurrence[6] + Editor.GetCatAmount(path) + 4;
            if (allData[pos] != 0x34)
            {
                Editor.Error();
            }
            int catfruit_types = allData[pos + 44];
            pos += 48;
            if (pos < 100)
            {
                Editor.Error();
            }
            return Tuple.Create(pos, catfruit_types);
        }
        public static int[] GetCatFruit(string path)
        {
            int catfruit_types = GetCatFruitPos(path).Item2;
            int pos = GetCatFruitPos(path).Item1;

            int[] catfruits = Editor.GetItemData(path, catfruit_types, 4, pos);
            return catfruits;
        }
        public static void SetCatFruit(string path, int[] catfruits)
        {
            int pos = GetCatFruitPos(path).Item1;
            Editor.SetItemData(path, catfruits, 4, pos);
        }
    }
}
