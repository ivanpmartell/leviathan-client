using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class BtnClickOnEscape : MonoBehaviour
{
	// Token: 0x0600004D RID: 77 RVA: 0x000034F8 File Offset: 0x000016F8
	public static bool IsAnyButtonsActive()
	{
		if (BtnClickOnEscape.m_wsObjectList.Count == 0)
		{
			return false;
		}
		foreach (BtnClickOnEscape btnClickOnEscape in BtnClickOnEscape.m_wsObjectList)
		{
			if (btnClickOnEscape.gameObject.active)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003584 File Offset: 0x00001784
	private void Start()
	{
		BtnClickOnEscape.m_wsObjectList.Add(this);
		this.button = base.GetComponent<UIButton>();
	}

	// Token: 0x0600004F RID: 79 RVA: 0x000035A0 File Offset: 0x000017A0
	private void OnDestroy()
	{
		int num = BtnClickOnEscape.m_wsObjectList.IndexOf(this);
		if (num >= 0)
		{
			int index = BtnClickOnEscape.m_wsObjectList.Count - 1;
			BtnClickOnEscape.m_wsObjectList[num] = BtnClickOnEscape.m_wsObjectList[index];
			BtnClickOnEscape.m_wsObjectList.RemoveAt(index);
		}
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000035F0 File Offset: 0x000017F0
	private bool IsFront()
	{
		foreach (BtnClickOnEscape btnClickOnEscape in BtnClickOnEscape.m_wsObjectList)
		{
			if (btnClickOnEscape != this && btnClickOnEscape.gameObject.active && base.transform.position.z > btnClickOnEscape.transform.position.z)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x000036A4 File Offset: 0x000018A4
	private void Update()
	{
		if (!this.IsFront())
		{
			return;
		}
		if (Utils.AndroidBack())
		{
			UIManager component = this.button.RenderCamera.GetComponent<UIManager>();
			component.FocusObject = null;
			POINTER_INFO ptr = default(POINTER_INFO);
			ptr.evt = this.button.whenToInvoke;
			this.button.OnInput(ptr);
		}
	}

	// Token: 0x04000027 RID: 39
	private static List<BtnClickOnEscape> m_wsObjectList = new List<BtnClickOnEscape>();

	// Token: 0x04000028 RID: 40
	private UIButton button;
}
