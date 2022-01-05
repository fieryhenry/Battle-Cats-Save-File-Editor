using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class HeavenlyTower
    {
        public static void ClearHeavenlyTower(string path)
        {
            int level = GetHeavenlyTower(path);

            Editor.ColouredText($"&You have currently cleared up to level &{level}&\n");
            Editor.ColouredText($"&What level do you want to clear up to?(max 50):\n");
            level = (int)Editor.Inputed();
            SetHeavenlyTower(path, level);
        }
        public static int GetHeavenlyTower(string path)
        {
            byte[] allData = File.ReadAllBytes(path);
            int pos = Editor.GetOtotoPos(path) + 337;
            return allData[pos];
        }
        public static void SetHeavenlyTower(string path, int level)
        {
            int pos = Editor.GetOtotoPos(path) + 337;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            stream.Position = pos;
            stream.WriteByte((byte)level);

            pos += 124;
            stream.Position = pos;
            for (int i = 0; i < level; i++)
            {
                stream.Position = pos + (16 * i);
                stream.WriteByte(1);
            }
        }
    }
}
