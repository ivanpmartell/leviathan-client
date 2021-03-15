using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200011B RID: 283
[AddComponentMenu("Scripts/Units/Unit")]
public class Unit : NetObj, IOrderable
{
	// Token: 0x06000AE6 RID: 2790 RVA: 0x00050E00 File Offset: 0x0004F000
	public override void Awake()
	{
		base.Awake();
		if (Unit.m_onCreated != null)
		{
			Unit.m_onCreated(this);
		}
		this.m_settings = ComponentDB.instance.GetUnit(base.name);
		DebugUtils.Assert(this.m_settings != null);
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00050E50 File Offset: 0x0004F050
	public override void OnDestroy()
	{
		if (Unit.m_onRemoved != null)
		{
			Unit.m_onRemoved(this);
		}
		base.OnDestroy();
		while (this.m_orders.Count > 0)
		{
			this.RemoveLastOrder();
		}
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x00050E98 File Offset: 0x0004F098
	public virtual void Start()
	{
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x00050E9C File Offset: 0x0004F09C
	protected virtual void FixedUpdate()
	{
		if (NetObj.m_simulating && this.m_dead)
		{
			this.m_deadTime += Time.fixedDeltaTime * 0.2f;
			if (this.m_deadTime > 1f)
			{
				this.m_deadTime = 1f;
			}
		}
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x00050EF4 File Offset: 0x0004F0F4
	public virtual void Update()
	{
		this.UpdateMarker();
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00050EFC File Offset: 0x0004F0FC
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(1);
		writer.Write(this.m_name);
		writer.Write(this.m_dead);
		writer.Write(this.m_deadTime);
		writer.Write(this.m_king);
		writer.Write(this.m_cloaked);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(base.transform.rotation.x);
		writer.Write(base.transform.rotation.y);
		writer.Write(base.transform.rotation.z);
		writer.Write(base.transform.rotation.w);
		writer.Write((byte)this.m_lastDamageDealer);
		writer.Write(this.m_group);
		writer.Write((int)this.m_objective);
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0005102C File Offset: 0x0004F22C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		short num = reader.ReadInt16();
		this.m_name = reader.ReadString();
		this.m_dead = reader.ReadBoolean();
		this.m_deadTime = reader.ReadSingle();
		this.m_king = reader.ReadBoolean();
		this.m_cloaked = reader.ReadBoolean();
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
		this.m_lastDamageDealer = (int)reader.ReadByte();
		this.m_group = reader.ReadString();
		Unit.ObjectiveTypes objective = (Unit.ObjectiveTypes)reader.ReadInt32();
		this.SetObjective(objective);
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0005112C File Offset: 0x0004F32C
	public virtual void SaveOrders(BinaryWriter stream)
	{
		stream.Write(1);
		stream.Write(this.m_blockedRoute);
		stream.Write(this.m_orders.Count);
		foreach (Order order in this.m_orders)
		{
			order.Save(stream);
		}
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x000511B8 File Offset: 0x0004F3B8
	public virtual void LoadOrders(BinaryReader stream)
	{
		short num = stream.ReadInt16();
		this.m_blockedRoute = stream.ReadBoolean();
		this.ClearOrders();
		int num2 = stream.ReadInt32();
		for (int i = 0; i < num2; i++)
		{
			Order order = new Order(this, stream);
			this.AddOrder(order);
		}
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x00051208 File Offset: 0x0004F408
	public void SetBlockedRoute(bool blocked)
	{
		this.m_blockedRoute = blocked;
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x00051214 File Offset: 0x0004F414
	public bool IsRouteBlocked()
	{
		return this.m_blockedRoute;
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0005121C File Offset: 0x0004F41C
	public bool IsSelected()
	{
		return this.m_marker != null;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0005122C File Offset: 0x0004F42C
	public virtual void SetSelected(bool selected, bool explicitSelected)
	{
		if (selected == this.IsSelected())
		{
			return;
		}
		if (selected && this.m_selectionMarkerPrefab != null)
		{
			Color color;
			TurnMan.instance.GetPlayerColors(base.GetOwner(), out color);
			if (this.m_marker != null)
			{
				UnityEngine.Object.Destroy(this.m_marker);
			}
			this.m_marker = (UnityEngine.Object.Instantiate(this.m_selectionMarkerPrefab, base.transform.position, base.transform.rotation) as GameObject);
			this.m_marker.transform.parent = base.transform;
			this.m_marker.GetComponent<UnitSelectionMarker>().Setup(this.GetMarkerSize(), color);
		}
		else if (this.m_marker != null)
		{
			UnityEngine.Object.Destroy(this.m_marker);
			this.m_marker = null;
		}
		if (selected && this.m_selectSound != null && explicitSelected)
		{
			UnityEngine.Object.Instantiate(this.m_selectSound, base.transform.position, Quaternion.identity);
		}
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00051348 File Offset: 0x0004F548
	protected Vector3 GetMarkerSize()
	{
		return new Vector3(this.GetWidth() * 1.2f, 0f, this.GetLength() * 1.2f);
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00051378 File Offset: 0x0004F578
	public virtual float GetLength()
	{
		return 0f;
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00051380 File Offset: 0x0004F580
	public virtual float GetWidth()
	{
		return 0f;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00051388 File Offset: 0x0004F588
	public virtual Vector3 GetVelocity()
	{
		return Vector3.zero;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00051390 File Offset: 0x0004F590
	protected override void OnSetDrawOrders(bool enabled)
	{
		foreach (Order order in this.m_orders)
		{
			order.SetMarkerEnabled(enabled, this.m_orderMarkerPrefab);
		}
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x000513FC File Offset: 0x0004F5FC
	public void AddOrder(Order order)
	{
		this.m_orders.AddLast(order);
		order.SetMarkerEnabled(NetObj.m_drawOrders, this.m_orderMarkerPrefab);
		this.OnOrdersChanged();
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x00051430 File Offset: 0x0004F630
	public bool RemoveOrder(Order order)
	{
		if (this.m_orders.Remove(order))
		{
			order.SetMarkerEnabled(false, this.m_orderMarkerPrefab);
			this.OnOrdersChanged();
			return true;
		}
		return false;
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0005145C File Offset: 0x0004F65C
	public bool RemoveFirstOrder()
	{
		if (this.m_orders.Count > 0)
		{
			Order value = this.m_orders.First.Value;
			this.m_orders.RemoveFirst();
			value.SetMarkerEnabled(false, this.m_orderMarkerPrefab);
			this.OnOrdersChanged();
			return true;
		}
		return false;
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x000514AC File Offset: 0x0004F6AC
	public bool RemoveLastOrder()
	{
		if (this.m_orders.Count > 0)
		{
			Order value = this.m_orders.Last.Value;
			this.m_orders.RemoveLast();
			value.SetMarkerEnabled(false, this.m_orderMarkerPrefab);
			this.OnOrdersChanged();
			return true;
		}
		return false;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x000514FC File Offset: 0x0004F6FC
	public virtual void OnOrdersChanged()
	{
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00051500 File Offset: 0x0004F700
	public bool IsOrdersEmpty()
	{
		return this.m_orders.Count == 0;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x00051510 File Offset: 0x0004F710
	public bool IsLastOrder(Order order)
	{
		return this.m_orders.Count > 0 && this.m_orders.Last.Value == order;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00051544 File Offset: 0x0004F744
	public virtual void ClearOrders()
	{
		while (this.m_orders.Count > 0)
		{
			this.RemoveLastOrder();
		}
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00051564 File Offset: 0x0004F764
	public virtual void ClearMoveOrders()
	{
		while (this.m_orders.Count > 0)
		{
			this.RemoveLastOrder();
		}
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x00051584 File Offset: 0x0004F784
	public bool IsMoveOrdersValid()
	{
		Vector3 from = base.transform.position;
		foreach (Order order in this.m_orders)
		{
			if (order.m_type == Order.Type.MoveForward || order.m_type == Order.Type.MoveBackward)
			{
				Vector3 pos = order.GetPos();
				if (!this.CanMove(from, pos))
				{
					return false;
				}
				from = pos;
			}
		}
		return true;
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0005162C File Offset: 0x0004F82C
	public virtual void OnChildOrderLostFocus(Order order)
	{
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00051630 File Offset: 0x0004F830
	public virtual void OnChildOrderGotFocus(Order order)
	{
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00051634 File Offset: 0x0004F834
	protected virtual bool CanMove(Vector3 from, Vector3 to)
	{
		from.y = -2f;
		to.y = -2f;
		int layerMask = 1;
		RaycastHit raycastHit;
		return !Physics.Linecast(from, to, out raycastHit, layerMask);
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00051668 File Offset: 0x0004F868
	public virtual string GetTooltip()
	{
		return this.m_name;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00051670 File Offset: 0x0004F870
	public virtual bool Damage(Hit hit)
	{
		return false;
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00051674 File Offset: 0x0004F874
	public virtual bool TestLOS(NetObj obj)
	{
		return false;
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x00051678 File Offset: 0x0004F878
	public virtual bool TestLOS(Vector3 pos)
	{
		return false;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0005167C File Offset: 0x0004F87C
	public virtual bool IsInSmoke()
	{
		return false;
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x00051680 File Offset: 0x0004F880
	public bool IsCloaked()
	{
		return this.m_cloaked;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x00051688 File Offset: 0x0004F888
	public virtual bool IsDoingMaintenance()
	{
		return false;
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0005168C File Offset: 0x0004F88C
	public void SetCloaked(bool cloaked)
	{
		if (cloaked)
		{
			this.ClearGunOrdersAndTargets();
		}
		this.m_cloaked = cloaked;
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x000516A4 File Offset: 0x0004F8A4
	protected virtual void ClearGunOrdersAndTargets()
	{
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x000516A8 File Offset: 0x0004F8A8
	public virtual Vector3[] GetViewPoints()
	{
		return new Vector3[]
		{
			base.transform.position
		};
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x000516D4 File Offset: 0x0004F8D4
	public virtual Vector3[] GetTargetPoints()
	{
		return new Vector3[]
		{
			base.transform.position
		};
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00051700 File Offset: 0x0004F900
	public bool IsDead()
	{
		return this.m_dead;
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00051708 File Offset: 0x0004F908
	public virtual bool IsValidTarget()
	{
		return !this.IsDead();
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x00051718 File Offset: 0x0004F918
	public void SetName(string name)
	{
		this.m_name = name;
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00051724 File Offset: 0x0004F924
	public virtual int GetTotalValue()
	{
		return 0;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00051728 File Offset: 0x0004F928
	public string GetGroup()
	{
		return this.m_group;
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x00051730 File Offset: 0x0004F930
	public void SetGroup(string group)
	{
		this.m_group = group;
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0005173C File Offset: 0x0004F93C
	public virtual void Supply(ref int resources)
	{
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x00051740 File Offset: 0x0004F940
	protected virtual void OnKilled()
	{
		if (this.m_dead)
		{
			return;
		}
		this.SetObjective(Unit.ObjectiveTypes.None);
		this.m_dead = true;
		if (Unit.m_onKilled != null)
		{
			Unit.m_onKilled(this);
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x00051774 File Offset: 0x0004F974
	public virtual string GetName()
	{
		return this.m_name;
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0005177C File Offset: 0x0004F97C
	public virtual int GetHealth()
	{
		return 0;
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00051780 File Offset: 0x0004F980
	public virtual int GetMaxHealth()
	{
		return 0;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x00051784 File Offset: 0x0004F984
	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (this.m_objectiveIcon)
		{
			Transform transform = this.m_objectiveIcon.transform.FindChild("particle");
			if (transform == null)
			{
				return;
			}
			if (this.IsKing())
			{
				visible = true;
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
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x000517FC File Offset: 0x0004F9FC
	public void UpdateMarker()
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		if (this.m_objectiveIcon == null)
		{
			return;
		}
		float num = Vector3.Distance(main.transform.position, base.transform.position);
		float num2 = Mathf.Tan(0.017453292f * main.fieldOfView * 0.5f) * 0.04f * num;
		this.m_objectiveIcon.transform.localScale = new Vector3(num2, num2, num2);
		Transform transform = this.m_objectiveIcon.transform.FindChild("particle");
		if (transform != null)
		{
			transform.GetComponent<ParticleSystem>().startSize = Mathf.Tan(0.017453292f * main.fieldOfView * 0.5f) * num;
		}
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x000518C8 File Offset: 0x0004FAC8
	public void SetObjective(Unit.ObjectiveTypes objectivety)
	{
		if (this.IsKing())
		{
			int localPlayer = NetObj.GetLocalPlayer();
			if (localPlayer < 0)
			{
				return;
			}
			int playerTeam = TurnMan.instance.GetPlayerTeam(localPlayer);
			if (base.GetOwnerTeam() == playerTeam)
			{
				objectivety = Unit.ObjectiveTypes.Defend;
			}
			else
			{
				objectivety = Unit.ObjectiveTypes.Destroy;
			}
		}
		if (objectivety == this.m_objective)
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		this.m_objective = objectivety;
		if (this.m_objectiveIcon != null)
		{
			UnityEngine.Object.Destroy(this.m_objectiveIcon);
			this.m_objectiveIcon = null;
		}
		if (objectivety == Unit.ObjectiveTypes.None)
		{
			return;
		}
		string name = "Defend";
		if (objectivety == Unit.ObjectiveTypes.Move)
		{
			name = "ObjectiveIcon";
		}
		if (objectivety == Unit.ObjectiveTypes.Destroy)
		{
			name = "AttackIcon";
		}
		if (objectivety == Unit.ObjectiveTypes.Defend)
		{
			name = "DefendIcon";
		}
		this.m_objectiveIcon = ObjectFactory.instance.Create(name, base.transform.position, Quaternion.identity);
		this.m_objectiveIcon.transform.parent = base.transform;
		this.m_objectiveIcon.transform.localPosition = new Vector3(0f, 10f, 0f);
		this.SetVisible(this.IsVisible());
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x000519F0 File Offset: 0x0004FBF0
	public bool CanLOS()
	{
		return !this.m_dead || this.m_deadTime < 1f;
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x00051A20 File Offset: 0x0004FC20
	public virtual float GetSightRange()
	{
		if (!this.m_dead)
		{
			return this.m_sightRange;
		}
		return Mathf.Lerp(this.m_sightRange, 0f, this.m_deadTime);
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x00051A58 File Offset: 0x0004FC58
	public void SetSightRange(float sightrange)
	{
		this.m_sightRange = sightrange;
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x00051A64 File Offset: 0x0004FC64
	public void SetKing(bool king)
	{
		this.m_king = king;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x00051A70 File Offset: 0x0004FC70
	public bool IsKing()
	{
		return this.m_king;
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x00051A78 File Offset: 0x0004FC78
	public virtual bool IsTakingWater()
	{
		return false;
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x00051A7C File Offset: 0x0004FC7C
	public virtual bool IsSinking()
	{
		return false;
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00051A80 File Offset: 0x0004FC80
	public UnitAi GetAi()
	{
		return this.m_Ai;
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x00051A88 File Offset: 0x0004FC88
	public int GetLastDamageDealer()
	{
		return this.m_lastDamageDealer;
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x00051A90 File Offset: 0x0004FC90
	public GameObject GetObjectiveIcon()
	{
		return this.m_objectiveIcon;
	}

	// Token: 0x04000915 RID: 2325
	public static Action<Unit> m_onKilled;

	// Token: 0x04000916 RID: 2326
	public static Action<Unit> m_onCreated;

	// Token: 0x04000917 RID: 2327
	public static Action<Unit> m_onRemoved;

	// Token: 0x04000918 RID: 2328
	public Action m_onTakenDamage;

	// Token: 0x04000919 RID: 2329
	public Action m_onFireWeapon;

	// Token: 0x0400091A RID: 2330
	public Action m_onMaintenanceActivation;

	// Token: 0x0400091B RID: 2331
	public GameObject m_selectionMarkerPrefab;

	// Token: 0x0400091C RID: 2332
	public GameObject m_orderMarkerPrefab;

	// Token: 0x0400091D RID: 2333
	public GameObject m_selectSound;

	// Token: 0x0400091E RID: 2334
	private string m_name = "Unknown";

	// Token: 0x0400091F RID: 2335
	public UnitSettings m_settings;

	// Token: 0x04000920 RID: 2336
	public bool m_king;

	// Token: 0x04000921 RID: 2337
	public bool m_allowAutotarget;

	// Token: 0x04000922 RID: 2338
	public Unit.ShipTagType m_shipTag;

	// Token: 0x04000923 RID: 2339
	public bool m_centerShipTag;

	// Token: 0x04000924 RID: 2340
	protected LinkedList<Order> m_orders = new LinkedList<Order>();

	// Token: 0x04000925 RID: 2341
	protected bool m_dead;

	// Token: 0x04000926 RID: 2342
	protected float m_deadTime;

	// Token: 0x04000927 RID: 2343
	protected bool m_cloaked;

	// Token: 0x04000928 RID: 2344
	protected int m_lastDamageDealer = -1;

	// Token: 0x04000929 RID: 2345
	private GameObject m_marker;

	// Token: 0x0400092A RID: 2346
	protected string m_group = string.Empty;

	// Token: 0x0400092B RID: 2347
	private bool m_blockedRoute;

	// Token: 0x0400092C RID: 2348
	private float m_sightRange;

	// Token: 0x0400092D RID: 2349
	private Unit.ObjectiveTypes m_objective;

	// Token: 0x0400092E RID: 2350
	private GameObject m_objectiveIcon;

	// Token: 0x0400092F RID: 2351
	protected UnitAi m_Ai;

	// Token: 0x0200011C RID: 284
	public enum ShipTagType
	{
		// Token: 0x04000931 RID: 2353
		Normal,
		// Token: 0x04000932 RID: 2354
		Mini,
		// Token: 0x04000933 RID: 2355
		None
	}

	// Token: 0x0200011D RID: 285
	public enum ObjectiveTypes
	{
		// Token: 0x04000935 RID: 2357
		None,
		// Token: 0x04000936 RID: 2358
		Move,
		// Token: 0x04000937 RID: 2359
		Destroy,
		// Token: 0x04000938 RID: 2360
		Defend
	}
}
