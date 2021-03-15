using System;
using System.IO;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class Blocker : NetObj
{
	// Token: 0x06000730 RID: 1840 RVA: 0x0003602C File Offset: 0x0003422C
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00036034 File Offset: 0x00034234
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
		writer.Write(base.transform.localScale.x);
		writer.Write(base.transform.localScale.y);
		writer.Write(base.transform.localScale.z);
		writer.Write(base.gameObject.active);
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x0003615C File Offset: 0x0003435C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		Vector3 position = default(Vector3);
		position.x = reader.ReadSingle();
		position.y = reader.ReadSingle();
		position.z = reader.ReadSingle();
		base.transform.position = position;
		Quaternion rotation = default(Quaternion);
		rotation.x = reader.ReadSingle();
		rotation.y = reader.ReadSingle();
		rotation.z = reader.ReadSingle();
		rotation.w = reader.ReadSingle();
		base.transform.rotation = rotation;
		Vector3 localScale = default(Vector3);
		localScale.x = reader.ReadSingle();
		localScale.y = reader.ReadSingle();
		localScale.z = reader.ReadSingle();
		base.transform.localScale = localScale;
		base.gameObject.SetActiveRecursively(reader.ReadBoolean());
	}
}
