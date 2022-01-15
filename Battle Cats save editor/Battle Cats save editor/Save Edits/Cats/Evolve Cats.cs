using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class EvolveCats
    {
        public static void Evolves(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "Evolve all cats",
                "Evolve specific cats"
            };
            string toOutput = $"&What would you like to edit?&\n{Editor.CreateOptionsList<string>(Features, first:"Go back")}&";
            Editor.ColouredText(toOutput);
            switch ((int)Editor.Inputed())
            {
                case 0:
                    Editor.Options();
                    break;
                case 1:
                    Evolve(path);
                    break;
                case 2:
                    EvolvesSpecific.EvolveSpecific(path);
                    break;
                default:
                    Console.WriteLine($"Please enter a number between 0 and {Features.Length}");
                    break;
            }
        }
        public static void Evolve(string path)
        {
            int pos1 = Editor.GetEvolvePos(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            stream.Position = pos1;

            int[] form = Editor.EvolvedFormsGetter();
            bool stop = false;
            int t = 0;
            int pos = (int)stream.Position;
            while (stream.Position < pos + (Editor.catAmount * 4) - 37 && !stop)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (allData[stream.Position + i] != 0x01 && allData[stream.Position + i] != 0 && allData[stream.Position + i] != 0x02 && allData[stream.Position + i] != 0x03)
                    {
                        stop = true;
                        break;
                    }
                }
                try
                {
                    stream.WriteByte((byte)form[t]);
                }
                catch
                {
                    stream.WriteByte(0);
                }
                stream.Position += 3;
                t++;
            }
            Console.WriteLine("Successfully evolved all cats");
        }
    }
}
