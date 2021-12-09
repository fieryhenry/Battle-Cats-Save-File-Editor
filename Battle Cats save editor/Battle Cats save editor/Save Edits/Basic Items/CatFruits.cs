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
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            stream.Close();

            int[] occurrence = Editor.OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int catFruitNums = 18;
            try
            {
                stream2.Position = occurrence[7] - (catFruitNums * 4);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("You either can't evolve cats or the tool is bugged and if the tool is bugged then:\ntell me on discord\nThank you");
                Editor.Options();
            }
            byte[] catfruit = new byte[4];
            int[] FruitCat = new int[catFruitNums];
            string[] fruits = { "Purple Seed", "Red Seed", "Blue Seed", "Green Seed", "Yellow Seed", "Purple Fruit", "Red Fruit", "Blue Fruit", "Green Fruit", "Yellow Fruit", "Epic Fruit", "Elder Seed", "Elder Fruit", "Epic Seed", "Gold Fruit", "Aku Seed", "Aku Fruit", "Gold Seed" };

            int j = 0;
            for (int i = occurrence[7] - (catFruitNums * 4); i < occurrence[7] - 3; i += 4)
            {
                catfruit[0] = allData[i];
                catfruit[1] = allData[i + 1];
                FruitCat[j] = BitConverter.ToInt32(catfruit, 0);
                j++;
            }
            Editor.ColouredText("&Total catfruit/seeds: &" + FruitCat.Sum() + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            Editor.ColouredText("&Do you want to edit all the cat fruits individually(&1&) or all at once? (&2&), (&1& or &2&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string input = Console.ReadLine();

            if (input == "2")
            {
                Editor.ColouredText("&How many do you want?(max &28&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                int num = (int)Editor.Inputed();
                if (num > 32) num = 32;
                else if (num < 0) num = 0;

                byte[] bytes2 = Editor.Endian(num);

                for (int i = 0; i < catFruitNums; i++)
                {
                    int choice2 = i;
                    stream2.Position = occurrence[7] - (catFruitNums * 4) + ((choice2) * 4);
                    stream2.WriteByte(bytes2[0]);
                    stream2.WriteByte(bytes2[1]);
                    Editor.ColouredText("&Set &" + fruits[choice2] + "& to &" + num + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }

            }
            else if (input == "1")
            {
                Console.WriteLine("Enter a number to edit that type of catfruit, enter multiple numbers separated by spaces to change multiple at a time");
                for (int i = 0; i < fruits.Length; i++)
                {
                    Editor.ColouredText("&" + (i + 1) + ".& " + fruits[i] + "&:& " + FruitCat[i] + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
                string[] enteredIDs = Console.ReadLine().Split(' ');
                for (int i = 0; i < enteredIDs.Length; i++)
                {
                    bool skip = false;
                    int choice = 0;
                    try
                    {
                        choice = int.Parse(enteredIDs[i]);
                    }
                    catch
                    {
                        skip = true;
                    }
                    if (!skip)
                    {
                        if (choice > catFruitNums) choice = catFruitNums;
                        else if (choice < 1) choice = 1;

                        Editor.ColouredText("&How many &" + fruits[choice - 1] + "s& do you want (max &256&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                        int amount = (int)Editor.Inputed();
                        if (amount > 256) amount = 256;
                        else if (amount < 0) amount = 0;

                        byte[] bytes = Editor.Endian(amount);

                        stream2.Position = occurrence[7] - (catFruitNums * 4) + ((choice - 1) * 4);
                        stream2.WriteByte(bytes[0]);
                        stream2.WriteByte(bytes[1]);
                    }
                }
            }

            Editor.ColouredText("&Have you finished editing cat fruits?(&yes&/&no&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string answer = Console.ReadLine();
            stream2.Close();
            if (answer.ToLower() == "no") CatFruit(path);
        }
    }
}
