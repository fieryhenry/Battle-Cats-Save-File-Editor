using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class GetSpecificCats
    {
        public static void SpecifiCat(string path)
        {
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            Console.WriteLine("What is the cat ID?, input multiple ids separated by spaces to add multiple cats at a time");
            string input = Console.ReadLine();
            string[] catIds = input.Split(' ');
            for (int i = 0; i < catIds.Length; i++)
            {
                int catID = int.Parse(catIds[i]);
                if (catID >= Editor.GetCatAmount(path))
                {
                    Console.WriteLine($"Error, cat : {catID} doesn't exist in the current game version");
                    continue;
                }
                int startPos = occurrence[0] + 4;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(01);
                Editor.ColouredText("&Gave cat: &" + catID + "\n");
            }

        }
    }
}
