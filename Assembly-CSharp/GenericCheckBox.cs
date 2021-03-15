using System;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class GenericCheckBox : MonoBehaviour
{
	// Token: 0x06000065 RID: 101 RVA: 0x00003F68 File Offset: 0x00002168
	private void Start()
	{
		this.SetState(0);
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003F74 File Offset: 0x00002174
	private void Update()
	{
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003F78 File Offset: 0x00002178
	public void OnPress()
	{
		this.ToggleCheck();
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00003F80 File Offset: 0x00002180
	public void OnTap()
	{
		this.ToggleCheck();
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00003F88 File Offset: 0x00002188
	private void ToggleCheck()
	{
		this.m_isChecked = !this.m_isChecked;
		if (this.m_isChecked)
		{
			if (this.m_onCheckedDelegate != null)
			{
				this.m_onCheckedDelegate(base.gameObject);
			}
			this.SetState(1);
		}
		else
		{
			if (this.m_onUnCheckedDelegate != null)
			{
				this.m_onUnCheckedDelegate(base.gameObject);
			}
			this.SetState(0);
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00003FFC File Offset: 0x000021FC
	private void SetState(int stateNumber)
	{
		UIStateToggleBtn component = base.gameObject.GetComponent<UIStateToggleBtn>();
		if (component != null)
		{
			component.SetState(stateNumber);
		}
	}

	// Token: 0x04000033 RID: 51
	private const int STATE_UnChecked = 0;

	// Token: 0x04000034 RID: 52
	private const int STATE_Checked = 1;

	// Token: 0x04000035 RID: 53
	private bool m_isChecked;

	// Token: 0x04000036 RID: 54
	public GenericCheckBox.OnChecked m_onCheckedDelegate;

	// Token: 0x04000037 RID: 55
	public GenericCheckBox.OnUnChecked m_onUnCheckedDelegate;

	// Token: 0x02000194 RID: 404
	// (Invoke) Token: 0x06000F00 RID: 3840
	public delegate void OnChecked(GameObject checkBox);

	// Token: 0x02000195 RID: 405
	// (Invoke) Token: 0x06000F04 RID: 3844
	public delegate void OnUnChecked(GameObject checkBox);
}
