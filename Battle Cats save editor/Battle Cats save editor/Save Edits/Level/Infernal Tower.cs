using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class InfernalTower
    {
        public static void ClearInfernalTower(string path)
        {
            int level = GetInfernalTower(path);

            Editor.ColouredText($"&You have currently cleared up to level &{level}&\n");
            Editor.ColouredText($"&What level do you want to clear up to?(max 50):\n");
            level = (int)Editor.Inputed();
            SetInfernalTower(path, level);
        }
        public static int GetInfernalTower(string path)
        {
            byte[] allData = File.ReadAllBytes(path);
            int pos = Editor.GetOtotoPos(path) + 433;
            return allData[pos];
        }
        public static void SetInfernalTower(string path, int level)
        {
            int pos = Editor.GetOtotoPos(path) + 433;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            stream.Position = pos;
            stream.WriteByte((byte)level);

            pos += 28 + (16*300);
            stream.Position = pos;
            for (int i = 0; i < level; i++)
            {
                stream.Position = pos + (16 * i);
                stream.WriteByte(1);
            }
        }
    }
}
