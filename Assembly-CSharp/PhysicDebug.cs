using System;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class PhysicDebug : MonoBehaviour
{
	// Token: 0x06000E92 RID: 3730 RVA: 0x00066488 File Offset: 0x00064688
	private void Awake()
	{
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0006648C File Offset: 0x0006468C
	private void OnCollisionStay(Collision collisionInfo)
	{
		foreach (ContactPoint contactPoint in collisionInfo.contacts)
		{
			PLog.Log("OnCollisionStay: " + base.gameObject.name);
			if (collisionInfo.rigidbody == null)
			{
				Debug.DrawRay(contactPoint.point, contactPoint.normal * 10f, Color.white, 50f, false);
			}
		}
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x00066514 File Offset: 0x00064714
	public static void SetScripts()
	{
		Collider[] array = UnityEngine.Object.FindObjectsOfType(typeof(Collider)) as Collider[];
		foreach (Collider collider in array)
		{
			if (collider.GetComponent<PhysicDebug>() == null)
			{
				collider.gameObject.AddComponent<PhysicDebug>();
			}
		}
		Rigidbody[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
		foreach (Rigidbody rigidbody in array3)
		{
			if (rigidbody.GetComponent<PhysicDebug>() == null)
			{
				rigidbody.gameObject.AddComponent<PhysicDebug>();
			}
		}
	}
}
