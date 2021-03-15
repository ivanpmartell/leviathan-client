using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000173 RID: 371
[Serializable]
public class LOD_DEBUGGER : MonoBehaviour
{
	// Token: 0x06000E23 RID: 3619 RVA: 0x00063B08 File Offset: 0x00061D08
	private void Start()
	{
		if (this.m_prefabs == null || this.m_prefabs.Count == 0)
		{
			return;
		}
		float num = (this.m_extendDir_X != LOD_DEBUGGER.ExtendDir_X.Right) ? -1f : 1f;
		float num2 = (this.m_extendDir_Z != LOD_DEBUGGER.ExtendDir_Z.Up) ? -1f : 1f;
		Vector3 position = base.transform.position;
		Vector3 vector = position;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.m_repeatCollection; i++)
		{
			for (int j = 0; j < this.m_prefabs.Count; j++)
			{
				if (num3 == this.m_ObjectsPerRow)
				{
					num4++;
					num3 = 0;
					vector = position + new Vector3(0f, 0f, this.m_padding * (float)num4 * num2);
				}
				UnityEngine.Object.Instantiate(this.m_prefabs[j], vector, Quaternion.identity);
				vector += new Vector3(this.m_padding * num, 0f, 0f);
				num3++;
			}
		}
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x00063C2C File Offset: 0x00061E2C
	private void Update()
	{
	}

	// Token: 0x04000B4C RID: 2892
	public LOD_DEBUGGER.ExtendDir_X m_extendDir_X = LOD_DEBUGGER.ExtendDir_X.Right;

	// Token: 0x04000B4D RID: 2893
	public LOD_DEBUGGER.ExtendDir_Z m_extendDir_Z = LOD_DEBUGGER.ExtendDir_Z.Down;

	// Token: 0x04000B4E RID: 2894
	public float m_padding = 15f;

	// Token: 0x04000B4F RID: 2895
	public int m_ObjectsPerRow = 3;

	// Token: 0x04000B50 RID: 2896
	public int m_repeatCollection = 1;

	// Token: 0x04000B51 RID: 2897
	[SerializeField]
	public List<GameObject> m_prefabs;

	// Token: 0x02000174 RID: 372
	public enum ExtendDir_X
	{
		// Token: 0x04000B53 RID: 2899
		Left,
		// Token: 0x04000B54 RID: 2900
		Right
	}

	// Token: 0x02000175 RID: 373
	public enum ExtendDir_Z
	{
		// Token: 0x04000B56 RID: 2902
		Up,
		// Token: 0x04000B57 RID: 2903
		Down
	}
}
