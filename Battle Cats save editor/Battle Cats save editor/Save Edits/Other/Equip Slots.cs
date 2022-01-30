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
            byte[] conditions = { 0x2c, 0x01, 0x00, 0x00 };
            int pos = Editor.Search(path, conditions, false, 0)[1];
            return pos -5;
        }
        public static int GetEquipSlots(string path)
        {
            int pos = GetSlotPos(path);
            int slots = Editor.GetItemData(path, 1, 1, pos)[0];
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
