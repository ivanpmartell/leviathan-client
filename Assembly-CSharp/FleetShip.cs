using System;
using PTech;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class FleetShip
{
	// Token: 0x06000262 RID: 610 RVA: 0x00011170 File Offset: 0x0000F370
	public void Destroy()
	{
		if (this.m_ship)
		{
			UnityEngine.Object.Destroy(this.m_ship);
		}
		if (this.m_floatingInfo)
		{
			UnityEngine.Object.Destroy(this.m_floatingInfo);
		}
		this.m_ship = null;
		this.m_floatingInfo = null;
	}

	// Token: 0x06000263 RID: 611 RVA: 0x000111C4 File Offset: 0x0000F3C4
	public void Update(GameObject sceneCamera, GameObject guiCamera)
	{
		if (this.m_floatingInfo == null)
		{
			return;
		}
		Vector3 position = sceneCamera.camera.WorldToScreenPoint(this.m_basePosition);
		Vector3 position2 = guiCamera.camera.ScreenToWorldPoint(position);
		position2.z = -2f;
		this.m_floatingInfo.transform.position = position2;
		GuiUtils.FindChildOf(this.m_floatingInfo, "FloatingInfoCostLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(this.m_cost.ToString() + " $label_pointssmall");
		GuiUtils.FindChildOf(this.m_floatingInfo, "FloatingInfoNameLabel").GetComponent<SpriteText>().Text = this.m_definition.m_name;
		string text = string.Concat(new string[]
		{
			this.m_definition.NumberOfHardpoints().ToString(),
			"/",
			this.m_maxHardpoints.ToString(),
			" ",
			Localize.instance.Translate("$arms")
		});
		GuiUtils.FindChildOf(this.m_floatingInfo, "FloatingInfoArmsLabel").GetComponent<SpriteText>().Text = text;
	}

	// Token: 0x040001C2 RID: 450
	public Vector3 m_basePosition = default(Vector3);

	// Token: 0x040001C3 RID: 451
	public Vector3 m_shipPosition = default(Vector3);

	// Token: 0x040001C4 RID: 452
	public int m_cost;

	// Token: 0x040001C5 RID: 453
	public ShipDef m_definition;

	// Token: 0x040001C6 RID: 454
	public string m_name = "Undefined";

	// Token: 0x040001C7 RID: 455
	public GameObject m_ship;

	// Token: 0x040001C8 RID: 456
	public GameObject m_floatingInfo;

	// Token: 0x040001C9 RID: 457
	public float m_width;

	// Token: 0x040001CA RID: 458
	public float m_length;

	// Token: 0x040001CB RID: 459
	public int m_maxHardpoints;
}
