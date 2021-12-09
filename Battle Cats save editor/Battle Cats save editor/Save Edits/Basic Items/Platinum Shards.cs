using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class PlatinumShards
    {
        public static void PlatShards(string path)
        {
            using var stream1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream1.Length;
            byte[] allData = new byte[length];
            stream1.Read(allData, 0, length);

            stream1.Close();

            // Search for plat shard position
            byte[] condtions2 = { 0x00, 0xF8, 0x88, 0x01, 0x00 };
            int pos = Editor.Search(path, condtions2, false, allData.Length - 600)[0];

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (pos <= 0)
            {
                Console.WriteLine("Error, your plat shard position couldn't be found, please report this on discord");
                return;
            }
            byte[] shardB = new byte[4];
            stream.Position = pos - 4;
            stream.Read(shardB, 0, 4);

            int shards = BitConverter.ToInt16(shardB, 0);
            Console.WriteLine($"You have {shards} platinum shards");
            Console.WriteLine("How many platinum shards do you want? (10 shards = 1 platinum ticket)");
            shards = (int)Editor.Inputed();
            byte[] bytes = Editor.Endian(shards);

            stream.Position = pos - 4;
            stream.Write(bytes, 0, 4);
            Console.WriteLine($"Set platinum shards to {shards}");
        }
    }
}
