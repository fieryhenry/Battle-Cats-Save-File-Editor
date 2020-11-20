using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor
{
    class Program
    {

        static void Main()
        {
            string path = Path.Combine("C:/Users/henry_5ufuxnx/Downloads/LethaL-EN/My save editor/save");
            Console.WriteLine("\nWhat do you want to do?\n1. Change Cat food\n2. Change XP\n3. All treasures\n4. All cats upgraded 40+80\n5. Change leadership");
            int Choice = Convert.ToInt32(Console.ReadLine());


            switch (Choice)
            {
                case 1:
                    CatFood(path);
                    break;
                case 2:
                    XP(path);
                    break;
                case 3:
                    Treasure(path);
                    break;
                case 4:
                    CatUpgrades(path);
                    break;
                case 5:
                    Leadership(path);
                    break;
                case 6:
                    NP(path);
                    break;
                default:
                    Console.WriteLine("Please input a number that is recognised");
                    break;

            }
            Main();
        }
        static void CatFood(string path)
        {
            Console.WriteLine("How much cat food do you want?(max 45000)");
            int CatFood = Convert.ToInt32(Console.ReadLine());
            if (CatFood > 45000) CatFood = 45000;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            string CatFoodHex = Convert.ToString(CatFood, 16);
            Console.WriteLine("Set Cat food to " + CatFood);

            int StrLength = CatFoodHex.Length;

            char[] CatArr = { '0', '0', '0', '0' };
            char[] CatArr2 = { '0', '0' };
            char[] CatArr3 = { '0', '0' };

            if (StrLength == 3)
            {
                CatArr = CatFoodHex.ToCharArray(0, StrLength);
                CatArr2 = CatFoodHex.ToCharArray(0, StrLength - 1);
                CatArr3 = CatFoodHex.ToCharArray(0, StrLength - 1);

                CatArr2[0] = CatArr[1];
                CatArr2[1] = CatArr[2];
                CatArr3[0] = '0';
                CatArr3[1] = CatArr[0];

                string CatFoodStr = new string(CatArr2);
                string CatFoodStr2 = new string(CatArr3);

                int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);


                stream.Position = 7;
                stream.WriteByte(CatFoodByte);
                stream.Position = 8;
                stream.WriteByte(CatFoodByte2);
            }
            if (StrLength == 4)
            {
                CatArr = CatFoodHex.ToCharArray(0, StrLength);
                CatArr2 = CatFoodHex.ToCharArray(0, StrLength - 2);
                CatArr3 = CatFoodHex.ToCharArray(0, StrLength - 2);

                CatArr2[0] = CatArr[2];
                CatArr2[1] = CatArr[3];
                CatArr3[0] = CatArr[0];
                CatArr3[1] = CatArr[1];

                string CatFoodStr = new string(CatArr2);
                string CatFoodStr2 = new string(CatArr3);

                int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                stream.Position = 7;
                stream.WriteByte(CatFoodByte);
                stream.Position = 8;
                stream.WriteByte(CatFoodByte2);
            }
            if (StrLength == 2)
            {
                CatArr = CatFoodHex.ToCharArray(0, StrLength);
                CatArr2 = CatFoodHex.ToCharArray(0, StrLength);

                CatArr2[0] = CatArr[0];
                CatArr2[1] = CatArr[1];
                string CatFoodStr = new string(CatArr2);
                string CatFoodStr2 = new string(CatArr3);

                int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                stream.Position = 7;
                stream.WriteByte(CatFoodByte);
                stream.Position = 8;
                stream.WriteByte(CatFoodByte2);
            }
            if (StrLength == 1)
            {
                CatArr = CatFoodHex.ToCharArray(0, StrLength);
                CatArr2 = CatFoodHex.ToCharArray(0, StrLength);
                CatArr3 = CatFoodHex.ToCharArray(0, StrLength);

                CatArr2[0] = CatArr[0];

                string CatFoodStr = new string(CatArr2);
                string CatFoodStr2 = new string(CatArr3);

                int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                stream.Position = 7;
                stream.WriteByte(CatFoodByte);
                stream.Position = 8;
                stream.WriteByte(CatFoodByte2);
            }
            string[] test = { };

        }
        static void XP(string path)
        {

            Console.WriteLine("How much XP do you want?(max 99999999)");
            int XP = Convert.ToInt32(Console.ReadLine());
            if (XP > 99999999) XP = 99999999;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            string XPHex = Convert.ToString(XP, 16);
            Console.WriteLine("Set XP to " + XP);

            int StrLength = XPHex.Length;

            char[] XPArr = { '0', '0', '0', '0' };
            char[] XPArr2 = { '0', '0', '0', '0' };
            char[] XPArr3 = { '0', '0', '0', '0' };
            char[] XPArr4 = { '0', '0', '0', '0' };
            char[] XPArr5 = { '0', '0', '0', '0' };

            if (StrLength == 7)
            {
                XPArr = XPHex.ToCharArray(0, StrLength);
                XPArr2 = XPHex.ToCharArray(0, StrLength - 5);
                XPArr3 = XPHex.ToCharArray(0, StrLength - 5);
                XPArr4 = XPHex.ToCharArray(0, StrLength - 5);
                XPArr5 = XPHex.ToCharArray(0, StrLength - 5);

                XPArr2[0] = XPArr[5];
                XPArr2[1] = XPArr[6];
                XPArr3[0] = XPArr[3];
                XPArr3[1] = XPArr[4];
                XPArr4[0] = XPArr[1];
                XPArr4[1] = XPArr[2];
                XPArr5[0] = '0';
                XPArr5[1] = XPArr[0];

                string XPStr = new string(XPArr2);
                string XPStr2 = new string(XPArr3);
                string XPStr3 = new string(XPArr4);
                string XPStr4 = new string(XPArr5);

                int XPint = int.Parse(XPStr, System.Globalization.NumberStyles.HexNumber);
                int XP2int = int.Parse(XPStr2, System.Globalization.NumberStyles.HexNumber);
                int XP3int = int.Parse(XPStr3, System.Globalization.NumberStyles.HexNumber);
                int XP4int = int.Parse(XPStr4, System.Globalization.NumberStyles.HexNumber);

                string XPStrBase10 = Convert.ToString(XPint, 10);
                string XPStr2Base10 = Convert.ToString(XP2int, 10);
                string XPStr3Base10 = Convert.ToString(XP3int, 10);
                string XPStr4Base10 = Convert.ToString(XP4int, 10);

                byte XPByte = Convert.ToByte(XPStrBase10);
                byte XPByte2 = Convert.ToByte(XPStr2Base10);
                byte XPByte3 = Convert.ToByte(XPStr3Base10);
                byte XPByte4 = Convert.ToByte(XPStr4Base10);

                stream.Position = 76;
                stream.WriteByte(XPByte);
                stream.Position = 77;
                stream.WriteByte(XPByte2);
                stream.Position = 78;
                stream.WriteByte(XPByte3);
                stream.Position = 79;
                stream.WriteByte(XPByte4);
            }
            if (StrLength == 6)
            {
                XPArr = XPHex.ToCharArray(0, StrLength);
                XPArr2 = XPHex.ToCharArray(0, StrLength - 4);
                XPArr3 = XPHex.ToCharArray(0, StrLength - 4);
                XPArr4 = XPHex.ToCharArray(0, StrLength - 4);
                XPArr5 = XPHex.ToCharArray(0, StrLength - 4);

                XPArr2[0] = XPArr[5];
                XPArr2[1] = XPArr[6];
                XPArr3[0] = XPArr[3];
                XPArr3[1] = XPArr[4];
                XPArr4[0] = XPArr[1];
                XPArr4[1] = XPArr[2];

                string XPStr = new string(XPArr2);
                string XPStr2 = new string(XPArr3);
                string XPStr3 = new string(XPArr4);
                string XPStr4 = new string(XPArr5);

                int XPint = int.Parse(XPStr, System.Globalization.NumberStyles.HexNumber);
                int XP2int = int.Parse(XPStr2, System.Globalization.NumberStyles.HexNumber);
                int XP3int = int.Parse(XPStr3, System.Globalization.NumberStyles.HexNumber);
                int XP4int = int.Parse(XPStr4, System.Globalization.NumberStyles.HexNumber);

                string XPStrBase10 = Convert.ToString(XPint, 10);
                string XPStr2Base10 = Convert.ToString(XP2int, 10);
                string XPStr3Base10 = Convert.ToString(XP3int, 10);
                string XPStr4Base10 = Convert.ToString(XP4int, 10);

                byte XPByte = Convert.ToByte(XPStrBase10);
                byte XPByte2 = Convert.ToByte(XPStr2Base10);
                byte XPByte3 = Convert.ToByte(XPStr3Base10);
                byte XPByte4 = Convert.ToByte(XPStr4Base10);

                stream.Position = 76;
                stream.WriteByte(XPByte);
                stream.Position = 77;
                stream.WriteByte(XPByte2);
                stream.Position = 78;
                stream.WriteByte(XPByte3);
                stream.Position = 79;
                stream.WriteByte(XPByte4);
            }
            if (StrLength == 5)
            {
                XPArr = XPHex.ToCharArray(0, StrLength);
                XPArr2 = XPHex.ToCharArray(0, StrLength - 3);
                XPArr3 = XPHex.ToCharArray(0, StrLength - 3);
                XPArr4 = XPHex.ToCharArray(0, StrLength - 3);
                XPArr5 = XPHex.ToCharArray(0, StrLength - 3);

                XPArr2[0] = XPArr[5];
                XPArr2[1] = XPArr[6];
                XPArr3[0] = XPArr[3];
                XPArr3[1] = XPArr[4];
                XPArr4[0] = '0';
                XPArr4[1] = XPArr[2];

                string XPStr = new string(XPArr2);
                string XPStr2 = new string(XPArr3);
                string XPStr3 = new string(XPArr4);
                string XPStr4 = new string(XPArr5);

                int XPint = int.Parse(XPStr, System.Globalization.NumberStyles.HexNumber);
                int XP2int = int.Parse(XPStr2, System.Globalization.NumberStyles.HexNumber);
                int XP3int = int.Parse(XPStr3, System.Globalization.NumberStyles.HexNumber);
                int XP4int = int.Parse(XPStr4, System.Globalization.NumberStyles.HexNumber);

                string XPStrBase10 = Convert.ToString(XPint, 10);
                string XPStr2Base10 = Convert.ToString(XP2int, 10);
                string XPStr3Base10 = Convert.ToString(XP3int, 10);
                string XPStr4Base10 = Convert.ToString(XP4int, 10);

                byte XPByte = Convert.ToByte(XPStrBase10);
                byte XPByte2 = Convert.ToByte(XPStr2Base10);
                byte XPByte3 = Convert.ToByte(XPStr3Base10);
                byte XPByte4 = Convert.ToByte(XPStr4Base10);

                stream.Position = 76;
                stream.WriteByte(XPByte);
                stream.Position = 77;
                stream.WriteByte(XPByte2);
                stream.Position = 78;
                stream.WriteByte(XPByte3);
                stream.Position = 79;
                stream.WriteByte(XPByte4);
            }
            if (StrLength == 4)
            {
                XPArr = XPHex.ToCharArray(0, StrLength);
                XPArr2 = XPHex.ToCharArray(0, StrLength - 2);
                XPArr3 = XPHex.ToCharArray(0, StrLength - 2);
                XPArr4 = XPHex.ToCharArray(0, StrLength - 2);
                XPArr5 = XPHex.ToCharArray(0, StrLength - 2);

                XPArr2[0] = XPArr[5];
                XPArr2[1] = XPArr[6];
                XPArr3[0] = XPArr[3];
                XPArr3[1] = XPArr[4];

                string XPStr = new string(XPArr2);
                string XPStr2 = new string(XPArr3);
                string XPStr3 = new string(XPArr4);
                string XPStr4 = new string(XPArr5);

                int XPint = int.Parse(XPStr, System.Globalization.NumberStyles.HexNumber);
                int XP2int = int.Parse(XPStr2, System.Globalization.NumberStyles.HexNumber);
                int XP3int = int.Parse(XPStr3, System.Globalization.NumberStyles.HexNumber);
                int XP4int = int.Parse(XPStr4, System.Globalization.NumberStyles.HexNumber);

                string XPStrBase10 = Convert.ToString(XPint, 10);
                string XPStr2Base10 = Convert.ToString(XP2int, 10);
                string XPStr3Base10 = Convert.ToString(XP3int, 10);
                string XPStr4Base10 = Convert.ToString(XP4int, 10);

                byte XPByte = Convert.ToByte(XPStrBase10);
                byte XPByte2 = Convert.ToByte(XPStr2Base10);
                byte XPByte3 = Convert.ToByte(XPStr3Base10);
                byte XPByte4 = Convert.ToByte(XPStr4Base10);

                stream.Position = 76;
                stream.WriteByte(XPByte);
                stream.Position = 77;
                stream.WriteByte(XPByte2);
                stream.Position = 78;
                stream.WriteByte(XPByte3);
                stream.Position = 79;
                stream.WriteByte(XPByte4);
            }
            if (StrLength == 3)
            {
                XPArr = XPHex.ToCharArray(0, StrLength);
                XPArr2 = XPHex.ToCharArray(0, StrLength - 1);
                XPArr3 = XPHex.ToCharArray(0, StrLength - 1);
                XPArr4 = XPHex.ToCharArray(0, StrLength - 1);
                XPArr5 = XPHex.ToCharArray(0, StrLength - 1);

                XPArr2[0] = XPArr[5];
                XPArr2[1] = XPArr[6];
                XPArr3[0] = '0';
                XPArr3[1] = XPArr[4];

                string XPStr = new string(XPArr2);
                string XPStr2 = new string(XPArr3);
                string XPStr3 = new string(XPArr4);
                string XPStr4 = new string(XPArr5);

                int XPint = int.Parse(XPStr, System.Globalization.NumberStyles.HexNumber);
                int XP2int = int.Parse(XPStr2, System.Globalization.NumberStyles.HexNumber);
                int XP3int = int.Parse(XPStr3, System.Globalization.NumberStyles.HexNumber);
                int XP4int = int.Parse(XPStr4, System.Globalization.NumberStyles.HexNumber);

                string XPStrBase10 = Convert.ToString(XPint, 10);
                string XPStr2Base10 = Convert.ToString(XP2int, 10);
                string XPStr3Base10 = Convert.ToString(XP3int, 10);
                string XPStr4Base10 = Convert.ToString(XP4int, 10);

                byte XPByte = Convert.ToByte(XPStrBase10);
                byte XPByte2 = Convert.ToByte(XPStr2Base10);
                byte XPByte3 = Convert.ToByte(XPStr3Base10);
                byte XPByte4 = Convert.ToByte(XPStr4Base10);

                stream.Position = 76;
                stream.WriteByte(XPByte);
                stream.Position = 77;
                stream.WriteByte(XPByte2);
                stream.Position = 78;
                stream.WriteByte(XPByte3);
                stream.Position = 79;
                stream.WriteByte(XPByte4);
            }
            if (StrLength == 2)
            {
                XPArr = XPHex.ToCharArray(0, StrLength);
                XPArr2 = XPHex.ToCharArray(0, StrLength);
                XPArr3 = XPHex.ToCharArray(0, StrLength);
                XPArr4 = XPHex.ToCharArray(0, StrLength);
                XPArr5 = XPHex.ToCharArray(0, StrLength);

                XPArr2[0] = XPArr[5];
                XPArr2[1] = XPArr[6];

                string XPStr = new string(XPArr2);
                string XPStr2 = new string(XPArr3);
                string XPStr3 = new string(XPArr4);
                string XPStr4 = new string(XPArr5);

                int XPint = int.Parse(XPStr, System.Globalization.NumberStyles.HexNumber);
                int XP2int = int.Parse(XPStr2, System.Globalization.NumberStyles.HexNumber);
                int XP3int = int.Parse(XPStr3, System.Globalization.NumberStyles.HexNumber);
                int XP4int = int.Parse(XPStr4, System.Globalization.NumberStyles.HexNumber);

                string XPStrBase10 = Convert.ToString(XPint, 10);
                string XPStr2Base10 = Convert.ToString(XP2int, 10);
                string XPStr3Base10 = Convert.ToString(XP3int, 10);
                string XPStr4Base10 = Convert.ToString(XP4int, 10);

                byte XPByte = Convert.ToByte(XPStrBase10);
                byte XPByte2 = Convert.ToByte(XPStr2Base10);
                byte XPByte3 = Convert.ToByte(XPStr3Base10);
                byte XPByte4 = Convert.ToByte(XPStr4Base10);

                stream.Position = 76;
                stream.WriteByte(XPByte);
                stream.Position = 77;
                stream.WriteByte(XPByte2);
                stream.Position = 78;
                stream.WriteByte(XPByte3);
                stream.Position = 79;
                stream.WriteByte(XPByte4);
            }
            if (StrLength == 1)
            {
                XPArr = XPHex.ToCharArray(0, StrLength);
                XPArr2 = XPHex.ToCharArray(0, StrLength);
                XPArr3 = XPHex.ToCharArray(0, StrLength);
                XPArr4 = XPHex.ToCharArray(0, StrLength);
                XPArr5 = XPHex.ToCharArray(0, StrLength);

                XPArr2[0] = '0';
                XPArr2[1] = XPArr[6];

                string XPStr = new string(XPArr2);
                string XPStr2 = new string(XPArr3);
                string XPStr3 = new string(XPArr4);
                string XPStr4 = new string(XPArr5);

                int XPint = int.Parse(XPStr, System.Globalization.NumberStyles.HexNumber);
                int XP2int = int.Parse(XPStr2, System.Globalization.NumberStyles.HexNumber);
                int XP3int = int.Parse(XPStr3, System.Globalization.NumberStyles.HexNumber);
                int XP4int = int.Parse(XPStr4, System.Globalization.NumberStyles.HexNumber);

                string XPStrBase10 = Convert.ToString(XPint, 10);
                string XPStr2Base10 = Convert.ToString(XP2int, 10);
                string XPStr3Base10 = Convert.ToString(XP3int, 10);
                string XPStr4Base10 = Convert.ToString(XP4int, 10);

                byte XPByte = Convert.ToByte(XPStrBase10);
                byte XPByte2 = Convert.ToByte(XPStr2Base10);
                byte XPByte3 = Convert.ToByte(XPStr3Base10);
                byte XPByte4 = Convert.ToByte(XPStr4Base10);

                stream.Position = 76;
                stream.WriteByte(XPByte);
                stream.Position = 77;
                stream.WriteByte(XPByte2);
                stream.Position = 78;
                stream.WriteByte(XPByte3);
                stream.Position = 79;
                stream.WriteByte(XPByte4);
            }
        }

        static void Treasure(string path)
        {
            using var stream = new FileStream("C:/Users/henry_5ufuxnx/Downloads/LethaL-EN/test/test", FileMode.Open, FileAccess.ReadWrite);
            for (int i = 2986; i <= 3566; i += 4)
            {
                stream.Position = i;
                stream.WriteByte(03);
            }
            for (int i = 3770; i <= 4942; i += 4)
            {
                stream.Position = i;
                stream.WriteByte(03);
                Console.WriteLine("All Treasures");
            }
        }

        static void CatUpgrades(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            for (int i = 9650; i <= 11878; i += 4)
            {
                stream.Position = i;
                stream.WriteByte(Convert.ToByte(80));
                Console.WriteLine("All cats +80");
            }
            for (int i = 9688; i <= 11880; i += 4)
            {
                stream.Position = i;
                stream.WriteByte(Convert.ToByte(28));
                Console.WriteLine("All cats level 40+80");
            }
        }

        static void Leadership(string path)
        {



            Console.WriteLine("How much leadership do you want(max 255)");
            byte leadership = Convert.ToByte(Console.ReadLine());
            Console.WriteLine("How much leadership do you have currently?");
            byte leadershipCurrent = Convert.ToByte(Console.ReadLine());
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            for (int i = 0; i <= length; i++)
            {

            }
            Console.WriteLine("Scan Complete");
            for (int j = 0; j < length; j++)
            {
                //Console.WriteLine(j);
                if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 5] == leadershipCurrent && allData[j + 11] == Convert.ToByte(72) && allData[j + 12] == Convert.ToByte(57))
                {
                    stream.Position = j + 5;
                    stream.WriteByte(leadership);
                    Console.WriteLine("Success");
                }
            }
        }

        static void NP(string path)
        {
            Console.WriteLine("How much NP do you want(max 65535)");
            int NP = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("How much leadership do you have currently?");
            byte leadershipCurrent = Convert.ToByte(Console.ReadLine());
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            for (int i = 0; i <= length; i++)
            {

            }
            Console.WriteLine("Scan Complete");
            for (int j = 0; j < length; j++)
            {

                if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 5] == leadershipCurrent && allData[j + 11] == Convert.ToByte(72) && allData[j + 12] == Convert.ToByte(57))
                {
                    string NPHex = Convert.ToString(NP, 16);
                    Console.WriteLine("Set NP to " + NP);

                    int StrLength = NPHex.Length;

                    char[] CatArr = { '0', '0', '0', '0' };
                    char[] CatArr2 = { '0', '0' };
                    char[] CatArr3 = { '0', '0' };

                    if (StrLength == 3)
                    {
                        CatArr = NPHex.ToCharArray(0, StrLength);
                        CatArr2 = NPHex.ToCharArray(0, StrLength - 1);
                        CatArr3 = NPHex.ToCharArray(0, StrLength - 1);

                        CatArr2[0] = CatArr[1];
                        CatArr2[1] = CatArr[2];
                        CatArr3[0] = '0';
                        CatArr3[1] = CatArr[0];

                        string CatFoodStr = new string(CatArr2);
                        string CatFoodStr2 = new string(CatArr3);

                        int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                        int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                        string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                        string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                        byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                        byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);


                        stream.Position = j - 5;
                        stream.WriteByte(CatFoodByte);
                        stream.Position = j - 4;
                        stream.WriteByte(CatFoodByte2);
                    }
                    if (StrLength == 4)
                    {
                        CatArr = NPHex.ToCharArray(0, StrLength);
                        CatArr2 = NPHex.ToCharArray(0, StrLength - 2);
                        CatArr3 = NPHex.ToCharArray(0, StrLength - 2);

                        CatArr2[0] = CatArr[2];
                        CatArr2[1] = CatArr[3];
                        CatArr3[0] = CatArr[0];
                        CatArr3[1] = CatArr[1];

                        string CatFoodStr = new string(CatArr2);
                        string CatFoodStr2 = new string(CatArr3);

                        int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                        int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                        string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                        string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                        byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                        byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                        stream.Position = j - 5;
                        stream.WriteByte(CatFoodByte);
                        stream.Position = j - 4;
                        stream.WriteByte(CatFoodByte2);
                    }
                    if (StrLength == 2)
                    {
                        CatArr = NPHex.ToCharArray(0, StrLength);
                        CatArr2 = NPHex.ToCharArray(0, StrLength);

                        CatArr2[0] = CatArr[0];
                        CatArr2[1] = CatArr[1];
                        string CatFoodStr = new string(CatArr2);
                        string CatFoodStr2 = new string(CatArr3);

                        int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                        int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                        string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                        string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                        byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                        byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                        stream.Position = j - 5;
                        stream.WriteByte(CatFoodByte);
                        stream.Position = j - 4;
                        stream.WriteByte(CatFoodByte2);
                    }
                    if (StrLength == 1)
                    {
                        CatArr = NPHex.ToCharArray(0, StrLength);
                        CatArr2 = NPHex.ToCharArray(0, StrLength);
                        CatArr3 = NPHex.ToCharArray(0, StrLength);

                        CatArr2[0] = CatArr[0];

                        string CatFoodStr = new string(CatArr2);
                        string CatFoodStr2 = new string(CatArr3);

                        int CatFoodint = int.Parse(CatFoodStr, System.Globalization.NumberStyles.HexNumber);
                        int CatFood2int = int.Parse(CatFoodStr2, System.Globalization.NumberStyles.HexNumber);

                        string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                        string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                        byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                        byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                        stream.Position = j - 5;
                        stream.WriteByte(CatFoodByte);
                        stream.Position = j - 4;
                        stream.WriteByte(CatFoodByte2);
                    }
                }
            }
        }
    }
}
    

