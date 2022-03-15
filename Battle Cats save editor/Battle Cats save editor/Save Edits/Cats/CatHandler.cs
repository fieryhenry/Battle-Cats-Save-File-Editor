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
        public static int[] GetCatUpgrades(string path)
        {
            int cat_amount = Editor.GetCatAmount(path);
            int pos = Editor.GetCatRelatedHackPositions(path)[1] + 4;
            int[] cat_data = Editor.GetItemData(path, cat_amount * 2, 2, pos, false);
            return cat_data;
        }
        public static void SetCatUpgrades(string path, int[] cat_data)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[1] + 4;
            Editor.SetItemData(path, cat_data, 2, pos);
        }
        public static Tuple<int, int, int> FindIgnore(string answer)
        {
            int baselevel = 0;
            int leave = 0;
            int plusLevel = 0;
            try
            {
                baselevel = int.Parse(answer.Split('+')[0]) - 1;
            }
            catch
            {
                leave = 1;
            }
            try
            {
                plusLevel = int.Parse(answer.Split('+')[1]);
            }
            catch
            {
                leave = 2;
            }
            return Tuple.Create(baselevel, plusLevel, leave);
        }
        public static void UpgradeCats(string path, int[] catIDs, int[] plusLevels, int[] baseLevels, int ignore = 0)
        {
            // ignore = 1 means ignore base levels
            // ignore = 2 means ignore plus levels
            int[] occurrence = Editor.GetCatRelatedHackPositions(path);
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            int pos = occurrence[1] + 1;
            for (int i = 0; i < catIDs.Length; i++)
            {
                stream.Position = pos + (catIDs[i] * 4) + 3;
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
        public static void UpgradeCatsAll(string path, int[] catIDs, int[] plusLevels, int[] baseLevels, int ignore = 0)
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
            return Editor.GetItemData(path, Editor.GetCatAmount(path), 4, startPos, false);
        }
    }
}
