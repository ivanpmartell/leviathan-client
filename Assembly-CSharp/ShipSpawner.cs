using System;
using System.IO;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class ShipSpawner : NetObj
{
	// Token: 0x060009BF RID: 2495 RVA: 0x00046414 File Offset: 0x00044614
	private void FixedUpdate()
	{
		if (NetObj.m_simulating && !this.m_spawned)
		{
			this.m_spawned = true;
			ShipFactory.instance.CreateShip(this.m_spawnShip, base.transform.position, base.transform.rotation, this.m_targetOwner);
		}
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0004646C File Offset: 0x0004466C
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(base.transform.rotation.x);
		writer.Write(base.transform.rotation.y);
		writer.Write(base.transform.rotation.z);
		writer.Write(base.transform.rotation.w);
		writer.Write(this.m_spawnShip);
		writer.Write(this.m_targetOwner);
		writer.Write(this.m_spawned);
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00046558 File Offset: 0x00044758
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		Vector3 position = new Vector3(0f, 0f, 0f);
		Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
		position.x = reader.ReadSingle();
		position.y = reader.ReadSingle();
		position.z = reader.ReadSingle();
		rotation.x = reader.ReadSingle();
		rotation.y = reader.ReadSingle();
		rotation.z = reader.ReadSingle();
		rotation.w = reader.ReadSingle();
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.m_spawnShip = reader.ReadString();
		this.m_targetOwner = reader.ReadInt32();
		this.m_spawned = reader.ReadBoolean();
	}

	// Token: 0x040007FB RID: 2043
	public string m_spawnShip;

	// Token: 0x040007FC RID: 2044
	public int m_targetOwner = 7;

	// Token: 0x040007FD RID: 2045
	private bool m_spawned;
}
