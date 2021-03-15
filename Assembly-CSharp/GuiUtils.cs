using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
internal class GuiUtils
{
	// Token: 0x06000E66 RID: 3686 RVA: 0x00065258 File Offset: 0x00063458
	public static GameObject CreateGui(string name, GameObject guiCamera)
	{
		GameObject gameObject = Resources.Load("gui/" + name) as GameObject;
		if (gameObject == null)
		{
			PLog.LogWarning(string.Format("GuiUtils::CreateGui( {0}, {1} ) Failed !", name, (!(guiCamera == null)) ? "guiCamera" : "NULL"));
			return null;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
		gameObject2.transform.parent = guiCamera.transform;
		EZScreenPlacement[] componentsInChildren = gameObject2.GetComponentsInChildren<EZScreenPlacement>(true);
		foreach (EZScreenPlacement ezscreenPlacement in componentsInChildren)
		{
			ezscreenPlacement.SetCamera(guiCamera.camera);
		}
		GuiUtils.LocalizeGui(gameObject2);
		return gameObject2;
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00065310 File Offset: 0x00063510
	public static void FixedItemContainerInstance(UIListItemContainer item)
	{
		AutoSpriteBase[] componentsInChildren = item.transform.GetComponentsInChildren<AutoSpriteBase>(true);
		foreach (AutoSpriteBase autoSpriteBase in componentsInChildren)
		{
			autoSpriteBase.isClone = true;
		}
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x0006534C File Offset: 0x0006354C
	public static Vector3 GetDimensionsAndDestroy(string name, GameObject guiCamera)
	{
		GameObject gameObject = GuiUtils.CreateGui(name, guiCamera);
		float x;
		float y;
		float z;
		Utils.GetDimensionsOfGameObject(gameObject, out x, out y, out z);
		UnityEngine.Object.DestroyImmediate(gameObject);
		return new Vector3(x, y, z);
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0006537C File Offset: 0x0006357C
	public static void LocalizeGui(GameObject root)
	{
		Localize instance = Localize.instance;
		Component[] componentsInChildren = root.GetComponentsInChildren<Component>(true);
		foreach (Component component in componentsInChildren)
		{
			SpriteText spriteText = component as SpriteText;
			if (spriteText != null)
			{
				spriteText.Text = instance.Translate(spriteText.Text);
			}
			UIButton uibutton = component as UIButton;
			if (uibutton != null)
			{
				uibutton.Text = instance.Translate(uibutton.Text);
			}
		}
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x00065408 File Offset: 0x00063608
	public static T FindChildOfComponent<T>(GameObject go, string name) where T : Component
	{
		GameObject gameObject = GuiUtils.FindChildOf(go.transform, name);
		if (gameObject != null)
		{
			return gameObject.GetComponent<T>();
		}
		return (T)((object)null);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x0006543C File Offset: 0x0006363C
	public static GameObject FindChildOf(GameObject go, string name)
	{
		return GuiUtils.FindChildOf(go.transform, name);
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x0006544C File Offset: 0x0006364C
	public static GameObject FindChildOf(Transform t, string name)
	{
		if (t.gameObject.name == name)
		{
			return t.gameObject;
		}
		for (int i = 0; i < t.childCount; i++)
		{
			GameObject gameObject = GuiUtils.FindChildOf(t.GetChild(i), name);
			if (gameObject)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x000654AC File Offset: 0x000636AC
	public static GameObject FindParent(Transform t, string name)
	{
		Transform parent = t.parent;
		while (parent)
		{
			if (parent.gameObject.name == name)
			{
				return parent.gameObject;
			}
			parent = parent.parent;
		}
		return null;
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x000654F8 File Offset: 0x000636F8
	public static GameObject ValidateGetObject(GameObject gui, string name)
	{
		Transform transform = gui.transform.FindChild(name);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x0006552C File Offset: 0x0006372C
	public static bool ValidateGuiButton(GameObject gui, string name, out GameObject go)
	{
		go = GuiUtils.ValidateGetObject(gui, name);
		if (go == null)
		{
			PLog.Log("Failed to find GuiButton: " + name);
			return false;
		}
		return go.GetComponent<UIButton>() != null;
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x00065570 File Offset: 0x00063770
	public static bool ValidateGuiList(GameObject gui, string name, out GameObject go)
	{
		go = GuiUtils.ValidateGetObject(gui, name);
		if (go == null)
		{
			PLog.Log("Failed to find GuiList: " + name);
			return false;
		}
		return go.GetComponent<UIScrollList>() != null;
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x000655B4 File Offset: 0x000637B4
	public static bool ValidateGuLabel(GameObject gui, string name, out GameObject go)
	{
		go = GuiUtils.ValidateGetObject(gui, name);
		if (go == null)
		{
			PLog.Log("Failed to find GuLabel: " + name);
			return false;
		}
		return go.GetComponent<SpriteText>() != null;
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x000655F8 File Offset: 0x000637F8
	public static bool ValidateSimpelSprite(GameObject gui, string name, out GameObject go)
	{
		go = GuiUtils.ValidateGetObject(gui, name);
		if (go == null)
		{
			PLog.Log("Failed to find SimpleSprite: " + name);
			return false;
		}
		return go.GetComponent<SimpleSprite>() != null;
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x0006563C File Offset: 0x0006383C
	public static Vector3 WorldToGuiPos(Camera gameCamera, Camera guiCamera, Vector3 pos)
	{
		Vector3 result = gameCamera.WorldToViewportPoint(pos);
		result.x = (result.x - 0.5f) * 2f;
		result.y = (result.y - 0.5f) * 2f;
		result.x *= guiCamera.orthographicSize * guiCamera.aspect;
		result.y *= guiCamera.orthographicSize;
		result.z = 0f;
		result.x = (float)((int)result.x);
		result.y = (float)((int)result.y);
		result.x += 0.01f;
		result.y += 0.01f;
		return result;
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x00065708 File Offset: 0x00063908
	public static Vector3 WorldToGuiPosf(Camera gameCamera, Camera guiCamera, Vector3 pos)
	{
		Vector3 result = gameCamera.WorldToViewportPoint(pos);
		result.x = (result.x - 0.5f) * 2f;
		result.y = (result.y - 0.5f) * 2f;
		result.x *= guiCamera.orthographicSize * guiCamera.aspect;
		result.y *= guiCamera.orthographicSize;
		result.z = 0f;
		return result;
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x00065790 File Offset: 0x00063990
	public static void SetAnimationSetProgress(GameObject animationRoot, float i)
	{
		PackedSprite[] componentsInChildren = animationRoot.GetComponentsInChildren<PackedSprite>(true);
		int num = 0;
		foreach (PackedSprite packedSprite in componentsInChildren)
		{
			num += packedSprite.animations[0].GetFrameCount();
		}
		int num2 = (int)((float)num * i);
		int num3 = 0;
		foreach (PackedSprite packedSprite2 in componentsInChildren)
		{
			int frameCount = packedSprite2.animations[0].GetFrameCount();
			if (num2 >= num3 && num2 < num3 + frameCount)
			{
				packedSprite2.Hide(false);
				packedSprite2.SetFrame(0, num2 - num3);
			}
			else
			{
				packedSprite2.Hide(true);
			}
			num3 += frameCount;
		}
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x00065854 File Offset: 0x00063A54
	public static Texture2D GetFlagTexture(int id)
	{
		string text = "Flags/AvatarFlag_" + id.ToString();
		Texture2D texture2D = Resources.Load(text) as Texture2D;
		if (texture2D == null)
		{
			string o = string.Format("HUD_Player failed to load flag \"{0}\"", text);
			PLog.LogError(o);
		}
		return texture2D;
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x000658A0 File Offset: 0x00063AA0
	public static Texture2D GetAchievementIconTexture(int id, bool unlocked)
	{
		string text = id.ToString();
		if (text.Length < 2)
		{
			text = "0" + id.ToString();
		}
		text = "Achievements/Achievement_Icon_" + text;
		if (!unlocked)
		{
			text += "_Locked";
		}
		Texture2D texture2D = Resources.Load(text) as Texture2D;
		if (texture2D == null)
		{
			string o = string.Format("HUD_Player failed to load flag \"{0}\"", text);
			PLog.LogError(o);
		}
		return texture2D;
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0006591C File Offset: 0x00063B1C
	public static Texture2D GetShopIconTexture(string name)
	{
		Texture2D texture2D = Resources.Load("ShopImages/" + name + "_icon") as Texture2D;
		if (texture2D == null)
		{
			PLog.LogWarning("Missing shop item icon " + name);
		}
		return texture2D;
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x00065964 File Offset: 0x00063B64
	public static Texture2D GetShopImageTexture(string name)
	{
		Texture2D texture2D = Resources.Load("ShopImages/" + name + "_image") as Texture2D;
		if (texture2D == null)
		{
			PLog.LogWarning("Missing shop item image " + name);
		}
		return texture2D;
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x000659AC File Offset: 0x00063BAC
	public static Texture2D GetArmamentThumbnail(string prefabName)
	{
		return Resources.Load("ArmamentThumbnails/ArmamentThumb_" + prefabName) as Texture2D;
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x000659D0 File Offset: 0x00063BD0
	public static Texture2D GetShipThumbnail(string prefabName)
	{
		return Resources.Load("ClassImages/ClassImage" + prefabName) as Texture2D;
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x000659F4 File Offset: 0x00063BF4
	public static Texture2D GetShipSilhouette(string prefabName)
	{
		return Resources.Load("ClassSilouettes/Silouette" + prefabName) as Texture2D;
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x00065A18 File Offset: 0x00063C18
	public static Texture2D GetProfileShipSilhouette(string prefabName)
	{
		return Resources.Load("Renders/Class/render_" + prefabName) as Texture2D;
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x00065A3C File Offset: 0x00063C3C
	public static Texture2D GetProfileArmamentThumbnail(string prefabName)
	{
		return Resources.Load("Renders/Modules/render_" + prefabName) as Texture2D;
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x00065A60 File Offset: 0x00063C60
	public static void SetImage(SimpleSprite sprite, string name)
	{
		Texture2D texture2D = Resources.Load(name) as Texture2D;
		if (texture2D == null)
		{
			PLog.LogWarning("Failed to load texture " + name);
			return;
		}
		GuiUtils.SetImage(sprite, texture2D);
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x00065AA0 File Offset: 0x00063CA0
	public static void SetImage(SimpleSprite sprite, Texture2D f)
	{
		float width = sprite.width;
		float height = sprite.height;
		float x = (float)f.width;
		float y = (float)f.height;
		sprite.SetTexture(f);
		sprite.Setup(width, height, new Vector2(0f, y), new Vector2(x, y));
		sprite.UpdateUVs();
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x00065AF4 File Offset: 0x00063CF4
	public static void SetButtonImage(UIButton uiButton, Texture2D tex)
	{
		float width = uiButton.width;
		float height = uiButton.height;
		uiButton.SetTexture(tex);
		foreach (UVAnimation uvanimation in uiButton.animations)
		{
			SPRITE_FRAME[] array = new SPRITE_FRAME[]
			{
				uvanimation.GetFrame(0)
			};
			array[0].uvs.Set(0f, 0f, 1f, 1f);
			uvanimation.SetAnim(array);
		}
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x00065B84 File Offset: 0x00063D84
	public static void SetButtonImageSheet(UIButton uiButton, Texture2D tex)
	{
		uiButton.SetTexture(tex);
		int num = 0;
		float width = uiButton.width;
		float height = uiButton.height;
		int width2 = tex.width;
		int height2 = tex.height;
		float num2 = width / (float)width2;
		float num3 = height / (float)height2;
		foreach (UVAnimation uvanimation in uiButton.animations)
		{
			float left = num2 * (float)num;
			SPRITE_FRAME[] array = new SPRITE_FRAME[]
			{
				uvanimation.GetFrame(0)
			};
			array[0].uvs.Set(left, 1f - num3, num2, num3);
			uvanimation.SetAnim(array);
			num++;
		}
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x00065C40 File Offset: 0x00063E40
	public static GameObject OpenInputDialog(GameObject guiCamera, string title, string text, GenericTextInput.InputTextCancel cancel, GenericTextInput.InputTextCommit ok)
	{
		GameObject gameObject = GuiUtils.CreateGui("GenericInputDialog", guiCamera);
		GenericTextInput component = gameObject.GetComponent<GenericTextInput>();
		DebugUtils.Assert(component != null, "Failed to create GenericTextInput, prefab does not have a GenericTextInput-script on it!");
		component.Initialize(title, "$button_cancel", "$button_ok", text, cancel, ok);
		component.AllowEmptyInput = false;
		return gameObject;
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x00065C90 File Offset: 0x00063E90
	public static GameObject OpenAlertDialog(GameObject guiCamera, string title, string text)
	{
		GameObject gameObject = GuiUtils.CreateGui("dialogs/Dialog_Alert", guiCamera);
		GuiUtils.FindChildOf(gameObject.transform, "Header").GetComponent<SpriteText>().Text = title;
		GuiUtils.FindChildOf(gameObject.transform, "Message").GetComponent<SpriteText>().Text = text;
		return gameObject;
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x00065CE0 File Offset: 0x00063EE0
	public static GameObject OpenMultiChoiceDialog(GameObject guiCamera, string text, EZValueChangedDelegate cancel, EZValueChangedDelegate nosave, EZValueChangedDelegate save)
	{
		GameObject gameObject = GuiUtils.CreateGui("MsgBoxMultichoice", guiCamera);
		GuiUtils.FindChildOf(gameObject.transform, "TextLabel").GetComponent<SpriteText>().Text = text;
		GuiUtils.FindChildOf(gameObject.transform, "BtnCancel").GetComponent<UIButton>().AddValueChangedDelegate(cancel);
		GuiUtils.FindChildOf(gameObject.transform, "BtnDontSave").GetComponent<UIButton>().AddValueChangedDelegate(nosave);
		GuiUtils.FindChildOf(gameObject.transform, "BtnSave").GetComponent<UIButton>().AddValueChangedDelegate(save);
		return gameObject;
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x00065D68 File Offset: 0x00063F68
	public static bool HasPointerRecursive(UIManager uiMan, GameObject root)
	{
		IUIObject component = root.GetComponent<AutoSpriteControlBase>();
		if (component == null)
		{
			component = root.GetComponent<UIScrollList>();
		}
		POINTER_INFO pointer_INFO;
		if (component != null && uiMan.GetPointer(component, out pointer_INFO))
		{
			return true;
		}
		int childCount = root.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (GuiUtils.HasPointerRecursive(uiMan, root.transform.GetChild(i).gameObject))
			{
				return true;
			}
		}
		return false;
	}
}
