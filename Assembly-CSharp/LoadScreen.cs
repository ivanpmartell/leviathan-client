using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class LoadScreen
{
	// Token: 0x06000332 RID: 818 RVA: 0x00019448 File Offset: 0x00017648
	public LoadScreen(GameObject guiCamera)
	{
		this.m_guiCamera = guiCamera;
		this.m_gui = GuiUtils.CreateGui("LoadScreen", this.m_guiCamera);
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui.transform, "BackgroundImg");
		DebugUtils.Assert(gameObject);
		this.m_bkg = GuiUtils.FindChildOf(this.m_gui.transform, "BackgroundWnd").GetComponent<UIButton>();
		this.m_sprite = gameObject.GetComponent<SimpleSprite>();
		this.m_material = gameObject.renderer.material;
		this.m_defaultTexture = this.m_material.mainTexture;
		this.m_gui.SetActiveRecursively(false);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x00019500 File Offset: 0x00017700
	public void Close()
	{
		this.Clear();
	}

	// Token: 0x06000334 RID: 820 RVA: 0x00019508 File Offset: 0x00017708
	public void SetVisible(bool visible)
	{
		if (this.m_gui == null)
		{
			return;
		}
		if (visible)
		{
			this.m_gui.SetActiveRecursively(true);
			this.m_sprite.SetColor(new Color(1f, 1f, 1f, 1f));
			this.m_bkg.SetColor(new Color(1f, 1f, 1f, 1f));
			this.m_fadeTimer = -1f;
		}
		else
		{
			this.m_fadeTimer = 0f;
		}
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0001959C File Offset: 0x0001779C
	public string GetRandomImage()
	{
		XmlDocument xmlDocument = Utils.LoadXml("loadscreens/manifest");
		DebugUtils.Assert(xmlDocument != null);
		List<string> list = new List<string>();
		for (XmlNode xmlNode = xmlDocument.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "image")
			{
				list.Add(xmlNode.FirstChild.Value);
			}
		}
		PLog.Log("Found " + list.Count + " loadscreens");
		if (list.Count > 0)
		{
			System.Random random = new System.Random();
			return list[random.Next(0, list.Count)];
		}
		return null;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x00019650 File Offset: 0x00017850
	public void SetImage(string name)
	{
		if (this.m_gui == null)
		{
			return;
		}
		if (name == string.Empty)
		{
			name = this.GetRandomImage();
			if (name == null)
			{
				name = "default";
			}
		}
		Texture texture = Resources.Load("loadscreens/" + name) as Texture;
		if (texture != null)
		{
			this.m_material.mainTexture = texture;
		}
		else
		{
			PLog.LogWarning("Missing loadscreen texture " + name);
		}
	}

	// Token: 0x06000337 RID: 823 RVA: 0x000196D8 File Offset: 0x000178D8
	public void Update()
	{
		if (this.m_gui == null)
		{
			return;
		}
		if (this.m_fadeTimer >= 0f)
		{
			this.m_fadeTimer += Time.deltaTime;
			if (this.m_fadeTimer > 0.5f)
			{
				this.m_gui.SetActiveRecursively(false);
				this.m_fadeTimer = -1f;
				this.Clear();
			}
			else
			{
				float a = 1f - this.m_fadeTimer / 0.5f;
				this.m_sprite.SetColor(new Color(1f, 1f, 1f, a));
				this.m_bkg.SetColor(new Color(1f, 1f, 1f, a));
			}
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x000197A0 File Offset: 0x000179A0
	private void Clear()
	{
		UnityEngine.Object.Destroy(this.m_gui);
		this.m_gui = null;
		this.m_material = null;
		this.m_sprite = null;
		this.m_bkg = null;
		this.m_defaultTexture = null;
	}

	// Token: 0x04000299 RID: 665
	private const float m_fadeTime = 0.5f;

	// Token: 0x0400029A RID: 666
	private GameObject m_gui;

	// Token: 0x0400029B RID: 667
	private GameObject m_guiCamera;

	// Token: 0x0400029C RID: 668
	private Material m_material;

	// Token: 0x0400029D RID: 669
	private SimpleSprite m_sprite;

	// Token: 0x0400029E RID: 670
	private UIButton m_bkg;

	// Token: 0x0400029F RID: 671
	private Texture m_defaultTexture;

	// Token: 0x040002A0 RID: 672
	private float m_fadeTimer = -1f;
}
