using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Newtonsoft.Json;

using LocalizationItem = System.Tuple<string, string[]>;

public static class LocalizationTool
{
	private class OutputJson
	{
		public string CultureCode;
		public Dictionary<string, string> KeyToStringDictionary;
	}

	private static string _configPath = "./config.ini";
	private static int _headerRow = 1;
	private static string _inputPath = "./localization_strings.xlsx";
	private static string _outPathTemplate = "./location_string_<lang>.json";
	private static string _resourcesPath = "../../../Assets/Resources/";

	public static void Main(string[] args)
	{
		try
		{
			// Get config
			int arg = 1;
			for (; arg < args.Length; ++arg)
			{
				if (arg == 1)
				{
					_configPath = args[1];
				}
			}

			Log("----------------------- Localization Export Tool v1.0 -----------------------");
			Log($"Config path:  \"{Path.GetFullPath(_configPath)}\" {(arg >= 1 ? "" : "(default)")}");


			// Get paths
			var lines = File.ReadAllLines(_configPath);
			foreach (var line in lines)
			{
				var tokens = line.Split('=');
				if (tokens.Length != 2)
				{
					Log($"- Line\"{line}\" is faulty, skipped.", textColor: ConsoleColor.Yellow);
				}

				var argument = tokens[0];
				var value = tokens[1];

				if (argument == "INPUT_PATH")
				{
					_inputPath = value;
				}
				else if (argument == "OUTPUT_TEMPLATE")
				{
					_outPathTemplate = value;
				}
				else if (argument == "RESOURCES_DESTINATION")
				{
					_resourcesPath = value;
				}
				else if (argument == "HEADER_ROW")
				{
					if (int.TryParse(value, out int res))
					{
						_headerRow = res;
					}
					else
					{
						Log($"- HEADER_ROW received value \"{value}\", which cannot be parsed to int, skipped.", textColor: ConsoleColor.Yellow);
					}
				}
			}

			Log("\n------# Config:", ConsoleColor.Yellow);
			Log($"Header row:  	{_headerRow}");
			Log($"Input path:  	\"{Path.GetFullPath(_inputPath)}\"");
			Log($"Output path: 	\"{Path.GetFullPath(_outPathTemplate)}\"");
			Log($"Resouces path: 	\"{Path.GetFullPath(_resourcesPath)}\"");


			// Get book
			Log("\n------# Now reading xlsx book:", ConsoleColor.Yellow);
			IWorkbook book = ReadBook(_inputPath);

			if (book == null)
			{
				throw new ArgumentNullException("ReadBook returns null.");
			}
			Log($"------# Read book at {_inputPath} completed");


			// Get local strings

			Log("\n------# Now getting localization strings:", ConsoleColor.Yellow);
			var outputs = new Dictionary<string, OutputJson>();

			foreach (var sheet in IterateSheets(book))
			{
				Log($"+ Now Reading sheet {sheet.SheetName}");
				foreach (var item in GetLocalizationStrings(sheet, out string[] languages))
				{
					var key = item.Item1;
					var strings = item.Item2;

					foreach (var language in languages)
					{
						if (!outputs.ContainsKey(language))
						{
							outputs[language] = new OutputJson();
							outputs[language].CultureCode = language;
							Log($"+ New language detected: {language}");
						}
					}

					for (int i = 0; i < strings.Length; ++i)
					{
						var language = languages[i];
						var localString = strings[i];
						if (localString == "")
						{
							Log($"- String \"{key}\" in language \"{language}\" is empty, this might not be desirable.", textColor: ConsoleColor.Yellow);
						}

						outputs[language].KeyToStringDictionary[key] = localString;
					}

				}
			}

			Log("------# Getting localization strings done.");


			// Output

			Log("\n------# Now outputing jsons:", ConsoleColor.Yellow);
			foreach (var output in outputs)
			{
				var path = _outPathTemplate.Replace("<lang>", output.Key);
				var writableText = JsonConvert.SerializeObject(output, Formatting.Indented);
				File.WriteAllText(path, writableText);
			}
			Log("------# Outputing jsons done.");


			// Copy to destionation
			Log("\n------# Now copying to RESOURCE_DESTINATION", ConsoleColor.Yellow);

			Log("------# Copying to RESOURCE_DESTINATION done.");

		}
		catch (Exception e)
		{
			ThrowAndTerminate(e);
		}
	}

	private static void Log(string log, ConsoleColor backColor=ConsoleColor.Black, ConsoleColor textColor=ConsoleColor.White)
	{
		Console.BackgroundColor = backColor;
		Console.ForegroundColor = textColor;
		Console.WriteLine(log);
		Console.ResetColor();
	}

	private static IWorkbook ReadBook(string inputPath)
	{
		using (FileStream fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
		{
			return WorkbookFactory.Create(fileStream);
		}
	}

	private static IEnumerable<ISheet> IterateSheets(IWorkbook book)
	{
		var total = book.NumberOfSheets;

		for (int i = 0; i < book.NumberOfSheets; ++i)
		{
			yield return book.GetSheetAt(i);
		}
	}

	private static List<LocalizationItem> GetLocalizationStrings(ISheet sheet, out string[] languages)
	{
		throw new NotImplementedException();
	}

	private static void ThrowAndTerminate(Exception e)
	{
		Log("Execution failed, exception is detailed below:", ConsoleColor.Red);
		throw e;
	}
}