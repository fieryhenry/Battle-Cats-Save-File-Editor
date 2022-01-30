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
        public static void Inquiry(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "Change Inquiry code",
                "Fix save is used elsewhere error (or unban account) - whilst selecting a save that has the error / ban (the one you select when you open the editor) select a new save that doesn't have the \"save is used elsewhere\" bug / is not banned (you can re-install the game to get a save like that)"
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            Editor.ColouredText(toOutput);
            switch ((int)Editor.Inputed())
            {
                case 0:
                    Editor.Options();
                    break;
                case 1:
                    NewIQ(path);
                    break;
                case 2:
                    FixElsewhere.Elsewhere(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }
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
