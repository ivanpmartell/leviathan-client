using System;
using System.IO;

// Token: 0x02000125 RID: 293
public interface IOrderable
{
	// Token: 0x06000B89 RID: 2953
	void SaveOrders(BinaryWriter stream);

	// Token: 0x06000B8A RID: 2954
	void LoadOrders(BinaryReader stream);

	// Token: 0x06000B8B RID: 2955
	void AddOrder(Order order);

	// Token: 0x06000B8C RID: 2956
	bool RemoveOrder(Order order);

	// Token: 0x06000B8D RID: 2957
	bool RemoveFirstOrder();

	// Token: 0x06000B8E RID: 2958
	void ClearOrders();

	// Token: 0x06000B8F RID: 2959
	bool RemoveLastOrder();

	// Token: 0x06000B90 RID: 2960
	bool IsLastOrder(Order order);

	// Token: 0x06000B91 RID: 2961
	void OnOrdersChanged();
}
