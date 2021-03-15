using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000122 RID: 290
internal class OfflineGameDB
{
	// Token: 0x06000B47 RID: 2887 RVA: 0x00052600 File Offset: 0x00050800
	public List<GamePost> GetGameList()
	{
		List<GamePost> list = new List<GamePost>();
		string[] files = Directory.GetFiles(Application.persistentDataPath);
		foreach (string text in files)
		{
			if (System.IO.Path.GetExtension(text) == ".gam")
			{
				GamePost item = this.LoadGamePost(text);
				list.Add(item);
			}
		}
		return list;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00052664 File Offset: 0x00050864
	public List<GamePost> GetReplayList()
	{
		List<GamePost> list = new List<GamePost>();
		string[] files = Directory.GetFiles(Application.persistentDataPath);
		foreach (string text in files)
		{
			if (System.IO.Path.GetExtension(text) == ".rep")
			{
				GamePost item = this.LoadGamePost(text);
				list.Add(item);
			}
		}
		return list;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x000526C8 File Offset: 0x000508C8
	private GamePost LoadGamePost(string fileName)
	{
		FileStream fileStream = new FileStream(fileName, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		int count = binaryReader.ReadInt32();
		byte[] data = binaryReader.ReadBytes(count);
		GamePost gamePost = new GamePost();
		gamePost.FromArray(data);
		fileStream.Close();
		return gamePost;
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0005270C File Offset: 0x0005090C
	private string GetGameFileName(int gameID)
	{
		return Application.persistentDataPath + "/game" + gameID.ToString() + ".gam";
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00052738 File Offset: 0x00050938
	private string GetReplayFileName(int gameID)
	{
		return Application.persistentDataPath + "/replay_" + gameID.ToString() + ".rep";
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00052764 File Offset: 0x00050964
	public byte[] LoadGame(int gameID, bool replay)
	{
		string path;
		if (replay)
		{
			path = this.GetReplayFileName(gameID);
		}
		else
		{
			path = this.GetGameFileName(gameID);
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		int count = binaryReader.ReadInt32();
		byte[] array = binaryReader.ReadBytes(count);
		int count2 = binaryReader.ReadInt32();
		byte[] result = binaryReader.ReadBytes(count2);
		fileStream.Close();
		return result;
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x000527C8 File Offset: 0x000509C8
	public bool SaveGame(GamePost post, byte[] gameData, bool replay)
	{
		string text;
		if (replay)
		{
			text = this.GetReplayFileName(post.m_gameID);
			if (File.Exists(text))
			{
				PLog.LogError("File exist " + text);
				return false;
			}
		}
		else
		{
			text = this.GetGameFileName(post.m_gameID);
		}
		FileStream fileStream = new FileStream(text, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		byte[] array = post.ToArray();
		binaryWriter.Write(array.Length);
		binaryWriter.Write(array);
		binaryWriter.Write(gameData.Length);
		binaryWriter.Write(gameData);
		fileStream.Close();
		return true;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x00052854 File Offset: 0x00050A54
	public void RemoveGame(int gameID)
	{
		string gameFileName = this.GetGameFileName(gameID);
		try
		{
			File.Delete(gameFileName);
		}
		catch
		{
			PLog.LogError("Failed to remove offline game " + gameID);
		}
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x000528AC File Offset: 0x00050AAC
	public void RemoveReplay(int gameID)
	{
		string replayFileName = this.GetReplayFileName(gameID);
		try
		{
			File.Delete(replayFileName);
		}
		catch
		{
			PLog.LogError("Failed to remove offline replay " + gameID);
		}
	}
}
