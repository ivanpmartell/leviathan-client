using System;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class ShieldGeometry : MonoBehaviour
{
	// Token: 0x060009B6 RID: 2486 RVA: 0x00045FF8 File Offset: 0x000441F8
	public void Setup(Unit unit, Shield ownerShild, bool firstTime)
	{
		this.m_unit = unit;
		this.m_ownerShield = ownerShild;
		if (firstTime)
		{
			this.m_activateTimer = 0f;
			this.m_fadeAlpha = 0f;
			this.UpdateAlpha();
			this.SetScale(0f);
			if (ownerShild.IsVisible())
			{
				this.m_activateSound.audio.Play();
			}
		}
		else
		{
			this.m_fadeAlpha = 1f;
			this.UpdateAlpha();
			this.SetScale(1f);
		}
		this.SetVisible(this.m_ownerShield.IsVisible());
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00046090 File Offset: 0x00044290
	public void Deactivate(bool destroyed)
	{
		base.collider.enabled = false;
		this.m_deactivateTimer = 0f;
		if (this.m_ownerShield.IsVisible())
		{
			if (destroyed)
			{
				this.m_destroyedSound.audio.Play();
			}
			else
			{
				this.m_deactivateSound.audio.Play();
			}
		}
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x000460F0 File Offset: 0x000442F0
	public Unit GetUnit()
	{
		return this.m_unit;
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x000460F8 File Offset: 0x000442F8
	private void UpdateAlpha()
	{
		float value = this.m_fadeAlpha * 0.7f + this.m_hitAlpha * 0.5f;
		this.m_visual.renderer.material.SetFloat("_Opacity", value);
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0004613C File Offset: 0x0004433C
	private void SetScale(float i)
	{
		this.m_visual.transform.localScale = new Vector3(i, i, i);
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00046158 File Offset: 0x00044358
	protected void Update()
	{
		if (this.m_activateTimer >= 0f)
		{
			this.m_activateTimer += Time.deltaTime;
			float num = this.m_activateTimer / this.m_activateDuration;
			if (num >= 1f)
			{
				this.m_activateTimer = -1f;
				num = 1f;
			}
			this.m_fadeAlpha = num;
			this.SetScale(num);
			this.UpdateAlpha();
		}
		else if (this.m_deactivateTimer >= 0f)
		{
			this.m_deactivateTimer += Time.deltaTime;
			float num2 = this.m_deactivateTimer / this.m_activateDuration;
			if (num2 >= 1f)
			{
				this.m_deactivateTimer = -1f;
				num2 = 1f;
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.m_fadeAlpha = 1f - num2;
			this.SetScale(1f - num2);
			this.UpdateAlpha();
		}
		if (this.m_hitTimer >= 0f)
		{
			this.m_hitTimer += Time.deltaTime;
			float num3 = this.m_hitTimer / this.m_hitDuration;
			if (num3 >= 1f)
			{
				this.m_hitAlpha = 0f;
				this.m_hitTimer = -1f;
			}
			else
			{
				this.m_hitAlpha = 1f - num3;
			}
			this.UpdateAlpha();
		}
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x000462AC File Offset: 0x000444AC
	public void Damage(Hit hit, bool showDmgText)
	{
		TurnMan.instance.AddShieldAbsorb(this.m_ownerShield.GetOwner(), hit.m_damage);
		float num = (float)hit.m_damage;
		this.m_ownerShield.Drain(num, true);
		this.m_hitTimer = 0f;
		if (this.m_ownerShield.IsVisible() && hit.m_havePoint)
		{
			bool flag = num >= this.m_ownerShield.GetMaxEnergy() / 6f;
			GameObject gameObject;
			if (flag)
			{
				gameObject = this.m_hitEffectBig;
			}
			else
			{
				gameObject = this.m_hitEffect;
			}
			if (gameObject != null)
			{
				UnityEngine.Object.Instantiate(gameObject, hit.m_point, Quaternion.LookRotation(-hit.m_dir));
			}
		}
		if (this.m_ownerShield.IsVisible() && showDmgText)
		{
			Vector3 pos = (!hit.m_havePoint) ? this.m_ownerShield.transform.position : hit.m_point;
			HitText.instance.AddDmgText(this.m_ownerShield.GetNetID(), pos, string.Empty, Constants.m_shieldAbsorbedText);
		}
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x000463C8 File Offset: 0x000445C8
	public void SetVisible(bool visible)
	{
		Renderer[] componentsInChildren = this.m_visual.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = visible;
		}
	}

	// Token: 0x040007EA RID: 2026
	public GameObject m_hitEffectLow;

	// Token: 0x040007EB RID: 2027
	public GameObject m_hitEffect;

	// Token: 0x040007EC RID: 2028
	public GameObject m_hitEffectBigLow;

	// Token: 0x040007ED RID: 2029
	public GameObject m_hitEffectBig;

	// Token: 0x040007EE RID: 2030
	public GameObject m_activateSound;

	// Token: 0x040007EF RID: 2031
	public GameObject m_deactivateSound;

	// Token: 0x040007F0 RID: 2032
	public GameObject m_destroyedSound;

	// Token: 0x040007F1 RID: 2033
	public GameObject m_visual;

	// Token: 0x040007F2 RID: 2034
	private Unit m_unit;

	// Token: 0x040007F3 RID: 2035
	private Shield m_ownerShield;

	// Token: 0x040007F4 RID: 2036
	private float m_hitAlpha;

	// Token: 0x040007F5 RID: 2037
	private float m_fadeAlpha;

	// Token: 0x040007F6 RID: 2038
	private float m_hitTimer = -1f;

	// Token: 0x040007F7 RID: 2039
	private float m_hitDuration = 0.5f;

	// Token: 0x040007F8 RID: 2040
	private float m_activateTimer = -1f;

	// Token: 0x040007F9 RID: 2041
	private float m_deactivateTimer = -1f;

	// Token: 0x040007FA RID: 2042
	private float m_activateDuration = 1f;
}
