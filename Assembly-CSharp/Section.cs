using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class Section : MonoBehaviour
{
	// Token: 0x06000978 RID: 2424 RVA: 0x00044C2C File Offset: 0x00042E2C
	private void Awake()
	{
		this.m_settings = ComponentDB.instance.GetSection(base.name.Substring(0, base.name.Length - 7));
		DebugUtils.Assert(this.m_settings != null);
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x00044C74 File Offset: 0x00042E74
	public Vector3 GetCenter()
	{
		if (base.collider != null)
		{
			return base.collider.bounds.center;
		}
		LODGroup component = base.GetComponent<LODGroup>();
		if (component != null)
		{
			return base.transform.TransformPoint(component.localReferencePoint);
		}
		if (base.renderer != null)
		{
			return base.renderer.bounds.center;
		}
		PLog.LogError("Failed to aquire any valid center point of section " + base.name);
		return base.transform.position;
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00044D10 File Offset: 0x00042F10
	public void FixedUpdate()
	{
		if (NetObj.IsSimulating())
		{
			if (this.m_inSmokeTimer >= 0f)
			{
				this.m_inSmokeTimer += Time.fixedDeltaTime;
			}
			if (this.m_explodeTimer >= 0f)
			{
				this.m_explodeTimer -= Time.fixedDeltaTime;
				if (this.m_explodeTimer <= 0f)
				{
					this.m_explodeTimer = -1f;
					if (this.m_unit.IsVisible())
					{
						GameObject explosionPrefab = this.m_explosionPrefab;
						if (explosionPrefab != null)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(explosionPrefab, base.transform.localToWorldMatrix.MultiplyPoint3x4(this.m_explosionPos), Quaternion.identity) as GameObject;
							gameObject.transform.parent = base.transform;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00044DE4 File Offset: 0x00042FE4
	public void Setup(Ship unit)
	{
		this.m_unit = unit;
		int num = 0;
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			battery.Setup(this.m_unit, num);
			num++;
		}
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00044E38 File Offset: 0x00043038
	public int GetBatterySlots()
	{
		int num = 0;
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			num += battery.GetSlots();
		}
		return num;
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00044E7C File Offset: 0x0004307C
	public void GetAllHPModules(ref List<HPModule> modules)
	{
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			battery.GetAllHPModules(ref modules);
		}
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00044EB8 File Offset: 0x000430B8
	public Material GetMaterial()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Renderer renderer = child.renderer;
			if (renderer != null)
			{
				return UnityEngine.Object.Instantiate(renderer.sharedMaterial) as Material;
			}
			Animation animation = child.animation;
			if (animation)
			{
				for (int j = 0; j < child.transform.childCount; j++)
				{
					Transform child2 = child.transform.GetChild(j);
					Renderer renderer2 = child2.renderer;
					if (renderer2 != null)
					{
						return UnityEngine.Object.Instantiate(renderer2.sharedMaterial) as Material;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00044F7C File Offset: 0x0004317C
	public void SetMaterial(Material material)
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Renderer renderer = child.renderer;
			if (renderer != null)
			{
				Material[] array = new Material[renderer.sharedMaterials.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = material;
				}
				renderer.sharedMaterials = array;
			}
			Animation animation = child.animation;
			if (animation)
			{
				for (int k = 0; k < child.transform.childCount; k++)
				{
					Transform child2 = child.transform.GetChild(k);
					Renderer renderer2 = child2.renderer;
					if (renderer2 != null)
					{
						Material[] array2 = new Material[renderer2.sharedMaterials.Length];
						for (int l = 0; l < renderer2.materials.Length; l++)
						{
							array2[l] = material;
						}
						renderer2.sharedMaterials = array2;
					}
				}
			}
		}
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00045090 File Offset: 0x00043290
	public void SetOwner(int owner)
	{
		this.m_owner = owner;
		Battery[] componentsInChildren = base.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			battery.SetOwner(owner);
		}
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x000450CC File Offset: 0x000432CC
	public void Supply(ref int resources)
	{
		HPModule[] componentsInChildren = base.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			hpmodule.Supply(ref resources);
		}
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00045104 File Offset: 0x00043304
	public Section.SectionType GetSectionType()
	{
		return this.m_type;
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x0004510C File Offset: 0x0004330C
	public Battery GetBattery(int id)
	{
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		if (id < 0 || id >= componentsInChildren.Length)
		{
			return null;
		}
		return componentsInChildren[id];
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x0004513C File Offset: 0x0004333C
	public void SaveState(BinaryWriter writer)
	{
		writer.Write(this.m_inSmokeTimer);
		writer.Write(this.m_explodeTimer);
		writer.Write(this.m_explosionPos.x);
		writer.Write(this.m_explosionPos.y);
		writer.Write(this.m_explosionPos.z);
		writer.Write(this.IsColliderEnabled());
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		writer.Write((byte)componentsInChildren.Length);
		foreach (Battery battery in componentsInChildren)
		{
			battery.SaveState(writer);
		}
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x000451D8 File Offset: 0x000433D8
	public void LoadState(BinaryReader reader)
	{
		this.m_inSmokeTimer = reader.ReadSingle();
		this.m_explodeTimer = reader.ReadSingle();
		this.m_explosionPos.x = reader.ReadSingle();
		this.m_explosionPos.y = reader.ReadSingle();
		this.m_explosionPos.z = reader.ReadSingle();
		bool colliderEnabled = reader.ReadBoolean();
		this.SetColliderEnabled(colliderEnabled);
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		int num = (int)reader.ReadByte();
		DebugUtils.Assert(num == componentsInChildren.Length, "number of batteries missmatch");
		foreach (Battery battery in componentsInChildren)
		{
			battery.LoadState(reader);
		}
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x0004528C File Offset: 0x0004348C
	public void SaveOrders(BinaryWriter writer)
	{
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		writer.Write(componentsInChildren.Length);
		foreach (Battery battery in componentsInChildren)
		{
			battery.SaveOrders(writer);
		}
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x000452D0 File Offset: 0x000434D0
	public void LoadOrders(BinaryReader reader)
	{
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		int num = reader.ReadInt32();
		DebugUtils.Assert(componentsInChildren.Length == num);
		foreach (Battery battery in componentsInChildren)
		{
			battery.LoadOrders(reader);
		}
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00045324 File Offset: 0x00043524
	public void ClearOrders()
	{
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			battery.ClearOrders();
		}
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00045360 File Offset: 0x00043560
	public Unit GetUnit()
	{
		return this.m_unit;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00045368 File Offset: 0x00043568
	public int GetOwner()
	{
		return this.m_owner;
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x00045370 File Offset: 0x00043570
	public bool Damage(Hit hit)
	{
		return this.m_unit.Damage(hit, this);
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x00045380 File Offset: 0x00043580
	public void SetVisible(bool visible)
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child.animation != null && child.childCount > 0)
			{
				for (int j = 0; j < child.childCount; j++)
				{
					if (child.GetChild(j).renderer)
					{
						child.GetChild(j).renderer.enabled = visible;
					}
				}
			}
			else if (child.renderer != null)
			{
				child.renderer.enabled = visible;
			}
		}
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x00045434 File Offset: 0x00043634
	public void Explode()
	{
		this.m_explosionPos = base.transform.worldToLocalMatrix.MultiplyPoint3x4(this.GetCenter());
		this.m_explodeTimer = UnityEngine.Random.Range(1.5f, 4f);
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			battery.DestroyAll();
		}
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x000454A4 File Offset: 0x000436A4
	public void OnKilled()
	{
		this.SetColliderEnabled(false);
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x000454B0 File Offset: 0x000436B0
	private bool IsColliderEnabled()
	{
		if (base.collider != null)
		{
			return base.collider.enabled;
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Collider collider = base.transform.GetChild(i).collider;
			if (collider != null)
			{
				return collider.enabled;
			}
		}
		return false;
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x0004551C File Offset: 0x0004371C
	private void SetColliderEnabled(bool enabled)
	{
		if (base.collider != null)
		{
			base.collider.enabled = enabled;
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Collider collider = base.transform.GetChild(i).collider;
			if (collider != null)
			{
				collider.enabled = enabled;
			}
		}
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00045588 File Offset: 0x00043788
	public int GetTotalValue()
	{
		int num = this.m_settings.m_value;
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			num += battery.GetTotalValue();
		}
		return num;
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x000455D8 File Offset: 0x000437D8
	public int GetValue()
	{
		return this.m_settings.m_value;
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x000455E8 File Offset: 0x000437E8
	public void OnSmokeEnter()
	{
		this.m_inSmokeTimer = 0f;
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x000455F8 File Offset: 0x000437F8
	public bool IsInSmoke()
	{
		return this.m_inSmokeTimer >= 0f && this.m_inSmokeTimer < 0.5f;
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00045628 File Offset: 0x00043828
	public void OnSetSimulating(bool enabled)
	{
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0004562C File Offset: 0x0004382C
	public virtual Dictionary<string, string> GetShipEditorInfo()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["Health"] = this.m_maxHealth.ToString();
		dictionary["AC"] = this.m_armorClass.ToString();
		dictionary["Forward Speed"] = this.m_modifiers.m_speed.ToString();
		dictionary["Reverse Speed"] = this.m_modifiers.m_reverseSpeed.ToString();
		dictionary["Sight Range"] = this.m_modifiers.m_sightRange.ToString();
		return dictionary;
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x000456C0 File Offset: 0x000438C0
	public string GetName()
	{
		return Localize.instance.Translate("$" + base.name + "_name");
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x000456E4 File Offset: 0x000438E4
	public void ClearGunOrdersAndTargets()
	{
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			battery.ClearGunOrdersAndTargets();
		}
	}

	// Token: 0x040007B9 RID: 1977
	public string m_series;

	// Token: 0x040007BA RID: 1978
	public bool m_defaultSection;

	// Token: 0x040007BB RID: 1979
	public Section.SectionType m_type;

	// Token: 0x040007BC RID: 1980
	public int m_maxHealth = 100;

	// Token: 0x040007BD RID: 1981
	public int m_armorClass = 10;

	// Token: 0x040007BE RID: 1982
	public Section.ModifierSettings m_modifiers;

	// Token: 0x040007BF RID: 1983
	public Texture2D m_GUITexture;

	// Token: 0x040007C0 RID: 1984
	public GameObject m_explosionPrefab;

	// Token: 0x040007C1 RID: 1985
	public GameObject m_explosionLowPrefab;

	// Token: 0x040007C2 RID: 1986
	public SectionSettings m_settings;

	// Token: 0x040007C3 RID: 1987
	private int m_owner = -1;

	// Token: 0x040007C4 RID: 1988
	private Ship m_unit;

	// Token: 0x040007C5 RID: 1989
	private float m_inSmokeTimer = -1f;

	// Token: 0x040007C6 RID: 1990
	private float m_explodeTimer = -1f;

	// Token: 0x040007C7 RID: 1991
	private Vector3 m_explosionPos = new Vector3(0f, 0f, 0f);

	// Token: 0x040007C8 RID: 1992
	public Section.SectionRating m_rating = new Section.SectionRating();

	// Token: 0x020000F8 RID: 248
	public enum SectionType
	{
		// Token: 0x040007CA RID: 1994
		Front,
		// Token: 0x040007CB RID: 1995
		Mid,
		// Token: 0x040007CC RID: 1996
		Rear,
		// Token: 0x040007CD RID: 1997
		Top
	}

	// Token: 0x020000F9 RID: 249
	[Serializable]
	public class ModifierSettings
	{
		// Token: 0x040007CE RID: 1998
		public float m_speed;

		// Token: 0x040007CF RID: 1999
		public float m_reverseSpeed;

		// Token: 0x040007D0 RID: 2000
		public float m_acceleration;

		// Token: 0x040007D1 RID: 2001
		public float m_reverseAcceleration;

		// Token: 0x040007D2 RID: 2002
		public float m_turnSpeed;

		// Token: 0x040007D3 RID: 2003
		public float m_sightRange;

		// Token: 0x040007D4 RID: 2004
		public float m_breakAcceleration;
	}

	// Token: 0x020000FA RID: 250
	[Serializable]
	public class SectionRating
	{
		// Token: 0x040007D5 RID: 2005
		public int m_health = 3;

		// Token: 0x040007D6 RID: 2006
		public int m_armor = 3;

		// Token: 0x040007D7 RID: 2007
		public int m_speed = 3;

		// Token: 0x040007D8 RID: 2008
		public int m_sight = 3;
	}
}
