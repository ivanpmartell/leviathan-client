using System;
using System.IO;
using UnityEngine;

// Token: 0x020000E3 RID: 227
[AddComponentMenu("Scripts/Weapons/MineDecoy")]
public class MineDecoy : Mine
{
	// Token: 0x060008B7 RID: 2231 RVA: 0x0003FF68 File Offset: 0x0003E168
	public override void Setup(int ownerID, int unitID, bool visible, int seenByMask, int damage, int ap, float splashRadius, float splashDmgFactor)
	{
		base.Setup(ownerID, unitID, visible, seenByMask, damage, ap, splashRadius, splashDmgFactor);
		this.SetOwner(ownerID);
		this.SetVisible(visible);
		base.SetSeenByMask(seenByMask);
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0003FFA0 File Offset: 0x0003E1A0
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0003FFA8 File Offset: 0x0003E1A8
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x0003FFB0 File Offset: 0x0003E1B0
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_decoySize);
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0003FFC8 File Offset: 0x0003E1C8
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_decoySize = reader.ReadSingle();
		base.transform.GetChild(0).animation.CrossFadeQueued("idle", 0f, QueueMode.PlayNow);
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0004000C File Offset: 0x0003E20C
	protected override void OnTimeout()
	{
		base.Explode();
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00040014 File Offset: 0x0003E214
	protected override void OnDeploy()
	{
		base.transform.GetChild(0).animation.CrossFadeQueued("spawn", 0.2f, QueueMode.PlayNow);
		base.transform.GetChild(0).animation.CrossFadeQueued("idle", 0.2f, QueueMode.CompleteOthers);
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00040068 File Offset: 0x0003E268
	public override Vector3[] GetTargetPoints()
	{
		Vector3[] array = new Vector3[]
		{
			base.transform.position
		};
		Vector3[] array2 = array;
		int num = 0;
		array2[num].y = array2[num].y + 1.5f;
		return array;
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x000400AC File Offset: 0x0003E2AC
	public override void SetOwner(int owner)
	{
		base.SetOwner(owner);
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Color white = Color.white;
		if (TurnMan.instance != null)
		{
			TurnMan.instance.GetPlayerColors(base.GetOwner(), out white);
		}
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.material.SetColor("_TeamColor0", white);
		}
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00040120 File Offset: 0x0003E320
	public override float GetLength()
	{
		return this.m_decoySize;
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00040128 File Offset: 0x0003E328
	private void OnTriggerEnter(Collider other)
	{
		if (!NetObj.m_simulating)
		{
			return;
		}
		Section component = other.GetComponent<Section>();
		if (!component && other.gameObject.transform.parent != null)
		{
			component = other.gameObject.transform.parent.GetComponent<Section>();
		}
		if (component != null)
		{
			base.Explode();
		}
	}

	// Token: 0x04000711 RID: 1809
	public float m_decoySize = 20f;
}
