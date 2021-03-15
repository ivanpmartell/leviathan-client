using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000053 RID: 83
internal class NewsTicker
{
	// Token: 0x06000384 RID: 900 RVA: 0x0001C4AC File Offset: 0x0001A6AC
	public NewsTicker(PdxNews pdxNews, GDPBackend gdp, GameObject guiCamera)
	{
		this.m_guiCamera = guiCamera;
		this.m_pdxNews = pdxNews;
		this.m_gdpBackend = gdp;
		this.m_gui = GuiUtils.CreateGui("Ticker", this.m_guiCamera);
		this.m_itemPrefab = GuiUtils.FindChildOf(this.m_gui, "TickerListItem");
		this.m_scrollList = GuiUtils.FindChildOfComponent<UIScrollList>(this.m_gui, "TickerScrollList");
		this.m_item = (UnityEngine.Object.Instantiate(this.m_itemPrefab) as GameObject);
		this.m_scrollList.AddItem(this.m_item.GetComponent<UIListItem>());
		this.m_scrollList.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnItemSelected));
	}

	// Token: 0x06000385 RID: 901 RVA: 0x0001C55C File Offset: 0x0001A75C
	public void Close()
	{
		UnityEngine.Object.Destroy(this.m_gui);
	}

	// Token: 0x06000386 RID: 902 RVA: 0x0001C56C File Offset: 0x0001A76C
	public void Update(float dt)
	{
		if (this.GetItems())
		{
			this.UpdateGui(dt);
		}
	}

	// Token: 0x06000387 RID: 903 RVA: 0x0001C580 File Offset: 0x0001A780
	private bool GetItems()
	{
		if (this.m_entries != null)
		{
			return true;
		}
		this.m_entries = this.m_pdxNews.GetEntries();
		if (this.m_entries == null || this.m_entries.Count == 0)
		{
			return false;
		}
		this.SetupNextItem();
		this.m_scrollList.ScrollPosition = 0f;
		return true;
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0001C5E0 File Offset: 0x0001A7E0
	private void UpdateGui(float dt)
	{
		float num = this.m_scrollList.ScrollPosition + dt * 0.1f;
		if ((double)num > 1.5)
		{
			num = 0f;
			this.SetupNextItem();
		}
		this.m_scrollList.ScrollPosition = num;
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0001C62C File Offset: 0x0001A82C
	private void SetupNextItem()
	{
		UIListItem component = this.m_item.GetComponent<UIListItem>();
		if (this.m_itemID < this.m_entries.Count)
		{
			component.Text = this.m_entries[this.m_itemID].m_title;
			this.m_currentUrl = this.m_entries[this.m_itemID].m_url;
		}
		this.m_itemID++;
		if (this.m_itemID >= this.m_entries.Count)
		{
			this.m_itemID = 0;
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0001C6C0 File Offset: 0x0001A8C0
	private void OnItemSelected(IUIObject obj)
	{
		if (string.IsNullOrEmpty(this.m_currentUrl))
		{
			return;
		}
		if (this.m_gdpBackend != null)
		{
			this.m_gdpBackend.OpenWebUrl(this.m_currentUrl);
		}
		else
		{
			Application.OpenURL(this.m_currentUrl);
		}
	}

	// Token: 0x040002F9 RID: 761
	private PdxNews m_pdxNews;

	// Token: 0x040002FA RID: 762
	private GDPBackend m_gdpBackend;

	// Token: 0x040002FB RID: 763
	private GameObject m_gui;

	// Token: 0x040002FC RID: 764
	private GameObject m_guiCamera;

	// Token: 0x040002FD RID: 765
	private GameObject m_itemPrefab;

	// Token: 0x040002FE RID: 766
	private UIScrollList m_scrollList;

	// Token: 0x040002FF RID: 767
	private GameObject m_item;

	// Token: 0x04000300 RID: 768
	private List<PdxNews.Entry> m_entries;

	// Token: 0x04000301 RID: 769
	private int m_itemID;

	// Token: 0x04000302 RID: 770
	private string m_currentUrl;
}
