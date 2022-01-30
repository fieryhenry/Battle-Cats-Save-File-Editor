using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class GamatotoXP
    {
        public static int GetGamXPPos(string path)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[8];
            pos += Editor.GetCatAmount(path) * 4;
            pos += 53;
            return pos;
        }
        public static int GetGamXP(string path)
        {
            int pos = GetGamXPPos(path);
            if (pos < 200) Editor.Error();
            int xp = Editor.GetItemData(path, 1, 4, pos)[0];
            return xp;
        }
        public static void SetGamXP(string path, int xp)
        {
            int pos = GetGamXPPos(path);
            Editor.SetItemData(path, new int[] { xp }, 4, pos);
        }
        public static void GamXP(string path)
        {
            int xp = GetGamXP(path);
            Editor.ColouredText($"&You have &{xp}& gamatoto xp\n");
            Console.WriteLine("How much gamatoto xp do you want?\nLevel bounderies: https://battle-cats.fandom.com/wiki/Gamatoto_Expedition#Level-up_Requirements");
            xp = (int)Editor.Inputed();
            SetGamXP(path, xp);
            Editor.ColouredText($"&Set gamatoto xp to &{xp}&\n");
        }
    }
}
