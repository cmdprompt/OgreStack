using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System;

namespace OgreStack.PersistentData
{
	internal static class DataUtil
	{
		internal static readonly string _OVERRIDES_FILE_NAME = "Overrides.xml";

		//=====================================================================================================\\

		internal static string GenerateFilePath(string fileName)
		{
			// hugslib persistentdatamanager
			string path = Path.Combine(Verse.GenFilePaths.SaveDataFolderPath, "OgreStack");
			DirectoryInfo di = new DirectoryInfo(path);
			if (!di.Exists) { di.Create(); }

			return Path.Combine(path, fileName);
		}

		//=====================================================================================================\\

		internal static void WriteFisherPriceCSV(List<string[]> data, string fileName)
		{
			if (data != null && data.Any())
			{
				string filePath = DataUtil.GenerateFilePath(fileName);

				using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				{
					using (StreamWriter w = new StreamWriter(fs))
					{
						foreach (string[] dt in data)
						{
							int count = dt.GetLength(0);
							for (int i = 0; i < count - 1; i++)
							{
								w.Write(dt[i]);
								w.Write(",");
							}
							w.WriteLine(dt[count - 1]);
						}
					}
				}
			}
		}

		//=====================================================================================================\\

		internal static XDocument GetUserOverrides()
		{
			string fileName = DataUtil.GenerateFilePath(_OVERRIDES_FILE_NAME);

			if (!File.Exists(fileName))
			{
				DataUtil.WriteDefaultOverrides();
			}

			return XDocument.Load(fileName);
		}

		//=====================================================================================================\\

		internal static void WriteDefaultOverrides()
		{
			string fileName = DataUtil.GenerateFilePath(_OVERRIDES_FILE_NAME);
			if (File.Exists(fileName)) { return; }

			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter w = new StreamWriter(fs))
				{
					w.Write(IndividualOverrides.GenerateDefaultOverridesXml());
				}
			}
		}
	}
}
