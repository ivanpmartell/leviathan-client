using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class GameCamera : MonoBehaviour
{
	// Token: 0x06000769 RID: 1897 RVA: 0x0003728C File Offset: 0x0003548C
	private void Start()
	{
		AudioListener component = base.gameObject.transform.GetChild(0).GetComponent<AudioListener>();
		component.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
		this.m_terrainMask = (1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("Default"));
		this.m_markersMask = 1 << LayerMask.NameToLayer("markers");
		this.m_pickMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("units") | 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("Water"));
		int num = ~(1 << LayerMask.NameToLayer("low_vis"));
		base.camera.cullingMask = (base.camera.cullingMask & num);
		base.camera.depthTextureMode = DepthTextureMode.Depth;
		if (this.m_effectPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_effectPrefab, base.gameObject.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = base.transform;
		}
		this.UpdateClipPlanes();
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x000373BC File Offset: 0x000355BC
	private void OnDestroy()
	{
		this.CloseStatusWindow_Ship();
		this.CloseStatusWindow_HPModule();
		this.CloseFlowerMenu();
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x000373D0 File Offset: 0x000355D0
	public void SetMode(GameCamera.Mode mode)
	{
		this.m_mode = mode;
		this.CloseFlowerMenu();
		if (mode == GameCamera.Mode.Passive || mode == GameCamera.Mode.Disabled)
		{
			this.m_contextMenu = null;
		}
		if (mode == GameCamera.Mode.Active || mode == GameCamera.Mode.Passive)
		{
			this.RestoreNetObjSelection();
		}
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00037408 File Offset: 0x00035608
	public GameCamera.Mode GetMode()
	{
		return this.m_mode;
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00037410 File Offset: 0x00035610
	public void Setup(int localPlayerID, float levelSize, GameObject guiCamera)
	{
		this.m_guiCamera = guiCamera;
		this.m_localPlayerID = localPlayerID;
		this.m_levelSize = levelSize;
		float num = Mathf.Tan(0.017453292f * base.camera.fieldOfView * 0.5f);
		if (this.m_firstSetup)
		{
			this.Zoom(-1000f, base.transform.forward, 1f);
			this.m_firstSetup = false;
		}
		if (this.m_localPlayerID >= 0)
		{
			this.RestoreNetObjSelection();
			this.SetHovered(null);
		}
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00037498 File Offset: 0x00035698
	private void OnGUI()
	{
		if (this.m_mode == GameCamera.Mode.Disabled)
		{
			return;
		}
		if (this.m_hover != null && Debug.isDebugBuild)
		{
			int num = 0;
			if (CheatMan.instance.DebugAi())
			{
				num = 300;
			}
			Unit component = this.m_hover.GetComponent<Unit>();
			if (component != null)
			{
				GUI.TextField(new Rect(Input.mousePosition.x + 10f, (float)Screen.height - Input.mousePosition.y - 25f, (float)(160 + num), (float)(num + 80)), component.GetTooltip());
			}
			HPModule component2 = this.m_hover.GetComponent<HPModule>();
			if (component2 != null)
			{
				GUI.TextField(new Rect(Input.mousePosition.x + 10f, (float)Screen.height - Input.mousePosition.y - 25f, (float)(200 + num), (float)(num + 70)), component2.GetTooltip());
			}
		}
		if (this.m_contextMenu != null)
		{
			this.m_contextMenu.DrawGui(base.camera);
		}
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x000375CC File Offset: 0x000357CC
	private void CursorRayCast(Ray ray, out GameObject markerObject, out GameObject hitObject, out Vector3 hitPoint)
	{
		hitObject = null;
		markerObject = null;
		hitPoint = Vector3.zero;
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 10000f, this.m_markersMask))
		{
			markerObject = raycastHit.collider.gameObject;
			if (markerObject.transform.parent != null && markerObject.transform.parent.GetComponent<OrderMarker>() != null)
			{
				markerObject = markerObject.transform.parent.gameObject;
			}
			hitPoint = raycastHit.point;
		}
		float num = 9999999f;
		RaycastHit[] array = Physics.RaycastAll(ray, 10000f, this.m_pickMask);
		foreach (RaycastHit raycastHit2 in array)
		{
			GameObject gameObject = raycastHit2.collider.gameObject;
			if (gameObject.GetComponent<HPModule>() == null)
			{
				Section component = gameObject.GetComponent<Section>();
				if (component != null)
				{
					gameObject = component.GetUnit().gameObject;
				}
				if (component == null && gameObject.GetComponent<Unit>() == null && gameObject.collider != null && gameObject.collider.attachedRigidbody)
				{
					gameObject = gameObject.collider.attachedRigidbody.gameObject;
				}
			}
			NetObj component2 = gameObject.GetComponent<NetObj>();
			if (!(component2 != null) || component2.IsVisible())
			{
				HPModule component3 = gameObject.GetComponent<HPModule>();
				if (component3 != null)
				{
					gameObject = component3.GetUnit().gameObject;
				}
				if (raycastHit2.distance < num)
				{
					hitObject = gameObject;
					hitPoint = raycastHit2.point;
					num = raycastHit2.distance;
				}
			}
		}
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x000377B8 File Offset: 0x000359B8
	public void Update()
	{
		if (this.m_contextMenu != null)
		{
			this.m_contextMenu.Update(Time.deltaTime);
		}
		if (this.m_flowerMenu != null)
		{
			this.m_flowerMenu.Update(Time.deltaTime);
		}
		this.UpdateShake(Time.deltaTime);
		if (this.m_hasFocusPos)
		{
			this.UpdateFocus(Time.deltaTime);
			return;
		}
		if (this.m_mode != GameCamera.Mode.Disabled)
		{
			Ray ray = base.camera.ScreenPointToRay(Input.mousePosition);
			GameObject gameObject;
			GameObject gameObject2;
			Vector3 vector;
			this.CursorRayCast(ray, out gameObject, out gameObject2, out vector);
			if (gameObject2 == null)
			{
				this.SetHovered(null);
				return;
			}
			this.m_targetPos = vector;
			this.SetHovered(gameObject2);
			gameObject2 = ((!(gameObject != null)) ? gameObject2 : gameObject);
			UIManager component = this.m_guiCamera.GetComponent<UIManager>();
			bool flag = component.DidAnyPointerHitUI() || (this.m_contextMenu != null && this.m_contextMenu.IsMouseOver());
			bool flag2 = this.m_flowerMenu != null && this.m_flowerMenu.IsMouseOver();
			if (!flag)
			{
				if (Input.touchCount <= 1)
				{
					if (!this.m_draging)
					{
						if (Input.GetMouseButtonDown(0))
						{
							this.PrepareDraging(vector, Input.mousePosition, gameObject2);
						}
						if (Input.GetMouseButton(0))
						{
							float num = Vector3.Distance(this.m_dragStartMousePos, Input.mousePosition);
							if (num > 10f)
							{
								this.StartDraging(Input.mousePosition);
							}
						}
						if (Input.GetMouseButtonUp(0))
						{
							this.OnMouseReleased(vector, gameObject2);
						}
					}
					else
					{
						if (Input.GetMouseButton(0))
						{
							Vector3 mouseDelta = Input.mousePosition - this.m_dragLastPos;
							this.m_dragLastPos = Input.mousePosition;
							if (this.m_dragObject != null)
							{
								this.OnDragUpdate(this.m_dragStart, vector, mouseDelta, this.m_dragObject);
							}
						}
						if (Input.GetMouseButtonUp(0) && this.m_draging)
						{
							this.m_draging = false;
							if (this.m_dragObject != null)
							{
								this.OnDragStoped(vector, this.m_dragObject);
							}
						}
					}
				}
				else
				{
					this.m_draging = false;
				}
				this.UpdatePinchZoom(ray);
			}
			if (!flag || flag2)
			{
				this.m_totalScrollZoomDelta += Input.GetAxis("Mouse ScrollWheel");
				if (Mathf.Abs(this.m_totalScrollZoomDelta) > 0.01f)
				{
					float num2 = this.m_totalScrollZoomDelta * 0.2f;
					this.m_totalScrollZoomDelta -= num2;
					this.Zoom(num2 * 100f, ray.direction, Time.deltaTime);
				}
			}
			if (component.FocusObject == null)
			{
				this.UpdateWASDControls(Time.deltaTime);
			}
			NetObj selectedNetObj = this.GetSelectedNetObj();
			if (selectedNetObj != null)
			{
				if (selectedNetObj.IsSeenByPlayer(this.m_localPlayerID))
				{
					if (this.m_statusWnd_Ship != null)
					{
						this.m_statusWnd_Ship.Update();
					}
					if (this.m_statusWnd_HPModule != null)
					{
						this.m_statusWnd_HPModule.Update();
					}
				}
				else
				{
					this.SetSelectedNetObj(null, false);
				}
			}
		}
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x00037AD8 File Offset: 0x00035CD8
	private void LateUpdate()
	{
		this.UpdateClipPlanes();
		if (this.m_flowerMenu != null)
		{
			this.m_flowerMenu.LateUpdate(Time.deltaTime);
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00037AFC File Offset: 0x00035CFC
	private void UpdateWASDControls(float dt)
	{
		Vector3 zero = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			zero.y -= this.m_keyboardScrollSpeed * dt;
		}
		if (Input.GetKey(KeyCode.A))
		{
			zero.x += this.m_keyboardScrollSpeed * dt;
		}
		if (Input.GetKey(KeyCode.S))
		{
			zero.y += this.m_keyboardScrollSpeed * dt;
		}
		if (Input.GetKey(KeyCode.D))
		{
			zero.x -= this.m_keyboardScrollSpeed * dt;
		}
		if (zero != Vector3.zero)
		{
			this.MoveCamera(zero);
		}
		float num = 0f;
		if (Input.GetKey(KeyCode.Q))
		{
			num -= this.m_keyboardZoomSpeed * dt;
		}
		if (Input.GetKey(KeyCode.E))
		{
			num += this.m_keyboardZoomSpeed * dt;
		}
		if (num != 0f)
		{
			this.Zoom(num, base.transform.forward, dt);
		}
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00037C00 File Offset: 0x00035E00
	private void UpdatePinchZoom(Ray ray)
	{
		if (Input.touchCount == 2 && !this.m_draging)
		{
			float pinchDistance = this.GetPinchDistance();
			if (!this.m_pinchZoom)
			{
				this.m_pinchZoom = true;
				this.m_pinchStartDistance = pinchDistance;
			}
			else
			{
				float zoomDelta = -(this.m_pinchStartDistance - pinchDistance) / 5f;
				this.m_pinchStartDistance = pinchDistance;
				this.Zoom(zoomDelta, ray.direction, Time.deltaTime);
			}
		}
		if (this.m_pinchZoom && Input.touchCount < 2)
		{
			this.m_pinchZoom = false;
		}
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00037C90 File Offset: 0x00035E90
	private void UpdateFocus(float dt)
	{
		Vector3 forward = base.transform.forward;
		Vector3 a = new Vector3(this.m_focusPos.x, 0f, this.m_focusPos.y) - forward * this.m_focusHeight;
		Vector3 vector = base.transform.position;
		Vector3 a2 = a - vector;
		vector += a2 * dt * 4f;
		base.transform.position = vector;
		if (a2.magnitude < 10f)
		{
			this.m_hasFocusPos = false;
		}
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00037D2C File Offset: 0x00035F2C
	private void UpdateShake(float dt)
	{
		if (this.m_shakeTimer < 0f)
		{
			return;
		}
		this.m_shakeTimer += dt;
		if (this.m_shakeTimer >= this.m_shakeLength)
		{
			base.transform.rotation = this.m_shakeRot;
			this.m_shakeTimer = -1f;
			return;
		}
		float num = (1f - this.m_shakeTimer / this.m_shakeLength) * this.m_shakeIntensity * this.m_shakeScale;
		float num2 = 50f;
		float x = (Mathf.Sin(this.m_shakeTimer * num2) * num + Mathf.Cos(this.m_shakeTimer * num2 * 2f) * num * 0.2f) * this.m_shakeDirection.x;
		float y = (Mathf.Cos(this.m_shakeTimer * num2) * num + Mathf.Sin(this.m_shakeTimer * num2 * 2f) * num * 0.2f) * this.m_shakeDirection.y;
		base.transform.localRotation = this.m_shakeRot;
		base.transform.Rotate(new Vector3(x, y, 0f));
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00037E48 File Offset: 0x00036048
	public void AddShake(Vector3 origin, float intensity)
	{
		float num = Vector3.Distance(base.transform.position, origin);
		float intensity2 = intensity / (num / 100f);
		this.AddShake(intensity2);
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00037E78 File Offset: 0x00036078
	private void AddShake(float intensity)
	{
		if (this.m_shakeTimer >= 0f)
		{
			float num = 1f - this.m_shakeTimer / this.m_shakeLength;
			float intensity2 = this.m_shakeIntensity * num + intensity;
			this.Shake(intensity2);
		}
		else
		{
			this.Shake(intensity);
		}
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00037EC8 File Offset: 0x000360C8
	private void Shake(float intensity)
	{
		intensity = Mathf.Min(this.m_shakeMaxIntensity, intensity);
		if (this.m_shakeTimer >= 0f)
		{
			base.transform.localRotation = this.m_shakeRot;
		}
		else
		{
			this.m_shakeDirection = new Vector3(Mathf.Sign(UnityEngine.Random.value - 0.5f), Mathf.Sign(UnityEngine.Random.value - 0.5f), Mathf.Sign(UnityEngine.Random.value - 0.5f));
			this.m_shakeRot = base.transform.localRotation;
		}
		this.m_shakeTimer = 0f;
		this.m_shakeIntensity = intensity;
		this.m_shakeLength = intensity;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00037F70 File Offset: 0x00036170
	public void SetFocus(Vector3 pos, float height)
	{
		this.SetSelected(null);
		this.m_hasFocusPos = true;
		this.m_focusPos = new Vector2(pos.x, pos.z);
		this.m_focusHeight = height;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00037FAC File Offset: 0x000361AC
	private void PrepareDraging(Vector3 hitPos, Vector3 mousePos, GameObject go)
	{
		this.m_dragStart = hitPos;
		this.m_dragStartMousePos = mousePos;
		this.m_dragObject = go;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00037FC4 File Offset: 0x000361C4
	private void StartDraging(Vector3 mousePos)
	{
		this.m_draging = true;
		this.m_flowerMenuFireDrag = false;
		this.m_dragLastPos = mousePos;
		if (this.m_dragObject != null)
		{
			this.OnDragStarted(this.m_dragStart, this.m_dragStartMousePos, this.m_dragObject);
		}
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00038010 File Offset: 0x00036210
	private void OnMouseReleased(Vector3 pos, GameObject go)
	{
		if (!this.m_allowSelection)
		{
			return;
		}
		this.SetSelected(go);
		OrderMarker component = go.GetComponent<OrderMarker>();
		if (component)
		{
			this.OpenOrderContextMenu(component);
		}
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0003804C File Offset: 0x0003624C
	private void OnDragStarted(Vector3 pos, Vector3 mousePos, GameObject go)
	{
		if ((1 << go.layer & this.m_terrainMask) == 0)
		{
			this.SetSelected(go);
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0003806C File Offset: 0x0003626C
	private void OnDragUpdate(Vector3 startPos, Vector3 pos, Vector3 mouseDelta, GameObject go)
	{
		if ((1 << go.layer & this.m_terrainMask) != 0)
		{
			this.MoveCamera(mouseDelta);
		}
		if (!this.m_allowSelection)
		{
			return;
		}
		if (this.m_mode == GameCamera.Mode.Active)
		{
			OrderMarker component = go.GetComponent<OrderMarker>();
			if (component != null)
			{
				this.MoveOrder(component, pos);
			}
		}
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x000380CC File Offset: 0x000362CC
	private void MoveCamera(Vector3 moveDelta)
	{
		Vector3 vector = -moveDelta * this.m_mouseMoveSpeed * base.transform.position.y;
		Vector3 vector2 = base.transform.position;
		vector2 += new Vector3(vector.x, 0f, vector.y);
		vector2.x = Mathf.Clamp(vector2.x, -this.m_levelSize / 2f, this.m_levelSize / 2f);
		float num = this.m_zBorderOffset * vector2.y;
		vector2.z = Mathf.Clamp(vector2.z, -this.m_levelSize / 2f + num, this.m_levelSize / 2f + num);
		base.transform.position = vector2;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x000381A8 File Offset: 0x000363A8
	private void OnDragStoped(Vector3 pos, GameObject go)
	{
		if (!this.m_allowSelection)
		{
			return;
		}
		OrderMarker component = go.GetComponent<OrderMarker>();
		if (component)
		{
			if (this.m_flowerMenuFireDrag)
			{
				Gun gun = component.GetOrder().GetOwner() as Gun;
				if (gun != null)
				{
					Unit unit = gun.GetUnit();
					this.SetSelected(unit.gameObject);
				}
			}
			else
			{
				this.OpenOrderContextMenu(component);
			}
		}
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0003821C File Offset: 0x0003641C
	public FlowerMenu GetFlowerMenu()
	{
		return this.m_flowerMenu;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00038224 File Offset: 0x00036424
	private GameCamera.TargetType GetTargetType(GameObject obj)
	{
		if (obj == null)
		{
			return GameCamera.TargetType.None;
		}
		Unit component = obj.GetComponent<Unit>();
		if (component != null)
		{
			return GameCamera.TargetType.Unit;
		}
		HPModule component2 = obj.GetComponent<HPModule>();
		if (component2 != null)
		{
			return GameCamera.TargetType.HPModule;
		}
		return GameCamera.TargetType.Land;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x0003826C File Offset: 0x0003646C
	public void SetSelected(GameObject selected)
	{
		if (!this.m_allowSelection)
		{
			return;
		}
		if (selected == null)
		{
			this.SetSelectedOrder(null);
			this.SetSelectedNetObj(null, false);
		}
		else if (selected.GetComponent<OrderMarker>() != null)
		{
			OrderMarker component = selected.GetComponent<OrderMarker>();
			MonoBehaviour monoBehaviour = component.GetOrder().GetOwner() as MonoBehaviour;
			if (monoBehaviour != null)
			{
				this.SetSelectedNetObj(monoBehaviour.gameObject, false);
			}
			else
			{
				this.SetSelectedNetObj(null, false);
			}
			this.SetSelectedOrder(selected);
		}
		else
		{
			this.SetSelectedOrder(null);
			this.SetSelectedNetObj(selected, true);
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00038318 File Offset: 0x00036518
	private bool SetSelectedOrder(GameObject selected)
	{
		if (this.m_selectedOrder != null)
		{
			OrderMarker component = this.m_selectedOrder.GetComponent<OrderMarker>();
			if (component != null && this.m_selectedOrder != selected)
			{
				component.SetSelected(false);
			}
		}
		this.CloseFlowerMenu();
		this.m_contextMenu = null;
		this.m_selectedOrder = null;
		this.m_settingMoveOrderFacing = false;
		if (selected != null)
		{
			OrderMarker component2 = selected.GetComponent<OrderMarker>();
			if (component2 != null)
			{
				component2.SetSelected(true);
				this.m_selectedOrder = selected;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x000383B4 File Offset: 0x000365B4
	private void RestoreNetObjSelection()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		if (selectedNetObj != null)
		{
			this.SetSelectedNetObj(selectedNetObj.gameObject, this.m_selectedNetObjExplicit);
		}
		else
		{
			this.SetSelectedNetObj(null, this.m_selectedNetObjExplicit);
		}
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x000383FC File Offset: 0x000365FC
	private bool SetSelectedNetObj(GameObject selected, bool explicitSelected)
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		if (selectedNetObj != null)
		{
			Unit component = selectedNetObj.GetComponent<Unit>();
			if (component != null)
			{
				component.SetSelected(false, explicitSelected);
			}
			HPModule component2 = selectedNetObj.GetComponent<HPModule>();
			if (component2 != null)
			{
				component2.SetSelected(false, explicitSelected);
			}
		}
		this.m_contextMenu = null;
		this.m_selectedNetID = -1;
		this.CloseStatusWindow_HPModule();
		this.CloseStatusWindow_Ship();
		this.CloseFlowerMenu();
		if (selected)
		{
			this.m_selectedNetObjExplicit = explicitSelected;
			HPModule component3 = selected.GetComponent<HPModule>();
			if (component3 != null && component3.IsSeenByPlayer(this.m_localPlayerID))
			{
				this.m_selectedNetID = component3.GetNetID();
				component3.SetSelected(true, explicitSelected);
				if (explicitSelected)
				{
					this.OpenModuleContextMenu(component3, true);
				}
				return true;
			}
			Unit component4 = selected.GetComponent<Unit>();
			if (!component4 && selected.collider.attachedRigidbody)
			{
				component4 = selected.collider.attachedRigidbody.GetComponent<Unit>();
			}
			if (component4 != null && !component4.IsDead() && component4.IsSeenByPlayer(this.m_localPlayerID))
			{
				this.m_selectedNetID = component4.GetNetID();
				component4.SetSelected(true, explicitSelected);
				if (explicitSelected)
				{
					this.OpenUnitContextMenu(component4);
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0003855C File Offset: 0x0003675C
	private void SetHovered(GameObject hover)
	{
		if (hover != null)
		{
			NetObj component = hover.GetComponent<NetObj>();
			if (component != null && !component.IsVisible())
			{
				return;
			}
			if (this.m_hover == hover)
			{
				return;
			}
			if (this.m_hover)
			{
				this.SetHighlight(this.m_hover, false);
				this.m_hover = null;
			}
			this.m_hover = hover;
			this.m_hoverType = this.GetTargetType(this.m_hover);
			this.SetHighlight(this.m_hover, true);
		}
		else if (this.m_hover)
		{
			this.SetHighlight(this.m_hover, false);
			this.m_hover = null;
		}
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x0003861C File Offset: 0x0003681C
	private void SetupFlowerMenu(Ship ship)
	{
		if (this.m_flowerMenu != null && this.m_flowerMenu.GetShip() != ship)
		{
			this.CloseFlowerMenu();
		}
		if (this.m_flowerMenu == null)
		{
			bool canOrder = ship.GetOwner() == this.m_localPlayerID && this.m_mode == GameCamera.Mode.Active;
			bool localOwner = ship.GetOwner() == this.m_localPlayerID;
			this.m_flowerMenu = new FlowerMenu(base.camera, this.m_guiCamera, ship, canOrder, localOwner);
			this.m_flowerMenu.m_onModuleSelected = new Action<HPModule>(this.OnFlowerMenuSelect);
			this.m_flowerMenu.m_onModuleDragged = new Action<HPModule>(this.OnFlowerMenuDragged);
			this.m_flowerMenu.m_onMoveForward = new Action<Ship>(this.OnFlowerMenuMoveForward);
			this.m_flowerMenu.m_onMoveReverse = new Action<Ship>(this.OnFlowerMenuMoveReverse);
			this.m_flowerMenu.m_onMoveRotate = new Action<Ship>(this.OnFlowerMenuMoveRotate);
			this.m_flowerMenu.m_onToggleSupply = new Action<Ship>(this.OnFlowerMenuSupplyToggle);
		}
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0003872C File Offset: 0x0003692C
	private void OnFlowerMenuSelect(HPModule module)
	{
		this.CloseFlowerMenu();
		this.SetSelected(module.gameObject);
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00038740 File Offset: 0x00036940
	private void OnFlowerMenuDragged(HPModule module)
	{
		if (this.m_mode != GameCamera.Mode.Active)
		{
			return;
		}
		if (module.GetOwner() != this.m_localPlayerID)
		{
			return;
		}
		this.CloseFlowerMenu();
		this.SetSelected(module.gameObject);
		if (module is Gun)
		{
			this.OnSetGunTarget();
			this.m_flowerMenuFireDrag = true;
		}
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00038798 File Offset: 0x00036998
	private void OnFlowerMenuMoveForward(Ship ship)
	{
		if (!this.m_allowFlowerMenu)
		{
			return;
		}
		if (this.m_mode != GameCamera.Mode.Active)
		{
			return;
		}
		if (ship.GetOwner() != this.m_localPlayerID)
		{
			return;
		}
		this.CloseFlowerMenu();
		this.OnMoveForward(ship);
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x000387D4 File Offset: 0x000369D4
	private void OnFlowerMenuMoveReverse(Ship ship)
	{
		if (!this.m_allowFlowerMenu)
		{
			return;
		}
		if (this.m_mode != GameCamera.Mode.Active)
		{
			return;
		}
		if (ship.GetOwner() != this.m_localPlayerID)
		{
			return;
		}
		this.CloseFlowerMenu();
		this.OnMoveBackward(ship);
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x00038810 File Offset: 0x00036A10
	private void OnFlowerMenuMoveRotate(Ship ship)
	{
		if (!this.m_allowFlowerMenu)
		{
			return;
		}
		if (this.m_mode != GameCamera.Mode.Active)
		{
			return;
		}
		if (ship.GetOwner() != this.m_localPlayerID)
		{
			return;
		}
		this.CloseFlowerMenu();
		this.OnMoveRotate(ship);
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x0003884C File Offset: 0x00036A4C
	private void OnFlowerMenuSupplyToggle(Ship ship)
	{
		if (this.m_mode != GameCamera.Mode.Active)
		{
			return;
		}
		if (ship.GetOwner() != this.m_localPlayerID)
		{
			return;
		}
		SupportShip supportShip = ship as SupportShip;
		if (supportShip == null)
		{
			return;
		}
		supportShip.SetSupplyEnabled(!supportShip.IsSupplyEnabled());
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0003889C File Offset: 0x00036A9C
	private void CloseFlowerMenu()
	{
		if (this.m_flowerMenu != null)
		{
			this.m_flowerMenu.Close();
			this.m_flowerMenu = null;
		}
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x000388BC File Offset: 0x00036ABC
	private void OpenUnitContextMenu(Unit unit)
	{
		bool flag = unit.GetOwner() == this.m_localPlayerID;
		Ship ship = unit as Ship;
		if (ship != null)
		{
			this.SetupFlowerMenu(ship);
		}
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x000388F4 File Offset: 0x00036AF4
	private void OpenModuleContextMenu(HPModule module, bool updateStatusWindow)
	{
		bool flag = module.GetOwner() == this.m_localPlayerID && this.m_mode == GameCamera.Mode.Active && !module.GetUnit().IsDoingMaintenance();
		float d = Vector3.Distance(base.transform.position, module.transform.position) * 0.006f;
		if (flag)
		{
			Radar radar = module as Radar;
			if (radar != null)
			{
				this.m_contextMenu = new global::ContextMenu(this.m_guiCamera.camera);
				if (!radar.GetDeploy())
				{
					this.m_contextMenu.AddClickButton(this.m_deployRadarIcon, Localize.instance.Translate("$radar_deploy"), module.transform.position, false, new global::ContextMenu.ButtonHandler(this.OnModuleDeploy));
				}
				else
				{
					this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$button_cancel"), module.transform.position, false, new global::ContextMenu.ButtonHandler(this.OnModuleAbortDeploy));
				}
			}
			Cloak cloak = module as Cloak;
			if (cloak != null)
			{
				this.m_contextMenu = new global::ContextMenu(this.m_guiCamera.camera);
				if (!cloak.GetDeploy())
				{
					this.m_contextMenu.AddClickButton(this.m_deployCloakIcon, Localize.instance.Translate("$cloak_deploy"), module.transform.position, false, new global::ContextMenu.ButtonHandler(this.OnModuleDeploy));
				}
				else
				{
					this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$button_cancel"), module.transform.position, false, new global::ContextMenu.ButtonHandler(this.OnModuleAbortDeploy));
				}
			}
			Shield shield = module as Shield;
			if (shield != null)
			{
				this.m_contextMenu = new global::ContextMenu(this.m_guiCamera.camera);
				Shield.DeployType deployShield = shield.GetDeployShield();
				Vector3 position = shield.transform.position;
				Vector3 b = shield.GetUnit().transform.forward * d * 5f;
				Vector3 b2 = shield.GetUnit().transform.right * d * 5f;
				if (deployShield == Shield.DeployType.Forward)
				{
					this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$button_cancel"), position + b, false, new global::ContextMenu.ButtonHandler(this.OnShieldAbortDeploy));
				}
				else
				{
					this.m_contextMenu.AddClickButton(this.m_deployShieldForwardIcon, Localize.instance.Translate("$shield_dep_front"), position + b, false, new global::ContextMenu.ButtonHandler(this.OnShieldDeployForward));
				}
				if (deployShield == Shield.DeployType.Backward)
				{
					this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$button_cancel"), position - b, false, new global::ContextMenu.ButtonHandler(this.OnShieldAbortDeploy));
				}
				else
				{
					this.m_contextMenu.AddClickButton(this.m_deployShieldBackwardIcon, Localize.instance.Translate("$shield_dep_back"), position - b, false, new global::ContextMenu.ButtonHandler(this.OnShieldDeployBackward));
				}
				if (deployShield == Shield.DeployType.Left)
				{
					this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$button_cancel"), position - b2, false, new global::ContextMenu.ButtonHandler(this.OnShieldAbortDeploy));
				}
				else
				{
					this.m_contextMenu.AddClickButton(this.m_deployShieldLeftIcon, Localize.instance.Translate("$shield_dep_left"), position - b2, false, new global::ContextMenu.ButtonHandler(this.OnShieldDeployLeft));
				}
				if (deployShield == Shield.DeployType.Right)
				{
					this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$button_cancel"), position + b2, false, new global::ContextMenu.ButtonHandler(this.OnShieldAbortDeploy));
				}
				else
				{
					this.m_contextMenu.AddClickButton(this.m_deployShieldRightIcon, Localize.instance.Translate("$shield_dep_right"), position + b2, false, new global::ContextMenu.ButtonHandler(this.OnShieldDeployRight));
				}
			}
			Gun gun = module as Gun;
			if (gun != null)
			{
				this.m_contextMenu = new global::ContextMenu(this.m_guiCamera.camera);
				if (!gun.m_canDeploy)
				{
					if (gun.m_aim.m_manualTarget)
					{
						this.m_contextMenu.AddDragButton(this.m_targetIcon, Localize.instance.Translate("$gun_target"), gun.transform.TransformPoint(new Vector3(0f, 0f, 4f) * d), new global::ContextMenu.ButtonHandler(this.OnSetGunTarget));
					}
					Gun.Stance stance = gun.GetStance();
					if (stance != Gun.Stance.FireAtWill)
					{
						if (stance == Gun.Stance.HoldFire)
						{
							this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$gun_holdfire"), gun.transform.TransformPoint(new Vector3(4f, 0f, 0f) * d), false, new global::ContextMenu.ButtonHandler(this.OnToggleGunStance));
						}
					}
					else
					{
						this.m_contextMenu.AddClickButton(this.m_fireAtWillIcon, Localize.instance.Translate("$gun_fireatwill"), gun.transform.TransformPoint(new Vector3(4f, 0f, 0f) * d), false, new global::ContextMenu.ButtonHandler(this.OnToggleGunStance));
					}
				}
				else if (!gun.GetDeploy())
				{
					this.m_contextMenu.AddClickButton(this.m_deployIcon, Localize.instance.Translate("$gun_deploy"), gun.transform.TransformPoint(new Vector3(0f, 0f, 5f) * d), false, new global::ContextMenu.ButtonHandler(this.OnModuleDeploy));
				}
				else
				{
					this.m_contextMenu.AddClickButton(this.m_holdFireIcon, Localize.instance.Translate("$gun_holdfire"), gun.transform.TransformPoint(new Vector3(0f, 0f, 5f) * d), false, new global::ContextMenu.ButtonHandler(this.OnModuleAbortDeploy));
				}
			}
		}
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x00038F38 File Offset: 0x00037138
	private void OpenOrderContextMenu(OrderMarker marker)
	{
		if (this.m_mode != GameCamera.Mode.Active)
		{
			return;
		}
		Order order = marker.GetOrder();
		float d = Vector3.Distance(base.transform.position, marker.transform.position) * 0.006f;
		float num = 5f;
		this.m_contextMenu = new global::ContextMenu(this.m_guiCamera.camera);
		this.m_contextMenu.AddClickButton(this.m_removeIcon, Localize.instance.Translate("$context_remove"), marker.transform.position + new Vector3(-num, 0f, num) * d, false, new global::ContextMenu.ButtonHandler(this.OnRemoveOrder));
		if (order.m_type == Order.Type.MoveForward || order.m_type == Order.Type.MoveBackward)
		{
			IOrderable owner = order.GetOwner();
			if (owner.IsLastOrder(order))
			{
				this.m_contextMenu.AddDragButton(this.m_addIcon, Localize.instance.Translate("$context_add"), marker.transform.position + new Vector3(num, 0f, num) * d, new global::ContextMenu.ButtonHandler(this.OnAddMoveOrder));
			}
			this.m_contextMenu.AddDragButton(this.m_facingIcon, Localize.instance.Translate("$waypoint_rotate"), marker.transform.position + new Vector3(-num, 0f, -num) * d, new global::ContextMenu.ButtonHandler(this.OnSetMoveOrderFaceing));
			this.m_contextMenu.AddClickButton(this.m_resetFacingIcon, Localize.instance.Translate("$waypoint_resetrot"), marker.transform.position + new Vector3(num, 0f, -num) * d, false, new global::ContextMenu.ButtonHandler(this.OnResetMoveOrderFaceing));
		}
		if (order.m_type == Order.Type.Fire)
		{
			this.m_contextMenu.AddDragButton(this.m_addIcon, Localize.instance.Translate("$context_add"), marker.transform.position + new Vector3(num, 0f, num) * d, new global::ContextMenu.ButtonHandler(this.OnAddFireOrder));
		}
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0003915C File Offset: 0x0003735C
	private void OnShieldDeployForward()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		Shield component = selectedNetObj.GetComponent<Shield>();
		if (component == null)
		{
			return;
		}
		component.SetDeployShield(Shield.DeployType.Forward);
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x00039194 File Offset: 0x00037394
	private void OnShieldDeployBackward()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		Shield component = selectedNetObj.GetComponent<Shield>();
		if (component == null)
		{
			return;
		}
		component.SetDeployShield(Shield.DeployType.Backward);
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x000391CC File Offset: 0x000373CC
	private void OnShieldDeployLeft()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		Shield component = selectedNetObj.GetComponent<Shield>();
		if (component == null)
		{
			return;
		}
		component.SetDeployShield(Shield.DeployType.Left);
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x00039204 File Offset: 0x00037404
	private void OnShieldDeployRight()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		Shield component = selectedNetObj.GetComponent<Shield>();
		if (component == null)
		{
			return;
		}
		component.SetDeployShield(Shield.DeployType.Right);
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0003923C File Offset: 0x0003743C
	private void OnShieldAbortDeploy()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		Shield component = selectedNetObj.GetComponent<Shield>();
		if (component == null)
		{
			return;
		}
		component.SetDeployShield(Shield.DeployType.None);
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00039274 File Offset: 0x00037474
	private void OnModuleDeploy()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		HPModule component = selectedNetObj.GetComponent<HPModule>();
		if (component == null)
		{
			return;
		}
		component.SetDeploy(true);
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x000392AC File Offset: 0x000374AC
	private void OnModuleAbortDeploy()
	{
		NetObj selectedNetObj = this.GetSelectedNetObj();
		HPModule component = selectedNetObj.GetComponent<HPModule>();
		if (component == null)
		{
			return;
		}
		component.SetDeploy(false);
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x000392E4 File Offset: 0x000374E4
	private void OnSetGunTarget()
	{
		this.m_contextMenu = null;
		NetObj selectedNetObj = this.GetSelectedNetObj();
		Gun component = selectedNetObj.GetComponent<Gun>();
		if (component == null)
		{
			return;
		}
		if (!component.m_aim.m_manualTarget)
		{
			return;
		}
		Order order = new Order(component, Order.Type.Fire, this.m_targetPos);
		order.SetDisplayRadius(component.GetTargetRadius(this.m_targetPos));
		order.SetStaticTargetOnly(component.GetStaticTargetOnly());
		order.m_fireVisual = component.GetOrderMarkerType();
		component.ClearOrders();
		component.AddOrder(order);
		this.PrepareDraging(this.m_targetPos, Input.mousePosition, order.GetMarker());
		this.StartDraging(Input.mousePosition);
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0003938C File Offset: 0x0003758C
	private void OnToggleGunStance()
	{
		PLog.Log("toggle stance");
		NetObj selectedNetObj = this.GetSelectedNetObj();
		Gun component = selectedNetObj.GetComponent<Gun>();
		if (component == null)
		{
			return;
		}
		Gun.Stance stance = component.GetStance();
		if (stance != Gun.Stance.FireAtWill)
		{
			if (stance == Gun.Stance.HoldFire)
			{
				component.SetStance(Gun.Stance.FireAtWill);
			}
		}
		else
		{
			component.SetStance(Gun.Stance.HoldFire);
		}
		this.OpenModuleContextMenu(component, false);
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x000393F8 File Offset: 0x000375F8
	private void OnMoveForward(Unit unitScript)
	{
		Order order = new Order(unitScript, Order.Type.MoveForward, this.m_targetPos);
		unitScript.ClearMoveOrders();
		unitScript.AddOrder(order);
		this.PrepareDraging(this.m_targetPos, Input.mousePosition, order.GetMarker());
		this.StartDraging(Input.mousePosition);
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00039444 File Offset: 0x00037644
	private void OnMoveBackward(Unit unitScript)
	{
		Order order = new Order(unitScript, Order.Type.MoveBackward, this.m_targetPos);
		unitScript.ClearMoveOrders();
		unitScript.AddOrder(order);
		this.PrepareDraging(this.m_targetPos, Input.mousePosition, order.GetMarker());
		this.StartDraging(Input.mousePosition);
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00039490 File Offset: 0x00037690
	private void OnMoveRotate(Unit unitScript)
	{
		unitScript.ClearMoveOrders();
		Order order = new Order(unitScript, Order.Type.MoveRotate, unitScript.transform.position);
		order.SetFacing(unitScript.transform.forward);
		unitScript.AddOrder(order);
		this.PrepareDraging(this.m_targetPos, Input.mousePosition, order.GetMarker());
		this.StartDraging(Input.mousePosition);
		this.m_settingMoveOrderFacing = true;
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x000394F8 File Offset: 0x000376F8
	private void OnRemoveOrder()
	{
		this.m_contextMenu = null;
		OrderMarker component = this.m_selectedOrder.GetComponent<OrderMarker>();
		if (component == null)
		{
			return;
		}
		Order order = component.GetOrder();
		IOrderable owner = order.GetOwner();
		Unit unit = owner as Unit;
		HPModule hpmodule = owner as HPModule;
		if (unit != null)
		{
			unit.RemoveOrder(order);
		}
		if (hpmodule != null)
		{
			hpmodule.RemoveOrder(order);
		}
		this.SetSelectedOrder(null);
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x00039574 File Offset: 0x00037774
	private void OnSetMoveOrderFaceing()
	{
		this.m_contextMenu = null;
		if (this.m_selectedOrder == null)
		{
			return;
		}
		OrderMarker component = this.m_selectedOrder.GetComponent<OrderMarker>();
		if (component == null)
		{
			return;
		}
		this.PrepareDraging(this.m_targetPos, Input.mousePosition, this.m_selectedOrder);
		this.StartDraging(Input.mousePosition);
		this.m_settingMoveOrderFacing = true;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x000395DC File Offset: 0x000377DC
	private void OnResetMoveOrderFaceing()
	{
		this.m_contextMenu = null;
		if (this.m_selectedOrder == null)
		{
			return;
		}
		OrderMarker component = this.m_selectedOrder.GetComponent<OrderMarker>();
		if (component == null)
		{
			return;
		}
		component.GetOrder().ResetFacing();
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x00039628 File Offset: 0x00037828
	private void OnAddMoveOrder()
	{
		this.m_contextMenu = null;
		if (this.m_selectedOrder == null)
		{
			return;
		}
		OrderMarker component = this.m_selectedOrder.GetComponent<OrderMarker>();
		if (component == null)
		{
			return;
		}
		Order order = component.GetOrder();
		IOrderable owner = order.GetOwner();
		Order order2 = new Order(owner, order.m_type, this.m_targetPos);
		owner.AddOrder(order2);
		this.PrepareDraging(this.m_targetPos, Input.mousePosition, order2.GetMarker());
		this.StartDraging(Input.mousePosition);
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x000396B4 File Offset: 0x000378B4
	private void OnAddFireOrder()
	{
		this.m_contextMenu = null;
		OrderMarker component = this.m_selectedOrder.GetComponent<OrderMarker>();
		if (component == null)
		{
			return;
		}
		Order order = component.GetOrder();
		IOrderable owner = order.GetOwner();
		Order order2 = new Order(owner, order.m_type, this.m_targetPos);
		order2.m_fireVisual = order.m_fireVisual;
		order2.SetStaticTargetOnly(order.GetStaticTargetOnly());
		order2.SetDisplayRadius(order.GetDisplayRadius());
		owner.AddOrder(order2);
		this.PrepareDraging(this.m_targetPos, Input.mousePosition, order2.GetMarker());
		this.StartDraging(Input.mousePosition);
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00039750 File Offset: 0x00037950
	private void CloseStatusWindow_Ship()
	{
		if (this.m_statusWnd_Ship != null)
		{
			this.m_statusWnd_Ship.Close();
			this.m_statusWnd_Ship = null;
		}
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00039770 File Offset: 0x00037970
	public void ShowStatusWindow_Ship(Ship ship, bool friendly)
	{
		this.CloseStatusWindow_Ship();
		this.m_statusWnd_Ship = new StatusWnd_Ship(ship, this.m_guiCamera, friendly);
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0003978C File Offset: 0x0003798C
	private void CloseStatusWindow_HPModule()
	{
		if (this.m_statusWnd_HPModule != null)
		{
			this.m_statusWnd_HPModule.Close();
			this.m_statusWnd_HPModule = null;
		}
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x000397AC File Offset: 0x000379AC
	public void ShowStatusWindow_HPModule(HPModule module)
	{
		this.CloseStatusWindow_HPModule();
		this.m_statusWnd_HPModule = module.CreateStatusWindow(this.m_guiCamera);
		Ship ship = module.GetUnit() as Ship;
		if (ship != null)
		{
			bool friendly = ship.GetOwner() == this.m_localPlayerID;
			this.ShowStatusWindow_Ship(ship, friendly);
		}
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00039800 File Offset: 0x00037A00
	private void MoveOrder(OrderMarker markerScript, Vector3 newPos)
	{
		Order order = markerScript.GetOrder();
		IOrderable owner = order.GetOwner();
		Unit unit = owner as Unit;
		if (order.m_type == Order.Type.MoveForward || order.m_type == Order.Type.MoveBackward || order.m_type == Order.Type.MoveRotate)
		{
			if (this.m_settingMoveOrderFacing)
			{
				Vector3 facing = newPos - order.GetPos();
				order.SetFacing(facing);
			}
			else if (order.m_type != Order.Type.MoveRotate)
			{
				order.SetTarget(newPos);
				DebugUtils.Assert(unit != null);
				unit.SetBlockedRoute(!unit.IsMoveOrdersValid());
			}
		}
		else if (order.m_type == Order.Type.Fire)
		{
			bool key = Input.GetKey(KeyCode.LeftControl);
			if (this.m_hoverType == GameCamera.TargetType.Land || key || this.m_hover == null || order.GetStaticTargetOnly())
			{
				order.SetTarget(newPos);
			}
			else if (this.m_hoverType == GameCamera.TargetType.Unit || this.m_hoverType == GameCamera.TargetType.HPModule)
			{
				NetObj component = this.m_hover.GetComponent<NetObj>();
				Gun gun = owner as Gun;
				Vector3 localPos = this.m_hover.transform.InverseTransformPoint(newPos);
				order.SetTarget(component.GetNetID(), localPos);
			}
		}
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x00039940 File Offset: 0x00037B40
	private void Zoom(float zoomDelta, Vector3 zoomInDirection, float dt)
	{
		if (zoomDelta == 0f)
		{
			return;
		}
		if (zoomDelta > 0f)
		{
			base.transform.position += zoomInDirection * zoomDelta * dt * base.transform.position.y;
			if (base.transform.position.y < this.m_maxZoom)
			{
				float num = this.m_maxZoom - base.transform.position.y;
				float num2 = Vector3.Dot(Vector3.up, -zoomInDirection);
				Vector3 b = -zoomInDirection * (num / num2);
				base.transform.position += b;
				return;
			}
		}
		else if (zoomDelta < 0f)
		{
			base.transform.position += zoomInDirection * zoomDelta * dt * base.transform.position.y;
			if (base.transform.position.y > this.m_minZoom)
			{
				float num3 = this.m_minZoom - base.transform.position.y;
				float num4 = Vector3.Dot(Vector3.up, zoomInDirection);
				Vector3 b2 = zoomInDirection * (num3 / num4);
				base.transform.position += b2;
				return;
			}
		}
		NetObj selectedNetObj = this.GetSelectedNetObj();
		if (selectedNetObj != null)
		{
			HPModule hpmodule = selectedNetObj as HPModule;
			if (hpmodule != null)
			{
				this.OpenModuleContextMenu(hpmodule, false);
			}
		}
		if (this.m_selectedOrder != null)
		{
			OrderMarker component = this.m_selectedOrder.GetComponent<OrderMarker>();
			this.OpenOrderContextMenu(component);
		}
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x00039B2C File Offset: 0x00037D2C
	private void UpdateClipPlanes()
	{
		float y = base.transform.position.y;
		float nearClipPlane = Mathf.Max(10f, y - this.m_clipPlaneDistance);
		base.camera.nearClipPlane = nearClipPlane;
		float farClipPlane = Mathf.Min(950f, y + this.m_clipPlaneDistance);
		base.camera.farClipPlane = farClipPlane;
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x00039B8C File Offset: 0x00037D8C
	private float GetPinchDistance()
	{
		if (Input.touchCount == 2)
		{
			return Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
		}
		return 0f;
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x00039BD4 File Offset: 0x00037DD4
	private void SetHighlight(GameObject obj, bool enabled)
	{
		if (obj)
		{
			HPModule component = obj.GetComponent<HPModule>();
			if (component != null)
			{
				component.SetHighlight(enabled);
			}
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x00039C08 File Offset: 0x00037E08
	public void SetEnabled(bool enabled)
	{
		base.camera.enabled = enabled;
		AudioManager.instance.SetSFXEnabled(enabled);
		if (!enabled)
		{
			this.CloseStatusWindow_HPModule();
			this.CloseStatusWindow_Ship();
		}
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00039C40 File Offset: 0x00037E40
	private NetObj GetSelectedNetObj()
	{
		if (this.m_selectedNetID >= 0)
		{
			return NetObj.GetByID(this.m_selectedNetID);
		}
		return null;
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x00039C5C File Offset: 0x00037E5C
	public void SetAllowSelection(bool allow)
	{
		this.m_allowSelection = allow;
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x00039C68 File Offset: 0x00037E68
	public void SetAllowFlowerMenu(bool allow)
	{
		this.m_allowFlowerMenu = allow;
	}

	// Token: 0x04000610 RID: 1552
	private const float m_grabDelay = 0.1f;

	// Token: 0x04000611 RID: 1553
	private const float m_pinchTreshold = 25f;

	// Token: 0x04000612 RID: 1554
	private const float m_moveTreshold = 20f;

	// Token: 0x04000613 RID: 1555
	public Texture m_moveIcon;

	// Token: 0x04000614 RID: 1556
	public Texture m_reverseIcon;

	// Token: 0x04000615 RID: 1557
	public Texture m_facingIcon;

	// Token: 0x04000616 RID: 1558
	public Texture m_resetFacingIcon;

	// Token: 0x04000617 RID: 1559
	public Texture m_targetIcon;

	// Token: 0x04000618 RID: 1560
	public Texture m_removeIcon;

	// Token: 0x04000619 RID: 1561
	public Texture m_addIcon;

	// Token: 0x0400061A RID: 1562
	public Texture m_deployIcon;

	// Token: 0x0400061B RID: 1563
	public Texture m_returnFireIcon;

	// Token: 0x0400061C RID: 1564
	public Texture m_fireAtWillIcon;

	// Token: 0x0400061D RID: 1565
	public Texture m_holdFireIcon;

	// Token: 0x0400061E RID: 1566
	public Texture m_supplyIcon;

	// Token: 0x0400061F RID: 1567
	public Texture m_dontSupplyIcon;

	// Token: 0x04000620 RID: 1568
	public Texture m_deployRadarIcon;

	// Token: 0x04000621 RID: 1569
	public Texture m_deployCloakIcon;

	// Token: 0x04000622 RID: 1570
	public Texture m_deployShieldLeftIcon;

	// Token: 0x04000623 RID: 1571
	public Texture m_deployShieldRightIcon;

	// Token: 0x04000624 RID: 1572
	public Texture m_deployShieldForwardIcon;

	// Token: 0x04000625 RID: 1573
	public Texture m_deployShieldBackwardIcon;

	// Token: 0x04000626 RID: 1574
	public GameObject m_effectPrefab;

	// Token: 0x04000627 RID: 1575
	public float m_shakeScale = 0.25f;

	// Token: 0x04000628 RID: 1576
	public float m_shakeMaxIntensity = 3f;

	// Token: 0x04000629 RID: 1577
	public float m_keyboardScrollSpeed = 700f;

	// Token: 0x0400062A RID: 1578
	public float m_keyboardZoomSpeed = 100f;

	// Token: 0x0400062B RID: 1579
	private int m_localPlayerID = -1;

	// Token: 0x0400062C RID: 1580
	private GameCamera.Mode m_mode = GameCamera.Mode.Disabled;

	// Token: 0x0400062D RID: 1581
	private GameObject m_guiCamera;

	// Token: 0x0400062E RID: 1582
	private GameObject m_selectedOrder;

	// Token: 0x0400062F RID: 1583
	private bool m_settingMoveOrderFacing;

	// Token: 0x04000630 RID: 1584
	private int m_selectedNetID = -1;

	// Token: 0x04000631 RID: 1585
	private bool m_selectedNetObjExplicit;

	// Token: 0x04000632 RID: 1586
	private GameObject m_hover;

	// Token: 0x04000633 RID: 1587
	private GameCamera.TargetType m_hoverType;

	// Token: 0x04000634 RID: 1588
	private Vector3 m_targetPos;

	// Token: 0x04000635 RID: 1589
	private float m_levelSize = 500f;

	// Token: 0x04000636 RID: 1590
	public float m_minZoom = 600f;

	// Token: 0x04000637 RID: 1591
	public float m_maxZoom = 90f;

	// Token: 0x04000638 RID: 1592
	public float m_clipPlaneDistance = 200f;

	// Token: 0x04000639 RID: 1593
	private float m_mouseMoveSpeed = 0.001f;

	// Token: 0x0400063A RID: 1594
	private float m_zBorderOffset = -0.3f;

	// Token: 0x0400063B RID: 1595
	private bool m_pinchZoom;

	// Token: 0x0400063C RID: 1596
	private float m_pinchStartDistance = -1f;

	// Token: 0x0400063D RID: 1597
	private float m_totalScrollZoomDelta;

	// Token: 0x0400063E RID: 1598
	private bool m_flowerMenuFireDrag;

	// Token: 0x0400063F RID: 1599
	private bool m_draging;

	// Token: 0x04000640 RID: 1600
	private Vector3 m_dragStart;

	// Token: 0x04000641 RID: 1601
	private Vector3 m_dragStartMousePos;

	// Token: 0x04000642 RID: 1602
	private Vector3 m_dragLastPos;

	// Token: 0x04000643 RID: 1603
	private GameObject m_dragObject;

	// Token: 0x04000644 RID: 1604
	private global::ContextMenu m_contextMenu;

	// Token: 0x04000645 RID: 1605
	private FlowerMenu m_flowerMenu;

	// Token: 0x04000646 RID: 1606
	private StatusWnd_HPModule m_statusWnd_HPModule;

	// Token: 0x04000647 RID: 1607
	private StatusWnd_Ship m_statusWnd_Ship;

	// Token: 0x04000648 RID: 1608
	private bool m_firstSetup = true;

	// Token: 0x04000649 RID: 1609
	private bool m_hasFocusPos;

	// Token: 0x0400064A RID: 1610
	private Vector2 m_focusPos = Vector3.zero;

	// Token: 0x0400064B RID: 1611
	private float m_focusHeight;

	// Token: 0x0400064C RID: 1612
	private float m_shakeTimer = -1f;

	// Token: 0x0400064D RID: 1613
	private Quaternion m_shakeRot;

	// Token: 0x0400064E RID: 1614
	private float m_shakeIntensity;

	// Token: 0x0400064F RID: 1615
	private float m_shakeLength;

	// Token: 0x04000650 RID: 1616
	private Vector3 m_shakeDirection;

	// Token: 0x04000651 RID: 1617
	private int m_markersMask;

	// Token: 0x04000652 RID: 1618
	private int m_pickMask;

	// Token: 0x04000653 RID: 1619
	private int m_terrainMask;

	// Token: 0x04000654 RID: 1620
	private bool m_allowSelection = true;

	// Token: 0x04000655 RID: 1621
	private bool m_allowFlowerMenu = true;

	// Token: 0x020000CE RID: 206
	public enum Mode
	{
		// Token: 0x04000657 RID: 1623
		Active,
		// Token: 0x04000658 RID: 1624
		Passive,
		// Token: 0x04000659 RID: 1625
		Disabled
	}

	// Token: 0x020000CF RID: 207
	private enum TargetType
	{
		// Token: 0x0400065B RID: 1627
		None,
		// Token: 0x0400065C RID: 1628
		Land,
		// Token: 0x0400065D RID: 1629
		Unit,
		// Token: 0x0400065E RID: 1630
		HPModule
	}
}
