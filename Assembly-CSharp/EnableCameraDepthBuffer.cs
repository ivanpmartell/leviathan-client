using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class EnableCameraDepthBuffer : MonoBehaviour
{
	// Token: 0x06000764 RID: 1892 RVA: 0x00037104 File Offset: 0x00035304
	private void Start()
	{
		base.camera.depthTextureMode = DepthTextureMode.Depth;
	}
}
