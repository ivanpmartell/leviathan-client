using System;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class OrderMarker : MonoBehaviour
{
	// Token: 0x06000918 RID: 2328 RVA: 0x00041E2C File Offset: 0x0004002C
	private void Start()
	{
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00041E30 File Offset: 0x00040030
	public void Setup(Order order)
	{
		this.m_order = order;
		this.UpdateModel();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00041E40 File Offset: 0x00040040
	public void Update()
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		float displayRadius = this.m_order.GetDisplayRadius();
		if (displayRadius != 0f)
		{
			LineDrawer component = main.GetComponent<LineDrawer>();
			if (component != null)
			{
				if (this.m_lineID == -1)
				{
					this.m_lineID = component.GetTypeID("attackArea");
				}
				component.DrawXZCircle(base.transform.position + new Vector3(0f, 3f, 0f), displayRadius, 40, this.m_lineID, 0.15f);
			}
		}
		float num = Vector3.Distance(main.transform.position, base.transform.position);
		float num2 = Mathf.Tan(0.017453292f * main.fieldOfView * 0.5f) * 0.04f * num;
		if (this.m_order.m_type != Order.Type.MoveRotate)
		{
			num2 *= 0.8f;
		}
		base.transform.localScale = new Vector3(num2, num2, num2);
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00041F50 File Offset: 0x00040150
	public void UpdateModel()
	{
		this.m_lookMarker.SetActiveRecursively(false);
		this.m_selectionMarker.SetActiveRecursively(false);
		this.m_moveForwardMarker.SetActiveRecursively(false);
		this.m_moveReverseMarker.SetActiveRecursively(false);
		this.m_firePointMarker.SetActiveRecursively(false);
		this.m_fireAreaMarker.SetActiveRecursively(false);
		this.m_fireLineMarker.SetActiveRecursively(false);
		if (this.m_order.m_type == Order.Type.MoveForward || this.m_order.m_type == Order.Type.MoveBackward || this.m_order.m_type == Order.Type.MoveRotate)
		{
			if (this.m_order.m_type == Order.Type.MoveForward)
			{
				this.m_moveForwardMarker.SetActiveRecursively(true);
			}
			else if (this.m_order.m_type == Order.Type.MoveBackward)
			{
				this.m_moveReverseMarker.SetActiveRecursively(true);
			}
			if (this.m_order.HaveFacing())
			{
				this.m_lookMarker.SetActiveRecursively(true);
				this.m_lookMarker.transform.rotation = Quaternion.LookRotation(this.m_order.GetFacing(), new Vector3(0f, 1f, 0f));
			}
		}
		else if (this.m_order.m_type == Order.Type.Fire)
		{
			switch (this.m_order.m_fireVisual)
			{
			case Order.FireVisual.Point:
				this.m_firePointMarker.SetActiveRecursively(true);
				this.m_activeMarker = this.m_firePointMarker.renderer;
				break;
			case Order.FireVisual.Line:
				this.m_fireLineMarker.SetActiveRecursively(true);
				this.UpdateFireLineMarker();
				this.m_activeMarker = this.m_fireLineMarker.renderer;
				break;
			case Order.FireVisual.Area:
				this.m_fireAreaMarker.SetActiveRecursively(true);
				this.m_activeMarker = this.m_fireAreaMarker.renderer;
				break;
			}
		}
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00042118 File Offset: 0x00040318
	public void OnInFiringConeChanged()
	{
		this.UpdateMaterial();
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00042120 File Offset: 0x00040320
	private void UpdateMaterial()
	{
		if (this.m_activeMarker == null)
		{
			return;
		}
		bool flag = this.m_order.IsInFiringCone();
		if (flag)
		{
			if (this.m_originalMaterial != null)
			{
				this.m_activeMarker.materials = this.m_originalMaterial;
				this.m_originalMaterial = null;
			}
		}
		else if (this.m_originalMaterial == null)
		{
			this.m_originalMaterial = this.m_activeMarker.sharedMaterials;
			this.m_activeMarker.materials[0].color = Color.red;
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x000421AC File Offset: 0x000403AC
	private void UpdateFireLineMarker()
	{
		NetObj netObj = this.m_order.GetOwner() as NetObj;
		Vector3 normalized = (base.transform.position - netObj.transform.position).normalized;
		this.m_fireLineMarker.transform.rotation = Quaternion.LookRotation(normalized, new Vector3(0f, 1f, 0f));
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00042218 File Offset: 0x00040418
	public Order GetOrder()
	{
		return this.m_order;
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00042220 File Offset: 0x00040420
	public void SetSelected(bool selected)
	{
		this.m_selectionMarker.SetActiveRecursively(selected);
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00042230 File Offset: 0x00040430
	public void OnPositionChanged()
	{
		base.transform.position = this.m_order.GetPos();
		if (this.m_order.m_type == Order.Type.Fire && this.m_order.m_fireVisual == Order.FireVisual.Line)
		{
			this.UpdateFireLineMarker();
		}
	}

	// Token: 0x0400074C RID: 1868
	public GameObject m_selectionMarker;

	// Token: 0x0400074D RID: 1869
	public GameObject m_moveForwardMarker;

	// Token: 0x0400074E RID: 1870
	public GameObject m_moveReverseMarker;

	// Token: 0x0400074F RID: 1871
	public GameObject m_firePointMarker;

	// Token: 0x04000750 RID: 1872
	public GameObject m_fireAreaMarker;

	// Token: 0x04000751 RID: 1873
	public GameObject m_fireLineMarker;

	// Token: 0x04000752 RID: 1874
	public GameObject m_lookMarker;

	// Token: 0x04000753 RID: 1875
	private Material[] m_originalMaterial;

	// Token: 0x04000754 RID: 1876
	private Renderer m_activeMarker;

	// Token: 0x04000755 RID: 1877
	private Order m_order;

	// Token: 0x04000756 RID: 1878
	private int m_lineID = -1;
}
