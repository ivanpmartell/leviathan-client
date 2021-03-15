using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200008E RID: 142
[AddComponentMenu("Scripts/Mission/MNode")]
public class MNode : NetObj
{
	// Token: 0x06000590 RID: 1424 RVA: 0x0002CAAC File Offset: 0x0002ACAC
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0002CAB4 File Offset: 0x0002ACB4
	public virtual void DoAction()
	{
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0002CAB8 File Offset: 0x0002ACB8
	public virtual void OnEvent(string eventName)
	{
		this.EventWarning(eventName);
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0002CAC4 File Offset: 0x0002ACC4
	public void EventWarning(string eventName)
	{
		if (Application.isEditor)
		{
			string text = string.Concat(new string[]
			{
				base.name,
				"(",
				base.GetNetID().ToString(),
				") of type ",
				base.GetType().ToString()
			});
			string text2 = "Recived event '" + eventName + "' that it do not care about.";
			MessageLog.instance.ShowMessage(MessageLog.TextPosition.Bottom, text, text2, string.Empty, 2f);
			PLog.Log(text + " " + text2);
		}
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0002CB58 File Offset: 0x0002AD58
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(base.transform.rotation.x);
		writer.Write(base.transform.rotation.y);
		writer.Write(base.transform.rotation.z);
		writer.Write(base.transform.rotation.w);
		writer.Write(base.transform.localScale.x);
		writer.Write(base.transform.localScale.y);
		writer.Write(base.transform.localScale.z);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0002CC6C File Offset: 0x0002AE6C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
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
		Vector3 localScale = default(Vector3);
		localScale.x = reader.ReadSingle();
		localScale.y = reader.ReadSingle();
		localScale.z = reader.ReadSingle();
		base.transform.localScale = localScale;
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0002CD40 File Offset: 0x0002AF40
	public static List<GameObject> GetTargets(GameObject target)
	{
		List<GameObject> list = new List<GameObject>();
		MNRepeater component = target.GetComponent<MNRepeater>();
		if (component)
		{
			for (int i = 0; i < component.m_repeatTargets.Length; i++)
			{
				GameObject targetObj = component.GetTargetObj(i);
				if (targetObj != null)
				{
					list.Add(targetObj);
				}
			}
			return list;
		}
		list.Add(target);
		return list;
	}
}
