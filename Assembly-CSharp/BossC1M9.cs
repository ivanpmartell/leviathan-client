using System;
using System.IO;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class BossC1M9 : NetObj
{
	// Token: 0x06000A1E RID: 2590 RVA: 0x0004A204 File Offset: 0x00048404
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0004A20C File Offset: 0x0004840C
	public void Start()
	{
		this.m_bossForehead = GameObject.Find("BossC1M9_forehead").transform.GetChild(0).gameObject;
		this.m_bossJaw = GameObject.Find("BossC1M9_jaw").transform.GetChild(0).gameObject;
		this.m_weaponPlatform = GameObject.Find("bossc1m9_platform").GetComponent<Platform>();
		this.m_weakspotPlatform = GameObject.Find("bossc1m9_weakspot").GetComponent<Platform>();
		this.StateSetup();
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0004A28C File Offset: 0x0004848C
	private void StateSetup()
	{
		if (this.m_isHeadOpen)
		{
			this.m_bossForehead.animation.Play("opened_forehead_idle");
			this.m_weakspotPlatform.m_immuneToDamage = false;
			this.m_weakspotPlatform.m_allowAutotarget = true;
		}
		else
		{
			this.m_bossForehead.animation.Play("closed_forehead_idle");
			this.m_weakspotPlatform.m_immuneToDamage = true;
			this.m_weakspotPlatform.m_allowAutotarget = false;
		}
		if (this.m_isJawOpen)
		{
			this.m_bossJaw.animation.Play("open_jaw_idle");
		}
		else
		{
			this.m_bossJaw.animation.Play("closed_jaw_idle");
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0004A344 File Offset: 0x00048544
	public void Update()
	{
		if (NetObj.m_simulating)
		{
		}
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0004A350 File Offset: 0x00048550
	private string StateDebug()
	{
		string text = string.Empty;
		text = text + " Phase: " + this.m_bossTurn.ToString();
		string text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			" HeadOpen: ",
			this.m_isHeadOpen.ToString(),
			"/",
			this.m_toggleHead.ToString()
		});
		text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			" JawOpen: ",
			this.m_isJawOpen.ToString(),
			"/",
			this.m_toggleJaw.ToString()
		});
		return text + " UseCann: " + this.m_fireCannon.ToString();
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0004A410 File Offset: 0x00048610
	private void UpdateRay()
	{
		if (this.m_rayVisualizer != null)
		{
			this.m_rayVisTime += Time.fixedDeltaTime;
			float num = Mathf.Clamp(this.m_rayVisTime / this.m_rayFadeTime, 0f, 1f);
			if (num >= 1f)
			{
				this.DisableRayVisualizer();
			}
			else if (this.m_rayVisualizer.renderer.material.HasProperty("_TintColor"))
			{
				Color color = this.m_rayVisualizer.renderer.material.GetColor("_TintColor");
				color.a = 1f - num;
				this.m_rayVisualizer.renderer.material.SetColor("_TintColor", color);
			}
		}
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0004A4D8 File Offset: 0x000486D8
	protected void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			this.UpdateRay();
			this.m_timeInTurn += Time.fixedDeltaTime;
			if (this.b_startOfTurn)
			{
				this.NextTurn();
				PLog.Log("Start Turn: " + this.StateDebug());
				this.b_startOfTurn = false;
			}
			this.HeadChangeState(!this.m_isHeadOpen);
			this.JawChangeState(!this.m_isJawOpen);
			if (this.m_fireCannon && this.m_timeInTurn > 4f)
			{
				this.FireMainCannon();
				this.m_fireCannon = false;
			}
		}
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x0004A57C File Offset: 0x0004877C
	private void HeadChangeState(bool openIt)
	{
		if (!this.m_toggleHead)
		{
			return;
		}
		float num = 10f - this.m_bossForehead.animation["open_forehead"].length;
		if (this.m_timeInTurn < num)
		{
			return;
		}
		this.m_toggleHead = false;
		if (openIt)
		{
			this.m_bossForehead.animation.Play("open_forehead");
			this.m_weakspotPlatform.m_immuneToDamage = false;
		}
		else
		{
			this.m_bossForehead.animation.Play("close_forehead");
			this.m_weakspotPlatform.m_immuneToDamage = true;
		}
		this.m_isHeadOpen = openIt;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0004A620 File Offset: 0x00048820
	private void JawChangeState(bool openIt)
	{
		if (!this.m_toggleJaw)
		{
			return;
		}
		float num = 10f - this.m_bossJaw.animation["open_jaw"].length;
		if (this.m_timeInTurn < num)
		{
			return;
		}
		this.m_toggleJaw = false;
		if (openIt)
		{
			this.m_bossJaw.animation.Play("open_jaw");
		}
		else
		{
			this.m_bossJaw.animation.Play("close_jaw");
		}
		this.m_isJawOpen = openIt;
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0004A6AC File Offset: 0x000488AC
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_timeInTurn);
		writer.Write(this.m_bossTurn);
		writer.Write(this.m_isHeadOpen);
		writer.Write(this.m_isJawOpen);
		writer.Write(this.m_cannonTarget.x);
		writer.Write(this.m_cannonTarget.y);
		writer.Write(this.m_cannonTarget.z);
		writer.Write(this.m_cannonAngle);
		writer.Write(this.m_beamAngle);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(base.transform.rotation.x);
		writer.Write(base.transform.rotation.y);
		writer.Write(base.transform.rotation.z);
		writer.Write(base.transform.rotation.w);
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0004A7F0 File Offset: 0x000489F0
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_timeInTurn = reader.ReadSingle();
		this.m_bossTurn = reader.ReadInt32();
		this.m_isHeadOpen = reader.ReadBoolean();
		this.m_isJawOpen = reader.ReadBoolean();
		this.m_cannonTarget = new Vector3
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle()
		};
		this.m_cannonAngle = reader.ReadSingle();
		this.m_beamAngle = reader.ReadSingle();
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
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0004A908 File Offset: 0x00048B08
	private void NextTurn()
	{
		this.m_bossTurn++;
		this.m_timeInTurn = 0f;
		if (this.m_bossTurn == 5)
		{
			this.m_bossTurn = 1;
		}
		if (this.m_bossTurn == 1)
		{
			if (this.m_isJawOpen)
			{
				this.m_bossJaw.animation.Play("close_jaw");
				this.m_isJawOpen = false;
			}
			this.m_toggleHead = true;
		}
		if (this.m_bossTurn == 2)
		{
			this.GetMineLauncher();
			this.m_toggleJaw = true;
		}
		if (this.m_bossTurn == 3)
		{
			this.m_toggleHead = true;
		}
		if (this.m_bossTurn == 4)
		{
			this.m_fireCannon = true;
		}
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0004A9BC File Offset: 0x00048BBC
	private void GetLinks()
	{
		Platform platform = UnityEngine.Object.FindObjectOfType(typeof(Platform)) as Platform;
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0004A9E0 File Offset: 0x00048BE0
	private Ship GetMainTarget()
	{
		Ship[] array = UnityEngine.Object.FindObjectsOfType(typeof(Ship)) as Ship[];
		if (array.Length == 0)
		{
			return null;
		}
		int num = PRand.Range(0, array.Length - 1);
		return array[num];
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0004AA1C File Offset: 0x00048C1C
	private float DistanceToLine(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0004AA4C File Offset: 0x00048C4C
	private bool InFiringCone(Unit unit, Ray ray)
	{
		Vector3[] targetPoints = unit.GetTargetPoints();
		foreach (Vector3 point in targetPoints)
		{
			float num = this.DistanceToLine(ray, point);
			if (num < this.m_rayWidth / 2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0004AAA4 File Offset: 0x00048CA4
	private void FireMainCannon()
	{
		GameObject gameObject = GuiUtils.FindChildOf(this.m_weaponPlatform.transform, "bossc1m9_cannon");
		Gun_Railgun component = gameObject.GetComponent<Gun_Railgun>();
		Vector3 position = gameObject.transform.position;
		Vector3 normalized = (this.m_cannonTarget - position).normalized;
		Vector3 targetPos = position + normalized * 3000f;
		position.y = 1f;
		targetPos.y = 1f;
		Ray ray = new Ray(position, normalized);
		Ship[] array = UnityEngine.Object.FindObjectsOfType(typeof(Ship)) as Ship[];
		foreach (Ship ship in array)
		{
			if (this.InFiringCone(ship, ray))
			{
				ship.Damage(new Hit(component, this.m_cannonDamage, this.m_cannonArmorPiercing, ship.transform.position, new Vector3(1f, 0f, 0f)), ship.GetSectionTop());
				this.DoHitEffect(ship.transform.position);
			}
		}
		Vector3 position2 = gameObject.transform.position;
		Quaternion rot = Quaternion.LookRotation(normalized, new Vector3(0f, 1f, 0f));
		this.AnimateFire(position2, rot);
		this.EnableRayVisualizer(position, targetPos);
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0004ABFC File Offset: 0x00048DFC
	private bool GetMinePositions(out Vector3 target1, out Vector3 target2)
	{
		float f = 0.017453292f * (float)PRand.Range(0, 360);
		Vector3 a = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		float num = (float)(PRand.Range(0, 30) + 20);
		target1 = this.m_cannonTarget + a * num;
		target2 = this.m_cannonTarget + a * -num;
		return false;
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0004AC74 File Offset: 0x00048E74
	private void FireMines()
	{
		Vector3 pos;
		Vector3 pos2;
		this.GetMinePositions(out pos, out pos2);
		GameObject gameObject = GuiUtils.FindChildOf(this.m_weaponPlatform.transform, "bossc1m9_mine");
		Gun_AutoCannon component = gameObject.GetComponent<Gun_AutoCannon>();
		Order order = new Order(component, Order.Type.Fire, pos);
		Order order2 = new Order(component, Order.Type.Fire, pos2);
		component.ClearOrders();
		component.AddOrder(order);
		component.AddOrder(order2);
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0004ACD4 File Offset: 0x00048ED4
	private GameObject GetMineLauncher()
	{
		Ship mainTarget = this.GetMainTarget();
		if (mainTarget == null)
		{
			return null;
		}
		this.m_cannonTarget = mainTarget.transform.position;
		this.FireMines();
		return null;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0004AD10 File Offset: 0x00048F10
	private void EnableRayVisualizer(Vector3 muzzlePos, Vector3 targetPos)
	{
		muzzlePos.y = 5f;
		targetPos.y = 5f;
		this.m_rayVisTime = 0f;
		this.m_rayTargetPos = targetPos;
		if (this.m_rayVisualizer == null)
		{
			this.m_rayVisualizer = (UnityEngine.Object.Instantiate(this.m_rayPrefab) as GameObject);
		}
		Vector3 position = (muzzlePos + targetPos) * 0.5f;
		float num = Vector3.Distance(muzzlePos, targetPos);
		this.m_rayVisualizer.transform.position = position;
		this.m_rayVisualizer.transform.localScale = new Vector3(this.m_rayWidth, 1f, num);
		this.m_rayVisualizer.transform.rotation = Quaternion.LookRotation(targetPos - muzzlePos, Vector3.up);
		float y = num / 8f;
		this.m_rayVisualizer.renderer.material.mainTextureScale = new Vector2(1f, y);
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0004AE04 File Offset: 0x00049004
	private void DisableRayVisualizer()
	{
		if (this.m_rayVisualizer != null)
		{
			UnityEngine.Object.Destroy(this.m_rayVisualizer);
			this.m_rayVisualizer = null;
		}
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0004AE2C File Offset: 0x0004902C
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.DisableRayVisualizer();
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0004AE3C File Offset: 0x0004903C
	private void AnimateFire(Vector3 pos, Quaternion rot)
	{
		if (this.m_muzzleEffect != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_muzzleEffect, pos, rot) as GameObject;
			gameObject.transform.parent = base.transform;
		}
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0004AE80 File Offset: 0x00049080
	private void DoHitEffect(Vector3 pos)
	{
		if (this.m_hitEffectHiPrefab != null)
		{
			UnityEngine.Object.Instantiate(this.m_hitEffectHiPrefab, pos, Quaternion.identity);
		}
	}

	// Token: 0x04000865 RID: 2149
	private bool b_startOfTurn = true;

	// Token: 0x04000866 RID: 2150
	private bool m_isHeadOpen;

	// Token: 0x04000867 RID: 2151
	private bool m_isJawOpen;

	// Token: 0x04000868 RID: 2152
	private bool m_toggleHead;

	// Token: 0x04000869 RID: 2153
	private bool m_toggleJaw;

	// Token: 0x0400086A RID: 2154
	private bool m_fireCannon;

	// Token: 0x0400086B RID: 2155
	private int m_bossTurn = -1;

	// Token: 0x0400086C RID: 2156
	private float m_timeInTurn;

	// Token: 0x0400086D RID: 2157
	private Vector3 m_cannonTarget = default(Vector3);

	// Token: 0x0400086E RID: 2158
	private float m_cannonAngle;

	// Token: 0x0400086F RID: 2159
	private float m_beamAngle;

	// Token: 0x04000870 RID: 2160
	private Platform m_weaponPlatform;

	// Token: 0x04000871 RID: 2161
	private Platform m_weakspotPlatform;

	// Token: 0x04000872 RID: 2162
	private GameObject m_bossForehead;

	// Token: 0x04000873 RID: 2163
	private GameObject m_bossJaw;

	// Token: 0x04000874 RID: 2164
	public GameObject m_rayPrefab;

	// Token: 0x04000875 RID: 2165
	private GameObject m_rayVisualizer;

	// Token: 0x04000876 RID: 2166
	public int m_cannonDamage = 250;

	// Token: 0x04000877 RID: 2167
	public int m_cannonArmorPiercing = 25;

	// Token: 0x04000878 RID: 2168
	private float m_rayVisTime;

	// Token: 0x04000879 RID: 2169
	private Vector3 m_rayTargetPos;

	// Token: 0x0400087A RID: 2170
	public float m_rayWidth = 1f;

	// Token: 0x0400087B RID: 2171
	public float m_rayFadeTime = 0.5f;

	// Token: 0x0400087C RID: 2172
	public GameObject m_muzzleEffect;

	// Token: 0x0400087D RID: 2173
	public GameObject m_muzzleEffectLow;

	// Token: 0x0400087E RID: 2174
	public GameObject m_hitEffectLowPrefab;

	// Token: 0x0400087F RID: 2175
	public GameObject m_hitEffectHiPrefab;
}
