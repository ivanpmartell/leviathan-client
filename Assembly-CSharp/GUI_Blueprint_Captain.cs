using System;
using UnityEngine;

// Token: 0x02000002 RID: 2
[AddComponentMenu("Scripts/Gui/GUI_Blueprint_Captain")]
public sealed class GUI_Blueprint_Captain : MonoBehaviour
{
	// Token: 0x06000002 RID: 2 RVA: 0x00002118 File Offset: 0x00000318
	private void Start()
	{
		this.Initialize();
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002120 File Offset: 0x00000320
	public void Initialize()
	{
		if (this.m_hasInitialized)
		{
			return;
		}
		this.m_listItemComponent = base.gameObject.GetComponent<UIListItem>();
		DebugUtils.Assert(this.m_listItemComponent != null, "GUI_Captain script must be attached to a GameObject that has an IUIListObject");
		this.m_simpleSpriteComponent = base.gameObject.GetComponentInChildren<SimpleSprite>();
		DebugUtils.Assert(this.m_simpleSpriteComponent != null, "GUI_Captain script must be attached to a GameObject that has an SimpleSprite");
		Debug.Log(string.Format("{0}::Initialize() called !", this.m_listItemComponent.Text));
		this.SetSizes(this.m_width, this.m_height);
		this.SetTexts(this.m_text);
		this.SetIcon(this.m_iconTexture, 32f, 32f);
		this.m_hasInitialized = true;
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000021DC File Offset: 0x000003DC
	private void SetSizes(float width, float height)
	{
		if (this.m_listItemComponent != null)
		{
			this.m_listItemComponent.SetSize(width, height);
		}
	}

	// Token: 0x06000005 RID: 5 RVA: 0x000021FC File Offset: 0x000003FC
	private void SetTexts(string text)
	{
		if (this.m_listItemComponent != null)
		{
			this.m_listItemComponent.Text = text;
			if (this.m_listItemComponent.stateLabels != null && this.m_listItemComponent.stateLabels.Length > 0)
			{
				for (int i = 0; i < this.m_listItemComponent.stateLabels.Length; i++)
				{
					this.m_listItemComponent.stateLabels[i] = text;
				}
			}
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002278 File Offset: 0x00000478
	private void SetIcon(Texture2D iconTexture, float iconWidth, float iconHeight)
	{
		if (this.m_simpleSpriteComponent != null)
		{
			this.m_simpleSpriteComponent.SetTexture(iconTexture);
			this.m_simpleSpriteComponent.Setup(iconWidth, iconHeight, new Vector2(0f, iconHeight), new Vector2(iconWidth, iconHeight));
			this.m_simpleSpriteComponent.renderer.castShadows = false;
			this.m_simpleSpriteComponent.renderer.receiveShadows = false;
		}
	}

	// Token: 0x06000007 RID: 7 RVA: 0x000022E4 File Offset: 0x000004E4
	public void OnTap()
	{
		Debug.Log(string.Format("{0}::OnTap() called !", this.m_listItemComponent.Text));
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00002300 File Offset: 0x00000500
	public void OnPress()
	{
		Debug.Log(string.Format("{0}::OnPress() called !", this.m_listItemComponent.Text));
	}

	// Token: 0x06000009 RID: 9 RVA: 0x0000231C File Offset: 0x0000051C
	public void OnRelease()
	{
		Debug.Log(string.Format("{0}::OnRelease() called !", this.m_listItemComponent.Text));
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002338 File Offset: 0x00000538
	public void OnMove()
	{
		Debug.Log(string.Format("{0}::OnMove() called !", this.m_listItemComponent.Text));
	}

	// Token: 0x04000001 RID: 1
	private const bool DEBUG = true;

	// Token: 0x04000002 RID: 2
	private UIListItem m_listItemComponent;

	// Token: 0x04000003 RID: 3
	private SimpleSprite m_simpleSpriteComponent;

	// Token: 0x04000004 RID: 4
	private bool m_hasInitialized;

	// Token: 0x04000005 RID: 5
	public float m_width = 100f;

	// Token: 0x04000006 RID: 6
	public float m_height = 100f;

	// Token: 0x04000007 RID: 7
	public string m_text = string.Empty;

	// Token: 0x04000008 RID: 8
	public Texture2D m_iconTexture;
}
