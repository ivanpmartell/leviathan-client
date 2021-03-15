using System;
using System.IO;
using UnityEngine;

// Token: 0x0200007A RID: 122
[AddComponentMenu("Scripts/Mission/MNOnDestroy")]
public class MNOnDestroy : MNTrigger
{
	// Token: 0x0600052A RID: 1322 RVA: 0x0002A798 File Offset: 0x00028998
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0002A7A0 File Offset: 0x000289A0
	public virtual void OnDrawGizmosSelected()
	{
		for (int i = 0; i < this.m_destroyed.Length; i++)
		{
			GameObject targetObj = this.GetTargetObj(i);
			if (targetObj != null)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(base.GetComponent<Transform>().position, targetObj.GetComponent<Transform>().position);
			}
		}
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0002A800 File Offset: 0x00028A00
	protected void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			if (this.m_disabled)
			{
				return;
			}
			if (this.CheckNrOfUnits(this.m_group) == 0)
			{
				this.Trigger();
			}
		}
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0002A830 File Offset: 0x00028A30
	private int CheckNrOfUnits(string group)
	{
		int num = 0;
		for (int i = 0; i < this.m_destroyed.Length; i++)
		{
			GameObject targetObj = this.GetTargetObj(i);
			if (targetObj != null)
			{
				Platform component = targetObj.GetComponent<Platform>();
				if (component && !component.IsDead())
				{
					num++;
				}
				MNSpawn component2 = targetObj.GetComponent<MNSpawn>();
				if (component2 && component2.ShouldSpawn() && !component2.SpawnedBeenDestroyed())
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0002A8C0 File Offset: 0x00028AC0
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_group);
		writer.Write(this.m_destroyed.Length);
		foreach (GameObject gameObject in this.m_destroyed)
		{
			if (gameObject == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(gameObject.GetComponent<NetObj>().GetNetID());
			}
		}
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0002A938 File Offset: 0x00028B38
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_group = reader.ReadString();
		int num = reader.ReadInt32();
		this.m_destroyedNetID = new int[num];
		this.m_destroyed = new GameObject[num];
		for (int i = 0; i < num; i++)
		{
			this.m_destroyedNetID[i] = reader.ReadInt32();
		}
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0002A998 File Offset: 0x00028B98
	public GameObject GetTargetObj(int index)
	{
		if (this.m_destroyed[index] != null)
		{
			return this.m_destroyed[index];
		}
		if (index >= this.m_destroyedNetID.Length)
		{
			return null;
		}
		if (this.m_destroyedNetID[index] == 0)
		{
			return null;
		}
		this.m_destroyed[index] = NetObj.GetByID(this.m_destroyedNetID[index]).gameObject;
		return this.m_destroyed[index];
	}

	// Token: 0x04000438 RID: 1080
	public string m_group = string.Empty;

	// Token: 0x04000439 RID: 1081
	public int[] m_destroyedNetID = new int[0];

	// Token: 0x0400043A RID: 1082
	public GameObject[] m_destroyed = new GameObject[0];
}
