using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class StatusLight_Advanced : MonoBehaviour
{
	// Token: 0x06000114 RID: 276 RVA: 0x00007134 File Offset: 0x00005334
	public static StatusLight_Advanced Create(GameObject guiCamera, string resourceName)
	{
		GameObject gameObject = GuiUtils.CreateGui("StatusLights/" + resourceName, guiCamera);
		StatusLight_Advanced component = gameObject.GetComponent<StatusLight_Advanced>();
		component.transform.position = Vector3.zero;
		component.Initialize();
		return component;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00007174 File Offset: 0x00005374
	private void Start()
	{
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00007178 File Offset: 0x00005378
	private void Update()
	{
	}

	// Token: 0x06000117 RID: 279 RVA: 0x0000717C File Offset: 0x0000537C
	public void Initialize()
	{
		this.ValidateComponents();
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00007184 File Offset: 0x00005384
	public void SetTint(Color color)
	{
		if (this.m_icon == null)
		{
			return;
		}
		this.m_icon.Color = color;
	}

	// Token: 0x06000119 RID: 281 RVA: 0x000071A4 File Offset: 0x000053A4
	public void SetIcon(int index)
	{
		if (this.m_icon == null)
		{
			return;
		}
		if (this.m_textures == null || index < 0 || index > this.m_textures.Count)
		{
			this.m_icon.Hide(true);
		}
		this.m_icon.Hide(false);
		if (index != this.m_currentTextureIndex)
		{
			this.m_currentTextureIndex = index;
			this.CurrentTexture = this.m_textures[this.m_currentTextureIndex];
		}
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00007228 File Offset: 0x00005428
	public void SetIcon_Percentage(float percentage)
	{
		if (this.m_icon == null || this.m_textures == null || this.m_textures.Count == 0)
		{
			return;
		}
		double num = (double)((percentage <= 100f) ? ((percentage >= 0f) ? percentage : 0f) : 100f) / 100.0;
		int num2 = (int)Math.Ceiling(num * (double)(this.m_textures.Count - 1));
		this.m_icon.Hide(false);
		if (num2 != this.m_currentTextureIndex)
		{
			this.m_currentTextureIndex = num2;
			this.CurrentTexture = this.m_textures[this.m_currentTextureIndex];
		}
	}

	// Token: 0x0600011B RID: 283 RVA: 0x000072E8 File Offset: 0x000054E8
	private void ValidateComponents()
	{
		if (this.m_isValid)
		{
			return;
		}
		this.m_icon = base.gameObject.GetComponent<SimpleSprite>();
		DebugUtils.Assert(this.m_icon != null, "StatusLight_Advanced failed to validate it's SimpleSprite component !");
		DebugUtils.Assert(this.m_textures != null && this.m_textures.Count > 0, "StatusLight_Advanced failed to validate. Has no textures !");
		this.m_isValid = true;
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00007358 File Offset: 0x00005558
	private bool ValidateTransform(string name, out GameObject go)
	{
		go = null;
		Transform transform = base.gameObject.transform.FindChild(name);
		if (transform == null)
		{
			return false;
		}
		go = transform.gameObject;
		return true;
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600011D RID: 285 RVA: 0x00007394 File Offset: 0x00005594
	// (set) Token: 0x0600011E RID: 286 RVA: 0x0000739C File Offset: 0x0000559C
	public bool IsValid
	{
		get
		{
			return this.m_isValid;
		}
		private set
		{
			this.m_isValid = value;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600011F RID: 287 RVA: 0x000073A8 File Offset: 0x000055A8
	// (set) Token: 0x06000120 RID: 288 RVA: 0x000073E8 File Offset: 0x000055E8
	public Texture2D CurrentTexture
	{
		get
		{
			return (!(this.m_icon == null)) ? (this.m_icon.renderer.material.mainTexture as Texture2D) : null;
		}
		set
		{
			if (this.m_icon == null)
			{
				return;
			}
			if (value == null)
			{
				base.gameObject.SetActiveRecursively(false);
				return;
			}
			base.gameObject.SetActiveRecursively(true);
			float width = this.m_icon.width;
			float height = this.m_icon.height;
			this.m_icon.SetTexture(value);
			float num = (float)value.width;
			float num2 = (float)value.height;
			this.m_icon.Setup(num, num2, new Vector2(0f, num2), new Vector2(num, num2));
			float num3 = width / (float)value.width;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			float num4 = height / (float)value.height;
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			this.m_icon.gameObject.transform.localScale = new Vector3(num3, num4, 1f);
			this.m_icon.UpdateCamera();
			this.m_icon.UpdateUVs();
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000121 RID: 289 RVA: 0x000074F4 File Offset: 0x000056F4
	public float ScaledHeight
	{
		get
		{
			if (this.CurrentTexture == null)
			{
				return 0f;
			}
			return (float)this.CurrentTexture.height * base.gameObject.transform.localScale.y;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000122 RID: 290 RVA: 0x00007540 File Offset: 0x00005740
	public float ScaledWidth
	{
		get
		{
			if (this.CurrentTexture == null)
			{
				return 0f;
			}
			return (float)this.CurrentTexture.width * base.gameObject.transform.localScale.x;
		}
	}

	// Token: 0x0400009F RID: 159
	public List<Texture2D> m_textures;

	// Token: 0x040000A0 RID: 160
	private bool m_isValid;

	// Token: 0x040000A1 RID: 161
	private int m_currentTextureIndex = -1;

	// Token: 0x040000A2 RID: 162
	private SimpleSprite m_icon;
}
