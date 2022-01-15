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
                if (catId >= Editor.catAmount)
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
            if (input.ToLower() == "yes")
            {
                Editor.ColouredText($"&What level do you want: &enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\n");
                string level = Console.ReadLine();

                int baselevel = int.Parse(level.Split('+')[0]) - 1;
                int plusLevel = int.Parse(level.Split('+')[1]);

                baseLevels = Enumerable.Repeat(baselevel, baseLevels.Length).ToArray();
                plusLevels = Enumerable.Repeat(plusLevel, plusLevels.Length).ToArray();
            }
            else
            {
                for (int i = 0; i < idInt.Count; i++)
                {
                    Editor.ColouredText($"&What level do you want to upgrade cat {idInt[i]} to: &enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\n");
                    string level = Console.ReadLine();

                    int baselevel = int.Parse(level.Split('+')[0]) - 1;
                    int plusLevel = int.Parse(level.Split('+')[1]);

                    baseLevels[i] = baselevel;
                    plusLevels[i] = plusLevel;
                }
            }
            CatHandler.UpgradeCats(path, idInt.ToArray(), plusLevels, baseLevels);

            for (int i = 0; i < idInt.Count; i++)
            {
                Editor.ColouredText($"Upgraded cat &{idInt[i]}& to level &{baseLevels[i] + 1}& +&{plusLevels[i]}\n");
            }
            CloseBundle.Bundle(path);
        }
    }
}
