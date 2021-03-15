using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class RandomRotation : MonoBehaviour
{
	// Token: 0x0600096B RID: 2411 RVA: 0x000446E4 File Offset: 0x000428E4
	private void Start()
	{
		base.transform.Rotate(new Vector3(0f, UnityEngine.Random.value * 360f, 0f));
	}
}
