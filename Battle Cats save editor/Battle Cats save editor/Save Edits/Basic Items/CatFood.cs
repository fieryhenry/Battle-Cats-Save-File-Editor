using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class CatFood
    {
        public static void catFood(string path)
        {
            Console.WriteLine("Warning, editing cat food at all can get you banned after a few days, would you like to continue? (yes/no):");
            string answer = Console.ReadLine();
            if (answer.ToLower() == "no")
            {
                return;
            }
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            byte[] catfoodB = new byte[4];
            stream.Position = 7;
            stream.Read(catfoodB, 0, 4);

            int CatFood = BitConverter.ToInt16(catfoodB, 0);
            Console.WriteLine($"You have {CatFood} cat food");

            Console.WriteLine("How much cat food do you want?(max 45000, but I recommend below 20k, to be safe");

            CatFood = (int)Editor.Inputed();
            if (CatFood > 45000) CatFood = 45000;
            else if (CatFood < 0) CatFood = 0;

            byte[] bytes = Editor.Endian(CatFood);

            stream.Position = 7;
            stream.Write(bytes, 0, 2);
            Console.WriteLine("Set Cat food to " + CatFood);
        }
    }
}
