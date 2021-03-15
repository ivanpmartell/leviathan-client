using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class StatusLight_Basic : MonoBehaviour
{
	// Token: 0x06000124 RID: 292 RVA: 0x0000759C File Offset: 0x0000579C
	public static StatusLight_Basic Create(GameObject guiCamera, string resourceName)
	{
		string text = "StatusLights/" + resourceName;
		GameObject gameObject = GuiUtils.CreateGui(text, guiCamera);
		DebugUtils.Assert(gameObject != null, string.Concat(new string[]
		{
			"\"",
			text,
			"\" failed to find prefab \"",
			text,
			"\" !"
		}));
		StatusLight_Basic component = gameObject.GetComponent<StatusLight_Basic>();
		DebugUtils.Assert(component != null, string.Concat(new string[]
		{
			"\"",
			text,
			"\" failed to find a StatusLight_Basic-script in prefab \"",
			text,
			"\" !"
		}));
		Transform transform = component.gameObject.transform;
		Vector3 zero = Vector3.zero;
		component.gameObject.transform.localPosition = zero;
		transform.position = zero;
		component.Initialize();
		return component;
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00007664 File Offset: 0x00005864
	internal void Initialize()
	{
		this.ValidateComponents();
		this.CurrentTexture = ((!(this.m_OffTexture == null)) ? ((!this.m_disableWhenOff) ? this.m_OffTexture : this.m_OnTexture) : this.m_OnTexture);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x000076B8 File Offset: 0x000058B8
	public void SetTint(Color color)
	{
		if (this.m_sprite == null)
		{
			return;
		}
		this.m_sprite.Color = color;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x000076D8 File Offset: 0x000058D8
	public void SetOnOff(bool onOff)
	{
		this.m_On = onOff;
		if (this.m_disableWhenOff && !this.m_On)
		{
			this.m_sprite.Hide(true);
		}
		else
		{
			this.m_sprite.Hide(false);
			this.CurrentTexture = ((!this.m_On) ? this.m_OffTexture : this.m_OnTexture);
		}
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00007744 File Offset: 0x00005944
	private void ValidateComponents()
	{
		if (this.m_isValid)
		{
			return;
		}
		this.m_sprite = base.GetComponent<SimpleSprite>();
		DebugUtils.Assert(this.m_sprite != null, "StatusLight failed to validate it's SimpleSprite component !");
		DebugUtils.Assert(this.m_OnTexture != null, "StatusLight failed to validate. Has no ON-texture !");
		if (this.m_OffTexture == null)
		{
			this.m_disableWhenOff = true;
		}
		this.m_isValid = true;
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000129 RID: 297 RVA: 0x000077B4 File Offset: 0x000059B4
	// (set) Token: 0x0600012A RID: 298 RVA: 0x000077BC File Offset: 0x000059BC
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

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600012B RID: 299 RVA: 0x000077C8 File Offset: 0x000059C8
	// (set) Token: 0x0600012C RID: 300 RVA: 0x000077D0 File Offset: 0x000059D0
	[SerializeField]
	public bool DisableWhenOff
	{
		get
		{
			return this.m_disableWhenOff;
		}
		set
		{
			this.m_disableWhenOff = value;
			if (this.m_disableWhenOff && !this.m_On)
			{
				this.m_sprite.Hide(true);
			}
			else if (!this.m_disableWhenOff && !this.m_On)
			{
				this.m_sprite.Hide(false);
			}
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x0600012D RID: 301 RVA: 0x00007830 File Offset: 0x00005A30
	// (set) Token: 0x0600012E RID: 302 RVA: 0x00007870 File Offset: 0x00005A70
	public Texture2D CurrentTexture
	{
		get
		{
			return (!(this.m_sprite == null)) ? (this.m_sprite.renderer.material.mainTexture as Texture2D) : null;
		}
		set
		{
			if (this.m_sprite == null)
			{
				return;
			}
			if (this.CurrentTexture != null && this.CurrentTexture == value)
			{
				return;
			}
			float width = this.m_sprite.width;
			float height = this.m_sprite.height;
			if (value == null)
			{
				this.m_sprite.Hide(true);
				return;
			}
			this.m_sprite.Hide(false);
			float num = (float)value.width;
			float num2 = (float)value.height;
			this.m_sprite.SetTexture(value);
			this.m_sprite.Setup(num, num2, new Vector2(0f, num2), new Vector2(num, num2));
			float x = width / (float)value.width;
			float y = height / (float)value.height;
			this.m_sprite.gameObject.transform.localScale = new Vector3(x, y, 1f);
			this.m_sprite.UpdateUVs();
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x0600012F RID: 303 RVA: 0x00007970 File Offset: 0x00005B70
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

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000130 RID: 304 RVA: 0x000079BC File Offset: 0x00005BBC
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

	// Token: 0x040000A3 RID: 163
	public bool m_disableWhenOff;

	// Token: 0x040000A4 RID: 164
	private bool m_isValid;

	// Token: 0x040000A5 RID: 165
	public Texture2D m_OnTexture;

	// Token: 0x040000A6 RID: 166
	public Texture2D m_OffTexture;

	// Token: 0x040000A7 RID: 167
	private bool m_On = true;

	// Token: 0x040000A8 RID: 168
	private SimpleSprite m_sprite;
}
