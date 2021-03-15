using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class Battery : MonoBehaviour
{
	// Token: 0x06000709 RID: 1801 RVA: 0x00035394 File Offset: 0x00033594
	private void Awake()
	{
		this.SetSize(this.m_width, this.m_length);
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000353A8 File Offset: 0x000335A8
	public void SetSize(int width, int length)
	{
		this.m_width = width;
		this.m_length = length;
		this.m_modules = new HPModule[this.m_width * this.m_length];
		Transform child = base.transform.GetChild(0);
		child.localScale = new Vector3((float)this.m_width, child.localScale.y, (float)this.m_length);
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00035410 File Offset: 0x00033610
	public int GetWidth()
	{
		return this.m_width;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00035418 File Offset: 0x00033618
	public int GetLength()
	{
		return this.m_length;
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00035420 File Offset: 0x00033620
	public int GetSlots()
	{
		return this.m_width * this.m_length;
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00035430 File Offset: 0x00033630
	public void Setup(Unit unit, int orderNumber)
	{
		this.m_unit = unit;
		this.m_orderNumber = orderNumber;
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00035440 File Offset: 0x00033640
	public void SetOwner(int owner)
	{
		this.m_owner = owner;
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			hpmodule.SetOwner(owner);
		}
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00035484 File Offset: 0x00033684
	public Section.SectionType GetSectionType()
	{
		Section component = base.transform.parent.gameObject.GetComponent<Section>();
		return component.GetSectionType();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x000354B0 File Offset: 0x000336B0
	public Section GetSection()
	{
		return base.transform.parent.gameObject.GetComponent<Section>();
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000354D4 File Offset: 0x000336D4
	public void FillUsableBuffer(bool[] usable, int px, int py, int w, int h)
	{
		if (px < 0 || px + w > this.m_width || py < 0 || py + h > this.m_length)
		{
			return;
		}
		for (int i = py; i < py + h; i++)
		{
			for (int j = px; j < px + w; j++)
			{
				usable[i * this.m_width + j] = true;
			}
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00035548 File Offset: 0x00033748
	public void GetFitTiles(out List<Vector3> points, int w, int h, bool fillall)
	{
		points = new List<Vector3>();
		bool[] array = new bool[this.m_length * this.m_width];
		for (int i = 0; i < this.m_length; i++)
		{
			for (int j = 0; j < this.m_width; j++)
			{
				if (this.CanPlaceAt(j, i, w, h, null))
				{
					array[i * this.m_width + j] = true;
					if (fillall)
					{
						this.FillUsableBuffer(array, j, i, w, h);
					}
				}
				if (array[i * this.m_width + j])
				{
					Vector3 vector = this.GetTileTopLeft(j, i) + new Vector3(0.5f, 0f, 0.5f);
					vector = base.transform.TransformPoint(vector);
					points.Add(vector);
				}
			}
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00035614 File Offset: 0x00033814
	public void GetBestFitTiles(out List<Vector3> points, Vector3 pos, int w, int h)
	{
		points = new List<Vector3>();
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x00035620 File Offset: 0x00033820
	public void GetUnusedTiles(out List<Vector3> points)
	{
		points = new List<Vector3>();
		for (int i = 0; i < this.m_length; i++)
		{
			for (int j = 0; j < this.m_width; j++)
			{
				if (this.m_modules[i * this.m_width + j] == null)
				{
					Vector3 vector = this.GetTileTopLeft(j, i) + new Vector3(0.5f, 0f, 0.5f);
					vector = base.transform.TransformPoint(vector);
					points.Add(vector);
				}
			}
		}
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x000356B8 File Offset: 0x000338B8
	public Vector3 GetModulePosition(int x, int y, int w, int h)
	{
		return this.GetTileTopLeft(x, y) + new Vector3((float)w * 1f * 0.5f, 0f, (float)h * 1f * 0.5f);
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x000356FC File Offset: 0x000338FC
	public static Quaternion GetRotation(Direction dir)
	{
		Quaternion result = Quaternion.Euler(0f, 0f, 0f);
		switch (dir)
		{
		case Direction.Right:
			result = Quaternion.Euler(0f, 90f, 0f);
			break;
		case Direction.Backward:
			result = Quaternion.Euler(0f, 180f, 0f);
			break;
		case Direction.Left:
			result = Quaternion.Euler(0f, 270f, 0f);
			break;
		}
		return result;
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00035790 File Offset: 0x00033990
	public HPModule AddHPModule(string name, int x, int y, Direction dir)
	{
		GameObject gameObject = ObjectFactory.instance.Create(name);
		if (gameObject == null)
		{
			PLog.LogError("Failed to AddHPModule " + name);
		}
		HPModule component = gameObject.GetComponent<HPModule>();
		component.SetDir(dir);
		if (!this.AllowedModule(component))
		{
			PLog.LogError("Error AddHPModule. Module " + name + " not allowed in battery on ship " + this.m_unit.GetName());
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		if (!this.CanPlaceAt(x, y, component.GetWidth(), component.GetLength(), null))
		{
			string text = string.Concat(new string[]
			{
				"<",
				x.ToString(),
				",",
				y.ToString(),
				">"
			});
			PLog.LogError(string.Concat(new string[]
			{
				"Error AddHPModule. Tried to place ",
				name,
				" outside battery at ",
				text,
				" on ship ",
				this.m_unit.GetName()
			}));
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = this.GetLocalPlacePos(x, y, component);
		gameObject.transform.localRotation = Quaternion.identity;
		component.Setup(this.m_unit, this, x, y, dir, new HPModule.DestroyedHandler(this.OnModuleDestroyed));
		component.SetOwner(this.m_owner);
		this.AddModuleToGrid(component);
		return component;
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00035908 File Offset: 0x00033B08
	private Vector3 GetLocalPlacePos(int x, int y, HPModule module)
	{
		return this.GetTileTopLeft(x, y) + new Vector3((float)module.GetWidth() * 1f * 0.5f, 0f, (float)module.GetLength() * 1f * 0.5f);
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00035954 File Offset: 0x00033B54
	public Vector3 GetWorldPlacePos(int x, int y, HPModule module)
	{
		Vector3 localPlacePos = this.GetLocalPlacePos(x, y, module);
		return base.transform.TransformPoint(localPlacePos);
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00035978 File Offset: 0x00033B78
	public void GetAllHPModules(ref List<HPModule> modules)
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule item in componentsInChildren)
		{
			modules.Add(item);
		}
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x000359B4 File Offset: 0x00033BB4
	public bool AllowedModule(HPModule module)
	{
		HPModule.HPModuleType type = module.m_type;
		if (type != HPModule.HPModuleType.Offensive)
		{
			return type == HPModule.HPModuleType.Defensive && this.m_allowDefensive;
		}
		return this.m_allowOffensive;
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x000359EC File Offset: 0x00033BEC
	private void OnModuleDestroyed(HPModule module)
	{
		this.RemoveModuleFromGrid(module);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x000359F8 File Offset: 0x00033BF8
	private void RemoveModuleFromGrid(HPModule module)
	{
		for (int i = 0; i < this.m_modules.Length; i++)
		{
			if (this.m_modules[i] == module)
			{
				this.m_modules[i] = null;
			}
		}
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00035A3C File Offset: 0x00033C3C
	private void AddModuleToGrid(HPModule module)
	{
		Vector2i gridPos = module.GetGridPos();
		for (int i = 0; i < module.GetLength(); i++)
		{
			for (int j = 0; j < module.GetWidth(); j++)
			{
				this.m_modules[(gridPos.y + i) * this.m_width + (gridPos.x + j)] = module;
			}
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00035AA0 File Offset: 0x00033CA0
	public bool CanPlaceAt(int px, int py, int w, int h, HPModule ignoreModule)
	{
		if (px < 0 || px + w > this.m_width || py < 0 || py + h > this.m_length)
		{
			return false;
		}
		for (int i = py; i < py + h; i++)
		{
			for (int j = px; j < px + w; j++)
			{
				HPModule x = this.m_modules[i * this.m_width + j];
				if (x != null)
				{
					if (!(x == ignoreModule))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00035B38 File Offset: 0x00033D38
	public HPModule GetModuleAt(int x, int y)
	{
		return this.m_modules[y * this.m_width + x];
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00035B4C File Offset: 0x00033D4C
	public void SaveState(BinaryWriter writer)
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		writer.Write((byte)componentsInChildren.Length);
		foreach (HPModule hpmodule in componentsInChildren)
		{
			writer.Write(hpmodule.gameObject.name);
			writer.Write((byte)hpmodule.GetGridPos().x);
			writer.Write((byte)hpmodule.GetGridPos().y);
			writer.Write((byte)hpmodule.GetDir());
			hpmodule.SaveState(writer);
		}
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00035BDC File Offset: 0x00033DDC
	public void LoadState(BinaryReader reader)
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			UnityEngine.Object.DestroyImmediate(hpmodule.gameObject);
		}
		int num = (int)reader.ReadByte();
		for (int j = 0; j < num; j++)
		{
			string name = reader.ReadString();
			int x = (int)reader.ReadByte();
			int y = (int)reader.ReadByte();
			Direction dir = (Direction)reader.ReadByte();
			HPModule hpmodule2 = this.AddHPModule(name, x, y, dir);
			hpmodule2.LoadState(reader);
		}
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00035C74 File Offset: 0x00033E74
	public void SaveOrders(BinaryWriter writer)
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		writer.Write(componentsInChildren.Length);
		foreach (HPModule hpmodule in componentsInChildren)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter stream = new BinaryWriter(memoryStream);
			hpmodule.SaveOrders(stream);
			byte[] array2 = memoryStream.ToArray();
			writer.Write(hpmodule.GetNetID());
			writer.Write(array2.Length);
			writer.Write(array2);
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00035CF0 File Offset: 0x00033EF0
	public void LoadOrders(BinaryReader reader)
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = reader.ReadInt32();
			int count = reader.ReadInt32();
			byte[] buffer = reader.ReadBytes(count);
			foreach (HPModule hpmodule in componentsInChildren)
			{
				if (hpmodule.GetNetID() == num2)
				{
					MemoryStream input = new MemoryStream(buffer);
					BinaryReader stream = new BinaryReader(input);
					hpmodule.LoadOrders(stream);
					break;
				}
			}
		}
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00035D8C File Offset: 0x00033F8C
	public void ClearOrders()
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			hpmodule.ClearOrders();
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00035DC8 File Offset: 0x00033FC8
	public void ClearGunOrdersAndTargets()
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			Gun gun = hpmodule as Gun;
			if (gun != null)
			{
				gun.SetTarget(null);
				gun.ClearOrders();
			}
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00035E20 File Offset: 0x00034020
	public int GetOrderNumber()
	{
		return this.m_orderNumber;
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00035E28 File Offset: 0x00034028
	public void SetVisible(bool visible)
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			hpmodule.SetVisible(visible);
		}
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00035E64 File Offset: 0x00034064
	public void DestroyAll()
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			hpmodule.TimedDestruction(PRand.Range(0.4f, 3f));
		}
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00035EAC File Offset: 0x000340AC
	public void RemoveAll()
	{
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			hpmodule.Remove();
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00035EE8 File Offset: 0x000340E8
	public int GetTotalValue()
	{
		int num = 0;
		HPModule[] componentsInChildren = base.transform.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			num += hpmodule.GetTotalValue();
		}
		return num;
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00035F2C File Offset: 0x0003412C
	private Vector3 GetTileTopLeft(int x, int y)
	{
		return new Vector3(((float)(-(float)this.m_width) / 2f + (float)x) * 1f, 0f, (float)(-(float)this.m_length) / 2f + (float)y) * 1f;
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00035F78 File Offset: 0x00034178
	public bool WorldToTile(Vector3 worldPos, out int x, out int y)
	{
		Vector3 vector = base.transform.InverseTransformPoint(worldPos);
		vector.x += (float)this.m_width / 2f * 1f;
		vector.z += (float)this.m_length / 2f * 1f;
		x = (int)(vector.x / 1f);
		y = (int)(vector.z / 1f);
		return x >= 0 && y >= 0 && x < this.m_width && y < this.m_length;
	}

	// Token: 0x040005E4 RID: 1508
	private const float m_tileSize = 1f;

	// Token: 0x040005E5 RID: 1509
	private Unit m_unit;

	// Token: 0x040005E6 RID: 1510
	private int m_owner = -1;

	// Token: 0x040005E7 RID: 1511
	private int m_orderNumber;

	// Token: 0x040005E8 RID: 1512
	private HPModule[] m_modules;

	// Token: 0x040005E9 RID: 1513
	public int m_width = 1;

	// Token: 0x040005EA RID: 1514
	public int m_length = 1;

	// Token: 0x040005EB RID: 1515
	public bool m_allowOffensive = true;

	// Token: 0x040005EC RID: 1516
	public bool m_allowDefensive = true;
}
