using System;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class ShowOnPlatform : MonoBehaviour
{
	// Token: 0x06000E3D RID: 3645 RVA: 0x00064724 File Offset: 0x00062924
	private void Start()
	{
		this.SetState();
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x0006472C File Offset: 0x0006292C
	private void FixedUpdate()
	{
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00064730 File Offset: 0x00062930
	private void Update()
	{
		this.SetState();
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x00064738 File Offset: 0x00062938
	private void SetState()
	{
		if (!this.m_visiblePc)
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}

	// Token: 0x04000B6A RID: 2922
	public bool m_visiblePc = true;

	// Token: 0x04000B6B RID: 2923
	public bool m_visibleIOS = true;

	// Token: 0x04000B6C RID: 2924
	public bool m_visibleAndroid = true;
}
