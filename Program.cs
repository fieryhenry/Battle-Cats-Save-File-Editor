using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor
{
    class Program
    {

        [STAThreadAttribute]
        static void Main()
        {
            string folderName = @"newversion.txt";



            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt", folderName);

            string[] lines = System.IO.File.ReadAllLines(@"newversion.txt");
            if (lines[0] == "2.8.1")
            {
                Console.WriteLine("Program is updated");
            }
            else
            {
                Console.WriteLine("Please update to the newest version \nhttps://github.com/fieryhenry/Battle-Cats-Save-File-Editor/releases/tag/{0}", lines[0]);
            }

            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;

                System.IO.FileInfo File = new System.IO.FileInfo(FD.FileName);

                string path = Path.Combine(fileToOpen);
                Console.WriteLine("\nWhat do you want to do?\n1. Change Cat food\n2. Change XP\n3. All treasures\n4. All cats upgraded 40+80\n5. Change leadership\n6. Change NP\n7. Change cat tickets\n8. change rare cat tickets" +
                    "\n9. Change platinum tickets\n10. All cats from clearing stages\n11. Change gacha seed\n12. All cats evolved\n13. Change battle item count\n14. Change Catamins" +
                    "\n15. Change base materials\n16. Change catseyes\n17. All cats\n18. Get a specific cat\n19. Upgrade a specific cat to a specific level");
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
                    case 7:
                        CatTicket(path);
                        break;
                    case 8:
                        CatTicketRare(path);
                        break;
                    case 9:
                        PlatTicketRare(path);
                        break;
                    case 10:
                        ClearStageCat(path);
                        break;
                    case 11:
                        Seed(path);
                        break;
                    case 12:
                        Evolve(path);
                        break;
                    case 13:
                        Items(path);
                        break;
                    case 14:
                        Catamin(path);
                        break;
                    case 15:
                        BaseMats(path);
                        break;
                    case 16:
                        Catseyes(path);
                        break;
                    case 17:
                        Cats(path);
                        break;
                    case 18:
                        SpecifiCat(path);
                        break;
                    case 19:
                        SpecifUpgrade(path);
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

                char[] CatArr = { };
                char[] CatArr0 = { '0' };
                CatArr = CatFoodHex.ToCharArray(0, StrLength);

                var bytes = ConvertByte(CatFood);
                byte byte1 = bytes.Item1;
                byte byte2 = bytes.Item2;
                byte byte3 = bytes.Item3;
                byte byte4 = bytes.Item4;

                stream.Position = 7;
                stream.WriteByte(byte1);
                stream.Position = 8;
                stream.WriteByte(byte2);
            }

            static void XP(string path)
            {

                Console.WriteLine("How much XP do you want?(max 99999999)");
                int XP = Convert.ToInt32(Console.ReadLine());
                if (XP > 99999999) XP = 99999999;

                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                Console.WriteLine("Set XP to " + XP);

                char[] XPArr = { };
                char[] XPArr0 = { '0' };

                var bytes = ConvertByte(XP);
                byte byte1 = bytes.Item1;
                byte byte2 = bytes.Item2;
                byte byte3 = bytes.Item3;
                byte byte4 = bytes.Item4;

                stream.Position = 76;
                stream.WriteByte(byte1);
                stream.Position = 77;
                stream.WriteByte(byte2);
                stream.Position = 78;
                stream.WriteByte(byte3);
                stream.Position = 79;
                stream.WriteByte(byte4);

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
                for (int i = 9694; i <= 11878; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(80));
                    Console.WriteLine("All cats +80");
                }
                for (int i = 9686; i <= 11880; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(40));
                    Console.WriteLine("All cats level 40+80");
                }
            }

            static void Leadership(string path)
            {
                Console.WriteLine("How much leadership do you want(max 65535)");
                int CatFood = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);


                char[] CatArrL = { };
                char[] CatArr0L = { '0' };

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 11] == Convert.ToByte(72) && allData[j + 12] == Convert.ToByte(57))
                    {
                        string CatFoodHex = Convert.ToString(CatFood, 16);


                        int StrLength = CatFoodHex.Length;

                        char[] CatArr = { };
                        char[] CatArr0 = { '0' };
                        CatArr = CatFoodHex.ToCharArray(0, StrLength);

                        var bytes = ConvertByte(CatFood);
                        byte byte1 = bytes.Item1;
                        byte byte2 = bytes.Item2;
                        byte byte3 = bytes.Item3;
                        byte byte4 = bytes.Item4;

                        stream.Position = j + 5;
                        stream.WriteByte(byte1);
                        stream.Position = j + 6;
                        stream.WriteByte(byte2);

                        Console.WriteLine("Success");
                    }
                }
            }

            static void NP(string path)
            {
                Console.WriteLine("How much NP do you want(max 65535)");
                int CatFood = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                char[] CatArrL = { };
                char[] CatArr0L = { '0' };


                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 11] == Convert.ToByte(72) && allData[j + 12] == Convert.ToByte(57))
                    {
                        string CatFoodHex = Convert.ToString(CatFood, 16);


                        char[] CatArr = { };
                        char[] CatArr0 = { '0' };

                        var bytes = ConvertByte(CatFood);
                        byte byte1 = bytes.Item1;
                        byte byte2 = bytes.Item2;
                        byte byte3 = bytes.Item3;
                        byte byte4 = bytes.Item4;

                        stream.Position = j - 5;
                        stream.WriteByte(byte1);
                        stream.Position = j - 4;
                        stream.WriteByte(byte2);
                        
                        Console.WriteLine("Success");
                    }
                }
            }
        
            static void CatTicket(string path)
            {
                Console.WriteLine("How much Cat Tickets do you want(max 255)");
                byte catTickets = Convert.ToByte(Console.ReadLine());
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
                    if (allData[j] == Convert.ToByte(131) && allData[j + 1] == Convert.ToByte(142) && allData[j + 2] == Convert.ToByte(123) && allData[j + 3] == Convert.ToByte(00) && allData[j - 2] == Convert.ToByte(122) && allData[j - 3] == Convert.ToByte(142))
                    {
                        stream.Position = j + 24;
                        stream.WriteByte(catTickets);
                        Console.WriteLine("Success");
                    }
                }

            }

            static void CatTicketRare(string path)
            {
                Console.WriteLine("How much Rare Cat Tickets do you want(max 255)");
                byte rareCatTickets = Convert.ToByte(Console.ReadLine());
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
                    if (allData[j] == Convert.ToByte(131) && allData[j + 1] == Convert.ToByte(142) && allData[j + 2] == Convert.ToByte(123) && allData[j + 3] == Convert.ToByte(00) && allData[j - 2] == Convert.ToByte(122) && allData[j + -3] == Convert.ToByte(142))
                    {
                        stream.Position = j + 28;
                        stream.WriteByte(rareCatTickets);
                        Console.WriteLine("Success");
                    }
                }
            }

            static void PlatTicketRare(string path)
            {
                Console.WriteLine("How much Platinum Cat Tickets do you want(max 9 any more and ban)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                if (platCatTickets > 9) platCatTickets = 9;
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
                    if (allData[j] == Convert.ToByte(255) && allData[j + 1] == Convert.ToByte(255) && allData[j + 2] == Convert.ToByte(255) && allData[j + 3] == Convert.ToByte(255) && allData[j + 4] == Convert.ToByte(255) && allData[j + 5] == Convert.ToByte(255) && allData[j + 6] == Convert.ToByte(255) && allData[j + 7] == Convert.ToByte(00) && allData[j + 8] == Convert.ToByte(54))
                    {
                        stream.Position = j + 24;
                        stream.WriteByte(platCatTickets);
                        Console.WriteLine("Success");
                    }
                }
            }

            static void ClearStageCat(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 1503; j++)
                {
                    if (allData[j] == Convert.ToByte(00) && allData[j + 2] == Convert.ToByte(00) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 6] == Convert.ToByte(00) && allData[j + 7] == Convert.ToByte(00) && allData[j + 8] == Convert.ToByte(00) && allData[j + 10] == Convert.ToByte(00) && allData[j + 145] == Convert.ToByte(44) && allData[j + 146] == Convert.ToByte(01) && allData[j + 1503] == 250)
                    {
                        Console.WriteLine("Success");
                        for (int i = 0; i <= j + 482; i += 4)
                        {
                            stream.Position = i;
                            stream.WriteByte(01);
                        }
                    }
                }

            }

            static void Seed(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 1503; j++)
                {
                    if (allData[j] == Convert.ToByte(01) && allData[j + 5] == Convert.ToByte(228) && allData[j + 6] == Convert.ToByte(07) && allData[j + 9] == Convert.ToByte(11) && allData[j + 1] == Convert.ToByte(00) && allData[j + 2] == Convert.ToByte(00) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 7] == Convert.ToByte(00))
                    {
                        Console.WriteLine("What seed do you want?(max 99999999)");
                        int XP = (int)Convert.ToInt64(Console.ReadLine());
                        if (XP > 99999999) XP = 99999999;

                        char[] XPArr = { };
                        char[] XPArr0 = { '0' };

                        var bytes = ConvertByte(XP);
                        byte byte1 = bytes.Item1;
                        byte byte2 = bytes.Item2;
                        byte byte3 = bytes.Item3;
                        byte byte4 = bytes.Item4;

                        stream.Position = j - 16;
                        stream.WriteByte(byte1);
                        stream.Position = j - 15;
                        stream.WriteByte(byte2);
                        stream.Position = j - 14;
                        stream.WriteByte(byte3);
                        stream.Position = j - 13;
                        stream.WriteByte(byte4);
                        
                    }
                }
            }

            static void Evolve(string path)
            {
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
                    if (allData[j] == Convert.ToByte(01) && allData[j + 1] == Convert.ToByte(01) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(01) && allData[j + 4] == Convert.ToByte(01) && allData[j + 5] == Convert.ToByte(01) && allData[j + 6] == Convert.ToByte(01) && allData[j + 7] == Convert.ToByte(00) && allData[j + 24] == Convert.ToByte(70) && allData[j + 25] == Convert.ToByte(02))
                    {
                        for (int i = j + 60; i < j + 2710; i += 4)
                        {
                            stream.Position = j;
                            stream.WriteByte(02);
                        }
                        Console.WriteLine("Success");
                    }
                }
            }

            static void Items(string path)
            {
                Console.WriteLine("How much of each item do you want(max 255)");
                byte catTickets = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                stream.Position = 14474;
                stream.WriteByte(catTickets);
                stream.Position = 14478;
                stream.WriteByte(catTickets);
                stream.Position = 14482;
                stream.WriteByte(catTickets);
                stream.Position = 14486;
                stream.WriteByte(catTickets);
                stream.Position = 14490;
                stream.WriteByte(catTickets);
                stream.Position = 14494;
                stream.WriteByte(catTickets);
            }

            static void Catamin(string path)
            {
                Console.WriteLine("How many Catimins do you want(max 255)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many Catimin A do you have");
                byte catA = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many Catimin B do you have");
                byte catB = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many Catimin C do you have");
                byte catC = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(03) && allData[j + 4] == catA && allData[j + 8] == catB && allData[j + 12] == catC && allData[j + 45] == Convert.ToByte(10) && allData[j + 49] == Convert.ToByte(01) && allData[j + 50] == Convert.ToByte(01) && allData[j + 59] == Convert.ToByte(27))
                    {
                        stream.Position = j + 4;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j + 8;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j + 12;
                        stream.WriteByte(platCatTickets);
                        Console.WriteLine("Success");
                    }
                }
            }

            static void BaseMats(string path)
            {
                Console.WriteLine("How many Base Materials do you want(max 255)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many bricks do you have");
                byte catA = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(02) && allData[j + 4] == Convert.ToByte(03) && allData[j + 12] == Convert.ToByte(01) && allData[j + 16] == Convert.ToByte(02) && allData[j + 178] == Convert.ToByte(64) && allData[j + 194] == Convert.ToByte(65) && allData[j + -61] == Convert.ToByte(08) && allData[j - 57] == catA)
                    {
                        stream.Position = j - 57;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 53;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 49;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 45;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 42;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 39;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 36;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 33;
                        stream.WriteByte(platCatTickets);
                        Console.WriteLine("Success");
                    }
                }
            }

            static void Catseyes(string path)
            {
                Console.WriteLine("How many Catseyes do you want(max 65535)");
                int platCatTickets = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j - 3] == Convert.ToByte(05) && allData[j + 20] == Convert.ToByte(03) && allData[j + 57] == Convert.ToByte(10) && allData[j + 71] == Convert.ToByte(27))
                    {
                        string CatFoodHex = Convert.ToString(platCatTickets, 16);


                        int StrLength = CatFoodHex.Length;

                        char[] CatArr = { };
                        char[] CatArr0 = { '0' };
                        CatArr = CatFoodHex.ToCharArray(0, StrLength);

                        if (StrLength == 4)
                        {
                            string add1 = new string(CatArr, 2, 2);
                            string add2 = new string(CatArr, 0, 2);


                            int CatFoodint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                            int CatFood2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);

                            string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                            string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);


                            byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                            byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                            stream.Position = j;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 1;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 4;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 5;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 8;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 9;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 12;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 13;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte2);
                        }
                        else if (StrLength == 3)
                        {

                            string add1 = new string(CatArr, 1, 2);
                            string add2 = new string(CatArr0, 0, 1) + (CatArr[0]);


                            int CatFoodint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                            int CatFood2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);

                            string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                            string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                            byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                            byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                            stream.Position = j;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 1;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 4;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 5;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 8;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 9;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 12;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 13;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte2);
                        }
                        else if (StrLength == 2)
                        {
                            string add1 = new string(CatArr, 0, 2);
                            string add2 = new string(CatArr0, 0, 1) + (CatArr0[0]);

                            int CatFoodint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                            int CatFood2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);

                            string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                            string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                            byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                            byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                            stream.Position = j;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 1;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 4;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 5;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 8;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 9;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 12;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 13;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte2);
                        }
                        else if (StrLength == 1)
                        {
                            string add1 = new string(CatArr, 0, 1) + (CatArr0[0]);
                            string add2 = new string(CatArr0, 0, 1) + (CatArr0[0]);

                            int CatFoodint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                            int CatFood2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);

                            string CatFoodStrBase10 = Convert.ToString(CatFoodint, 10);
                            string CatFoodStr2Base10 = Convert.ToString(CatFood2int, 10);

                            byte CatFoodByte = Convert.ToByte(CatFoodStrBase10);
                            byte CatFoodByte2 = Convert.ToByte(CatFoodStr2Base10);

                            stream.Position = j;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 1;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 4;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 5;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 8;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 9;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 12;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 13;
                            stream.WriteByte(CatFoodByte2);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte);
                            stream.Position = j + 16;
                            stream.WriteByte(CatFoodByte2);

                            Console.WriteLine("Success");
                        }
                    }
                }

            }

            static void Cats(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                for (int i = 7362; i <= 9686; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(01));
                }

            }

            static void SpecifiCat(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat ID?");
                int catID = Convert.ToInt32(Console.ReadLine());

                int startPos = 7362;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(01);

            }

            static void SpecifUpgrade(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat ID?");
                int catID = Convert.ToInt32(Console.ReadLine());
                int startPosID = 9694 + catID * 4;
                Console.WriteLine("What base level do you want?(max 40)");
                byte Levelbase = Convert.ToByte(Console.ReadLine());
                if (Levelbase > 40) Levelbase = 40;
                Console.WriteLine("What plus level do you want?(max +80)");
                byte Levelplus = Convert.ToByte(Console.ReadLine());
                if (Levelplus > 80) Levelplus = 80;

                stream.Position = startPosID;
                stream.WriteByte(Levelplus);
                Console.WriteLine("cat +" + Levelplus);
                
                stream.Position = startPosID + 2;
                stream.WriteByte(Levelbase);
                Console.WriteLine("cat level " + Levelbase);
                
            }

            static (byte, byte, byte, byte) ConvertByte(int input)
            {
                string XPHex = Convert.ToString(input, 16);
                int StrLength = XPHex.Length;

                char[] XPArr = { };
                char[] XPArr0 = { '0' };

                if (StrLength == 8)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 6, 2);
                    string add2 = new string(XPArr, 4, 2);
                    string add3 = new string(XPArr, 2, 2);
                    string add4 = new string(XPArr, 0, 2);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }

                if (StrLength == 7)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 5, 2);
                    string add2 = new string(XPArr, 3, 2);
                    string add3 = new string(XPArr, 1, 2);
                    string add4 = new string(XPArr0, 0, 1) + (XPArr[0]);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }
                if (StrLength == 6)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 4, 2);
                    string add2 = new string(XPArr, 2, 2);
                    string add3 = new string(XPArr, 0, 2);
                    string add4 = new string(XPArr0, 0, 1) + (XPArr0[0]);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }
                if (StrLength == 5)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 3, 2);
                    string add2 = new string(XPArr, 1, 2);
                    string add3 = new string(XPArr, 0, 1) + (XPArr0[0]);
                    string add4 = new string(XPArr0, 0, 1) + (XPArr0[0]);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }
                if (StrLength == 4)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 2, 2);
                    string add2 = new string(XPArr, 0, 2);
                    string add3 = new string(XPArr0, 0, 1) + (XPArr0[0]);
                    string add4 = new string(XPArr0, 0, 1) + (XPArr0[0]);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }
                if (StrLength == 3)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 1, 2);
                    string add2 = new string(XPArr, 0, 1) + (XPArr0[0]);
                    string add3 = new string(XPArr0, 0, 1) + (XPArr0[0]);
                    string add4 = new string(XPArr0, 0, 1) + (XPArr0[0]);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }
                if (StrLength == 2)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 0, 2);
                    string add2 = new string(XPArr0, 0, 1) + (XPArr0[0]);
                    string add3 = new string(XPArr0, 0, 1) + (XPArr0[0]);
                    string add4 = new string(XPArr0, 0, 1) + (XPArr0[0]);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }
                if (StrLength == 1)
                {
                    XPArr = XPHex.ToCharArray(0, StrLength);

                    string add1 = new string(XPArr, 0, 1) + (XPArr0[0]);
                    string add2 = new string(XPArr0, 0, 1) + (XPArr0[0]);
                    string add3 = new string(XPArr0, 0, 1) + (XPArr0[0]);
                    string add4 = new string(XPArr0, 0, 1) + (XPArr0[0]);

                    int XPint = int.Parse(add1, System.Globalization.NumberStyles.HexNumber);
                    int XP2int = int.Parse(add2, System.Globalization.NumberStyles.HexNumber);
                    int XP3int = int.Parse(add3, System.Globalization.NumberStyles.HexNumber);
                    int XP4int = int.Parse(add4, System.Globalization.NumberStyles.HexNumber);

                    string XPStrBase10 = Convert.ToString(XPint, 10);
                    string XPStr2Base10 = Convert.ToString(XP2int, 10);
                    string XPStr3Base10 = Convert.ToString(XP3int, 10);
                    string XPStr4Base10 = Convert.ToString(XP4int, 10);

                    byte XPByte = Convert.ToByte(XPStrBase10);
                    byte XPByte2 = Convert.ToByte(XPStr2Base10);
                    byte XPByte3 = Convert.ToByte(XPStr3Base10);
                    byte XPByte4 = Convert.ToByte(XPStr4Base10);

                    return (XPByte, XPByte2, XPByte3, XPByte4);
                }

                return (00, 00, 00, 00);
            }

        }
    }
}

    

