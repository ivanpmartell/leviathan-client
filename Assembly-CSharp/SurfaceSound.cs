using System;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class SurfaceSound : MonoBehaviour
{
	// Token: 0x060009E1 RID: 2529 RVA: 0x0004702C File Offset: 0x0004522C
	private void Update()
	{
		if (Camera.main != null)
		{
			Vector3 position = Camera.main.transform.position;
			position.y = 0f;
			base.transform.position = position;
		}
	}
}
