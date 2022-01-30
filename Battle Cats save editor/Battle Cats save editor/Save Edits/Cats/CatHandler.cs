using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class CatHandler
    {
        public static void UpgradeCats(string path, int[] catIDs, int[] plusLevels, int[] baseLevels, int ignore = 0)
        {
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            int pos = occurrence[1] + 1;
            for (int i = 0; i < catIDs.Length; i++)
            {
                if (catIDs[i] == 0) continue;
                stream.Position = pos + (i * 4) + 3;
                if (ignore != 2)
                {
                    stream.WriteByte((byte)plusLevels[i]);
                    FileStream fileStream = stream;
                    long position = fileStream.Position;
                    fileStream.Position = position - 1;
                }
                stream.Position += 2;
                if (ignore != 1)
                {
                    stream.WriteByte((byte)baseLevels[i]);
                }
            }
        }
        public static int[] GetCurrentCats(string path)
        {
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);
            int startPos = occurrence[0] + 4;
            return Editor.GetItemData(path, Editor.GetCatAmount(path), 4, startPos);
        }
    }
}
