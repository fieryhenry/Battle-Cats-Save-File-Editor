using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class RemoveSpecificCats
    {
        public static void RemSpecifiCat(string path)
        {
            int[] occurrence = Editor.OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            Console.WriteLine("What is the cat ID?, input multiple ids separated by spaces to add multiple cats at a time");
            string input = Console.ReadLine();
            string[] catIds = input.Split(' ');
            for (int i = 0; i < catIds.Length; i++)
            {
                int catID = int.Parse(catIds[i]);
                int startPos = occurrence[0] + 4;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(0);
                Editor.ColouredText("&Removed cat: &" + catID + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            }
        }
    }
}
