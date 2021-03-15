using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
internal class MoveButton
{
	// Token: 0x060002C6 RID: 710 RVA: 0x00014FC4 File Offset: 0x000131C4
	public MoveButton(MoveButton.MoveType type, GameObject guiCamera, EZDragDropDelegate onDraged)
	{
		this.m_moveType = type;
		switch (type)
		{
		case MoveButton.MoveType.Forward:
			this.m_button = GuiUtils.CreateGui("IngameGui/FlowerButtonForward", guiCamera);
			break;
		case MoveButton.MoveType.Reverse:
			this.m_button = GuiUtils.CreateGui("IngameGui/FlowerButtonReverse", guiCamera);
			break;
		case MoveButton.MoveType.Rotate:
			this.m_button = GuiUtils.CreateGui("IngameGui/FlowerButtonRotate", guiCamera);
			break;
		}
		this.m_button.GetComponent<UIButton>().SetDragDropDelegate(onDraged);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0001504C File Offset: 0x0001324C
	public bool MouseOver()
	{
		return GuiUtils.HasPointerRecursive(UIManager.instance, this.m_button);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00015060 File Offset: 0x00013260
	public void UpdatePosition(float guiScale, Ship ship, Camera guiCamera, Camera gameCamera, ref float lowestScreenPos)
	{
		float num = Mathf.Clamp(guiScale, 1f, FlowerMenu.m_maxGuiScale);
		float d = (ship.GetLength() / 2f + 6f) * num;
		Vector3 vector = ship.transform.position + new Vector3(0f, ship.m_deckHeight, 0f);
		Vector3 pos = Vector3.zero;
		switch (this.m_moveType)
		{
		case MoveButton.MoveType.Forward:
			pos = vector + ship.transform.forward * d;
			break;
		case MoveButton.MoveType.Reverse:
			pos = vector - ship.transform.forward * d;
			break;
		case MoveButton.MoveType.Rotate:
			pos = vector + ship.transform.forward * d - ship.transform.forward * num * 7f;
			break;
		}
		Vector3 b = GuiUtils.WorldToGuiPos(gameCamera, guiCamera, vector);
		Vector3 vector2 = GuiUtils.WorldToGuiPos(gameCamera, guiCamera, pos);
		this.m_button.transform.position = vector2;
		if (this.m_moveType == MoveButton.MoveType.Forward || this.m_moveType == MoveButton.MoveType.Reverse)
		{
			Vector3 normalized = (vector2 - b).normalized;
			Quaternion localRotation = Quaternion.LookRotation(normalized, new Vector3(0f, 0f, -1f));
			this.m_button.transform.localRotation = localRotation;
		}
		if (this.m_button.transform.position.y - 19f < lowestScreenPos)
		{
			lowestScreenPos = this.m_button.transform.position.y - 19f;
		}
	}

	// Token: 0x040001FA RID: 506
	public MoveButton.MoveType m_moveType;

	// Token: 0x040001FB RID: 507
	public GameObject m_button;

	// Token: 0x0200003F RID: 63
	public enum MoveType
	{
		// Token: 0x040001FD RID: 509
		Forward,
		// Token: 0x040001FE RID: 510
		Reverse,
		// Token: 0x040001FF RID: 511
		Rotate
	}
}
