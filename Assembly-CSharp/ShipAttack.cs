using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000098 RID: 152
internal class ShipAttack : Ship_Base
{
	// Token: 0x060005CD RID: 1485 RVA: 0x0002D780 File Offset: 0x0002B980
	public override string DebugString(Ship owner)
	{
		string empty = string.Empty;
		return base.DebugString(owner) + empty;
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x0002D7A0 File Offset: 0x0002B9A0
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x0002D7A4 File Offset: 0x0002B9A4
	public override void Exit(Ship owner)
	{
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x0002D7A8 File Offset: 0x0002B9A8
	public override void Save(BinaryWriter writer)
	{
		base.Save(writer);
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x0002D7B4 File Offset: 0x0002B9B4
	public override void Load(BinaryReader reader)
	{
		base.Load(reader);
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x0002D7C0 File Offset: 0x0002B9C0
	public List<GameObject> GetHumanShips()
	{
		List<GameObject> list = new List<GameObject>();
		List<NetObj> all = NetObj.GetAll();
		foreach (NetObj netObj in all)
		{
			Ship component = netObj.GetComponent<Ship>();
			if (component != null && !component.IsDead() && TurnMan.instance.IsHuman(component.GetOwner()))
			{
				list.Add(component.gameObject);
			}
		}
		return list;
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x0002D86C File Offset: 0x0002BA6C
	public void PickAttackTarget(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		if (owner.GetAi().VerifyTarget() != null)
		{
			return;
		}
		GameObject target = owner.GetAiSettings().GetTarget();
		List<GameObject> list;
		if (target)
		{
			list = MNode.GetTargets(target);
			if (list.Count == 0)
			{
				return;
			}
		}
		else
		{
			list = this.GetHumanShips();
			if (list.Count == 0)
			{
				return;
			}
		}
		int index = PRand.Range(0, list.Count - 1);
		GameObject gameObject = list[index];
		if (gameObject.GetComponent<MNSpawn>() != null)
		{
			gameObject = gameObject.GetComponent<MNSpawn>().GetSpawnedShip();
		}
		if (gameObject == null)
		{
			owner.GetAi().m_targetId = -1;
		}
		else
		{
			owner.GetAi().m_targetId = gameObject.GetComponent<NetObj>().GetNetID();
		}
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x0002D940 File Offset: 0x0002BB40
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		this.PickAttackTarget(owner, sm, dt);
		Unit unit = owner.GetAi().VerifyTarget();
		if (unit)
		{
			owner.GetAi().m_goalPosition = new Vector3?(unit.transform.position);
		}
		base.UpdateMovement(owner, sm, dt);
	}
}
