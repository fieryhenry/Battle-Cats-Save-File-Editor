using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class EquipSlots
    {
        public static int GetSlotPos(string path)
        {
            int game_version = Editor.GetGameVersion(path);
            byte[] conditions = { 0x90, 0x01, 0x00, 0x00 };
            int index = 0;
            if (game_version < 110300)
            {
                conditions = new byte[] { 0x2c, 0x01, 0x00, 0x00 };
                index = 1;
            }
            return Editor.Search(path, conditions, false, 0)[index] -5;
        }
        public static int GetEquipSlots(string path)
        {
            int pos = GetSlotPos(path);
            int slots = Editor.GetItemData(path, 1, 1, pos)[0];
            if (pos < 500)
            {
                Editor.Error();
            }
            return slots;
        }
        public static void SetEquipSlots(string path, int slots)
        {
            int pos = GetSlotPos(path);
            Editor.SetItemData(path, new int[] { slots }, 1, pos);
        }
        public static void Slots(string path)
        {
            int slots = GetEquipSlots(path);
            Editor.ColouredText($"&You have &{slots}& equip slots\n");
            Console.WriteLine("How many slots do you want to have unlocked?(max 15):");

            slots = (int)Editor.Inputed();
            slots = Editor.MaxMinCheck(slots, 15);

            SetEquipSlots(path, slots);
            Editor.ColouredText($"&Set equip slots to &{slots}&\n");
        }
    }
}
