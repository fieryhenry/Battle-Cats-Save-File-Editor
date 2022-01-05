using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class FixElsewhere
    {
        public static Tuple<int, bool> GetTokenPos(string path)
        {
            List<byte> allData = File.ReadAllBytes(path).ToList();
            byte[] condtions2 = { 0x78, 0x63, 0x01 };
            int start_pos = Editor.Search(path, condtions2, false, allData.Count - 800)[0];
            int new_pos = 0;

            for (int i = start_pos; i < start_pos + 100; i++)
            {
                if (allData[i + 11] == 0x28)
                {
                    new_pos = i + 15;
                    break;
                }
            }
            if (new_pos == 0)
            {
                allData[start_pos + 11] = 0x28;
                allData.InsertRange(start_pos + 14, new byte[40]);
                File.WriteAllBytes(path, allData.ToArray());
                return Tuple.Create(start_pos + 15, false);
            }
            return Tuple.Create(new_pos, true);
        }
        public static byte[] GetToken(string path)
        {
            List<byte> allData = File.ReadAllBytes(path).ToList();
            Tuple<int, bool> result = GetTokenPos(path);
            int pos = result.Item1;
            bool has_token = result.Item2;
            byte[] token_bytes = new byte[40];
            if (has_token)
            {
                token_bytes = allData.GetRange(pos, 40).ToArray();
            }
            return token_bytes;
        }
        public static void SetToken(string path, byte[] token_bytes)
        {
            int pos = GetTokenPos(path).Item1;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            stream.Position = pos;
            stream.Write(token_bytes, 0, 40);
        }
        public static void Elsewhere(string path)
        {
            string inquiry_code_first = NewInquiryCode.GetIQ(path);
            byte[] token_first_b = GetToken(path);

            Console.WriteLine("Please select a working save that doesn't have 'Save is used elsewhere' and has never had it in the past\nPress enter to select that save");
            Console.ReadLine();
            var FD = new OpenFileDialog
            {
                Filter = "working battle cats save(*.*)|*.*"
            };
            string path2 = "";
            if (FD.ShowDialog() == DialogResult.OK)
            {
                path2 = FD.FileName;
            }
            else
            {
                Editor.ColouredText("\nPlease select your save\n\n");
                Elsewhere(path);
            }
            string inquiry_code_second = NewInquiryCode.GetIQ(path2);
            byte[] token_second_b = GetToken(path2);

            NewInquiryCode.SetIQ(path, inquiry_code_second);
            SetToken(path, token_second_b);

            string token_first = Encoding.ASCII.GetString(token_first_b);
            string token_second = Encoding.ASCII.GetString(token_second_b);

            if (token_first_b[0] == 0)
            {
                token_first = "None";
            }
            if (token_second_b[0] == 0)
            {
                token_second = "None";
            }

            Editor.ColouredText($"&Replaced inquiry code: &{inquiry_code_first}& with &{inquiry_code_second}&\n");
            Editor.ColouredText($"&Replaced token: &{token_first}& with &{token_second}&\n");
        }
    }
}
