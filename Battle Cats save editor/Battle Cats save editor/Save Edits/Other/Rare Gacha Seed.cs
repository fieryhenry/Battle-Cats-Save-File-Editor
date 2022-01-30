using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Battle_Cats_save_editor.SaveEdits
{
	public class RareGachaSeed
	{
        public static int GetSeedPos(string path)
        {
            byte[] conditons = { 0x2c, 1, 0, 0 };
            int pos = Editor.Search(path, conditons)[0];
            pos += (300 * 4) + 4;
            return pos;
        }
        public static long GetSeed(string path)
        {
            int pos = GetSeedPos(path);
            List<byte> allData = File.ReadAllBytes(path).ToList();
            byte[] seed_ba = allData.GetRange(pos, 4).ToArray();
            long seed = BitConverter.ToUInt32(seed_ba, 0);
            return seed;
        }
        public static void SetSeed(string path, long seed)
        {
            int pos = GetSeedPos(path);
            byte[] seed_ba = Editor.Endian(seed);
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            stream.Position = pos;
            stream.Write(seed_ba, 0, 4);
        }
        public static void Seed(string path)
		{
            long seed = GetSeed(path);
            Editor.ColouredText($"&Current seed : &{seed}&\n");
            Editor.ColouredText("&What rare gacha seed do you want?\n");
            seed = Editor.Inputed();
            SetSeed(path, seed);
            Editor.ColouredText($"&Set gacha seed to &{seed}&\n");
        }
    }
}
