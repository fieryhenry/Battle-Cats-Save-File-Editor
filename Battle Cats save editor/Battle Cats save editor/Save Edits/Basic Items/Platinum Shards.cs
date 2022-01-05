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
            int platshards = GetPlatShards(path);
            platshards = Editor.AskSentances(platshards, "platinum shards");
            SetPlatShards(path, platshards);
            Editor.AskSentances(platshards, "platinum shards", true);
        }
        public static int GetPlatShardsPos(string path)
        {
            byte[] allData = File.ReadAllBytes(path);

            byte[] condtions2 = { 0x00, 0xF8, 0x88, 0x01, 0x00 };
            int pos = Editor.Search(path, condtions2, false, allData.Length - 600)[0] - 4;
            if (pos < 200)
            {
                Editor.Error();
            }
            return pos;
        }
        public static int GetPlatShards(string path)
        {
            int pos = GetPlatShardsPos(path);
            return Editor.GetItemData(path, 1, 4, pos)[0];
        }
        public static void SetPlatShards(string path, int platshards)
        {
            int pos = GetPlatShardsPos(path);
            Editor.SetItemData(path, new int[] { platshards }, 4, pos);
        }
    }
}
