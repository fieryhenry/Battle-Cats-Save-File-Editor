using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class EvolvesSpecific
    {
        public static void EvolveSpecific(string path)
        {
            Console.WriteLine("Do you want to evolve(1) or de-evolve(2)?:");
            string devolve = Console.ReadLine();
            if (devolve != "1" && devolve != "2")
            {
                Console.WriteLine("Answer must be 1 or 2!");
                EvolveSpecific(path);
            }
            int pos1 = Editor.GetEvolvePos(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            stream.Position = pos1;
            Console.WriteLine("What is the cat id?, input multiple ids separated by spaces to evolve multiple cats ids must be 9 or above, normal cats cannot be evolved this way");
            string[] input = Console.ReadLine().Split(' ');

            int[] form = Editor.EvolvedFormsGetter();
            int pos = (int)stream.Position;
            try
            {
                // Loop through all inputed ids
                for (int i = 0; i < input.Length; i++)
                {
                    int id = int.Parse(input[i]);
                    // Find its position
                    stream.Position = pos + (id - 9) * 4;
                    // Evolve
                    if (devolve == "1")
                    {
                        bool stay = false;
                        // If inputed id doesn't exist in cats.csv yet
                        if (id - 9 >= form.Length)
                        {
                            stay = true;
                            input[i] = "9";
                        }
                        if (form[id - 9] == 0 || stay)
                        {
                            stream.WriteByte(2);
                        }
                        else
                        {
                            stream.WriteByte((byte)form[id - 9]);
                        }
                    }
                    // Devolve
                    else
                    {
                        stream.WriteByte(0);
                    }
                    stream.Position--;
                }
            }
            catch
            {
                Console.WriteLine("You need to input an id that isn't a normal cat!");
            }
        }
    }
}
