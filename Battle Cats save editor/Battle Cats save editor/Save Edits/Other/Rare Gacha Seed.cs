using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class RareGachaSeed
    {
        public static void Seed(string path)
        {
            GetSeed(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            Console.WriteLine("What seed do you want?");
            long Seed = Editor.Inputed();
            if (Seed < 0) Seed = 0;
            byte[] bytes = Editor.Endian(Seed);

            byte[] year = new byte[2];
            year[0] = allData[15];
            year[1] = allData[16];

            stream.Close();

            int[] occurrence = Editor.OccurrenceE(path, year);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                stream2.Position = occurrence[4] - 21;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Sorry your seed position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
                Editor.Options();
            }

            Console.WriteLine("Set gacha seed to: {0}", Seed);
            for (int i = 0; i < 5; i++)
                stream2.WriteByte(bytes[i]);
        }
        public static void GetSeed(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] year = new byte[2];
            year[0] = allData[15];
            year[1] = allData[16];

            stream.Close();

            int[] occurrence = Editor.OccurrenceE(path, year);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                stream2.Position = occurrence[4] - 21;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Sorry your seed position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
                Editor.Options();
            }
            byte[] seed = new byte[100];
            int j = 0;
            for (int i = occurrence[4] - 21; i < occurrence[4] - 16; i++)
            {
                seed[j] = allData[i];
                j++;
            }
            seed = Editor.Endian(BitConverter.ToInt64(seed, 0));
            Console.WriteLine("Your Seed is:" + BitConverter.ToInt64(seed, 0));
        }
    }
}
