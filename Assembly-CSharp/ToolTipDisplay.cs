using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class ToolTipDisplay : MonoBehaviour
{
	// Token: 0x0600016C RID: 364 RVA: 0x00008E30 File Offset: 0x00007030
	private void Start()
	{
		this._tooltipText = base.transform.GetChild(0).GetComponent<SpriteText>();
		SimpleSprite component = base.GetComponent<SimpleSprite>();
		this.tooltipHalfWidth = (int)component.width / 2;
		this.tooltipHalfHeight = (int)component.height / 2;
		this.screenHalfWidth = (int)this._tooltipText.RenderCamera.GetScreenWidth() / 2;
		this.screenHalfHeight = (int)this._tooltipText.RenderCamera.GetScreenHeight() / 2;
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00008EAC File Offset: 0x000070AC
	private void LimitToScreen()
	{
		if (this.m_position.x - (float)this.tooltipHalfWidth < (float)(-(float)this.screenHalfWidth))
		{
			this.m_position.x = (float)(-(float)this.screenHalfWidth + this.tooltipHalfWidth);
		}
		if (this.m_position.x + (float)this.tooltipHalfWidth > (float)this.screenHalfWidth)
		{
			this.m_position.x = (float)(this.screenHalfWidth - this.tooltipHalfWidth);
		}
		if (this.m_position.y - (float)this.tooltipHalfHeight < (float)(-(float)this.screenHalfHeight))
		{
			this.m_position.y = this.m_from.y + (float)(this.tooltipHalfHeight * 2);
		}
		if (this.m_position.y + (float)this.tooltipHalfHeight > (float)this.screenHalfHeight)
		{
			this.m_position.y = (float)(this.screenHalfHeight - this.tooltipHalfHeight);
		}
	}

	// Token: 0x0600016E RID: 366 RVA: 0x00008FA4 File Offset: 0x000071A4
	public void SetupToolTip(GameObject go, string text, Vector3 from, Vector3 to)
	{
		if (go == this.m_gameObject)
		{
			return;
		}
		this.m_from = from;
		this.m_to = to;
		this.m_message = text;
		this.m_position = to;
		this.m_position.z = -80f;
		this.m_position.y = this.m_position.y - (float)this.tooltipHalfHeight;
		this.LimitToScreen();
		this.m_showTimer = 1f;
		this.m_hideTimer = -1f;
		this.m_gameObject = go;
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00009030 File Offset: 0x00007230
	public void NoDelay()
	{
		this.m_showTimer = 0f;
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00009040 File Offset: 0x00007240
	public void StopToolTip(GameObject go)
	{
		if (this.m_gameObject == go)
		{
			this.Hide();
		}
		this.m_gameObject = null;
	}

	// Token: 0x06000171 RID: 369 RVA: 0x00009060 File Offset: 0x00007260
	public void StopToolTip()
	{
		this.Hide();
		this.m_gameObject = null;
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00009070 File Offset: 0x00007270
	private void Show()
	{
		this.m_position.x = Mathf.Floor(this.m_position.x);
		this.m_position.y = Mathf.Floor(this.m_position.y);
		this.m_position.z = Mathf.Floor(this.m_position.z);
		base.transform.transform.position = this.m_position;
		this._tooltipText.Text = this.m_message;
		this.m_hideTimer = 4f;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00009100 File Offset: 0x00007300
	private void Hide()
	{
		base.transform.position = new Vector3(0f, 5000f, 0f);
		this._tooltipText.Text = "***";
		this.m_showTimer = -1f;
		this.m_hideTimer = -1f;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00009154 File Offset: 0x00007354
	private void FixedUpdate()
	{
		if (this.m_showTimer >= 0f)
		{
			this.m_showTimer -= Time.fixedDeltaTime;
			if (this.m_showTimer <= 0f)
			{
				this.Show();
			}
		}
		if (this.m_hideTimer >= 0f)
		{
			this.m_hideTimer -= Time.fixedDeltaTime;
			if (this.m_hideTimer <= 0f)
			{
				this.Hide();
			}
		}
	}

	// Token: 0x06000175 RID: 373 RVA: 0x000091D4 File Offset: 0x000073D4
	public void SetHelpMode(bool helpMode)
	{
		this.m_helpModeOn = helpMode;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x000091E0 File Offset: 0x000073E0
	public bool GetHelpMode()
	{
		return this.m_helpModeOn;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x000091E8 File Offset: 0x000073E8
	public static ToolTipDisplay GetToolTip(GameObject src)
	{
		GameObject gameObject = GuiUtils.FindParent(src.transform, "GuiCamera");
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = GuiUtils.FindChildOf(gameObject, "Tooltip(Clone)");
		if (gameObject2 == null)
		{
			gameObject2 = GuiUtils.CreateGui("dialogs/Tooltip", gameObject);
			gameObject2.transform.position = new Vector3(0f, 5000f, -80f);
		}
		return gameObject2.GetComponent<ToolTipDisplay>();
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00009260 File Offset: 0x00007460
	public bool IsOnTouch()
	{
		return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
	}

	// Token: 0x040000DA RID: 218
	private string m_message = string.Empty;

	// Token: 0x040000DB RID: 219
	private float m_showTimer = -1f;

	// Token: 0x040000DC RID: 220
	private float m_hideTimer = -1f;

	// Token: 0x040000DD RID: 221
	private Vector3 m_position = default(Vector3);

	// Token: 0x040000DE RID: 222
	private GameObject m_gameObject;

	// Token: 0x040000DF RID: 223
	private SpriteText _tooltipText;

	// Token: 0x040000E0 RID: 224
	private bool m_helpModeOn;

	// Token: 0x040000E1 RID: 225
	private Vector3 m_from = default(Vector3);

	// Token: 0x040000E2 RID: 226
	private Vector3 m_to = default(Vector3);

	// Token: 0x040000E3 RID: 227
	private int tooltipHalfWidth = 128;

	// Token: 0x040000E4 RID: 228
	private int tooltipHalfHeight = 32;

	// Token: 0x040000E5 RID: 229
	private int screenHalfWidth = 512;

	// Token: 0x040000E6 RID: 230
	private int screenHalfHeight = 384;
}
