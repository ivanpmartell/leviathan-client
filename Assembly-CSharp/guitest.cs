using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class guitest : MonoBehaviour
{
	// Token: 0x06000D77 RID: 3447 RVA: 0x00060758 File Offset: 0x0005E958
	private void Awake()
	{
		UIActionBtn component = this.m_gui.GetComponent<UIActionBtn>();
		component.SetValueChangedDelegate(new EZValueChangedDelegate(this.Button));
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00060784 File Offset: 0x0005E984
	public void Button(IUIObject info)
	{
		PLog.Log("Pressed button");
	}

	// Token: 0x04000B0E RID: 2830
	public GameObject m_gui;
}
