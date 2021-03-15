using System;
using System.IO;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class DamageEffect : MonoBehaviour
{
	// Token: 0x06000750 RID: 1872 RVA: 0x00036BA4 File Offset: 0x00034DA4
	private void Awake()
	{
		this.m_health = this.m_maxHealth;
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00036BB4 File Offset: 0x00034DB4
	public void Activate(bool firstTime)
	{
		if (this.m_active)
		{
			return;
		}
		this.m_active = true;
		if (firstTime)
		{
			this.m_activateTimer = this.m_delay;
		}
		else
		{
			this.InternalActivate(false);
		}
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00036BE8 File Offset: 0x00034DE8
	public void Deactivate()
	{
		if (!this.m_active)
		{
			return;
		}
		this.m_active = false;
		if (this.m_startEffectInstance != null)
		{
			UnityEngine.Object.Destroy(this.m_startEffectInstance);
			this.m_startEffectInstance = null;
		}
		if (this.m_persistentEffectInstance != null)
		{
			UnityEngine.Object.Destroy(this.m_persistentEffectInstance);
			this.m_persistentEffectInstance = null;
		}
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00036C50 File Offset: 0x00034E50
	public void OnShipHealthChange(float healthPercentage)
	{
		if (this.m_shipHealthActivationPercentage >= 0f)
		{
			if (this.m_active)
			{
				if (healthPercentage > this.m_shipHealthActivationPercentage)
				{
					this.Deactivate();
				}
			}
			else if (healthPercentage <= this.m_shipHealthActivationPercentage)
			{
				this.Activate(true);
			}
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00036CA4 File Offset: 0x00034EA4
	public int Damage(int dmg)
	{
		if (this.m_shipHealthActivationPercentage >= 0f)
		{
			return dmg;
		}
		int num = dmg;
		if (num > this.m_health)
		{
			num = this.m_health;
		}
		this.m_health -= num;
		if (this.m_health <= 0)
		{
			this.Activate(true);
		}
		return num;
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00036CFC File Offset: 0x00034EFC
	public bool Heal(int health)
	{
		if (this.m_health >= this.m_maxHealth)
		{
			return true;
		}
		this.m_health += health;
		if (this.m_health > this.m_maxHealth)
		{
			this.m_health = this.m_maxHealth;
		}
		if (this.m_health == this.m_maxHealth)
		{
			this.Deactivate();
		}
		return false;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00036D60 File Offset: 0x00034F60
	public bool IsActive()
	{
		return this.m_active;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00036D68 File Offset: 0x00034F68
	public void Update()
	{
		if (this.m_activateTimer >= 0f)
		{
			this.m_activateTimer -= Time.deltaTime;
			if (this.m_activateTimer < 0f)
			{
				this.InternalActivate(true);
			}
		}
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00036DA4 File Offset: 0x00034FA4
	public void OnSinkingUpdate()
	{
		if (this.m_hatesWater && this.m_active && base.transform.position.y < -1f)
		{
			this.Deactivate();
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00036DEC File Offset: 0x00034FEC
	public void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.m_radius);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00036E04 File Offset: 0x00035004
	public void SaveState(BinaryWriter writer)
	{
		writer.Write(this.m_health);
		writer.Write(this.m_active);
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00036E20 File Offset: 0x00035020
	public void LoadState(BinaryReader reader)
	{
		this.m_health = reader.ReadInt32();
		bool flag = reader.ReadBoolean();
		if (flag)
		{
			this.Activate(false);
		}
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00036E50 File Offset: 0x00035050
	public void SetVisible(bool visible)
	{
		if (this.m_visible == visible)
		{
			return;
		}
		this.m_visible = visible;
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = visible;
		}
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00036EA0 File Offset: 0x000350A0
	private void InternalActivate(bool firstTime)
	{
		GameObject startEffect = this.m_startEffect;
		GameObject persistentEffect = this.m_persistentEffect;
		if (firstTime && startEffect != null)
		{
			this.m_startEffectInstance = (UnityEngine.Object.Instantiate(startEffect, base.transform.position, base.transform.rotation) as GameObject);
			this.m_startEffectInstance.transform.parent = base.gameObject.transform;
			CamShaker component = base.gameObject.GetComponent<CamShaker>();
			if (component != null)
			{
				component.Trigger();
			}
		}
		if (persistentEffect != null)
		{
			this.m_persistentEffectInstance = (UnityEngine.Object.Instantiate(persistentEffect, base.transform.position, base.transform.rotation) as GameObject);
			this.m_persistentEffectInstance.transform.parent = base.gameObject.transform;
			if (!firstTime)
			{
				ParticleSystem[] componentsInChildren = this.m_persistentEffectInstance.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Simulate(10f);
				}
			}
		}
		ParticleSystem[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem2 in componentsInChildren2)
		{
			if (this.m_simulating)
			{
				particleSystem2.Play();
			}
			else
			{
				particleSystem2.Pause();
			}
		}
		Renderer[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren3)
		{
			renderer.enabled = this.m_visible;
		}
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x0003704C File Offset: 0x0003524C
	public void SetSimulating(bool enabled)
	{
		this.m_simulating = enabled;
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (this.m_simulating)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Pause();
			}
		}
	}

	// Token: 0x040005FC RID: 1532
	public float m_radius = 5f;

	// Token: 0x040005FD RID: 1533
	public float m_delay = 0.1f;

	// Token: 0x040005FE RID: 1534
	public int m_maxHealth = 30;

	// Token: 0x040005FF RID: 1535
	public float m_shipHealthActivationPercentage = -1f;

	// Token: 0x04000600 RID: 1536
	public bool m_hatesWater;

	// Token: 0x04000601 RID: 1537
	public GameObject m_startEffect;

	// Token: 0x04000602 RID: 1538
	public GameObject m_persistentEffect;

	// Token: 0x04000603 RID: 1539
	public GameObject m_startEffectLow;

	// Token: 0x04000604 RID: 1540
	public GameObject m_persistentEffectLow;

	// Token: 0x04000605 RID: 1541
	private bool m_simulating;

	// Token: 0x04000606 RID: 1542
	private bool m_active;

	// Token: 0x04000607 RID: 1543
	private bool m_visible = true;

	// Token: 0x04000608 RID: 1544
	private bool m_sinking;

	// Token: 0x04000609 RID: 1545
	private int m_health = 30;

	// Token: 0x0400060A RID: 1546
	private float m_activateTimer = -1f;

	// Token: 0x0400060B RID: 1547
	private GameObject m_startEffectInstance;

	// Token: 0x0400060C RID: 1548
	private GameObject m_persistentEffectInstance;
}
