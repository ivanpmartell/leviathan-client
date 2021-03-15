using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000EF RID: 239
[AddComponentMenu("Scripts/Projectiles/Projectile_MultiTarget")]
public class Projectile_MultiTarget : Projectile
{
	// Token: 0x06000948 RID: 2376 RVA: 0x00043594 File Offset: 0x00041794
	public Projectile_MultiTarget()
	{
		this.targets = new List<Vector3>();
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x000435C0 File Offset: 0x000417C0
	public Projectile_MultiTarget(List<Vector3> inTargets)
	{
		foreach (Vector3 inTarget in inTargets)
		{
			this.AddTarget(inTarget);
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x0600094A RID: 2378 RVA: 0x00043640 File Offset: 0x00041840
	// (set) Token: 0x0600094B RID: 2379 RVA: 0x000436C4 File Offset: 0x000418C4
	public Vector3 CurrentTarget
	{
		get
		{
			if (this.targets == null || this.targets.Count == 0)
			{
				return base.transform.position;
			}
			if (this.targets.Count > this.currentTargetIndex)
			{
				return this.targets[this.currentTargetIndex];
			}
			Debug.LogWarning(string.Format("Projectile_MultiTarget.CurrentTarget can not be found, invalid currentTargetIndex of {0}", this.currentTargetIndex));
			return base.transform.position;
		}
		private set
		{
			if (this.targets.Count > this.currentTargetIndex)
			{
				this.targets[this.currentTargetIndex] = value;
			}
			else
			{
				this.targets.Add(value);
			}
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x0600094C RID: 2380 RVA: 0x00043700 File Offset: 0x00041900
	public Vector3 NextTarget
	{
		get
		{
			if (this.targets.Count > this.currentTargetIndex + 1)
			{
				return this.targets[this.currentTargetIndex + 1];
			}
			Debug.LogWarning(string.Format("Trying to get next pos. current = {0}, next = {1}, count = {2}", this.currentTargetIndex, this.currentTargetIndex + 1, this.targets.Count));
			return this.CurrentTarget;
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00043778 File Offset: 0x00041978
	public void Start()
	{
		base.transform.LookAt(this.NextTarget);
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x0004378C File Offset: 0x0004198C
	public void AddTarget(GameObject inTarget)
	{
		this.AddTarget(inTarget.transform.position);
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x000437A0 File Offset: 0x000419A0
	public void AddTarget(Vector3 inTarget)
	{
		if (this.targets == null)
		{
			this.targets = new List<Vector3>();
		}
		this.targets.Add(inTarget);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x000437D0 File Offset: 0x000419D0
	public void ClearTargets()
	{
		if (this.targets != null)
		{
			this.targets.Clear();
		}
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x000437E8 File Offset: 0x000419E8
	public void SmoothPath(int numNew)
	{
		this.SmoothPath(numNew, Vector3.zero);
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x000437F8 File Offset: 0x000419F8
	protected static List<Vector3> AddPointsBetween(Vector3 start, Vector3 end, int numNew, Vector3 randomAmmount)
	{
		List<Vector3> list = new List<Vector3>();
		Vector3 vector = end - start;
		Vector3 normalized = vector.normalized;
		float num = vector.magnitude * (1f / (float)numNew);
		for (int i = 0; i < numNew; i++)
		{
			Vector3 vector2 = start + normalized * (num * (float)i);
			float x = (UnityEngine.Random.value - 0.5f) * randomAmmount.x;
			float y = (UnityEngine.Random.value - 0.5f) * randomAmmount.y;
			float z = (UnityEngine.Random.value - 0.5f) * randomAmmount.z;
			vector2 += new Vector3(x, y, z);
			list.Add(vector2);
		}
		return list;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x000438B4 File Offset: 0x00041AB4
	public void SmoothPath(int numNew, Vector3 randomAmmount)
	{
		if (this.currentTargetIndex >= this.targets.Count - 1)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		if (this.currentTargetIndex > 0)
		{
			for (int i = 0; i < this.currentTargetIndex; i++)
			{
				list.Add(this.targets[i]);
			}
		}
		for (int j = this.currentTargetIndex; j < this.targets.Count - 2; j++)
		{
			Vector3 vector = this.targets[j];
			list.Add(vector);
			Vector3 end = this.targets[j + 1];
			list.AddRange(Projectile_MultiTarget.AddPointsBetween(vector, end, numNew, randomAmmount));
		}
		list.AddRange(Projectile_MultiTarget.AddPointsBetween(this.targets[this.targets.Count - 2], this.targets[this.targets.Count - 1], numNew, randomAmmount));
		list.Add(this.targets[this.targets.Count - 1]);
		this.targets.Clear();
		this.targets.AddRange(list);
		list.Clear();
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x000439E4 File Offset: 0x00041BE4
	protected override void SpecializedFixedUpdate()
	{
		if (this.targets == null || this.targets.Count == 0)
		{
			return;
		}
		this.m_velocity += this.m_acceleration * Time.fixedDeltaTime;
		if (this.m_velocity > this.m_originalPower)
		{
			this.m_velocity = this.m_originalPower;
		}
		Vector3 vector = this.CurrentTarget - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		Vector3 a = Vector3.Normalize(vector);
		float d = this.m_velocity * Time.fixedDeltaTime;
		base.transform.position += a * d;
		Quaternion to = Quaternion.LookRotation(vector);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.m_turnspeed * Time.fixedDeltaTime);
		if (sqrMagnitude <= 5f + Time.fixedDeltaTime)
		{
			this.currentTargetIndex++;
			if (this.currentTargetIndex >= this.targets.Count - 1)
			{
				base.HitEffect(base.transform.position, Projectile.HitType.Normal);
				base.DoSplashDamage(base.transform.position, null, this.m_damage);
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return;
		}
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00043B34 File Offset: 0x00041D34
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_velocity);
		if (this.targets == null)
		{
			writer.Write(0);
		}
		else
		{
			writer.Write(this.targets.Count);
		}
		if (this.targets != null && this.targets.Count > 0)
		{
			foreach (Vector3 vector in this.targets)
			{
				writer.Write(vector.x);
				writer.Write(vector.y);
				writer.Write(vector.z);
			}
		}
		writer.Write(this.currentTargetIndex);
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00043C20 File Offset: 0x00041E20
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_velocity = reader.ReadSingle();
		int num = reader.ReadInt32();
		this.targets = new List<Vector3>();
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				float x = reader.ReadSingle();
				float y = reader.ReadSingle();
				float z = reader.ReadSingle();
				this.targets.Add(new Vector3(x, y, z));
			}
		}
		this.currentTargetIndex = reader.ReadInt32();
	}

	// Token: 0x0400077F RID: 1919
	protected List<Vector3> targets;

	// Token: 0x04000780 RID: 1920
	protected int currentTargetIndex;

	// Token: 0x04000781 RID: 1921
	private float m_velocity;

	// Token: 0x04000782 RID: 1922
	public float m_acceleration = 10f;

	// Token: 0x04000783 RID: 1923
	public float m_turnspeed = 45f;
}
