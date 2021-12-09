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
            int[] occurrence = Editor.OccurrenceB(path);
            Console.WriteLine("What base level do you want to upgrade all of your cats to? (max 50) - plus levels will be specified later, enter -1 to keep the base level the same");
            int baseLev = (int)(Editor.Inputed() - 1);
            Console.WriteLine("What plus level do you want to upgrade all of your cats to? (max 90), enter -1, to keep plus level the same");
            int plusLev = (int)Editor.Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (baseLev > 49) baseLev = 49;

            if (plusLev > 90) plusLev = 90;

            int pos = occurrence[1] + 1;
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            // Loop through cat upgrade data
            for (int i = pos + 3; i <= pos + (Editor.catAmount * 4) - 2; i += 4)
            {
                // If base data wants to be modified
                if (baseLev > -1)
                {
                    stream.Position = i + 2;
                    stream.WriteByte(Convert.ToByte(baseLev));
                }
                // If plus data wants to be modified
                if (plusLev > -1)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(plusLev));
                }
            }

            Console.WriteLine("Upgraded all cats to level " + (baseLev + 1) + " +" + plusLev);
            stream.Close();
            // Close rank up bundle menu offer thing popping up 100s of times
            CloseBundle.Bundle(path);
        }
    }
}
