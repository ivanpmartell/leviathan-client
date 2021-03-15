using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

// Token: 0x0200014F RID: 335
internal class PwdUtils
{
	// Token: 0x06000CAE RID: 3246 RVA: 0x0005AD4C File Offset: 0x00058F4C
	public static void GeneratePasswordHash(string password, out byte[] key, out byte[] salt)
	{
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, 20);
		key = rfc2898DeriveBytes.GetBytes(60);
		salt = rfc2898DeriveBytes.Salt;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0005AD74 File Offset: 0x00058F74
	public static bool CheckPassword(string password, byte[] key, byte[] salt)
	{
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt);
		byte[] bytes = rfc2898DeriveBytes.GetBytes(key.Length);
		return bytes.SequenceEqual(key);
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0005AD9C File Offset: 0x00058F9C
	public static string GenerateWeakPasswordHash(string password)
	{
		UTF8Encoding utf8Encoding = new UTF8Encoding();
		byte[] bytes = utf8Encoding.GetBytes("sdfjk23409fmspep");
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, bytes);
		byte[] bytes2 = rfc2898DeriveBytes.GetBytes(60);
		return utf8Encoding.GetString(bytes2);
	}

	// Token: 0x04000A6F RID: 2671
	private const int keySize = 60;

	// Token: 0x04000A70 RID: 2672
	private const int saltSize = 20;
}
