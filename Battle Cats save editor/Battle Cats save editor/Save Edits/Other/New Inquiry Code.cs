using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class NewInquiryCode
    {
        public static int GetIQPos(string path)
        {
            List<byte> allData = File.ReadAllBytes(path).ToList();

            byte[] conditions = { 0x2d, 0x00, 0x00, 0x00, 0x2e };
            int pos = Editor.Search(path, conditions)[0];

            for (int j = 1900; j < 2108; j++)
            {
                if (allData[pos - j] == 09 && allData[pos - j -1] == 0 && allData[pos - j + 1] == 0)
                {
                    return pos - j + 4;
                }
            }
            return -1;
        }
        public static string GetIQ(string path)
        {
            List<byte> allData = File.ReadAllBytes(path).ToList();

            int pos = GetIQPos(path);

            List<byte> iq_bytes = allData.GetRange(pos, 9);

            return Encoding.ASCII.GetString(iq_bytes.ToArray());
        }
        public static void SetIQ(string path, string inquiry_code)
        {
            int pos = GetIQPos(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            byte[] iq_bytes = Encoding.ASCII.GetBytes(inquiry_code);

            stream.Position = pos;
            stream.Write(iq_bytes, 0, iq_bytes.Length);
        }
        public static void NewIQ(string path)
        {
            string inquiry_code = GetIQ(path);
            Editor.ColouredText($"&Current inquiry code: &{inquiry_code}&\n");
            Editor.ColouredText($"&What do you want to set your inquiry code to?\n");
            inquiry_code = Console.ReadLine();
            SetIQ(path, inquiry_code);
            Editor.ColouredText($"&Set inquiry code to: &{inquiry_code}&\n");
        }
    }
}
