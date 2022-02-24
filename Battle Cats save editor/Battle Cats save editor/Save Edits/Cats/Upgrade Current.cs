using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class UpgradeCurrent
    {
        public static void UpgradeCurrentCats(string path)
        {
            int[] idInt = CatHandler.GetCurrentCats(path);

            Editor.ColouredText("&What level do you want?:enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\nEnter the base followed by a plus with nothing else to leave the plus value as it is, e.g 50+, or 20+\nEnter " +
                "a plus followed by the plus value to leave the base values as they are e.g +20, +50\n");
            string answer = Console.ReadLine();
            Tuple<int, int, int> data = CatHandler.FindIgnore(answer);

            int baselevel = data.Item1;
            int plusLevel = data.Item2;
            int ignore = data.Item3;

            int[] plusLevels = Enumerable.Repeat(plusLevel, Editor.GetCatAmount(path)).ToArray();
            int[] baseLevels = Enumerable.Repeat(baselevel, Editor.GetCatAmount(path)).ToArray();

            CatHandler.UpgradeCatsAll(path, idInt, plusLevels, baseLevels, ignore);
            Console.WriteLine("Success");
            CloseBundle.Bundle(path);
        }
    }
}
