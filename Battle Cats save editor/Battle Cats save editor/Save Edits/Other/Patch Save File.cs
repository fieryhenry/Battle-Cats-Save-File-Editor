using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Battle_Cats_save_editor.SaveEdits
{
	public class PatchSaveFile
	{
		public static string GetMD5SumBC(string path, string game_ver)
		{
			bool flag = game_ver == "jp";
			if (flag)
			{
				game_ver = "";
			}
			List<byte> allData = File.ReadAllBytes(path).ToList();
			List<byte> game_version_bytes = Encoding.ASCII.GetBytes("battlecats" + game_ver).ToList();
			MD5 md5 = MD5.Create();
			List<byte> toUse = allData;
			toUse.RemoveRange(toUse.Count - 32, 32);
			game_version_bytes.AddRange(toUse);
			List<byte> hash_data = game_version_bytes;
			byte[] hash = md5.ComputeHash(hash_data.ToArray());
			string hash_str = Editor.ByteArrayToString(hash);
			return hash_str.ToLower();
		}

		public static string DetectGameVersion(string path)
		{
			string[] gameVersions = new string[]
			{
				"jp",
				"en",
				"kr",
				"tw"
			};
			List<byte> allData = File.ReadAllBytes(path).ToList();
			byte[] curr_hash = allData.GetRange(allData.Count - 32, 32).ToArray();
			string curr_hash_str = Encoding.ASCII.GetString(curr_hash);
			foreach (string game_version in gameVersions)
			{
				string hash_str = GetMD5SumBC(path, game_version);
				bool flag = hash_str == curr_hash_str;
				if (flag)
				{
					return game_version;
				}
			}
			return "";
		}

		public static void patchSaveFile(string path)
		{
			string name = Path.GetFileName(path);
			bool flag = name.EndsWith(".pack") || name.EndsWith(".list") || name.EndsWith(".so") || name.EndsWith(".csv");
			if (!flag)
			{
				string hash = GetMD5SumBC(path, Editor.gameVer);
				int len = File.ReadAllBytes(path).Length;
                using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
                stream.Position = len - 32;
                stream.Write(Encoding.ASCII.GetBytes(hash), 0, hash.Length);
                Editor.ColouredText("&Patched save data for game version: &" + Editor.gameVer + "&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            }
		}
	}
}
