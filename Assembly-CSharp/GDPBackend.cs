using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000180 RID: 384
public abstract class GDPBackend
{
	// Token: 0x06000E50 RID: 3664 RVA: 0x0006503C File Offset: 0x0006323C
	public void UnlockAchievement(int id)
	{
		this.UnlockAchievement("achievement_" + id.ToString());
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x00065058 File Offset: 0x00063258
	protected void AddPacks(ref List<GDPBackend.GDPOwnedItem> items, string[] packNames)
	{
		foreach (string packName in packNames)
		{
			GDPBackend.GDPOwnedItem gdpownedItem = new GDPBackend.GDPOwnedItem();
			gdpownedItem.m_itemType = string.Empty;
			gdpownedItem.m_instance = 0;
			gdpownedItem.m_packName = packName;
			items.Add(gdpownedItem);
		}
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x000650A8 File Offset: 0x000632A8
	public virtual void Close()
	{
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x000650AC File Offset: 0x000632AC
	public virtual void Update()
	{
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x000650B0 File Offset: 0x000632B0
	public virtual void RestoreOwned()
	{
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x000650B4 File Offset: 0x000632B4
	public virtual bool CanPlaceOrders()
	{
		return true;
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x000650B8 File Offset: 0x000632B8
	public virtual bool IsBackendOnline()
	{
		return true;
	}

	// Token: 0x06000E57 RID: 3671
	public abstract void RequestOffers();

	// Token: 0x06000E58 RID: 3672
	public abstract List<GDPBackend.GDPOwnedItem> RequestOwned();

	// Token: 0x06000E59 RID: 3673
	public abstract void PlaceOrder(GDPBackend.GDPShopItem item, string description);

	// Token: 0x06000E5A RID: 3674 RVA: 0x000650BC File Offset: 0x000632BC
	public virtual void UnlockAchievement(string name)
	{
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x000650C0 File Offset: 0x000632C0
	public virtual void RedeemCode(string code)
	{
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x000650C4 File Offset: 0x000632C4
	public virtual void OpenWebUrl(string url)
	{
		Application.OpenURL(url);
	}

	// Token: 0x04000B73 RID: 2931
	public Action<string> m_onBoughtItem;

	// Token: 0x04000B74 RID: 2932
	public Action<bool, string> m_onRedeemRespons;

	// Token: 0x04000B75 RID: 2933
	public Action<string> m_onOrderFailed;

	// Token: 0x04000B76 RID: 2934
	public Action<bool, string> m_onRestoreOwnedFinished;

	// Token: 0x04000B77 RID: 2935
	public Action<List<GDPBackend.GDPShopItem>> m_onGotOfferList;

	// Token: 0x02000181 RID: 385
	public class GDPShopItem
	{
		// Token: 0x04000B78 RID: 2936
		public int m_id;

		// Token: 0x04000B79 RID: 2937
		public string m_presentationKey = string.Empty;

		// Token: 0x04000B7A RID: 2938
		public string m_price = string.Empty;

		// Token: 0x04000B7B RID: 2939
		public bool m_discounted;

		// Token: 0x04000B7C RID: 2940
		public double m_discountPercentage;

		// Token: 0x04000B7D RID: 2941
		public string m_undiscountedPrice = string.Empty;

		// Token: 0x04000B7E RID: 2942
		public int m_dateYear;

		// Token: 0x04000B7F RID: 2943
		public int m_dateMonth;

		// Token: 0x04000B80 RID: 2944
		public int m_dateDay;

		// Token: 0x04000B81 RID: 2945
		public string m_packName;
	}

	// Token: 0x02000182 RID: 386
	public class GDPOwnedItem : IEquatable<GDPBackend.GDPOwnedItem>
	{
		// Token: 0x06000E5F RID: 3679 RVA: 0x00065118 File Offset: 0x00063318
		public bool Equals(GDPBackend.GDPOwnedItem other)
		{
			return this.m_itemType == other.m_itemType && this.m_packName == other.m_packName;
		}

		// Token: 0x04000B82 RID: 2946
		public string m_itemType = string.Empty;

		// Token: 0x04000B83 RID: 2947
		public int m_instance;

		// Token: 0x04000B84 RID: 2948
		public string m_packName = string.Empty;
	}
}
