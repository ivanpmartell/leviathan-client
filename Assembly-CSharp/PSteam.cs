using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using MiniJSON;

// Token: 0x0200018A RID: 394
internal class PSteam : GDPBackend
{
	// Token: 0x06000E99 RID: 3737 RVA: 0x0006665C File Offset: 0x0006485C
	public PSteam(bool live)
	{
		this.m_printCallback = new PSteam.PrintDelegate(this.PSteamLog);
		this.m_offerCallback = new PSteam.OfferDelegate(this.OnOffer);
		this.m_ownedCallback = new PSteam.OwnedDelegate(this.OnOwned);
		this.m_boughtCallback = new PSteam.BoughtDelegate(this.OnBought);
		PLog.Log("Initializing psteam");
		string catalogue = (!live) ? "leviathan-stage" : "leviathan";
		if (!PSteam.PSteamInitialize(this.m_printCallback, "Leviathan", catalogue, "1.0.0", live))
		{
			PLog.LogError("Failed to initialize PSteam");
			throw new Exception("Steam setup failed");
		}
	}

	// Token: 0x06000E9A RID: 3738
	[DllImport("psteam")]
	private static extern bool PSteamInitialize(PSteam.PrintDelegate logFunction, string gameName, string catalogue, string gameVersion, bool live);

	// Token: 0x06000E9B RID: 3739
	[DllImport("psteam")]
	private static extern void PSteamDeinitialize();

	// Token: 0x06000E9C RID: 3740
	[DllImport("psteam")]
	private static extern void PSteamUpdate();

	// Token: 0x06000E9D RID: 3741
	[DllImport("psteam")]
	private static extern void PSteamRequestOffers(PSteam.OfferDelegate callback);

	// Token: 0x06000E9E RID: 3742
	[DllImport("psteam")]
	private static extern void PSteamRequestOwned(PSteam.OwnedDelegate callback);

	// Token: 0x06000E9F RID: 3743
	[DllImport("psteam")]
	private static extern bool PSteamPlaceOrder(int offerId, string description, PSteam.BoughtDelegate callback);

	// Token: 0x06000EA0 RID: 3744
	[DllImport("psteam")]
	private static extern bool PSteamGetFreeOffer(int offerId, PSteam.BoughtDelegate callback);

	// Token: 0x06000EA1 RID: 3745
	[DllImport("psteam")]
	private static extern void PSteamUnlockAchievement(string name);

	// Token: 0x06000EA2 RID: 3746
	[DllImport("psteam")]
	private static extern void PSteamOpenWebUrl(string name);

	// Token: 0x06000EA3 RID: 3747
	[DllImport("psteam")]
	private static extern string PSteamGetSteamID();

	// Token: 0x06000EA4 RID: 3748
	[DllImport("psteam")]
	private static extern bool PSteamIsSubscribedApp(uint appID);

	// Token: 0x06000EA5 RID: 3749
	[DllImport("psteam")]
	private static extern bool PSteamIsPigsOnline();

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0006672C File Offset: 0x0006492C
	public override void RedeemCode(string code)
	{
		string str = PSteam.PSteamGetSteamID();
		PLog.Log("Steam id " + str);
		string arg;
		bool flag = this.Redeem(code, out arg);
		if (flag)
		{
			this.m_onRedeemRespons(true, string.Empty);
		}
		else
		{
			this.m_onRedeemRespons(false, arg);
		}
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x00066784 File Offset: 0x00064984
	private bool Redeem(string code, out string message)
	{
		string arg = PSteam.PSteamGetSteamID();
		string format = "http://api.paradoxplaza.com/redeem/claim?universe={0}&userid={1}&code={2}";
		string address = string.Format(format, "steam", arg, code);
		bool result;
		try
		{
			string text = new WebClient().DownloadString(address);
			message = string.Empty;
			result = true;
		}
		catch (WebException ex)
		{
			WebResponse response = ex.Response;
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			Stream responseStream = response.GetResponseStream();
			string json = new StreamReader(responseStream).ReadToEnd();
			IDictionary dictionary = (IDictionary)Json.Deserialize(json);
			message = "error";
			if (dictionary.Contains("errorMessage"))
			{
				message = (dictionary["errorMessage"] as string);
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0006685C File Offset: 0x00064A5C
	private void OnDownloaded(object sender, DownloadStringCompletedEventArgs e)
	{
		PLog.Log("Respons is here...finally been waiting for hours...");
		if (!e.Cancelled && e.Error == null)
		{
			string result = e.Result;
			PLog.Log("Derp:" + e.Result);
		}
		else
		{
			PLog.Log("Error " + e.Error.ToString());
		}
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000668C4 File Offset: 0x00064AC4
	public override void RequestOffers()
	{
		this.m_offer.Clear();
		PSteam.PSteamRequestOffers(this.m_offerCallback);
		this.m_onGotOfferList(this.m_offer);
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x000668F0 File Offset: 0x00064AF0
	public override List<GDPBackend.GDPOwnedItem> RequestOwned()
	{
		this.m_owned.Clear();
		PSteam.PSteamRequestOwned(this.m_ownedCallback);
		this.AddDLCContent(ref this.m_owned);
		return this.m_owned;
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x00066928 File Offset: 0x00064B28
	protected void AddDLCContent(ref List<GDPBackend.GDPOwnedItem> items)
	{
		if (PSteam.PSteamIsSubscribedApp(Constants.m_SteamPrePurchaseDlcID))
		{
			base.AddPacks(ref items, Constants.m_SteamPrePurchaseDlcPacks);
		}
		if (PSteam.PSteamIsSubscribedApp(Constants.m_Steam_CommonWealth_pack2_DlcID))
		{
			base.AddPacks(ref items, Constants.m_Steam_CommonWealth_pack2_DlcPacks);
		}
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x0006696C File Offset: 0x00064B6C
	public override void PlaceOrder(GDPBackend.GDPShopItem item, string description)
	{
		if (item.m_price == string.Empty)
		{
			if (!PSteam.PSteamGetFreeOffer(item.m_id, this.m_boughtCallback))
			{
				this.m_onOrderFailed("unknown");
			}
		}
		else if (!PSteam.PSteamPlaceOrder(item.m_id, description, this.m_boughtCallback))
		{
			this.m_onOrderFailed("unknown");
		}
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x000669E4 File Offset: 0x00064BE4
	public override void Close()
	{
		PLog.Log("Shutting down psteam");
		PSteam.PSteamDeinitialize();
		PLog.Log("  done");
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x00066A00 File Offset: 0x00064C00
	public override void Update()
	{
		PSteam.PSteamUpdate();
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x00066A08 File Offset: 0x00064C08
	public override void UnlockAchievement(string name)
	{
		PSteam.PSteamUnlockAchievement(name);
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x00066A10 File Offset: 0x00064C10
	public override void OpenWebUrl(string url)
	{
		PSteam.PSteamOpenWebUrl(url);
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x00066A18 File Offset: 0x00064C18
	private void PSteamLog(string text)
	{
		PLog.Log("PSTEAM: " + text);
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x00066A2C File Offset: 0x00064C2C
	private string FormatPrice(double price, string currency)
	{
		if (currency == "EUR")
		{
			return Localize.instance.Translate("$shop_eur") + price.ToString();
		}
		if (currency == "USD")
		{
			return "$" + price.ToString();
		}
		if (currency == "GBP")
		{
			return Localize.instance.Translate("$shop_gbp") + price.ToString();
		}
		if (currency == "RUB")
		{
			return Localize.instance.Translate("$shop_rub ") + price.ToString();
		}
		PLog.LogWarning("Unkown currency: " + currency);
		return price.ToString();
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x00066AF8 File Offset: 0x00064CF8
	private void OnOffer(int id, string presentationKey, string currency, double price, bool discounted, double undiscountedPrice, int dateYear, int dateMonth, int dateDay)
	{
		PLog.Log(string.Concat(new object[]
		{
			"Got offer ",
			id,
			" ",
			presentationKey,
			" ",
			currency,
			" ",
			price,
			" ",
			discounted,
			" ",
			undiscountedPrice,
			"  ",
			dateYear,
			" ",
			dateMonth,
			" ",
			dateDay
		}));
		if (!presentationKey.StartsWith("leviathan."))
		{
			return;
		}
		GDPBackend.GDPShopItem gdpshopItem = new GDPBackend.GDPShopItem();
		gdpshopItem.m_id = id;
		gdpshopItem.m_presentationKey = presentationKey;
		gdpshopItem.m_price = ((price != 0.0) ? this.FormatPrice(price, currency) : string.Empty);
		gdpshopItem.m_discounted = discounted;
		if (discounted)
		{
			gdpshopItem.m_undiscountedPrice = this.FormatPrice(undiscountedPrice, currency);
			gdpshopItem.m_discountPercentage = 1.0 - price / undiscountedPrice;
		}
		gdpshopItem.m_dateYear = dateYear;
		gdpshopItem.m_dateMonth = dateMonth;
		gdpshopItem.m_dateDay = dateDay;
		gdpshopItem.m_packName = this.GetPackName(presentationKey);
		this.m_offer.Add(gdpshopItem);
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x00066C68 File Offset: 0x00064E68
	private void OnOwned(string presentationKey, int instance)
	{
		PLog.Log(string.Concat(new object[]
		{
			"Got offer ",
			presentationKey,
			" ",
			instance
		}));
		if (!presentationKey.StartsWith("leviathan."))
		{
			return;
		}
		GDPBackend.GDPOwnedItem gdpownedItem = new GDPBackend.GDPOwnedItem();
		gdpownedItem.m_itemType = presentationKey;
		gdpownedItem.m_instance = instance;
		gdpownedItem.m_packName = this.GetPackName(presentationKey);
		this.m_owned.Add(gdpownedItem);
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x00066CE0 File Offset: 0x00064EE0
	private void OnBought(string presentationKey)
	{
		PLog.Log("bought item " + presentationKey);
		string packName = this.GetPackName(presentationKey);
		this.m_onBoughtItem(packName);
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x00066D14 File Offset: 0x00064F14
	private string GetPackName(string presentationKey)
	{
		int length = "leviathan.".Length;
		return presentationKey.Substring(length);
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x00066D34 File Offset: 0x00064F34
	public override bool IsBackendOnline()
	{
		return PSteam.PSteamIsPigsOnline();
	}

	// Token: 0x04000B9E RID: 2974
	private PSteam.PrintDelegate m_printCallback;

	// Token: 0x04000B9F RID: 2975
	private PSteam.OfferDelegate m_offerCallback;

	// Token: 0x04000BA0 RID: 2976
	private PSteam.OwnedDelegate m_ownedCallback;

	// Token: 0x04000BA1 RID: 2977
	private PSteam.BoughtDelegate m_boughtCallback;

	// Token: 0x04000BA2 RID: 2978
	private List<GDPBackend.GDPShopItem> m_offer = new List<GDPBackend.GDPShopItem>();

	// Token: 0x04000BA3 RID: 2979
	private List<GDPBackend.GDPOwnedItem> m_owned = new List<GDPBackend.GDPOwnedItem>();

	// Token: 0x04000BA4 RID: 2980
	private string m_redeemBaseUrl = "http://api.paradoxplaza.com/redeem/claim";

	// Token: 0x04000BA5 RID: 2981
	private WebClient m_redeemWebclient;

	// Token: 0x020001B5 RID: 437
	// (Invoke) Token: 0x06000F84 RID: 3972
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void PrintDelegate(string text);

	// Token: 0x020001B6 RID: 438
	// (Invoke) Token: 0x06000F88 RID: 3976
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void OfferDelegate(int id, string presentationKey, string currency, double price, bool discounted, double undiscountedPrice, int dateYear, int dateMonth, int dateDay);

	// Token: 0x020001B7 RID: 439
	// (Invoke) Token: 0x06000F8C RID: 3980
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void OwnedDelegate(string presentationKey, int instance);

	// Token: 0x020001B8 RID: 440
	// (Invoke) Token: 0x06000F90 RID: 3984
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void BoughtDelegate(string presentationKey);
}
