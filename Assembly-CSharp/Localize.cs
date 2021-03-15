using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

// Token: 0x020000BE RID: 190
internal class Localize
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00033848 File Offset: 0x00031A48
	public static Localize instance
	{
		get
		{
			if (Localize.m_instance == null)
			{
				Localize.m_instance = new Localize();
			}
			return Localize.m_instance;
		}
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00033864 File Offset: 0x00031A64
	public string GetLanguage()
	{
		return this.m_language;
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x0003386C File Offset: 0x00031A6C
	public bool SetLanguage(string language)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreComments = true;
		this.m_language = language;
		this.m_translations.Clear();
		UnityEngine.Object[] array = Resources.LoadAll("localization/" + language);
		foreach (UnityEngine.Object @object in array)
		{
			TextAsset textAsset = @object as TextAsset;
			if (!(textAsset == null))
			{
				XmlReader xmlReader = XmlReader.Create(new StringReader(textAsset.text), xmlReaderSettings);
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.Load(xmlReader);
				}
				catch (XmlException ex)
				{
					PLog.LogError("Parse error " + ex.ToString());
					goto IL_CA;
				}
				if (!this.AddTranslation(xmlDocument))
				{
					PLog.LogError("Error adding localization file " + @object.name);
				}
			}
			IL_CA:;
		}
		PLog.Log("nr of translation entries: " + this.m_translations.Count);
		return true;
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00033990 File Offset: 0x00031B90
	private bool AddTranslation(XmlDocument xmlDoc)
	{
		for (XmlNode xmlNode = xmlDoc.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			XmlNode xmlNode2 = xmlNode.Attributes["text"];
			if (xmlNode2 == null)
			{
				PLog.LogError("missing text attribute in node " + xmlNode.Name + " in file " + xmlDoc.Name);
				xmlNode = xmlNode.NextSibling;
				return false;
			}
			string value = xmlNode2.Value.Replace("\\n", "\n");
			this.m_translations[xmlNode.Name] = value;
		}
		return true;
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00033A24 File Offset: 0x00031C24
	private bool IsBreakCharacter(char c)
	{
		return c == ' ' || c == '\n' || c == ':' || c == '.' || c == ',' || c == '!';
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x00033A5C File Offset: 0x00031C5C
	public string TranslateMacros(string text, Dictionary<string, string> macros)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int length = text.Length;
		int num = -1;
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			if (i == length - 1 && num != -1)
			{
				string text2 = text.Substring(num);
				if (this.IsBreakCharacter(c))
				{
					text2 = text.Substring(num, i - num);
				}
				PLog.Log("end key " + text2);
				stringBuilder.Append(this.MacroKey(text2, macros));
			}
			else if (this.IsBreakCharacter(c))
			{
				if (num != -1)
				{
					string key = text.Substring(num, i - num);
					stringBuilder.Append(this.MacroKey(key, macros));
					num = -1;
				}
				stringBuilder.Append(c);
			}
			else if (c == '@')
			{
				if (num != -1)
				{
					string key2 = text.Substring(num, i - num);
					stringBuilder.Append(this.MacroKey(key2, macros));
				}
				num = i + 1;
			}
			else if (num == -1)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00033B78 File Offset: 0x00031D78
	public string MacroKey(string key, Dictionary<string, string> macros)
	{
		string result;
		if (macros.TryGetValue(key, out result))
		{
			return result;
		}
		PLog.LogWarning("missing macro for key : " + key);
		return key;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00033BA8 File Offset: 0x00031DA8
	public string TranslateRecursive(string text)
	{
		text = this.Translate(text);
		text = this.Translate(text);
		return text;
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00033BC0 File Offset: 0x00031DC0
	public string Translate(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int length = text.Length;
		int num = -1;
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			if (i == length - 1 && num != -1)
			{
				string key = text.Substring(num);
				if (this.IsBreakCharacter(c))
				{
					key = text.Substring(num, i - num);
				}
				stringBuilder.Append(this.TranslateKey(key));
			}
			else if (this.IsBreakCharacter(c))
			{
				if (num != -1)
				{
					string key2 = text.Substring(num, i - num);
					stringBuilder.Append(this.TranslateKey(key2));
					num = -1;
				}
				stringBuilder.Append(c);
			}
			else if (c == '$')
			{
				if (num != -1)
				{
					string key3 = text.Substring(num, i - num);
					stringBuilder.Append(this.TranslateKey(key3));
				}
				num = i + 1;
			}
			else if (num == -1)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00033CC8 File Offset: 0x00031EC8
	public string TranslateKey(string key)
	{
		string result;
		if (this.m_translations.TryGetValue(key, out result))
		{
			return result;
		}
		return "[#FF0000][" + key + "]";
	}

	// Token: 0x040005B7 RID: 1463
	private static Localize m_instance;

	// Token: 0x040005B8 RID: 1464
	private Dictionary<string, string> m_translations = new Dictionary<string, string>();

	// Token: 0x040005B9 RID: 1465
	private string m_language;
}
