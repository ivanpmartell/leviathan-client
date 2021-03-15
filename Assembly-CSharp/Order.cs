using System;
using System.IO;
using UnityEngine;

// Token: 0x02000126 RID: 294
public class Order
{
	// Token: 0x06000B92 RID: 2962 RVA: 0x00053D24 File Offset: 0x00051F24
	public Order(IOrderable owner, Order.Type type, int targetNetID, Vector3 localPos)
	{
		this.m_owner = owner;
		this.m_type = type;
		this.m_targetNetID = targetNetID;
		this.m_pos = localPos;
		this.m_displayRadius = 0f;
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x00053D80 File Offset: 0x00051F80
	public Order(IOrderable owner, Order.Type type, Vector3 pos)
	{
		this.m_owner = owner;
		this.m_type = type;
		this.m_pos = pos;
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x00053DCC File Offset: 0x00051FCC
	public Order(IOrderable owner, BinaryReader stream)
	{
		this.m_owner = owner;
		this.Load(stream);
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x00053E04 File Offset: 0x00052004
	public void SetMarkerEnabled(bool enabled, GameObject orderMarkerPrefab)
	{
		if (enabled)
		{
			this.Enable(orderMarkerPrefab);
		}
		else
		{
			this.Disable();
		}
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x00053E20 File Offset: 0x00052020
	private void Enable(GameObject orderMarkerPrefab)
	{
		if (this.m_marker != null)
		{
			return;
		}
		Vector3 pos = this.GetPos();
		this.m_marker = (UnityEngine.Object.Instantiate(orderMarkerPrefab, pos, Quaternion.identity) as GameObject);
		OrderMarker component = this.m_marker.GetComponent<OrderMarker>();
		component.Setup(this);
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x00053E70 File Offset: 0x00052070
	private void Disable()
	{
		if (this.m_marker)
		{
			UnityEngine.Object.DestroyObject(this.m_marker);
			this.m_marker = null;
		}
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x00053EA0 File Offset: 0x000520A0
	public Vector3 GetPos()
	{
		if (this.m_targetNetID != 0)
		{
			NetObj byID = NetObj.GetByID(this.m_targetNetID);
			if (byID != null)
			{
				return byID.transform.TransformPoint(this.m_pos);
			}
		}
		return this.m_pos;
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x00053EE8 File Offset: 0x000520E8
	public Vector3 GetLocalTargetPos()
	{
		return this.m_pos;
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x00053EF0 File Offset: 0x000520F0
	public void Save(BinaryWriter stream)
	{
		stream.Write((byte)this.m_type);
		stream.Write((byte)this.m_fireVisual);
		stream.Write(this.m_pos.x);
		stream.Write(this.m_pos.y);
		stream.Write(this.m_pos.z);
		stream.Write(this.m_haveFacing);
		stream.Write(this.m_facing.x);
		stream.Write(this.m_facing.y);
		stream.Write(this.m_facing.z);
		stream.Write(this.m_displayRadius);
		stream.Write(this.m_blockedLOS);
		stream.Write(this.m_inFiringCone);
		stream.Write(this.m_staticTargetOnly);
		stream.Write(this.m_reachedPosition);
		stream.Write(this.m_targetNetID);
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x00053FD4 File Offset: 0x000521D4
	public void Load(BinaryReader stream)
	{
		this.m_type = (Order.Type)stream.ReadByte();
		this.m_fireVisual = (Order.FireVisual)stream.ReadByte();
		this.m_pos.x = stream.ReadSingle();
		this.m_pos.y = stream.ReadSingle();
		this.m_pos.z = stream.ReadSingle();
		this.m_haveFacing = stream.ReadBoolean();
		this.m_facing.x = stream.ReadSingle();
		this.m_facing.y = stream.ReadSingle();
		this.m_facing.z = stream.ReadSingle();
		this.m_displayRadius = stream.ReadSingle();
		this.m_blockedLOS = stream.ReadBoolean();
		this.m_inFiringCone = stream.ReadBoolean();
		this.m_staticTargetOnly = stream.ReadBoolean();
		this.m_reachedPosition = stream.ReadBoolean();
		this.m_targetNetID = stream.ReadInt32();
		if (this.m_type == Order.Type.MoveRotate)
		{
			Unit unit = this.GetOwner() as Unit;
			if (unit != null)
			{
				this.m_pos = unit.transform.position;
			}
		}
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x000540E8 File Offset: 0x000522E8
	public void SetTarget(Vector3 pos)
	{
		this.m_targetNetID = 0;
		this.m_pos = pos;
		this.m_reachedPosition = false;
		this.UpdateMarker();
		this.OnChanged();
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0005410C File Offset: 0x0005230C
	public void SetTarget(int unitID, Vector3 localPos)
	{
		this.m_targetNetID = unitID;
		this.m_pos = localPos;
		this.m_reachedPosition = false;
		this.UpdateMarker();
		this.OnChanged();
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x00054130 File Offset: 0x00052330
	private void UpdateMarker()
	{
		if (this.m_marker != null)
		{
			this.m_marker.GetComponent<OrderMarker>().OnPositionChanged();
		}
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00054154 File Offset: 0x00052354
	public bool IsStaticTarget()
	{
		return this.m_targetNetID == 0;
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x00054160 File Offset: 0x00052360
	public bool IsLOSBlocked()
	{
		return this.m_blockedLOS;
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x00054168 File Offset: 0x00052368
	public bool IsInFiringCone()
	{
		return this.m_inFiringCone;
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00054170 File Offset: 0x00052370
	public void SetInFiringCone(bool inCone)
	{
		this.m_inFiringCone = inCone;
		if (this.m_marker != null)
		{
			this.m_marker.GetComponent<OrderMarker>().OnInFiringConeChanged();
		}
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x000541A8 File Offset: 0x000523A8
	public void SetLOSBlocked(bool blocked)
	{
		this.m_blockedLOS = blocked;
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x000541B4 File Offset: 0x000523B4
	public NetObj GetTargetObj()
	{
		if (this.m_targetNetID == 0)
		{
			return null;
		}
		return NetObj.GetByID(this.m_targetNetID);
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x000541D0 File Offset: 0x000523D0
	public int GetTargetID()
	{
		return this.m_targetNetID;
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x000541D8 File Offset: 0x000523D8
	public void SetFacing(Vector3 facing)
	{
		if (facing.magnitude < 0.01f)
		{
			return;
		}
		facing.y = 0f;
		facing.Normalize();
		this.m_haveFacing = true;
		this.m_facing = facing;
		if (this.m_marker != null)
		{
			this.m_marker.GetComponent<OrderMarker>().UpdateModel();
		}
		this.OnChanged();
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00054240 File Offset: 0x00052440
	public void ResetFacing()
	{
		this.m_haveFacing = false;
		if (this.m_marker != null)
		{
			this.m_marker.GetComponent<OrderMarker>().UpdateModel();
		}
		this.OnChanged();
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0005427C File Offset: 0x0005247C
	public bool HaveFacing()
	{
		return this.m_haveFacing;
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x00054284 File Offset: 0x00052484
	public Vector3 GetFacing()
	{
		return this.m_facing;
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0005428C File Offset: 0x0005248C
	public IOrderable GetOwner()
	{
		return this.m_owner;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x00054294 File Offset: 0x00052494
	public GameObject GetMarker()
	{
		return this.m_marker;
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0005429C File Offset: 0x0005249C
	private void OnChanged()
	{
		this.m_owner.OnOrdersChanged();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x000542AC File Offset: 0x000524AC
	public void SetDisplayRadius(float radius)
	{
		this.m_displayRadius = radius;
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x000542B8 File Offset: 0x000524B8
	public float GetDisplayRadius()
	{
		return this.m_displayRadius;
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x000542C0 File Offset: 0x000524C0
	public void SetStaticTargetOnly(bool enabled)
	{
		this.m_staticTargetOnly = enabled;
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x000542CC File Offset: 0x000524CC
	public bool GetStaticTargetOnly()
	{
		return this.m_staticTargetOnly;
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x000542D4 File Offset: 0x000524D4
	public bool HasReachedPosition()
	{
		return (this.m_type != Order.Type.MoveForward && this.m_type != Order.Type.MoveBackward) || this.m_reachedPosition;
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x00054304 File Offset: 0x00052504
	public void SetReachedPosition(bool reached)
	{
		this.m_reachedPosition = reached;
	}

	// Token: 0x0400096E RID: 2414
	public GameObject m_marker;

	// Token: 0x0400096F RID: 2415
	public Order.Type m_type;

	// Token: 0x04000970 RID: 2416
	public Order.FireVisual m_fireVisual;

	// Token: 0x04000971 RID: 2417
	private IOrderable m_owner;

	// Token: 0x04000972 RID: 2418
	private Vector3 m_pos = new Vector3(0f, 0f, 0f);

	// Token: 0x04000973 RID: 2419
	private int m_targetNetID;

	// Token: 0x04000974 RID: 2420
	private bool m_reachedPosition;

	// Token: 0x04000975 RID: 2421
	private bool m_haveFacing;

	// Token: 0x04000976 RID: 2422
	private Vector3 m_facing;

	// Token: 0x04000977 RID: 2423
	private float m_displayRadius;

	// Token: 0x04000978 RID: 2424
	private bool m_staticTargetOnly;

	// Token: 0x04000979 RID: 2425
	private bool m_blockedLOS;

	// Token: 0x0400097A RID: 2426
	private bool m_inFiringCone = true;

	// Token: 0x02000127 RID: 295
	public enum Type
	{
		// Token: 0x0400097C RID: 2428
		None,
		// Token: 0x0400097D RID: 2429
		MoveForward,
		// Token: 0x0400097E RID: 2430
		MoveBackward,
		// Token: 0x0400097F RID: 2431
		MoveRotate,
		// Token: 0x04000980 RID: 2432
		Fire
	}

	// Token: 0x02000128 RID: 296
	public enum FireVisual
	{
		// Token: 0x04000982 RID: 2434
		Point,
		// Token: 0x04000983 RID: 2435
		Line,
		// Token: 0x04000984 RID: 2436
		Area
	}
}
