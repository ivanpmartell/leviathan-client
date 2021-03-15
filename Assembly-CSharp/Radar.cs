using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000F0 RID: 240
[AddComponentMenu("Scripts/Modules/Radar")]
public class Radar : HPModule
{
	// Token: 0x06000958 RID: 2392 RVA: 0x00043D28 File Offset: 0x00041F28
	public override void Awake()
	{
		base.Awake();
		this.m_energy = this.m_maxEnergy;
		this.m_mesh = new Mesh();
		this.m_mesh.name = "RadarMesh";
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00043D58 File Offset: 0x00041F58
	public override void SetDeploy(bool deploy)
	{
		this.m_deploy = deploy;
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00043D64 File Offset: 0x00041F64
	public override bool GetDeploy()
	{
		return this.m_deploy;
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00043D6C File Offset: 0x00041F6C
	public override StatusWnd_HPModule CreateStatusWindow(GameObject guiCamera)
	{
		bool friendly = base.GetOwner() == NetObj.m_localPlayerID;
		return new StatusWnd_Shield(this, guiCamera, friendly);
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00043D90 File Offset: 0x00041F90
	public override string GetStatusText()
	{
		return "Doing fart sounds";
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00043D98 File Offset: 0x00041F98
	public override void GetChargeLevel(out float i, out float time)
	{
		if (this.m_disabled)
		{
			base.GetChargeLevel(out i, out time);
			return;
		}
		i = this.m_energy / this.m_maxEnergy;
		time = (this.m_maxEnergy - this.m_energy) / this.m_chargeRate;
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x00043DD4 File Offset: 0x00041FD4
	public override void SaveOrders(BinaryWriter stream)
	{
		base.SaveOrders(stream);
		stream.Write(this.m_deploy);
		stream.Write(this.m_energy);
		stream.Write(this.m_visibleTimer);
		stream.Write(this.m_echoes.Count);
		foreach (Radar.RadarEcho radarEcho in this.m_echoes)
		{
			stream.Write(radarEcho.m_point.x);
			stream.Write(radarEcho.m_point.y);
			stream.Write(radarEcho.m_point.z);
			stream.Write(radarEcho.m_size);
		}
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00043EB0 File Offset: 0x000420B0
	public override void LoadOrders(BinaryReader stream)
	{
		base.LoadOrders(stream);
		this.m_deploy = stream.ReadBoolean();
		this.m_energy = stream.ReadSingle();
		this.m_visibleTimer = stream.ReadSingle();
		int num = stream.ReadInt32();
		this.m_echoes.Clear();
		for (int i = 0; i < num; i++)
		{
			Radar.RadarEcho radarEcho = new Radar.RadarEcho();
			radarEcho.m_point.x = stream.ReadSingle();
			radarEcho.m_point.y = stream.ReadSingle();
			radarEcho.m_point.z = stream.ReadSingle();
			radarEcho.m_size = stream.ReadSingle();
			this.m_echoes.Add(radarEcho);
		}
		this.m_dirty = true;
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00043F64 File Offset: 0x00042164
	protected override void FixedUpdate()
	{
		if (this.m_unit == null)
		{
			return;
		}
		if (TurnMan.instance == null)
		{
			return;
		}
		base.FixedUpdate();
		if (NetObj.m_simulating)
		{
			if (this.m_visibleTimer > 0f)
			{
				this.m_visibleTimer -= Time.fixedDeltaTime;
				if (this.m_visibleTimer <= 0f)
				{
					this.m_echoes.Clear();
					this.m_dirty = true;
				}
			}
			if (!base.IsDisabled() && !this.m_unit.IsDead())
			{
				if (this.m_deploy && this.m_energy >= this.m_maxEnergy)
				{
					this.m_deploy = false;
					this.m_energy = 0f;
					this.m_visibleTimer = this.m_visibleDuration;
					this.DoRadarPing();
				}
				if (this.m_energy < this.m_maxEnergy)
				{
					this.m_energy += this.m_chargeRate * Time.fixedDeltaTime;
					if (this.m_energy > this.m_maxEnergy)
					{
						this.m_energy = this.m_maxEnergy;
					}
				}
			}
		}
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00044084 File Offset: 0x00042284
	public override void Update()
	{
		if (this.m_unit == null)
		{
			return;
		}
		if (TurnMan.instance == null)
		{
			return;
		}
		base.Update();
		if (!base.IsDisabled() && !this.m_unit.IsDead())
		{
			this.DrawRadarArea();
			if (base.GetOwnerTeam() == TurnMan.instance.GetPlayerTeam(NetObj.m_localPlayerID) && this.m_echoes.Count > 0)
			{
				if (this.m_dirty)
				{
					this.m_dirty = false;
					this.UpdateMesh();
				}
				float a = Mathf.Clamp(this.m_visibleDuration - this.m_visibleTimer, 0f, 1f);
				float b = Mathf.Clamp(this.m_visibleTimer, 0f, 1f);
				float a2 = Mathf.Min(a, b);
				Color color = this.m_radarMaterial.color;
				color.a = a2;
				this.m_radarMaterial.color = color;
				Graphics.DrawMesh(this.m_mesh, Matrix4x4.identity, this.m_radarMaterial, 0, null, 0, null, false, false);
			}
		}
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00044190 File Offset: 0x00042390
	protected virtual bool SetupLineDrawer()
	{
		if (this.m_lineDrawer == null)
		{
			this.m_lineDrawer = Camera.main.GetComponent<LineDrawer>();
			if (this.m_lineDrawer == null)
			{
				return false;
			}
			this.m_supplyAreaLineType = this.m_lineDrawer.GetTypeID("radar");
			DebugUtils.Assert(this.m_supplyAreaLineType >= 0);
		}
		return true;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x000441FC File Offset: 0x000423FC
	private void DrawRadarArea()
	{
		if (!this.SetupLineDrawer())
		{
			return;
		}
		if (base.GetOwnerTeam() != TurnMan.instance.GetPlayerTeam(NetObj.m_localPlayerID))
		{
			return;
		}
		if (!this.m_unit.IsSelected())
		{
			return;
		}
		Vector3 position = base.transform.position;
		position.y += 2f;
		this.m_lineDrawer.DrawXZCircle(position, this.m_range, 40, this.m_supplyAreaLineType, 0.15f);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00044280 File Offset: 0x00042480
	private void UpdateMesh()
	{
		float y = 0f;
		foreach (Radar.RadarEcho radarEcho in this.m_echoes)
		{
			int count = this.m_vertises.Count;
			this.m_indices.Add(count);
			this.m_indices.Add(count + 1);
			this.m_indices.Add(count + 2);
			this.m_indices.Add(count + 2);
			this.m_indices.Add(count + 3);
			this.m_indices.Add(count);
			float size = radarEcho.m_size;
			this.m_vertises.Add(radarEcho.m_point + new Vector3(-size, y, -size));
			this.m_vertises.Add(radarEcho.m_point + new Vector3(size, y, -size));
			this.m_vertises.Add(radarEcho.m_point + new Vector3(size, y, size));
			this.m_vertises.Add(radarEcho.m_point + new Vector3(-size, y, size));
			this.m_uvs.Add(new Vector2(0f, 0f));
			this.m_uvs.Add(new Vector2(1f, 0f));
			this.m_uvs.Add(new Vector2(1f, 1f));
			this.m_uvs.Add(new Vector2(0f, 1f));
		}
		this.m_mesh.Clear();
		this.m_mesh.vertices = this.m_vertises.ToArray();
		this.m_mesh.uv = this.m_uvs.ToArray();
		this.m_mesh.triangles = this.m_indices.ToArray();
		this.m_vertises.Clear();
		this.m_indices.Clear();
		this.m_uvs.Clear();
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x000444A4 File Offset: 0x000426A4
	public override float GetMaxEnergy()
	{
		return this.m_maxEnergy;
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x000444AC File Offset: 0x000426AC
	public override float GetEnergy()
	{
		return this.m_energy;
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x000444B4 File Offset: 0x000426B4
	private void DoRadarPing()
	{
		if (MessageLog.instance != null && this.m_unit.GetOwner() != NetObj.m_localPlayerID)
		{
			MessageLog.instance.ShowMessage(MessageLog.TextPosition.Top, "$label_radarpingdetected", string.Empty, string.Empty, 2f);
		}
		if (this.IsVisible() && this.m_pingEffect)
		{
			UnityEngine.Object.Instantiate(this.m_pingEffect, base.transform.position + new Vector3(0f, 5f, 0f), Quaternion.identity);
		}
		if (NetObj.m_phase != TurnPhase.Testing)
		{
			List<NetObj> all = NetObj.GetAll();
			this.m_echoes.Clear();
			Vector3 position = base.transform.position;
			int owner = base.GetOwner();
			foreach (NetObj netObj in all)
			{
				Unit unit = netObj as Unit;
				if (!(unit == null))
				{
					if (!netObj.IsSeenByPlayer(owner))
					{
						Vector3 position2 = netObj.transform.position;
						float num = Vector3.Distance(position2, position);
						if (num < this.m_range && unit.GetVelocity().magnitude > this.m_minSpeed)
						{
							Radar.RadarEcho radarEcho = new Radar.RadarEcho();
							radarEcho.m_point = position2;
							radarEcho.m_size = unit.GetLength() / 2f;
							this.m_echoes.Add(radarEcho);
						}
					}
				}
			}
		}
		this.m_dirty = true;
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0004466C File Offset: 0x0004286C
	public override string GetTooltip()
	{
		string text = base.GetName() + "\nHP: " + this.m_health;
		string text2 = text;
		return string.Concat(new string[]
		{
			text2,
			"\nEnergy: ",
			this.m_energy.ToString(),
			" / ",
			this.m_maxEnergy.ToString()
		});
	}

	// Token: 0x04000784 RID: 1924
	public float m_maxEnergy = 100f;

	// Token: 0x04000785 RID: 1925
	public float m_chargeRate = 1f;

	// Token: 0x04000786 RID: 1926
	public float m_range = 200f;

	// Token: 0x04000787 RID: 1927
	public float m_minSpeed = 2f;

	// Token: 0x04000788 RID: 1928
	public float m_visibleDuration = 15f;

	// Token: 0x04000789 RID: 1929
	public Material m_radarMaterial;

	// Token: 0x0400078A RID: 1930
	public GameObject m_pingEffect;

	// Token: 0x0400078B RID: 1931
	private bool m_deploy;

	// Token: 0x0400078C RID: 1932
	private float m_energy = 100f;

	// Token: 0x0400078D RID: 1933
	private float m_visibleTimer;

	// Token: 0x0400078E RID: 1934
	private List<Radar.RadarEcho> m_echoes = new List<Radar.RadarEcho>();

	// Token: 0x0400078F RID: 1935
	private Mesh m_mesh;

	// Token: 0x04000790 RID: 1936
	private bool m_dirty;

	// Token: 0x04000791 RID: 1937
	private List<Vector3> m_vertises = new List<Vector3>();

	// Token: 0x04000792 RID: 1938
	private List<int> m_indices = new List<int>();

	// Token: 0x04000793 RID: 1939
	private List<Vector2> m_uvs = new List<Vector2>();

	// Token: 0x04000794 RID: 1940
	private LineDrawer m_lineDrawer;

	// Token: 0x04000795 RID: 1941
	private int m_supplyAreaLineType;

	// Token: 0x020000F1 RID: 241
	private class RadarEcho
	{
		// Token: 0x04000796 RID: 1942
		public Vector3 m_point;

		// Token: 0x04000797 RID: 1943
		public float m_size;
	}
}
