using System;
using System.IO;
using UnityEngine;

// Token: 0x020000C4 RID: 196
[AddComponentMenu("Scripts/Weapons/Monster Mine")]
public class AoeMonster : Unit
{
	// Token: 0x060006FA RID: 1786 RVA: 0x00035104 File Offset: 0x00033304
	public virtual void Setup(int ownerID, int gunID, bool visible, int seenByMask, int damage, int ap, float splashRadius, float splashDmgFactor)
	{
		this.m_gunID = gunID;
		this.SetOwner(ownerID);
		this.SetVisible(visible);
		base.SetSeenByMask(seenByMask);
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00035124 File Offset: 0x00033324
	public override void Awake()
	{
		base.Awake();
		GameObject gameObject = GameObject.Find("WaterSurface");
		if (gameObject != null)
		{
			this.m_waterSurface = gameObject.GetComponent<WaterSurface>();
		}
		this.m_armTimer = this.m_armDelay;
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00035168 File Offset: 0x00033368
	protected override void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			if (this.m_ttl != 0f)
			{
				this.m_ttl -= Time.fixedDeltaTime;
				if (this.m_ttl <= 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
			}
			if (this.m_waterSurface != null)
			{
				Vector3 position = base.transform.position;
				position.y = this.m_waterSurface.GetWorldWaveHeight(position);
				base.transform.position = position;
			}
			if (this.m_armTimer > 0f)
			{
				this.m_armTimer -= Time.fixedDeltaTime;
			}
			if (!this.m_deployed && this.m_armTimer <= 0f)
			{
				this.m_deployed = true;
				this.OnDeploy();
			}
		}
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00035244 File Offset: 0x00033444
	protected virtual void OnDeploy()
	{
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00035248 File Offset: 0x00033448
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_deployed);
		writer.Write(this.m_armTimer);
		writer.Write(this.m_gunID);
		writer.Write(this.m_ttl);
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x0003528C File Offset: 0x0003348C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_deployed = reader.ReadBoolean();
		this.m_armTimer = reader.ReadSingle();
		this.m_gunID = reader.ReadInt32();
		this.m_ttl = reader.ReadSingle();
		if (this.m_deployed)
		{
		}
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x000352DC File Offset: 0x000334DC
	public override void SetVisible(bool visible)
	{
		if (this.IsVisible() == visible)
		{
			return;
		}
		base.SetVisible(visible);
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = visible;
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x0003532C File Offset: 0x0003352C
	public float GetVisibleDistance()
	{
		return 1000f;
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00035334 File Offset: 0x00033534
	public float GetArmTimer()
	{
		return this.m_armTimer;
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0003533C File Offset: 0x0003353C
	public bool IsDeployed()
	{
		return this.m_deployed;
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x00035344 File Offset: 0x00033544
	public override float GetWidth()
	{
		return 2f;
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0003534C File Offset: 0x0003354C
	public override float GetLength()
	{
		return 2f;
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x00035354 File Offset: 0x00033554
	public override bool Damage(Hit hit)
	{
		return true;
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00035358 File Offset: 0x00033558
	public void Disarm()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x040005DE RID: 1502
	public float m_ttl;

	// Token: 0x040005DF RID: 1503
	public float m_armDelay = 20f;

	// Token: 0x040005E0 RID: 1504
	protected bool m_deployed;

	// Token: 0x040005E1 RID: 1505
	protected int m_gunID;

	// Token: 0x040005E2 RID: 1506
	private float m_armTimer;

	// Token: 0x040005E3 RID: 1507
	private WaterSurface m_waterSurface;
}
