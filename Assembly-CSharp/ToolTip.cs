using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class ToolTip : MonoBehaviour
{
	// Token: 0x06000166 RID: 358 RVA: 0x00008BA0 File Offset: 0x00006DA0
	private void Start()
	{
		this.button = base.GetComponent<AutoSpriteControlBase>();
		this.button.AddInputDelegate(new EZInputDelegate(this.OnInput));
		this._tooltip = ToolTipDisplay.GetToolTip(base.gameObject);
	}

	// Token: 0x06000167 RID: 359 RVA: 0x00008BE4 File Offset: 0x00006DE4
	private void OnDestroy()
	{
		if (this._tooltip)
		{
			this._tooltip.StopToolTip(base.gameObject);
		}
	}

	// Token: 0x06000168 RID: 360 RVA: 0x00008C08 File Offset: 0x00006E08
	private void FixedUpdate()
	{
		if (this._tooltip)
		{
			return;
		}
		this._tooltip = ToolTipDisplay.GetToolTip(base.gameObject);
	}

	// Token: 0x06000169 RID: 361 RVA: 0x00008C38 File Offset: 0x00006E38
	private void SetupToolTip()
	{
		string text = this.m_toolTip;
		if (text.Length == 0)
		{
			text = base.gameObject.name + "_tooltip";
			text = Localize.instance.TranslateKey(text);
		}
		else
		{
			text = Localize.instance.Translate(text);
		}
		AutoSpriteControlBase component = base.GetComponent<AutoSpriteControlBase>();
		Vector3 position = base.transform.position;
		position.x += component.BottomRight.x;
		position.y += component.BottomRight.y;
		this._tooltip.SetupToolTip(base.gameObject, text, base.transform.position, position);
	}

	// Token: 0x0600016A RID: 362 RVA: 0x00008CF4 File Offset: 0x00006EF4
	public void OnInput(ref POINTER_INFO ptr)
	{
		if (this._tooltip == null)
		{
			return;
		}
		if (this._tooltip.GetComponent<ToolTipDisplay>().GetHelpMode())
		{
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
			{
				ptr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				return;
			}
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				this.SetupToolTip();
				this._tooltip.NoDelay();
				ptr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				return;
			}
		}
		bool flag = true;
		if (flag)
		{
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE)
			{
				this.SetupToolTip();
			}
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF || ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				this._tooltip.StopToolTip(base.gameObject);
			}
		}
	}

	// Token: 0x040000D7 RID: 215
	public string m_toolTip;

	// Token: 0x040000D8 RID: 216
	private AutoSpriteControlBase button;

	// Token: 0x040000D9 RID: 217
	private ToolTipDisplay _tooltip;
}
