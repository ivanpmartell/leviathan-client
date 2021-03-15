using System;
using System.IO;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class Marker : NetObj
{
	// Token: 0x06000899 RID: 2201 RVA: 0x0003F6E0 File Offset: 0x0003D8E0
	public override void Awake()
	{
		base.Awake();
		base.renderer.enabled = false;
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0003F6F4 File Offset: 0x0003D8F4
	public void Update()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			float num = Vector3.Distance(main.transform.position, base.transform.position);
			float num2 = Mathf.Tan(0.017453292f * main.fieldOfView * 0.5f) * 0.04f * num;
			base.transform.localScale = new Vector3(num2, num2, num2);
			Transform transform = base.transform.FindChild("particle");
			if (transform != null)
			{
				transform.GetComponent<ParticleSystem>().startSize = Mathf.Tan(0.017453292f * main.fieldOfView * 0.5f) * num;
			}
		}
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0003F7A4 File Offset: 0x0003D9A4
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
		writer.Write(this.m_show);
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0003F8C4 File Offset: 0x0003DAC4
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
		this.m_show = reader.ReadBoolean();
		this.SetVisibleState(this.m_show);
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0003F9B0 File Offset: 0x0003DBB0
	public void SetVisibleState(bool visible)
	{
		this.m_show = visible;
		base.renderer.enabled = visible;
		Transform transform = base.transform.FindChild("particle");
		if (transform == null)
		{
			return;
		}
		if (visible)
		{
			transform.GetComponent<ParticleSystem>().Play();
		}
		else
		{
			transform.GetComponent<ParticleSystem>().Stop();
		}
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0003FA10 File Offset: 0x0003DC10
	public override void SetVisible(bool visible)
	{
	}

	// Token: 0x04000705 RID: 1797
	private bool m_show;
}
