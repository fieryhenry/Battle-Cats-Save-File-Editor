﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class AkuRealm
    {
        public static int GetAkuPos(string path)
        {
            byte[] akuConditions = { 0x31, 0x01 };
            byte[] tenPointNineConditions = { 0x24, 0x8A, 0x01 };
            byte[] allData = File.ReadAllBytes(path);
            // Search for version 10.9 content
            int verPos = Editor.Search(path, tenPointNineConditions, true, allData.Length - 16)[0];
            // Search for aku realm position from 10.9 content
            int pos = Editor.Search(path, akuConditions, true, verPos)[0];

            if (verPos == 0 || pos == 0)
            {
                Editor.Error();
            }
            return pos +2;
        }
        public static int[] GetAku(string path)
        {
            int pos = GetAkuPos(path);
            int[] aku_data = Editor.GetItemData(path, 49, 2, pos, false);
            return aku_data;
        }
        public static void SetAku(string path, int[] aku_data)
        {
            int pos = GetAkuPos(path);
            Editor.SetItemData(path, aku_data, 2, pos);
        }
        public static void ClearAku(string path)
        {
            byte[] akuConditions = { 0x31, 0x01 };
            byte[] tenPointNineConditions = { 0x24, 0x8A, 0x01 };

            using var stream1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream1.Length;
            byte[] allData = new byte[length];
            stream1.Read(allData, 0, length);
            stream1.Close();

            // Search for version 10.9 content
            int verPos = Editor.Search(path, tenPointNineConditions, true, allData.Length - 16)[0];
            // Search for aku realm position from 10.9 content
            int pos = Editor.Search(path, akuConditions, true, verPos)[0];

            if (verPos == 0 || pos == 0)
            {
                Editor.Error();
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            // Set stages to clear
            for (int j = 0; j < 49; j++)
            {
                stream.Position = pos + 2 + (j * 2);
                stream.WriteByte(1);
            }

            Console.WriteLine("Successfully cleared all aku stages, if you haven't unlocked the aku realm yet, this feature won't work for you");
        }
    }
}
