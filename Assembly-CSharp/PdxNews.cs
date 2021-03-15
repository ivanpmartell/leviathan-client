using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using MiniJSON;

// Token: 0x02000186 RID: 390
public class PdxNews
{
	// Token: 0x06000E8B RID: 3723 RVA: 0x000661D4 File Offset: 0x000643D4
	public PdxNews(string gameName, bool live)
	{
		this.RequestFeed(live, gameName);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x00066208 File Offset: 0x00064408
	private string GetPlatformString()
	{
		return "steam";
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00066210 File Offset: 0x00064410
	private void RequestFeed(bool live, string gameName)
	{
		string platformString = this.GetPlatformString();
		string uriString = string.Concat(new string[]
		{
			this.m_baseUrl,
			gameName,
			"-",
			platformString,
			"/",
			this.m_feedFile
		});
		Uri address = new Uri(uriString);
		this.m_client = new WebClient();
		this.m_client.DownloadStringCompleted += this.OnDownloaded;
		this.m_client.DownloadStringAsync(address);
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x00066290 File Offset: 0x00064490
	private void OnDownloaded(object sender, DownloadStringCompletedEventArgs e)
	{
		this.m_entries.Clear();
		if (!e.Cancelled && e.Error == null)
		{
			string result = e.Result;
			IDictionary dictionary = (IDictionary)Json.Deserialize(result);
			string a = dictionary["result"] as string;
			if (a == "OK")
			{
				IList list = dictionary["entries"] as IList;
				foreach (object obj in list)
				{
					IDictionary dictionary2 = (IDictionary)obj;
					PdxNews.Entry entry = new PdxNews.Entry();
					if (dictionary2.Contains("title"))
					{
						entry.m_title = (dictionary2["title"] as string);
					}
					if (dictionary2.Contains("url"))
					{
						entry.m_url = (dictionary2["url"] as string);
					}
					if (dictionary2.Contains("timestamp"))
					{
						entry.m_timeStamp = (dictionary2["timestamp"] as string);
					}
					if (dictionary2.Contains("steam-appid"))
					{
						entry.m_steamAppid = (long)dictionary2["steam-appid"];
					}
					if (dictionary2.Contains("in-game-store-offer"))
					{
						entry.m_inGameStoreOffer = (long)dictionary2["in-game-store-offer"];
					}
					this.m_entries.Add(entry);
				}
			}
			else
			{
				PLog.LogWarning("Invalid result from ticker host");
			}
		}
		this.m_client = null;
		this.m_gotData = true;
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x00066460 File Offset: 0x00064660
	public List<PdxNews.Entry> GetEntries()
	{
		if (this.m_gotData)
		{
			return this.m_entries;
		}
		return null;
	}

	// Token: 0x04000B94 RID: 2964
	private string m_baseUrl = "http://services.paradoxplaza.com/adam/feeds/";

	// Token: 0x04000B95 RID: 2965
	private string m_feedFile = "feed.json";

	// Token: 0x04000B96 RID: 2966
	private bool m_gotData;

	// Token: 0x04000B97 RID: 2967
	private WebClient m_client;

	// Token: 0x04000B98 RID: 2968
	private List<PdxNews.Entry> m_entries = new List<PdxNews.Entry>();

	// Token: 0x02000187 RID: 391
	public class Entry
	{
		// Token: 0x04000B99 RID: 2969
		public string m_title;

		// Token: 0x04000B9A RID: 2970
		public string m_url;

		// Token: 0x04000B9B RID: 2971
		public long m_steamAppid;

		// Token: 0x04000B9C RID: 2972
		public long m_inGameStoreOffer;

		// Token: 0x04000B9D RID: 2973
		public string m_timeStamp;
	}
}
