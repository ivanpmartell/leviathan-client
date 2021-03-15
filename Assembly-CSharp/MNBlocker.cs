using System;
using System.IO;
using UnityEngine;

// Token: 0x02000077 RID: 119
[AddComponentMenu("Scripts/Mission/MNBlocker")]
public class MNBlocker : MNRepeater
{
	// Token: 0x06000515 RID: 1301 RVA: 0x0002A4A0 File Offset: 0x000286A0
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0002A4A8 File Offset: 0x000286A8
	protected override void Destroy()
	{
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0002A4AC File Offset: 0x000286AC
	protected override void Update()
	{
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0002A4B0 File Offset: 0x000286B0
	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0002A4B8 File Offset: 0x000286B8
	public override void DoAction()
	{
		for (int i = 0; i < this.m_repeatTargets.Length; i++)
		{
			GameObject targetObj = base.GetTargetObj(i);
			if (targetObj != null)
			{
				targetObj.SetActiveRecursively(false);
			}
		}
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0002A4FC File Offset: 0x000286FC
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0002A508 File Offset: 0x00028708
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
	}
}
