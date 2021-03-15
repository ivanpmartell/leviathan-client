using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000076 RID: 118
[AddComponentMenu("Scripts/Mission/MNArea")]
public class MNArea : MNTrigger
{
	// Token: 0x06000506 RID: 1286 RVA: 0x0002A01C File Offset: 0x0002821C
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0002A024 File Offset: 0x00028224
	protected void Destroy()
	{
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0002A028 File Offset: 0x00028228
	protected void Update()
	{
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0002A02C File Offset: 0x0002822C
	protected void FixedUpdate()
	{
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0002A030 File Offset: 0x00028230
	public override void Trigger()
	{
		base.Trigger();
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0002A038 File Offset: 0x00028238
	public override void OnEvent(string eventName)
	{
		if (eventName == "on")
		{
			this.m_disabled = false;
			this.ResetTrigger();
		}
		else if (eventName == "off")
		{
			this.m_disabled = true;
		}
		else
		{
			base.EventWarning(eventName);
		}
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0002A08C File Offset: 0x0002828C
	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (component != null)
		{
			Color color = new Color(0.5f, 0.9f, 1f, 0.15f);
			Gizmos.color = color;
			Gizmos.DrawCube(component.bounds.center, component.bounds.size);
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0002A0F4 File Offset: 0x000282F4
	private void ResetTrigger()
	{
		foreach (int id in this.m_units)
		{
			NetObj byID = NetObj.GetByID(id);
			if (byID)
			{
				Ship component = byID.GetComponent<Ship>();
				if (component)
				{
					if (this.ShouldTrigger(component))
					{
						this.Trigger();
						if (this.m_disabled)
						{
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0002A1A4 File Offset: 0x000283A4
	private bool ShouldTrigger(Ship ship)
	{
		if (NetObj.m_phase == TurnPhase.Testing)
		{
			return false;
		}
		if (!this.m_onlyPlayers)
		{
			return this.m_group.Length == 0 || this.m_group == ship.GetGroup();
		}
		if (ship.GetOwner() > 3)
		{
			return false;
		}
		if (this.m_allPlayers)
		{
			return this.NrOfPlayers() == TurnMan.instance.GetNrOfPlayers();
		}
		return this.m_group.Length == 0 || this.m_group == ship.GetGroup();
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0002A254 File Offset: 0x00028454
	private void OnTriggerEnter(Collider other)
	{
		Ship component = other.transform.parent.gameObject.GetComponent<Ship>();
		if (this.m_units.Contains(component.GetNetID()))
		{
			return;
		}
		this.m_units.Add(component.GetNetID());
		if (this.ShouldTrigger(component))
		{
			this.Trigger();
		}
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0002A2B4 File Offset: 0x000284B4
	private void OnTriggerExit(Collider other)
	{
		Ship component = other.transform.parent.gameObject.GetComponent<Ship>();
		this.m_units.Remove(component.GetNetID());
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0002A2EC File Offset: 0x000284EC
	private int NrOfPlayers()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (int id in this.m_units)
		{
			GameObject gameObject = NetObj.GetByID(id).gameObject;
			Ship component = gameObject.GetComponent<Ship>();
			if (TurnMan.instance.IsHuman(component.GetOwner()))
			{
				hashSet.Add(component.GetOwner());
			}
		}
		return hashSet.Count;
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0002A390 File Offset: 0x00028590
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_group);
		writer.Write(this.m_onlyPlayers);
		writer.Write(this.m_allPlayers);
		writer.Write(this.m_units.Count);
		foreach (int value in this.m_units)
		{
			writer.Write(value);
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0002A434 File Offset: 0x00028634
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_group = reader.ReadString();
		this.m_onlyPlayers = reader.ReadBoolean();
		this.m_allPlayers = reader.ReadBoolean();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.m_units.Add(reader.ReadInt32());
		}
	}

	// Token: 0x04000430 RID: 1072
	public string m_group = string.Empty;

	// Token: 0x04000431 RID: 1073
	public bool m_onlyPlayers = true;

	// Token: 0x04000432 RID: 1074
	public bool m_allPlayers;

	// Token: 0x04000433 RID: 1075
	private HashSet<int> m_units = new HashSet<int>();
}
