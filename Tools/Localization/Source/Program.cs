using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Newtonsoft.Json;

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
	private static string _outPath = "./";
	private static string _outName = "location_string_<lang>.json";
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
				else if (argument == "OUTPUT_PATH")
				{
					_outPath = value;
				}
				else if (argument == "OUTPUT_NAME_TEMPLATE")
				{
					_outName = value;
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

			Log("\n------# Config:", textColor: ConsoleColor.Green);
			Log($"Header row:  	{_headerRow}");
			Log($"Input path:  	\"{Path.GetFullPath(_inputPath)}\"");
			Log($"Output path: 	\"{Path.GetFullPath(_outPath)}\"");
			Log($"Output name: 	\"{Path.GetFullPath(_outName)}\"");
			Log($"Resouces path: 	\"{Path.GetFullPath(_resourcesPath)}\"");


			// Get book
			Log("\n------# Now reading xlsx book:", textColor: ConsoleColor.Green);
			IWorkbook book = ReadBook(_inputPath);

			if (book == null)
			{
				throw new ArgumentNullException("ReadBook returns null.");
			}
			Log($"------# Read book at {_inputPath} completed");


			// Get local strings

			Log("\n------# Now getting localization strings:", textColor: ConsoleColor.Green);
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
							outputs[language].KeyToStringDictionary = new Dictionary<string, string>();
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

						if (outputs[language].KeyToStringDictionary.ContainsKey(key))
						{
							Log($"String \"{key}\" already exists in {language}, it is not overriden.", textColor: ConsoleColor.Yellow);
						}
						outputs[language].KeyToStringDictionary[key] = localString;
					}
					Log($"Added string \"{key}\"");
				}
			}

			Log("------# Getting localization strings done.");


			// Output

			Log("\n------# Now outputing jsons:", textColor: ConsoleColor.Green);
			foreach (var output in outputs)
			{
				if (!Directory.Exists(_outPath))
				{
					Log($"Directory {_outPath} does not exist, so it is created.", textColor: ConsoleColor.Cyan);
					Directory.CreateDirectory(_outPath);
				}

				var name = _outName.Replace("<lang>", output.Key);
				var path = System.IO.Path.Combine(_outPath, name);
				var writableText = JsonConvert.SerializeObject(output.Value, Formatting.Indented);
				File.WriteAllText(path, writableText);
				Log($"Wrote to {path} with localizations for language \"{output.Key}\".");
			}
			Log("------# Outputing jsons done.");


			// Copy to destionation
			Log("\n------# Now copying to RESOURCE_DESTINATION", textColor: ConsoleColor.Green);
			foreach (var output in outputs)
			{
				var name = _outName.Replace("<lang>", output.Key);
				var path = System.IO.Path.Combine(_outPath, name);
				var resPath = System.IO.Path.Combine(_resourcesPath, name);
				
				if (File.Exists(resPath))
				{
					Log($"File at path {resPath}, it will be overriden.", textColor: ConsoleColor.Cyan);
				}

				System.IO.File.Copy(path, resPath, overwrite: true);
				Log($"Copied to {resPath} with localizations for language \"{output.Key}\".");
			}
			Log("------# Copied to RESOURCE_DESTINATION done.");

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

	private static List<(string, string[])> GetLocalizationStrings(ISheet sheet, out string[] languages)
	{
		var languageList = new List<string>();
		var headerRow = sheet.GetRow(0);
		for (int j = 1; j < headerRow.LastCellNum; ++j)
		{
			languageList.Add(headerRow.GetCell(j).StringCellValue);
		}
		languages = languageList.ToArray();

		var items = new List<(string, string[])>();
		for (int i = _headerRow + 1; i <= sheet.LastRowNum; ++i)
		{
			var row = sheet.GetRow(i);
			var key = row.GetCell(0).StringCellValue;
			var strings = new List<string>();
			for (int j = 1; j < headerRow.LastCellNum; ++j)
			{
				strings.Add(row.GetCell(j).StringCellValue);
			}

			items.Add((key, strings.ToArray()));
		}

		return items;
	}

	private static void ThrowAndTerminate(Exception e)
	{
		Log("Execution failed, exception is detailed below:", ConsoleColor.Red);
		Console.ResetColor();
		throw e;
	}
}