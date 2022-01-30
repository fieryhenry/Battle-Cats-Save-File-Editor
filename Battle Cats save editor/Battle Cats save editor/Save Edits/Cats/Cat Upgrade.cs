using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class CatUpgrade
    {
        public static void CatUpgrades(string path)
        {
            Editor.ColouredText("&What level do you want?:enter the &base& followed by a &+& then the &plus& level you want, e.g 50+80, 30+0, 10+30\nEnter the base followed by a plus with nothing else to leave the plus value as it is, e.g 50+, or 20+\nEnter " +
                "a plus followed by the plus value to leave the base values as they are e.g +20, +50\n");
            string answer = Console.ReadLine();
            int leave = 0;
            int baselevel = 0;
            int plusLevel = 0;
            try
            {
                baselevel = int.Parse(answer.Split('+')[0]) - 1;
            }
            catch
            {
                leave = 1;
            }
            try
            {
                plusLevel = int.Parse(answer.Split('+')[1]);
            }
            catch
            {
                leave = 2;
            }
            int[] ids = Enumerable.Range(0, Editor.GetCatAmount(path)).ToArray();
            int[] plusLevels = Enumerable.Repeat(plusLevel, Editor.GetCatAmount(path)).ToArray();
            int[] baseLevels = Enumerable.Repeat(baselevel, Editor.GetCatAmount(path)).ToArray();
            CatHandler.UpgradeCats(path, ids, plusLevels, baseLevels, leave);
            
            Console.WriteLine($"Upgraded all cats to level {answer}");
            // Close rank up bundle menu offer thing popping up 100s of times
            CloseBundle.Bundle(path);
        }
    }
}
