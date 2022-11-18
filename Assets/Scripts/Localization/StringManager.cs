using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Newtonsoft.Json;
using System.Runtime.CompilerServices;

public class StringManager : MonoBehaviour
{
	public static StringManager Instance;

	private class LocalizationDictionary
	{
		public string CultureCode;
		public Dictionary<string, string> KeyToStringDictionary;
	}

	[SerializeField] private List<AssetReference> _localizationPaths;

	private Dictionary<string, LocalizationDictionary> _localizationDictionary = new Dictionary<string, LocalizationDictionary>();
	private string _languageInUse;
	private Dictionary<string, string> _parameterDictionary = new Dictionary<string, string>();
	private HashSet<ILocalizableText> _localizableTexts = new HashSet<ILocalizableText>();

	public bool LocalizationLoaded { get; private set; }

	private Dictionary<string, string> LocalizationDictionaryInUse => _localizationDictionary[_languageInUse].KeyToStringDictionary;


	private void Awake()
	{
		Instance = this;
	}

	private IEnumerator Start()
	{
		float time = Time.time;
		LocalizationLoaded = false;

		var handles = new List<AsyncOperationHandle<TextAsset>>();
		foreach (var asset in _localizationPaths)
		{
			handles.Add(asset.LoadAssetAsync<TextAsset>());
		}

		yield return new WaitUntil(() => handles.All(x => x.IsDone));

		foreach (var handle in handles)
		{
			var textAsset = handle.Result;
			var localization = JsonConvert.DeserializeObject<LocalizationDictionary>(textAsset.text);
			_localizationDictionary.Add(localization.CultureCode, localization);
		}

		// New line parsing
		foreach (var dict in _localizationDictionary.Values)
		{
			var keys = dict.KeyToStringDictionary.Keys.ToList();
			foreach (var key in keys)
			{
				dict.KeyToStringDictionary[key] = dict.KeyToStringDictionary[key].Replace("\\n", "\n");
			}
		}

		LocalizationLoaded = true;
		time = Time.time - time;
		Debug.Log($"{name}: Read strings done. Took {time}s");

		// Read language
		var language = PlayerPrefs.GetString("Language", "en");
		SwitchLanguage(language);
	}

	public void SwitchLanguage(string newLanguage)
	{
		_languageInUse = newLanguage;
		OnLocalizationChange();
	}

	public string Localize(string key)
	{
		if (!LocalizationDictionaryInUse.ContainsKey(key))
		{
			Debug.Log($"{name} StringManager: Missing key \"{key}\"");
			return key;
		}
		
		return LocalizationDictionaryInUse[key];
	}

	public string ReplaceWithParameters(string rawString, Dictionary<string, string> localParameters=null)
	{
		return Regex.Replace(rawString, "\\{(.*?)\\}", (match) =>
		{
			var parameter = match.Value;
			if (localParameters != null && localParameters.ContainsKey(parameter))
			{
				return localParameters[parameter];
			}
			else if (_parameterDictionary.ContainsKey(parameter))
			{
				return _parameterDictionary[parameter];
			}
			else
			{
				return "";
			}
		});
	}

	public void AddGlobalParameter(string key, string value)
	{
		AddParameterHelper(key, value);
		OnParameterChange();
	}

	public void AddGlobalParameters(params (string key, string value)[] pairs)
	{
		foreach (var pair in pairs)
		{
			AddParameterHelper(pair.key, pair.value);
		}
		OnParameterChange();
	}

	public void Subscribe(ILocalizableText text)
	{
		_localizableTexts.Add(text);

		if (LocalizationLoaded)
		{
			text.OnNotifiedUpdateText();
		}
	}

	public void Unsubscribe(ILocalizableText text)
	{
		_localizableTexts.Remove(text);
	}

	private void OnLocalizationChange()
	{
		foreach (var localizable in _localizableTexts)
		{
			localizable.OnNotifiedUpdateText();
		}
	}

	private void OnParameterChange()
	{
		foreach (var localizable in _localizableTexts)
		{
			localizable.OnNotifiedUpdateParameters();
		}
	}

	private void AddParameterHelper(string key, string value)
	{
		_parameterDictionary[$"{{{key}}}"] = value;
	}
}
