using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000068 RID: 104
internal class Shop
{
	// Token: 0x06000486 RID: 1158 RVA: 0x000270F8 File Offset: 0x000252F8
	public Shop(GameObject guiCamera, GameObject shopPanel, GDPBackend gdpBackend, PTech.RPC rpc, UserManClient userMan)
	{
		this.m_shopPanel = shopPanel;
		this.m_guiCamera = guiCamera;
		this.m_gdpBackend = gdpBackend;
		this.m_rpc = rpc;
		this.m_userMan = userMan;
		this.m_pacMan = new PackMan();
		this.m_rpc.Register("PackList", new PTech.RPC.Handler(this.RPC_PackList));
		this.m_shopItem = (Resources.Load("gui/shop/ShopItemContainer") as GameObject);
		this.m_shopItemOwned = (Resources.Load("gui/shop/ShopItemContainer_Owned") as GameObject);
		this.m_infoPanel = GuiUtils.FindChildOfComponent<UIPanel>(shopPanel, "ShopInfoPanel");
		this.m_infoPanelOwned = GuiUtils.FindChildOfComponent<UIPanel>(shopPanel, "ShopInfoPanelOwned");
		this.m_shopList = GuiUtils.FindChildOfComponent<UIScrollList>(shopPanel, "ShopScrollList");
		GuiUtils.FindChildOfComponent<UIButton>(shopPanel, "resetButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopRestoreOwned));
		GuiUtils.FindChildOfComponent<UIButton>(shopPanel, "redeemCodeButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenRedeemCodeDialog));
		GuiUtils.FindChildOfComponent<UIStateToggleBtn>(shopPanel, "FilterCampaignCheckbox").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopFilterChanged));
		GuiUtils.FindChildOfComponent<UIStateToggleBtn>(shopPanel, "FilterMapsCheckbox").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopFilterChanged));
		GuiUtils.FindChildOfComponent<UIStateToggleBtn>(shopPanel, "FilterShipsCheckbox").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopFilterChanged));
		GuiUtils.FindChildOfComponent<UIStateToggleBtn>(shopPanel, "HideOwnedItemsCheckbox").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopFilterChanged));
		GuiUtils.FindChildOfComponent<UIStateToggleBtn>(shopPanel, "FilterFlagsCheckbox").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopFilterChanged));
		GuiUtils.FindChildOfComponent<UIStateToggleBtn>(shopPanel, "FilterDiscCheckbox").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopFilterChanged));
		GuiUtils.FindChildOfComponent<UIStateToggleBtn>(shopPanel, "FilterNewCheckbox").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopFilterChanged));
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x000272C8 File Offset: 0x000254C8
	public void Close()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		this.m_rpc.Unregister("PackList");
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x000272F8 File Offset: 0x000254F8
	public void Update(float dt)
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Update();
		}
		if (this.m_updateListFlag)
		{
			this.m_updateListFlag = false;
			this.UpdateItemList();
		}
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00027334 File Offset: 0x00025534
	public void OnShowShop(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (!uipanelTab.Value)
		{
			return;
		}
		this.m_shopPanel.GetComponent<UIPanel>().AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnShopTransitionComplete));
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x00027370 File Offset: 0x00025570
	private void OnShopTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		if (this.m_gdpBackend == null || !this.m_gdpBackend.CanPlaceOrders())
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$store_is_locked", new MsgBox.OkHandler(this.OnNotAvailableOK));
			return;
		}
		this.m_updateListFlag = true;
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x000273C4 File Offset: 0x000255C4
	private void OnNotAvailableOK()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x000273D8 File Offset: 0x000255D8
	private void UpdateItemList()
	{
		if (this.m_gdpBackend != null)
		{
			if (!this.m_gdpBackend.CanPlaceOrders())
			{
				return;
			}
			this.m_gdpBackend.m_onGotOfferList = new Action<List<GDPBackend.GDPShopItem>>(this.OnGotGDPOffers);
			this.m_gdpBackend.RequestOffers();
		}
		else
		{
			this.RequestPackListFromServer();
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00027430 File Offset: 0x00025630
	private void RequestPackListFromServer()
	{
		this.m_rpc.Invoke("RequestPackList", new object[0]);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00027448 File Offset: 0x00025648
	private void OnGotGDPOffers(List<GDPBackend.GDPShopItem> offer)
	{
		DebugUtils.Assert(this.m_gdpBackend != null);
		this.m_gdpOffer = offer;
		this.m_gdpOwned = this.m_gdpBackend.RequestOwned();
		PLog.Log("offer " + this.m_gdpOffer.Count);
		PLog.Log("owned " + this.m_gdpOwned.Count);
		this.m_shopItemData.Clear();
		foreach (GDPBackend.GDPShopItem gdpshopItem in this.m_gdpOffer)
		{
			ContentPack pack = this.m_pacMan.GetPack(gdpshopItem.m_packName);
			if (pack == null)
			{
				PLog.LogError(string.Concat(new object[]
				{
					"Missing content pack ",
					gdpshopItem.m_packName,
					" used by shop item ",
					gdpshopItem.m_id,
					" : ",
					gdpshopItem.m_presentationKey
				}));
			}
			bool owned = this.IsItemOwned(gdpshopItem.m_packName);
			bool newItem = false;
			string undiscountedPrice = (!gdpshopItem.m_discounted) ? gdpshopItem.m_price : gdpshopItem.m_undiscountedPrice;
			ContentPack.Category type = ContentPack.Category.Ships;
			if (pack != null)
			{
				type = pack.m_type;
				newItem = pack.m_newItem;
			}
			this.m_shopItemData.Add(new ShopItemData(gdpshopItem, type, gdpshopItem.m_packName, owned, gdpshopItem.m_price, undiscountedPrice, gdpshopItem.m_discountPercentage, newItem));
		}
		this.FillShopItems();
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x000275F0 File Offset: 0x000257F0
	private bool IsItemOwned(string packName)
	{
		if (this.m_gdpBackend != null)
		{
			foreach (GDPBackend.GDPOwnedItem gdpownedItem in this.m_gdpOwned)
			{
				if (gdpownedItem.m_packName == packName)
				{
					return true;
				}
			}
		}
		else
		{
			foreach (ShopItemData shopItemData in this.m_shopItemData)
			{
				if (shopItemData.m_name == packName && shopItemData.m_owned)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x000276F0 File Offset: 0x000258F0
	private void RPC_PackList(PTech.RPC rpc, List<object> args)
	{
		this.m_shopItemData.Clear();
		int num = 0;
		int num2 = (int)args[num++];
		for (int i = 0; i < num2; i++)
		{
			string text = (string)args[num++];
			bool flag = (bool)args[num++];
			ContentPack.Category category = (ContentPack.Category)((int)args[num++]);
			double num3 = (double)UnityEngine.Random.Range(0, 5);
			bool flag2 = UnityEngine.Random.value > 0.5f;
		}
		PLog.Log("got packs " + this.m_listedItemData.Count);
		this.FillShopItems();
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x000277A8 File Offset: 0x000259A8
	private void GetShopListMask(out int categories, out bool hideOwned, out bool saleOnly, out bool newOnly)
	{
		UIStateToggleBtn uistateToggleBtn = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_shopPanel, "FilterCampaignCheckbox");
		UIStateToggleBtn uistateToggleBtn2 = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_shopPanel, "FilterMapsCheckbox");
		UIStateToggleBtn uistateToggleBtn3 = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_shopPanel, "FilterShipsCheckbox");
		UIStateToggleBtn uistateToggleBtn4 = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_shopPanel, "FilterFlagsCheckbox");
		UIStateToggleBtn uistateToggleBtn5 = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_shopPanel, "HideOwnedItemsCheckbox");
		UIStateToggleBtn uistateToggleBtn6 = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_shopPanel, "FilterDiscCheckbox");
		UIStateToggleBtn uistateToggleBtn7 = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_shopPanel, "FilterNewCheckbox");
		categories = 0;
		if (uistateToggleBtn.StateNum == 0)
		{
			categories |= 4;
		}
		if (uistateToggleBtn2.StateNum == 0)
		{
			categories |= 1;
		}
		if (uistateToggleBtn3.StateNum == 0)
		{
			categories |= 2;
		}
		if (uistateToggleBtn4.StateNum == 0)
		{
			categories |= 8;
		}
		hideOwned = (uistateToggleBtn5.StateNum == 0);
		saleOnly = (uistateToggleBtn6.StateNum == 0);
		newOnly = (uistateToggleBtn7.StateNum == 0);
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0002789C File Offset: 0x00025A9C
	private void OnShopRestoreOwned(IUIObject obj)
	{
		if (this.m_gdpBackend == null)
		{
			return;
		}
		this.m_msgBox = MsgBox.CreateTextOnlyMsgBox(this.m_guiCamera, "$store_restoring_owned");
		this.m_gdpBackend.m_onRestoreOwnedFinished = new Action<bool, string>(this.OnRestoreOwnedFinished);
		this.m_gdpBackend.RestoreOwned();
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x000278F0 File Offset: 0x00025AF0
	private void OnOpenRedeemCodeDialog(IUIObject obj)
	{
		this.m_redeemDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$shop_enterredeemcode"), string.Empty, new GenericTextInput.InputTextCancel(this.OnRedeemCancel), new GenericTextInput.InputTextCommit(this.OnRedeemOk));
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0002793C File Offset: 0x00025B3C
	private void OnRedeemOk(string text)
	{
		UnityEngine.Object.Destroy(this.m_redeemDialog);
		if (this.m_gdpBackend == null)
		{
			return;
		}
		this.m_gdpBackend.m_onRedeemRespons = new Action<bool, string>(this.OnRedeemRespons);
		this.m_gdpBackend.RedeemCode(text);
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00027984 File Offset: 0x00025B84
	private void OnRedeemCancel()
	{
		UnityEngine.Object.Destroy(this.m_redeemDialog);
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00027994 File Offset: 0x00025B94
	private void OnRestoreOwnedFinished(bool success, string error)
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		if (success)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$store_restore_owned_done", new MsgBox.OkHandler(this.OnRestoreFailedOK));
		}
		else
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$store_restore_owned_failed " + error, new MsgBox.OkHandler(this.OnRestoreFailedOK));
		}
		this.UpdateItemList();
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00027A1C File Offset: 0x00025C1C
	private void OnRestoreFailedOK()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00027A30 File Offset: 0x00025C30
	private void OnShopFilterChanged(IUIObject obj)
	{
		this.FillShopItems();
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x00027A38 File Offset: 0x00025C38
	private void FillShopItems()
	{
		int num;
		bool flag;
		bool flag2;
		bool flag3;
		this.GetShopListMask(out num, out flag, out flag2, out flag3);
		float scrollPosition = this.m_shopList.ScrollPosition;
		this.m_listedItemData.Clear();
		this.m_shopList.ClearList(true);
		foreach (ShopItemData shopItemData in this.m_shopItemData)
		{
			if ((shopItemData.m_type & (ContentPack.Category)num) != ContentPack.Category.None)
			{
				if (!shopItemData.m_owned || !flag)
				{
					if (!flag3 || shopItemData.m_newItem)
					{
						if (!flag2 || shopItemData.m_discountPercentage != 0.0)
						{
							GameObject gameObject;
							if (shopItemData.m_owned)
							{
								gameObject = (UnityEngine.Object.Instantiate(this.m_shopItemOwned) as GameObject);
							}
							else
							{
								gameObject = (UnityEngine.Object.Instantiate(this.m_shopItem) as GameObject);
							}
							GuiUtils.LocalizeGui(gameObject);
							SimpleSprite sprite = GuiUtils.FindChildOfComponent<SimpleSprite>(gameObject, "Image");
							GameObject gameObject2 = GuiUtils.FindChildOf(gameObject, "NewRibbon");
							SpriteText spriteText = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "TitleLabel");
							SpriteText spriteText2 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "DescriptionLabel");
							SpriteText spriteText3 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "PriceValueLabel");
							SpriteText spriteText4 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "PriceValueLabel_Discount");
							SpriteText spriteText5 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "DiscountLabel");
							SpriteText spriteText6 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "DiscountValueLabel");
							SpriteText spriteText7 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "FlavorSmallLabel");
							SpriteText spriteText8 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "FlavorBigLabel");
							UIButton uibutton = GuiUtils.FindChildOfComponent<UIButton>(gameObject, "MoreInfoButton");
							UIButton uibutton2 = GuiUtils.FindChildOfComponent<UIButton>(gameObject, "BuyButton");
							if (gameObject2 != null && !shopItemData.m_newItem)
							{
								gameObject2.transform.Translate(new Vector3(10000f, 0f, 0f));
							}
							Texture2D shopIconTexture = GuiUtils.GetShopIconTexture(shopItemData.m_name);
							if (shopIconTexture != null)
							{
								GuiUtils.SetImage(sprite, shopIconTexture);
							}
							spriteText.Text = Localize.instance.TranslateKey("shopitem_" + shopItemData.m_name + "_name");
							spriteText2.Text = Localize.instance.TranslateKey("shopitem_" + shopItemData.m_name + "_description");
							if (shopItemData.m_price == string.Empty)
							{
								spriteText3.Text = Localize.instance.Translate("$shop_free");
								spriteText4.Text = string.Empty;
								spriteText3.SetColor(Color.green);
							}
							else if (shopItemData.m_undiscountedPrice != shopItemData.m_price)
							{
								spriteText3.Text = string.Empty;
								spriteText4.Text = shopItemData.m_undiscountedPrice;
							}
							else
							{
								spriteText3.Text = shopItemData.m_price;
								spriteText4.Text = string.Empty;
							}
							if (shopItemData.m_discountPercentage != 0.0)
							{
								spriteText5.Text = "-" + ((int)(shopItemData.m_discountPercentage * 100.0)).ToString() + "%";
								spriteText6.Text = shopItemData.m_price;
							}
							else
							{
								spriteText5.Text = string.Empty;
								spriteText6.Text = string.Empty;
							}
							uibutton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopItemInfo));
							if (uibutton2 != null)
							{
								uibutton2.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopItemBuy));
								uibutton2.controlIsEnabled = !shopItemData.m_owned;
							}
							GuiUtils.FixedItemContainerInstance(gameObject.GetComponent<UIListItemContainer>());
							this.m_shopList.AddItem(gameObject);
							this.m_listedItemData.Add(shopItemData);
						}
					}
				}
			}
		}
		this.m_shopList.ScrollToItem(0, 0f);
		this.m_shopList.ScrollPosition = scrollPosition;
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00027E50 File Offset: 0x00026050
	private void OnShopItemBuy(IUIObject obj)
	{
		UIListItemContainer component = obj.transform.parent.GetComponent<UIListItemContainer>();
		int index = component.Index;
		this.m_selectedItem = this.m_listedItemData[index];
		this.BuyShopItem(this.m_selectedItem);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00027E94 File Offset: 0x00026094
	private void OnShopItemInfo(IUIObject obj)
	{
		UIListItemContainer component = obj.transform.parent.GetComponent<UIListItemContainer>();
		int index = component.Index;
		this.m_selectedItem = this.m_listedItemData[index];
		this.OpenItemInfoDialog();
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00027ED4 File Offset: 0x000260D4
	private void OpenItemInfoDialog()
	{
		ShopItemData selectedItem = this.m_selectedItem;
		UIPanel uipanel = (!selectedItem.m_owned) ? this.m_infoPanel : this.m_infoPanelOwned;
		SimpleSprite sprite = GuiUtils.FindChildOfComponent<SimpleSprite>(uipanel.gameObject, "iconImage");
		SpriteText spriteText = GuiUtils.FindChildOfComponent<SpriteText>(uipanel.gameObject, "nameLabel");
		SpriteText spriteText2 = GuiUtils.FindChildOfComponent<SpriteText>(uipanel.gameObject, "descriptionLabel");
		SpriteText spriteText3 = GuiUtils.FindChildOfComponent<SpriteText>(uipanel.gameObject, "longDescriptionLabel");
		UIScrollList uiscrollList = GuiUtils.FindChildOfComponent<UIScrollList>(uipanel.gameObject, "longDescriptionScrollList");
		SpriteText spriteText4 = GuiUtils.FindChildOfComponent<SpriteText>(uipanel.gameObject, "priceValueLabel");
		SpriteText spriteText5 = GuiUtils.FindChildOfComponent<SpriteText>(uipanel.gameObject, "priceValueLabel_Discount");
		SpriteText spriteText6 = GuiUtils.FindChildOfComponent<SpriteText>(uipanel.gameObject, "discountLabel");
		SpriteText spriteText7 = GuiUtils.FindChildOfComponent<SpriteText>(uipanel.gameObject, "discountValueLabel");
		UIButton uibutton = GuiUtils.FindChildOfComponent<UIButton>(uipanel.gameObject, "buyButton");
		UIButton uibutton2 = GuiUtils.FindChildOfComponent<UIButton>(uipanel.gameObject, "closeButton");
		if (uibutton != null)
		{
			uibutton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopInfoBuy));
			uibutton.controlIsEnabled = !selectedItem.m_owned;
		}
		uibutton2.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShopInfoClose));
		Texture2D shopImageTexture = GuiUtils.GetShopImageTexture(selectedItem.m_name);
		if (shopImageTexture != null)
		{
			GuiUtils.SetImage(sprite, shopImageTexture);
		}
		spriteText.Text = Localize.instance.TranslateKey("shopitem_" + selectedItem.m_name + "_name");
		spriteText2.Text = Localize.instance.TranslateKey("shopitem_" + selectedItem.m_name + "_description");
		spriteText3.Text = Localize.instance.TranslateRecursive("$shopitem_" + selectedItem.m_name + "_longdescription");
		uiscrollList.ScrollToItem(0, 0f);
		if (selectedItem.m_price == string.Empty)
		{
			spriteText4.Text = Localize.instance.Translate("$shop_free");
			spriteText5.Text = string.Empty;
			spriteText4.SetColor(Color.green);
		}
		else if (selectedItem.m_undiscountedPrice != selectedItem.m_price)
		{
			spriteText4.Text = string.Empty;
			spriteText5.Text = selectedItem.m_undiscountedPrice;
		}
		else
		{
			spriteText4.Text = selectedItem.m_price;
			spriteText5.Text = string.Empty;
		}
		if (selectedItem.m_discountPercentage != 0.0)
		{
			spriteText6.Text = "-" + ((int)(selectedItem.m_discountPercentage * 100.0)).ToString() + "%";
			spriteText7.Text = selectedItem.m_price;
		}
		else
		{
			spriteText6.Text = string.Empty;
			spriteText7.Text = string.Empty;
		}
		uipanel.BringIn();
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x000281B8 File Offset: 0x000263B8
	private void OnShopInfoClose(IUIObject obj)
	{
		this.m_infoPanel.Dismiss();
		this.m_infoPanelOwned.Dismiss();
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x000281D0 File Offset: 0x000263D0
	private void OnShopInfoBuy(IUIObject obj)
	{
		ShopItemData selectedItem = this.m_selectedItem;
		this.BuyShopItem(selectedItem);
		this.m_infoPanel.Dismiss();
		this.m_infoPanelOwned.Dismiss();
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00028204 File Offset: 0x00026404
	private void OnRedeemRespons(bool success, string errorMessage)
	{
		if (success)
		{
			if (this.m_onItemBought != null)
			{
				this.m_onItemBought();
			}
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$store_redeem_success ", new MsgBox.OkHandler(this.OnItemBoughtOk));
		}
		else
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$store_redeem_error " + errorMessage, new MsgBox.OkHandler(this.OnItemBoughtOk));
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0002827C File Offset: 0x0002647C
	private void OnBoughtItem(string pkgName)
	{
		if (this.m_onItemBought != null)
		{
			this.m_onItemBought();
		}
		string str = string.Empty;
		if (pkgName != string.Empty)
		{
			str = Localize.instance.TranslateKey("shopitem_" + pkgName + "_name");
		}
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$store_item_bought " + str, new MsgBox.OkHandler(this.OnItemBoughtOk));
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x000282F8 File Offset: 0x000264F8
	private void OnItemBoughtOk()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		this.UpdateItemList();
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x00028314 File Offset: 0x00026514
	private void BuyShopItem(ShopItemData item)
	{
		if (this.m_gdpBackend != null)
		{
			string description = Localize.instance.TranslateKey("shopitem_" + item.m_name + "_name");
			this.m_gdpBackend.m_onBoughtItem = new Action<string>(this.OnBoughtItem);
			this.m_gdpBackend.m_onOrderFailed = new Action<string>(this.OnOrderFailed);
			this.m_gdpBackend.PlaceOrder(item.m_gdpItem, description);
		}
		else
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$shop_bought", new MsgBox.OkHandler(this.OnShopBuyOk));
			this.m_userMan.BuyPackage(item.m_name);
			this.UpdateItemList();
		}
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x000283CC File Offset: 0x000265CC
	private void OnOrderFailed(string error)
	{
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$shop_buy_failed \"" + error + "\"", new MsgBox.OkHandler(this.OnShopFailedOk));
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x000283FC File Offset: 0x000265FC
	private void OnShopFailedOk()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x00028410 File Offset: 0x00026610
	private void OnShopBuyOk()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x040003E3 RID: 995
	public Action m_onItemBought;

	// Token: 0x040003E4 RID: 996
	private GameObject m_shopPanel;

	// Token: 0x040003E5 RID: 997
	private GDPBackend m_gdpBackend;

	// Token: 0x040003E6 RID: 998
	private PTech.RPC m_rpc;

	// Token: 0x040003E7 RID: 999
	private GameObject m_guiCamera;

	// Token: 0x040003E8 RID: 1000
	private MsgBox m_msgBox;

	// Token: 0x040003E9 RID: 1001
	private UserManClient m_userMan;

	// Token: 0x040003EA RID: 1002
	private PackMan m_pacMan;

	// Token: 0x040003EB RID: 1003
	private GameObject m_shopItem;

	// Token: 0x040003EC RID: 1004
	private GameObject m_shopItemOwned;

	// Token: 0x040003ED RID: 1005
	private UIPanel m_infoPanel;

	// Token: 0x040003EE RID: 1006
	private UIPanel m_infoPanelOwned;

	// Token: 0x040003EF RID: 1007
	private UIScrollList m_shopList;

	// Token: 0x040003F0 RID: 1008
	private List<GDPBackend.GDPShopItem> m_gdpOffer;

	// Token: 0x040003F1 RID: 1009
	private List<GDPBackend.GDPOwnedItem> m_gdpOwned;

	// Token: 0x040003F2 RID: 1010
	private List<ShopItemData> m_shopItemData = new List<ShopItemData>();

	// Token: 0x040003F3 RID: 1011
	private List<ShopItemData> m_listedItemData = new List<ShopItemData>();

	// Token: 0x040003F4 RID: 1012
	private ShopItemData m_selectedItem;

	// Token: 0x040003F5 RID: 1013
	private bool m_updateListFlag;

	// Token: 0x040003F6 RID: 1014
	private GameObject m_redeemDialog;
}
