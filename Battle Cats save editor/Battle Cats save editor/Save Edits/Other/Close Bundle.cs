using System;
using System.IO;

namespace Battle_Cats_save_editor.SaveEdits
{
	public class CloseBundle
	{
		public static void Bundle(string path)
		{
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
			{
				int length = (int)stream.Length;
				byte[] allData = new byte[length];
				stream.Read(allData, 0, length);
				bool found = false;
				for (int i = 0; i < length - 32; i++)
				{
					bool flag = allData[i] == 49 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 0 && allData[i + 4] == 50 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 51 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 0;
					if (flag)
					{
						stream.Position = i - 4;
						byte[] data = new byte[]
						{
							byte.MaxValue,
							byte.MaxValue,
							byte.MaxValue
						};
						stream.Write(data, 0, 3);
						found = true;
						break;
					}
				}
				bool flag2 = !found;
				if (flag2)
				{
					Editor.Error("Error, a position couldn't be found, please report this in #bug-reports on discord");
				}
				Console.WriteLine("Closed all bundle menus");
			}
		}
	}
}
