using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000172 RID: 370
[Serializable]
public sealed class LODManager : MonoBehaviour
{
	// Token: 0x06000E06 RID: 3590 RVA: 0x00063214 File Offset: 0x00061414
	private void Start()
	{
		this.InitializeCamera();
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x0006321C File Offset: 0x0006141C
	private void InitializeCamera()
	{
		if (this.cameraPtr == null)
		{
			this.cameraPtr = (UnityEngine.Object.FindObjectOfType(typeof(GameCamera)) as GameCamera);
			if (this.cameraPtr == null)
			{
				this.IsInLockedToHighdetail = true;
				string message = string.Format("LODManager for {0} failed to find the GameCamera on Start(), will only use High-detail untill it is properly set !", base.gameObject.name);
				Debug.LogWarning(message);
			}
			else
			{
				this.IsInLockedToHighdetail = false;
			}
		}
		else
		{
			this.IsInLockedToHighdetail = false;
		}
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x000632A0 File Offset: 0x000614A0
	public void OnCameraMove(GameCamera camera)
	{
		if (!this.IsInLockedToHighdetail)
		{
			this.Think(false);
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x000632B4 File Offset: 0x000614B4
	public GameObject ForceUpdateAndGetCurrent()
	{
		if (this.cameraPtr == null)
		{
			this.InitializeCamera();
		}
		if (!this.IsInLockedToHighdetail)
		{
			this.DisableAll();
			this.Think(true);
		}
		return this.GetCurrentLODObject();
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x000632F8 File Offset: 0x000614F8
	public GameObject UpdateFrom(int index)
	{
		if (!this.IsInLockedToHighdetail)
		{
			this.DisableAll();
			if (this.m_instructions != null && index > 0 && index < this.m_instructions.Count - 1)
			{
				this.currentIndex = index;
			}
			this.EnableCurrent();
		}
		return this.GetCurrentLODObject();
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00063350 File Offset: 0x00061550
	private void Think(bool forceUpdate)
	{
		if (this.m_instructions == null || this.m_instructions.Count <= 1)
		{
			this.currentIndex = 0;
			return;
		}
		if (this.CameraPtr == null)
		{
			return;
		}
		LODInstruction currentLODLevel = this.GetCurrentLODLevel();
		if (currentLODLevel == null)
		{
			return;
		}
		if (currentLODLevel.m_target == null)
		{
			return;
		}
		Vector3 vector = this.CameraPtr.transform.position - base.transform.position;
		vector.y *= 0.8f;
		float num = Mathf.Abs(vector.magnitude);
		if (num > currentLODLevel.m_maxDist)
		{
			if (this.m_isEnabled || forceUpdate)
			{
				this.EnableNext();
			}
			else
			{
				this.currentIndex++;
				if (this.currentIndex > this.m_instructions.Count - 1)
				{
					this.currentIndex = this.m_instructions.Count - 1;
				}
			}
		}
		else if (num < currentLODLevel.m_minDist)
		{
			if (this.m_isEnabled || forceUpdate)
			{
				this.EnablePrev();
			}
			else
			{
				this.currentIndex--;
				if (this.currentIndex < 0)
				{
					this.currentIndex = 0;
				}
			}
		}
		else
		{
			this.DisableAll();
			this.FindAndEnableCurrent(num);
		}
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x000634B8 File Offset: 0x000616B8
	public int GetCurrentLODIndex()
	{
		return this.currentIndex;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x000634C0 File Offset: 0x000616C0
	public LODInstruction GetCurrentLODLevel()
	{
		if (this.m_instructions == null || this.m_instructions.Count == 0)
		{
			return null;
		}
		if (this.IsInLockedToHighdetail)
		{
			this.currentIndex = 0;
		}
		return this.m_instructions[this.currentIndex];
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x00063510 File Offset: 0x00061710
	public GameObject GetCurrentLODObject()
	{
		LODInstruction currentLODLevel = this.GetCurrentLODLevel();
		if (currentLODLevel == null)
		{
			return null;
		}
		return currentLODLevel.m_target;
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x00063534 File Offset: 0x00061734
	private void EnableNext()
	{
		int num = this.currentIndex + 1;
		if (num > this.m_instructions.Count - 1)
		{
			num = this.m_instructions.Count - 1;
		}
		if (num != this.currentIndex)
		{
			this.DisableCurrent();
			this.currentIndex = num;
			this.EnableCurrent();
		}
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0006358C File Offset: 0x0006178C
	public void EnableCurrent()
	{
		if (!this.m_isEnabled)
		{
			return;
		}
		LODInstruction currentLODLevel = this.GetCurrentLODLevel();
		if (currentLODLevel.m_useAllRenderers)
		{
			if (currentLODLevel.m_target.renderer != null)
			{
				currentLODLevel.m_target.renderer.enabled = true;
			}
			foreach (Renderer renderer in currentLODLevel.m_target.GetComponentsInChildren<Renderer>())
			{
				if (!renderer.enabled)
				{
					renderer.enabled = true;
					if (this.currentIndex == 0)
					{
						renderer.castShadows = true;
						renderer.receiveShadows = true;
					}
				}
			}
		}
		else if (!currentLODLevel.m_target.renderer.enabled)
		{
			currentLODLevel.m_target.renderer.enabled = true;
		}
		if (this.OnLODLevelChangedDelegate != null)
		{
			this.OnLODLevelChangedDelegate(this.m_instructions[this.currentIndex].m_target);
		}
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x00063684 File Offset: 0x00061884
	public void FindAndEnableCurrent(float distToCamera)
	{
		int num = 0;
		foreach (LODInstruction lodinstruction in this.m_instructions)
		{
			if (lodinstruction.m_minDist <= distToCamera && lodinstruction.m_maxDist >= distToCamera)
			{
				break;
			}
			num++;
		}
		this.currentIndex = num;
		this.EnableCurrent();
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00063714 File Offset: 0x00061914
	private void EnablePrev()
	{
		int num = this.currentIndex - 1;
		if (num < 0)
		{
			num = 0;
		}
		if (num != this.currentIndex)
		{
			this.DisableCurrent();
			this.currentIndex = num;
			this.EnableCurrent();
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00063754 File Offset: 0x00061954
	private void DisableCurrent()
	{
		LODInstruction currentLODLevel = this.GetCurrentLODLevel();
		if (currentLODLevel.m_useAllRenderers)
		{
			foreach (Renderer renderer in currentLODLevel.m_target.GetComponentsInChildren<Renderer>())
			{
				if (renderer.enabled)
				{
					renderer.enabled = false;
				}
			}
		}
		else if (currentLODLevel.m_target.renderer.enabled)
		{
			currentLODLevel.m_target.renderer.enabled = false;
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x000637D4 File Offset: 0x000619D4
	public bool Add(LODInstruction instr)
	{
		if (this.m_instructions == null)
		{
			this.m_instructions = new List<LODInstruction>();
		}
		if (!this.m_instructions.Contains(instr))
		{
			if (instr.m_useAllRenderers)
			{
				Renderer[] componentsInChildren = instr.m_target.GetComponentsInChildren<Renderer>();
				DebugUtils.Assert(componentsInChildren.Length > 0, "Cant add LOD target, it has no renderers !");
				if (componentsInChildren.Length == 0)
				{
					return false;
				}
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.enabled = false;
				}
			}
			else
			{
				DebugUtils.Assert(instr.m_target.renderer != null, "LODInstruction is set to NOT serach children for renderers, yet it has no renderer of it's own !");
				if (instr.m_target.renderer == null)
				{
					return false;
				}
				instr.m_target.renderer.enabled = false;
			}
			this.m_instructions.Add(instr);
			this.SortList();
			return true;
		}
		DebugUtils.Assert(false, "Trying to add object to LODManager that already exists !");
		return false;
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x000638C8 File Offset: 0x00061AC8
	public void SortList()
	{
		if (this.m_instructions != null && this.m_instructions.Count > 1)
		{
			this.m_instructions.Sort((LODInstruction instr1, LODInstruction instr2) => instr1.CompareTo(instr2));
		}
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0006391C File Offset: 0x00061B1C
	public void AddOnLODChangedInterest(OnLODLevelChanged interest)
	{
		this.OnLODLevelChangedDelegate = (OnLODLevelChanged)Delegate.Remove(this.OnLODLevelChangedDelegate, interest);
		this.OnLODLevelChangedDelegate = (OnLODLevelChanged)Delegate.Combine(this.OnLODLevelChangedDelegate, interest);
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x00063958 File Offset: 0x00061B58
	public void RemoveLODChangedInterest(OnLODLevelChanged interest)
	{
		this.OnLODLevelChangedDelegate = (OnLODLevelChanged)Delegate.Remove(this.OnLODLevelChangedDelegate, interest);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00063974 File Offset: 0x00061B74
	private void Disable(LODInstruction instr)
	{
		if (instr.m_useAllRenderers)
		{
			Renderer[] componentsInChildren = instr.m_target.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.enabled = false;
			}
		}
		else
		{
			instr.m_target.renderer.enabled = false;
		}
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x000639D0 File Offset: 0x00061BD0
	private void DisableAll()
	{
		if (this.m_instructions == null || this.m_instructions.Count == 0)
		{
			return;
		}
		this.m_instructions.ForEach(delegate(LODInstruction instr)
		{
			this.Disable(instr);
		});
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x00063A08 File Offset: 0x00061C08
	public void SetEnable(bool enabled)
	{
		if (this.m_isEnabled == enabled)
		{
			return;
		}
		this.m_isEnabled = enabled;
		if (this.m_isEnabled)
		{
			this.EnableCurrent();
		}
		else
		{
			this.DisableAll();
		}
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x06000E1B RID: 3611 RVA: 0x00063A48 File Offset: 0x00061C48
	public bool IsEnabled
	{
		get
		{
			return this.m_isEnabled;
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x06000E1C RID: 3612 RVA: 0x00063A50 File Offset: 0x00061C50
	// (set) Token: 0x06000E1D RID: 3613 RVA: 0x00063A58 File Offset: 0x00061C58
	public bool IsInLockedToHighdetail { get; private set; }

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000E1E RID: 3614 RVA: 0x00063A64 File Offset: 0x00061C64
	// (set) Token: 0x06000E1F RID: 3615 RVA: 0x00063A6C File Offset: 0x00061C6C
	public GameCamera CameraPtr
	{
		private get
		{
			return this.cameraPtr;
		}
		set
		{
			if (value == null && this.cameraPtr != null)
			{
				this.IsInLockedToHighdetail = true;
				return;
			}
			this.IsInLockedToHighdetail = false;
			if (value != this.cameraPtr)
			{
				this.cameraPtr = value;
				return;
			}
		}
	}

	// Token: 0x04000B45 RID: 2885
	private GameCamera cameraPtr;

	// Token: 0x04000B46 RID: 2886
	private int currentIndex;

	// Token: 0x04000B47 RID: 2887
	private OnLODLevelChanged OnLODLevelChangedDelegate;

	// Token: 0x04000B48 RID: 2888
	private bool m_isEnabled = true;

	// Token: 0x04000B49 RID: 2889
	[SerializeField]
	public List<LODInstruction> m_instructions;
}
