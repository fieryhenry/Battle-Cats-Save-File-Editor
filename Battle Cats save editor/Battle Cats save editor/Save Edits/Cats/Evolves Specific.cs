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
            int[] occurrence = Editor.OccurrenceB(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("What is the cat id?, input multiple ids separated by spaces to evolve multiple cats ids must be 9 or above, normal cats cannot be evolved this way");
            string[] input = Console.ReadLine().Split(' ');

            try
            {
                // Game version correcting
                if (allData[occurrence[5] - 304] == 0x2c && allData[occurrence[5] - 303] == 0x01)
                {
                    stream.Position = occurrence[5] + 40;
                }
                else if (allData[occurrence[4] - 304] == 0x2c && allData[occurrence[4] - 303] == 0x01)
                {
                    stream.Position = occurrence[4] + 40;
                }
                else
                {
                    Console.WriteLine("Error, your evolved cat position couldn't be found, please report this to me on discord");
                }
            }
            catch { Console.WriteLine("You either haven't unlocked the ability to evolve cats or if you have - the tool is bugged and you should tell me on the discord"); return; }
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
                            Console.WriteLine("Does the cat need catfruit/catfruit seeds to evolve?(yes/no)");
                            string answer = Console.ReadLine().ToLower();
                            if (answer == "yes")
                            {
                                stream.WriteByte(2);
                            }
                            else
                            {
                                stream.WriteByte(1);
                            }
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
