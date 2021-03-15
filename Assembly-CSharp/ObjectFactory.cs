using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Token: 0x0200011E RID: 286
internal class ObjectFactory
{
	// Token: 0x06000B28 RID: 2856 RVA: 0x00051A98 File Offset: 0x0004FC98
	public ObjectFactory()
	{
		TextAsset textAsset = Resources.Load("objectfactorydb") as TextAsset;
		if (textAsset == null)
		{
			PLog.LogError("Failed to load objectfactorydb.xml");
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(textAsset.text.ToString());
		for (XmlNode xmlNode = xmlDocument.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "prefab")
			{
				string value = xmlNode.Attributes["name"].Value;
				string value2 = xmlNode.Attributes["path"].Value;
				ObjectFactory.FactoryInfo factoryInfo = new ObjectFactory.FactoryInfo();
				factoryInfo.m_path = value2;
				factoryInfo.m_prefab = null;
				this.m_objects.Add(value, factoryInfo);
			}
		}
		Resources.UnloadAsset(textAsset);
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000B29 RID: 2857 RVA: 0x00051B80 File Offset: 0x0004FD80
	public static ObjectFactory instance
	{
		get
		{
			if (ObjectFactory.m_instance == null)
			{
				ObjectFactory.m_instance = new ObjectFactory();
			}
			return ObjectFactory.m_instance;
		}
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x00051B9C File Offset: 0x0004FD9C
	public static void ResetInstance()
	{
		ObjectFactory.m_instance = null;
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00051BA4 File Offset: 0x0004FDA4
	public static GameObject Clone(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;
		gameObject.name = prefab.name;
		return gameObject;
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x00051BCC File Offset: 0x0004FDCC
	public static GameObject Clone(GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
		gameObject.name = prefab.name;
		return gameObject;
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x00051BF4 File Offset: 0x0004FDF4
	private void LoadPrefab(ObjectFactory.FactoryInfo info)
	{
		UnityEngine.Object @object = Resources.Load(info.m_path);
		info.m_prefab = (@object as GameObject);
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x00051C1C File Offset: 0x0004FE1C
	public GameObject Create(string name, Vector3 pos, Quaternion rot)
	{
		ObjectFactory.FactoryInfo factoryInfo;
		if (this.m_objects.TryGetValue(name, out factoryInfo))
		{
			if (factoryInfo.m_prefab == null)
			{
				this.LoadPrefab(factoryInfo);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(factoryInfo.m_prefab, pos, rot) as GameObject;
			gameObject.name = factoryInfo.m_prefab.name;
			return gameObject;
		}
		PLog.LogWarning("ObjectFactory: Failed to find prefab " + name + ". Do you need to tools/Build Object Factory.");
		return null;
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00051C90 File Offset: 0x0004FE90
	public GameObject Create(string name)
	{
		ObjectFactory.FactoryInfo factoryInfo;
		if (this.m_objects.TryGetValue(name, out factoryInfo))
		{
			if (factoryInfo.m_prefab == null)
			{
				this.LoadPrefab(factoryInfo);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(factoryInfo.m_prefab) as GameObject;
			gameObject.name = factoryInfo.m_prefab.name;
			return gameObject;
		}
		PLog.LogWarning("ObjectFactory: Failed to find prefab " + name + ". Do you need to tools/Build Object Factory.");
		return null;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00051D04 File Offset: 0x0004FF04
	public GameObject GetPrefab(string name)
	{
		ObjectFactory.FactoryInfo factoryInfo;
		if (this.m_objects.TryGetValue(name, out factoryInfo))
		{
			if (factoryInfo.m_prefab == null)
			{
				this.LoadPrefab(factoryInfo);
			}
			return factoryInfo.m_prefab;
		}
		PLog.LogWarning("ObjectFactory: Failed to find prefab " + name + ". Do you need to tools/Build Object Factory.");
		return null;
	}

	// Token: 0x04000939 RID: 2361
	private static ObjectFactory m_instance;

	// Token: 0x0400093A RID: 2362
	private Dictionary<string, ObjectFactory.FactoryInfo> m_objects = new Dictionary<string, ObjectFactory.FactoryInfo>();

	// Token: 0x0200011F RID: 287
	private class FactoryInfo
	{
		// Token: 0x0400093B RID: 2363
		public string m_path;

		// Token: 0x0400093C RID: 2364
		public GameObject m_prefab;
	}
}
