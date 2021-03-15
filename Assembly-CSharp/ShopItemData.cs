using System;
using PTech;

// Token: 0x02000067 RID: 103
public class ShopItemData
{
	// Token: 0x06000485 RID: 1157 RVA: 0x00027090 File Offset: 0x00025290
	public ShopItemData(GDPBackend.GDPShopItem item, ContentPack.Category type, string name, bool owned, string price, string undiscountedPrice, double discountPercentage, bool newItem)
	{
		this.m_gdpItem = item;
		this.m_type = type;
		this.m_name = name;
		this.m_price = price;
		this.m_undiscountedPrice = undiscountedPrice;
		this.m_owned = owned;
		this.m_newItem = newItem;
		this.m_discountPercentage = discountPercentage;
	}

	// Token: 0x040003DB RID: 987
	public GDPBackend.GDPShopItem m_gdpItem;

	// Token: 0x040003DC RID: 988
	public ContentPack.Category m_type;

	// Token: 0x040003DD RID: 989
	public string m_name;

	// Token: 0x040003DE RID: 990
	public bool m_owned;

	// Token: 0x040003DF RID: 991
	public string m_price = string.Empty;

	// Token: 0x040003E0 RID: 992
	public string m_undiscountedPrice = string.Empty;

	// Token: 0x040003E1 RID: 993
	public bool m_newItem;

	// Token: 0x040003E2 RID: 994
	public double m_discountPercentage;
}
