using System;
using System.IO;

namespace Battle_Cats_save_editor.SaveEdits
{
	public class RareGachaSeed
	{
		public static void Seed(string path)
		{
            GetSeed(path);
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            Console.WriteLine("Scan Complete");
            Console.WriteLine("What seed do you want?");
            long Seed = Editor.Inputed();
            bool flag = Seed < 0L;
            if (flag)
            {
                Seed = 0L;
            }
            byte[] bytes = Editor.Endian(Seed);
            byte[] year = new byte[]
            {
                    allData[15],
                    allData[16]
            };
            stream.Close();
            int[] occurrence = Editor.GetPositionsFromYear(path, year);
            using FileStream stream2 = new(path, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                stream2.Position = occurrence[4] - 21;
            }
            catch
            {
                Editor.Error("Error, a position couldn't be found, please report this in #bug-reports on discord");
            }
            Console.WriteLine("Set gacha seed to: {0}", Seed);
            for (int i = 0; i < 5; i++)
            {
                stream2.WriteByte(bytes[i]);
            }
        }

		public static void GetSeed(string path)
		{
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            byte[] year = new byte[]
            {
                    allData[15],
                    allData[16]
            };
            stream.Close();
            int[] occurrence = Editor.GetPositionsFromYear(path, year);
            using FileStream stream2 = new(path, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                stream2.Position = occurrence[4] - 21;
            }
            catch
            {
                Editor.Error("Error, a position couldn't be found, please report this in #bug-reports on discord");
            }
            byte[] seed = new byte[100];
            int i = 0;
            for (int j = occurrence[4] - 21; j < occurrence[4] - 16; j++)
            {
                seed[i] = allData[j];
                i++;
            }
            seed = Editor.Endian(BitConverter.ToInt64(seed, 0));
            Console.WriteLine("Your Seed is:" + BitConverter.ToInt64(seed, 0).ToString());
        }
	}
}
