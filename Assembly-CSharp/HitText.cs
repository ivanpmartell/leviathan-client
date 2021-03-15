using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000045 RID: 69
internal class HitText
{
	// Token: 0x060002EB RID: 747 RVA: 0x000166F8 File Offset: 0x000148F8
	public HitText(GameObject guiCamera)
	{
		HitText.m_instance = this;
		this.m_guiCamera = guiCamera.GetComponent<Camera>();
		this.m_textPrefabLarge = (Resources.Load("gui/IngameGui/DmgTextLarge") as GameObject);
		this.m_textPrefabMedium = (Resources.Load("gui/IngameGui/DmgTextMedium") as GameObject);
		this.m_textPrefabSmall = (Resources.Load("gui/IngameGui/DmgTextSmall") as GameObject);
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x060002ED RID: 749 RVA: 0x00016774 File Offset: 0x00014974
	public static HitText instance
	{
		get
		{
			return HitText.m_instance;
		}
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0001677C File Offset: 0x0001497C
	public void Close()
	{
		HitText.m_instance = null;
		this.Clear();
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0001678C File Offset: 0x0001498C
	public void Clear()
	{
		foreach (HitText.TextData textData in this.m_texts)
		{
			UnityEngine.Object.Destroy(textData.m_guiElement);
		}
		this.m_texts.Clear();
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x00016804 File Offset: 0x00014A04
	public void SetVisible(bool visible)
	{
		if (this.m_visible == visible)
		{
			return;
		}
		this.m_visible = visible;
		if (!this.m_visible)
		{
			this.Clear();
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0001682C File Offset: 0x00014A2C
	public void AddDmgText(int ownerID, Vector3 pos, string text, HitTextDef def)
	{
		if (!this.m_visible)
		{
			return;
		}
		text = Localize.instance.Translate(def.m_prefix + text + def.m_postfix);
		HitText.TextData textData = this.FindOldTextItem(ownerID, text);
		if (textData == null)
		{
			textData = this.CreateNewItem(ownerID, def, pos);
			textData.m_textElement.Text = text;
			textData.m_baseText = text;
		}
		textData.m_hits++;
		if (textData.m_hits > 1)
		{
			textData.m_textElement.Text = textData.m_baseText.ToString() + " x " + textData.m_hits.ToString();
		}
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x000168D8 File Offset: 0x00014AD8
	public void AddDmgText(int ownerID, Vector3 pos, int dmg, HitTextDef def)
	{
		if (!this.m_visible)
		{
			return;
		}
		HitText.TextData textData = this.FindOldDamageItem(ownerID, def.m_color);
		if (textData == null)
		{
			textData = this.CreateNewItem(ownerID, def, pos);
		}
		textData.m_hits++;
		if (textData.m_damage == -1)
		{
			textData.m_damage = dmg;
			textData.m_textElement.Text = textData.m_damage.ToString();
		}
		else
		{
			textData.m_damage += dmg;
			textData.m_textElement.Text = textData.m_damage.ToString();
		}
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x00016974 File Offset: 0x00014B74
	private HitText.TextData CreateNewItem(int ownerID, HitTextDef def, Vector3 pos)
	{
		HitText.TextData textData = new HitText.TextData();
		textData.m_ownerID = ownerID;
		textData.m_textColor = def.m_color;
		textData.m_pos = pos;
		GameObject gameObject = null;
		switch (def.m_fontSize)
		{
		case HitTextDef.FontSize.Small:
			gameObject = (UnityEngine.Object.Instantiate(this.m_textPrefabSmall, pos, Quaternion.identity) as GameObject);
			break;
		case HitTextDef.FontSize.Medium:
			gameObject = (UnityEngine.Object.Instantiate(this.m_textPrefabMedium, pos, Quaternion.identity) as GameObject);
			break;
		case HitTextDef.FontSize.Large:
			gameObject = (UnityEngine.Object.Instantiate(this.m_textPrefabLarge, pos, Quaternion.identity) as GameObject);
			break;
		}
		textData.m_guiElement = gameObject;
		gameObject.transform.parent = this.m_guiCamera.transform;
		textData.m_textElement = gameObject.transform.FindChild("Text").GetComponent<SpriteText>();
		textData.m_textElement.SetColor(textData.m_textColor);
		textData.m_textElement.text = string.Empty;
		this.m_texts.Add(textData);
		return textData;
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00016A7C File Offset: 0x00014C7C
	public void Update(float dt)
	{
		this.PurgeOldTexts();
		foreach (HitText.TextData textData in this.m_texts)
		{
			textData.m_time += dt;
		}
		for (int i = 0; i < this.m_texts.Count; i++)
		{
			if (this.m_texts[i].m_time > 7f)
			{
				UnityEngine.Object.Destroy(this.m_texts[i].m_guiElement);
				this.m_texts.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00016B50 File Offset: 0x00014D50
	private void PurgeOldTexts()
	{
		while (this.m_texts.Count > 20)
		{
			UnityEngine.Object.Destroy(this.m_texts[0].m_guiElement);
			this.m_texts.RemoveAt(0);
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00016B98 File Offset: 0x00014D98
	public void LateUpdate(Camera camera)
	{
		this.UpdateScales(camera);
		this.Separate(camera, Time.deltaTime);
		foreach (HitText.TextData textData in this.m_texts)
		{
			float num = textData.m_time / 7f;
			Vector3 guiPos = this.GetGuiPos(camera, textData);
			float num2 = 1f - num;
			textData.m_offset += 0.5f * textData.m_scale * num2;
			Color textColor = textData.m_textColor;
			textColor.a = 1f - num * num * num;
			textData.m_textElement.SetColor(textColor);
			textData.m_textElement.transform.position = guiPos;
			textData.m_textElement.transform.localScale = new Vector3(textData.m_scale * textData.m_textScale, textData.m_scale * textData.m_textScale, textData.m_scale * textData.m_textScale);
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00016CBC File Offset: 0x00014EBC
	private Vector3 GetGuiPos(Camera camera, HitText.TextData data)
	{
		Vector3 result = GuiUtils.WorldToGuiPos(camera, this.m_guiCamera, data.m_pos);
		result.z = data.m_textScale;
		result.y += data.m_offset * data.m_scale;
		return result;
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00016D08 File Offset: 0x00014F08
	private HitText.TextData FindOldDamageItem(int owner, Color textColor)
	{
		float num = 3.5f;
		foreach (HitText.TextData textData in this.m_texts)
		{
			if (textData.m_ownerID == owner && textData.m_damage >= 0 && textData.m_textColor == textColor && textData.m_time < num)
			{
				return textData;
			}
		}
		return null;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00016DAC File Offset: 0x00014FAC
	private HitText.TextData FindOldTextItem(int owner, string text)
	{
		float num = 3.5f;
		foreach (HitText.TextData textData in this.m_texts)
		{
			if (textData.m_ownerID == owner && textData.m_damage == -1 && textData.m_baseText == text && textData.m_time < num)
			{
				return textData;
			}
		}
		return null;
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00016E50 File Offset: 0x00015050
	private void Separate(Camera camera, float dt)
	{
		float num = 25f;
		float num2 = 40f;
		for (int i = 0; i < this.m_texts.Count; i++)
		{
			HitText.TextData textData = this.m_texts[i];
			Vector3 guiPos = this.GetGuiPos(camera, textData);
			for (int j = i + 1; j < this.m_texts.Count; j++)
			{
				HitText.TextData textData2 = this.m_texts[j];
				Vector3 guiPos2 = this.GetGuiPos(camera, textData2);
				float num3 = Vector3.Distance(guiPos, guiPos2);
				if (num3 < num)
				{
					if (guiPos.y > guiPos2.y)
					{
						textData.m_offset += dt * num2;
						guiPos = this.GetGuiPos(camera, textData);
					}
					else
					{
						textData2.m_offset += dt * num2;
					}
				}
			}
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00016F30 File Offset: 0x00015130
	private void UpdateScales(Camera camera)
	{
		float num = Mathf.Tan(0.017453292f * camera.fieldOfView * 0.5f);
		foreach (HitText.TextData textData in this.m_texts)
		{
			textData.m_scale = Vector3.Distance(textData.m_pos, camera.transform.position) * (1f / num);
			textData.m_scale = Mathf.Clamp(textData.m_scale, 0f, 1f);
		}
	}

	// Token: 0x04000232 RID: 562
	private const float m_flyTime = 7f;

	// Token: 0x04000233 RID: 563
	private const float m_flySpeed = 0.5f;

	// Token: 0x04000234 RID: 564
	private const int m_maxTexts = 20;

	// Token: 0x04000235 RID: 565
	private List<HitText.TextData> m_texts = new List<HitText.TextData>();

	// Token: 0x04000236 RID: 566
	private static HitText m_instance;

	// Token: 0x04000237 RID: 567
	private Camera m_guiCamera;

	// Token: 0x04000238 RID: 568
	private GameObject m_textPrefabLarge;

	// Token: 0x04000239 RID: 569
	private GameObject m_textPrefabMedium;

	// Token: 0x0400023A RID: 570
	private GameObject m_textPrefabSmall;

	// Token: 0x0400023B RID: 571
	private bool m_visible = true;

	// Token: 0x02000046 RID: 70
	private class TextData
	{
		// Token: 0x0400023C RID: 572
		public int m_ownerID = -1;

		// Token: 0x0400023D RID: 573
		public SpriteText m_textElement;

		// Token: 0x0400023E RID: 574
		public GameObject m_guiElement;

		// Token: 0x0400023F RID: 575
		public Vector3 m_pos;

		// Token: 0x04000240 RID: 576
		public float m_time;

		// Token: 0x04000241 RID: 577
		public float m_offset;

		// Token: 0x04000242 RID: 578
		public float m_scale = 1f;

		// Token: 0x04000243 RID: 579
		public float m_textScale = 1f;

		// Token: 0x04000244 RID: 580
		public Color m_textColor = Color.white;

		// Token: 0x04000245 RID: 581
		public int m_damage = -1;

		// Token: 0x04000246 RID: 582
		public string m_baseText = string.Empty;

		// Token: 0x04000247 RID: 583
		public int m_hits;
	}
}
