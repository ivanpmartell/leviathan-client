using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class Shallow : MonoBehaviour
{
	// Token: 0x0600099C RID: 2460 RVA: 0x00045760 File Offset: 0x00043960
	private void OnTriggerStay(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			Ship component = other.attachedRigidbody.GetComponent<Ship>();
			if (component != null)
			{
			}
		}
	}
}
