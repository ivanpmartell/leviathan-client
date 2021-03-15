using System;
using System.IO;

// Token: 0x020000CA RID: 202
public class Deployable : NetObj
{
	// Token: 0x06000760 RID: 1888 RVA: 0x000370AC File Offset: 0x000352AC
	public virtual void Setup(int ownerID, int gunID, bool visible, int seenByMask, int damage, int ap, float splashRadius, float splashDmgFactor)
	{
		this.SetOwner(ownerID);
		this.m_gunID = gunID;
		this.SetVisible(visible);
		base.SetSeenByMask(seenByMask);
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x000370CC File Offset: 0x000352CC
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_gunID);
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000370E4 File Offset: 0x000352E4
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_gunID = reader.ReadInt32();
	}

	// Token: 0x0400060D RID: 1549
	protected int m_gunID;
}
