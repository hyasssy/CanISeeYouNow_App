using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class CryptExpansion {
    /// <summary>
    ///SALT:なんでもいい文字列 string 短すぎるとエラー
    ///PASSWORD:なんでもいい文字列 string
    ///ITERATIONCOUNT:暗号化反復回数 int
    ///</summary>
    public static string Encrypt (string target) {
        byte[] src = System.Text.Encoding.ASCII.GetBytes (target);
        RijndaelManaged rijndael = new RijndaelManaged ();
        rijndael.KeySize = 128;
        rijndael.BlockSize = 128;
        byte[] bSalt = Encoding.UTF8.GetBytes (SecretKey.SALT);
        Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes (SecretKey.PASSWORD, bSalt);
        deriveBytes.IterationCount = SecretKey.ITERATIONCOUNT; //反復回数
        rijndael.Key = deriveBytes.GetBytes (rijndael.KeySize / 8); //あんまこれなんなのかわからん。
        rijndael.IV = deriveBytes.GetBytes (rijndael.BlockSize / 8);
        ICryptoTransform encryptor = rijndael.CreateEncryptor ();
        byte[] encrypted = encryptor.TransformFinalBlock (src, 0, src.Length);
        encryptor.Dispose ();
        string result = System.BitConverter.ToString (encrypted).Replace ("-", "");
        Debug.Log (result);
        return result;
    }
    public static string Decrypt (string encrypted) {
        byte[] b = new byte[encrypted.Length / 2];
        int cur = 0;
        for (int i = 0; i < encrypted.Length; i = i + 2) {
            string w = encrypted.Substring (i, 2);
            b[cur] = byte.Parse (w, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            cur++;
        }

        RijndaelManaged rijndael = new RijndaelManaged ();
        rijndael.KeySize = 128;
        rijndael.BlockSize = 128;
        byte[] bSalt = Encoding.UTF8.GetBytes (SecretKey.SALT);
        Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes (SecretKey.PASSWORD, bSalt);
        deriveBytes.IterationCount = SecretKey.ITERATIONCOUNT; //反復回数
        rijndael.Key = deriveBytes.GetBytes (rijndael.KeySize / 8); //あんまこれなんなのかわからん。
        rijndael.IV = deriveBytes.GetBytes (rijndael.BlockSize / 8);

        ICryptoTransform decryptor = rijndael.CreateDecryptor ();
        byte[] decrypted = decryptor.TransformFinalBlock (b, 0, b.Length);
        decryptor.Dispose ();
        string value = System.Text.Encoding.ASCII.GetString (decrypted);
        return value;
    }
}