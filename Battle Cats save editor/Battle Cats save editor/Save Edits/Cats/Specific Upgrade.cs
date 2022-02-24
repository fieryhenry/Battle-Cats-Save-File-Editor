using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class SpecificUpgrade
    {
        public static void SpecifUpgrade(string path)
        {
            Console.WriteLine("Enter the cat ids for the cats you want to upgrade(you can enter multiple values separated by spaces to edit multiple at once):");
            List<int> allIds = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse).ToList();
            List<int> idInt = new();

            string input = "no";
            foreach (int catId in allIds)
            {
                if (catId >= Editor.GetCatAmount(path))
                {
                    Console.WriteLine($"Error cat : {catId} doesn't exist in the current game version");
                }
                else
                {
                    idInt.Add(catId);
                }
            }
            if (idInt.Count > 1)
            {
                Console.WriteLine("Do you want to upgrade all of those cats to the same level?(yes/no)");
                input = Console.ReadLine();
            }

            int[] baseLevels = new int[idInt.Count];
            int[] plusLevels = new int[idInt.Count];
            int ignore = 0;
            if (input.ToLower() == "yes")
            {
                Editor.ColouredText("&What level do you want?:enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\nEnter the base followed by a plus with nothing else to leave the plus value as it is, e.g 50+, or 20+\nEnter " +
                    "a plus followed by the plus value to leave the base values as they are e.g +20, +50\n");
                string level = Console.ReadLine();

                Tuple<int, int, int> data = CatHandler.FindIgnore(level);

                int baselevel = data.Item1;
                int plusLevel = data.Item2;
                ignore = data.Item3;

                baseLevels = Enumerable.Repeat(baselevel, baseLevels.Length).ToArray();
                plusLevels = Enumerable.Repeat(plusLevel, plusLevels.Length).ToArray();
            }
            else
            {
                for (int i = 0; i < idInt.Count; i++)
                {
                    Editor.ColouredText($"&What level do you want to upgrade cat {idInt[i]} ?:enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\nEnter the base followed by a plus with nothing else to leave the plus value as it is, e.g 50+, or 20+\nEnter " +
                        "a plus followed by the plus value to leave the base values as they are e.g +20, +50\n"); string level = Console.ReadLine();
                    Tuple<int ,int ,int> data = CatHandler.FindIgnore(level);

                    int baselevel = data.Item1;
                    int plusLevel = data.Item2;
                    ignore = data.Item3;

                    baseLevels[i] = baselevel;
                    plusLevels[i] = plusLevel;
                }
            }
            CatHandler.UpgradeCats(path, idInt.ToArray(), plusLevels, baseLevels, ignore);

            for (int i = 0; i < idInt.Count; i++)
            {
                Editor.ColouredText($"Upgraded cat &{idInt[i]}& to level &{baseLevels[i] + 1}& +&{plusLevels[i]}\n");
            }
            CloseBundle.Bundle(path);
        }
    }
}
