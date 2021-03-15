using System;
using System.IO;
using UnityEngine;

// Token: 0x020000B8 RID: 184
internal class DamageMan
{
	// Token: 0x0600068F RID: 1679 RVA: 0x000326F8 File Offset: 0x000308F8
	public DamageMan(GameObject ship)
	{
		this.m_damageEffectRoot = ship.transform.Find("DamageEffects");
		if (this.m_damageEffectRoot != null && this.m_damageEffectRoot.childCount == 0)
		{
			this.m_damageEffectRoot = null;
		}
		if (this.m_damageEffectRoot)
		{
			this.m_damageEffects = this.m_damageEffectRoot.GetComponentsInChildren<DamageEffect>(true);
		}
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00032778 File Offset: 0x00030978
	public void SetVisible(bool visible)
	{
		foreach (DamageEffect damageEffect in this.m_damageEffects)
		{
			damageEffect.SetVisible(visible);
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x000327AC File Offset: 0x000309AC
	public void SetSimulating(bool enabled)
	{
		foreach (DamageEffect damageEffect in this.m_damageEffects)
		{
			damageEffect.SetSimulating(enabled);
		}
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x000327E0 File Offset: 0x000309E0
	public void OnSinkingUpdate()
	{
		foreach (DamageEffect damageEffect in this.m_damageEffects)
		{
			damageEffect.OnSinkingUpdate();
		}
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00032814 File Offset: 0x00030A14
	public void SaveDamageEffects(BinaryWriter writer)
	{
		writer.Write((byte)this.m_damageEffects.Length);
		for (int i = 0; i < this.m_damageEffects.Length; i++)
		{
			DamageEffect damageEffect = this.m_damageEffects[i];
			if (damageEffect != null)
			{
				damageEffect.SaveState(writer);
			}
		}
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00032868 File Offset: 0x00030A68
	public void LoadDamageEffects(BinaryReader reader)
	{
		int num = (int)reader.ReadByte();
		DebugUtils.Assert(num == this.m_damageEffects.Length);
		for (int i = 0; i < num; i++)
		{
			DamageEffect damageEffect = this.m_damageEffects[i];
			if (damageEffect != null)
			{
				damageEffect.LoadState(reader);
			}
		}
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x000328BC File Offset: 0x00030ABC
	public void HealDamageEffects(int health)
	{
		foreach (DamageEffect damageEffect in this.m_damageEffects)
		{
			if (!damageEffect.Heal(health))
			{
				return;
			}
		}
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x000328F8 File Offset: 0x00030AF8
	public void OnShipHealthChanged(float healthPercentage)
	{
		foreach (DamageEffect damageEffect in this.m_damageEffects)
		{
			damageEffect.OnShipHealthChange(healthPercentage);
		}
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0003292C File Offset: 0x00030B2C
	public void EnableCloseDamageEffect(Vector3 point, int damage)
	{
		foreach (DamageEffect damageEffect in this.m_damageEffects)
		{
			if (!damageEffect.IsActive())
			{
				float num = Vector3.Distance(damageEffect.transform.position, point);
				if (num < damageEffect.m_radius)
				{
					damage = damageEffect.Damage(damage);
					if (damage <= 0)
					{
						return;
					}
				}
			}
		}
	}

	// Token: 0x04000585 RID: 1413
	private Transform m_damageEffectRoot;

	// Token: 0x04000586 RID: 1414
	private DamageEffect[] m_damageEffects = new DamageEffect[0];
}
