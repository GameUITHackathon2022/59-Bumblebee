using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using TMPro;

public class LocalizationText : MonoBehaviour, ILocalizableText
{
	private const string USE_GAME_OBJECT_NAME = "STR_USE_GAME_OBJECT_NAME";
	private const string LEAVE_BLANK = "STR_LEAVE_BLANK";

	[SerializeField] private string _key;
	[SerializeField] private string _prefix;
	[SerializeField] private string _postfix;

	private TMP_Text _text;

	private string _localizedStringCache;
	private Dictionary<string, string> _localParameters;

	private void Awake()
	{
		_text = GetComponent<TMP_Text>();
	}

	private void OnEnable()
	{
		StringManager.Instance.Subscribe(this);
	}

	private void OnDisable()
	{
		StringManager.Instance.Unsubscribe(this);
	}

	private void OnDestroy()
	{
		StringManager.Instance.Unsubscribe(this);
	}

	public void OnNotifiedUpdateParameters()
	{
		if (_text == null) return;

		UpdateText();
	}

	public void OnNotifiedUpdateText()
	{
		if (_text == null) return;

		UpdateText();
	}

	public void AddLocalParameter(string key, string value)
	{
		AddLocalParameter(key, value);
		UpdateText();
	}

	public void AddLocalParameters(params (string key, string value)[] parameters)
	{
		foreach (var parameter in parameters)
		{
			AddLocalParameter(parameter.key, parameter.value);
		}
		UpdateText();
	}

	private void AddLocalParameterHelper(string key, string value)
	{
		if (_localParameters == null)
		{
			_localParameters = new Dictionary<string, string>();
		}

		_localParameters[$"{{{key}}}"] = value;
	}

	private void UpdateText()
	{
		if (_key == LEAVE_BLANK)
		{
			_localizedStringCache = "";
		}
		else
		{
			var key = _key;
			if (_key == USE_GAME_OBJECT_NAME)
			{
				key = name;
			}
			_localizedStringCache = StringManager.Instance.Localize(key);
		}

		RenderText();
	}

	private void RenderText()
	{
		var stringToWrite = StringManager.Instance.ReplaceWithParameters(_localizedStringCache, _localParameters);
		_text.text = $"{_prefix}{stringToWrite}{_postfix}";
	}
}