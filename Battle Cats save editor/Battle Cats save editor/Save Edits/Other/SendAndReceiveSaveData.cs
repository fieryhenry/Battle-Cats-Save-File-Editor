using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class SendAndReceiveSaveData
    {
        public static byte[] ReadFully(Stream input)
        {
            using MemoryStream ms = new();
            input.CopyTo(ms);
            return ms.ToArray();
        }
        public static string GetRandomHexNumber(int digits)
        {
            Random random = new();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
        public static byte[] ByteARequest(WebRequest request)
        {
            WebResponse response = request.GetResponse();
            using Stream dataStream = response.GetResponseStream();
            byte[] data = ReadFully(dataStream);
            return data;
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void Upload_Save_Data(string path)
        {
            Editor.ColouredText("Thanks to Lethal's editor this feature exists\nI thought its restore feature was broken, but apparently it's not\n", New:ConsoleColor.Green);
            string url = "https://nyanko.ponosgames.com/?action=store&country=en";
            string random_str = RandomString(22);
            string boundary = "__-----------------------" + random_str;
            string content_type = "multipart/form-data;boundary=" + boundary;

            List<byte> body_data = new();

            string top_body = "--" + boundary + "\n";
            top_body += "Content-Disposition: attachment; name=\"saveData\";filename=\"data.sav\"Content-Type: application/octet-stream\n\n";
           
            body_data.AddRange(Encoding.ASCII.GetBytes(top_body));
            body_data.AddRange(File.ReadAllBytes(path));

            string bottom_body = "\n--" + boundary + "--";
            body_data.AddRange(Encoding.ASCII.GetBytes(bottom_body));

            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = content_type;
            request.ContentLength = body_data.Count;
            request.Headers.Add("accept-encoding", "gzip, deflate");
            request.Headers.Add("charset", "UTF-8");

            Console.WriteLine("Adding save data to request...");
            Stream stream = request.GetRequestStream();
            stream.Write(body_data.ToArray(), 0, body_data.Count);
            stream.Close();

            string response;
            try
            {
                Console.WriteLine("Uploading save data to server...");
                response = Encoding.ASCII.GetString(ByteARequest(request));
            }
            catch
            {
                Console.WriteLine("Error, failed to upload save data");
                return;
            }
            JObject response_object = JObject.Parse(response);
            if (Convert.ToBoolean(response_object["success"])) Editor.ColouredText($"&\nSuccessfully uploaded save data\nTransfer code: &{response_object["accountId"]}&\nConfirmation code: &{response_object["pin"]}&\n\n");
            else Console.WriteLine("Error, failed to upload save data");
        }
        public static void Download_Save_Data()
        {
            Console.WriteLine("Enter transfer code:");
            string transfer_code = Console.ReadLine();
            Console.WriteLine("Enter confirmation code:");
            string confirmation_code = Console.ReadLine();
            Console.WriteLine("Enter game version:(e.g 11.0.0, 9.8.0, 10.4.1)");
            string game_version = Console.ReadLine();
            string[] game_version_parts = game_version.Split('.');
            for (int i = 0; i < game_version_parts.Length; i++)
            {
                if (game_version_parts[i].Length == 1)
                {
                    game_version_parts[i] = "0" + game_version_parts[i];
                }
            }
            game_version = string.Join("", game_version_parts);

            client client = new()
            {
                countryCode = Editor.gameVer,
                version = game_version,
            };
            device device = new()
            {
                model = "SM-G973N"
            };
            os os = new()
            {
                type = "android",
                version = "7.1.2"
            };

            clientInfo clientInfo = new()
            {
                client = client,
                device = device,
                os = os,
            };

            jsonData data = new()
            {
                clientInfo = clientInfo,
                nonce = GetRandomHexNumber(32).ToLower(),
                pin = confirmation_code
            };
            string json = JsonSerializer.Serialize(data);
            ASCIIEncoding encoding = new();
            byte[] byte1 = encoding.GetBytes(json);

            WebRequest webRequest = WebRequest.Create($"https://nyanko-save.ponosgames.com/v1/transfers/{transfer_code}/reception");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("accept-encoding", "gzip");
            webRequest.ContentLength = byte1.Length;

            Stream newStream = webRequest.GetRequestStream();

            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();
            byte[] saveData = null;
            try
            {
                Console.WriteLine("Downloading save data...");
                saveData = ByteARequest(webRequest);
            }
            catch
            {
                Console.WriteLine("Error, your transfer code or confirmation codes are incorrect or are no longer valid, please try again");
                Download_Save_Data();
            }
            if (saveData.Length < 2000)
            {
                Console.WriteLine("Error, your transfer code or confirmation codes are incorrect or are no longer valid, please try again");
                Download_Save_Data();
            }      
            var FD = new SaveFileDialog
            {
                Filter = "battle cats save(*.*)|*.*",
                Title = "Save save file"
            };
            string path;
            if (FD.ShowDialog() == DialogResult.OK)
            {
                path = FD.FileName;
            }
            else
            {
                path = $@"/{DateTime.Now.Ticks}";
            }
            File.WriteAllBytes(path, saveData);
            Console.WriteLine($"Successfully got save data, file found at {path}");
            Editor.main_path = path;
        }
    }
    public class responseJson
    {
        public int statusCode { get; set; }
        public string nonce { get; set; }
        public string payload { get; set; }
        public int timestamp { get; set; }
    }
    public class jsonData
    {
        public clientInfo clientInfo { get; set; }
        public string nonce { get; set; }
        public string pin { get; set; }

    }
    public class clientInfo
    {
        public client client { get; set; }
        public device device { get; set; }
        public os os { get; set; }
    }
    public class client
    {
        public string countryCode { get; set; }
        public string version { get; set; }
    }
    public class device
    {
        public string model { get; set; }
    }
    public class os
    {
        public string type { get; set; }
        public string version { get; set; }
    }

}
