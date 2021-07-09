using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class SaveSystem
{
    public static bool Save()
    {
        Save save = new Save();

        // Save piles
        foreach (Pile pile in CardManager.Instance.AllPiles)
        {
            List<Card> cards = new List<Card>();
            cards.AddRange(pile.Cards);
            save.Piles.Add(cards);
        }
        // Save data
        //GameManager.Instance.GameData.DateTime = DateTime.Now;
        //Debug.LogWarning("Save Time:" + DateTime.Now);
        save.GameData = GameManager.Instance.GameData;
        save.SettingsData = GameManager.Instance.SettingsData;
        save.StatisticsData = GameManager.Instance.StatisticsData;
        // Save skin data
        save.DeckSkinDatas = SkinManager.Instance.DeckSkinDatas;
        save.BackSkinDatas = SkinManager.Instance.BackSkinDatas;
        save.CurDeckSkinId = SkinManager.Instance.CurDeckSkinId;
        save.CurBackSkinId = SkinManager.Instance.CurBackSkinId;
        // Save undo data
        save.Undos = UndoManager.Instance.Undos;

        BinaryFormatter bf = new BinaryFormatter();

        // Convert class to bytes
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, save);
        byte[] byteSave = ms.GetBuffer();
        ms.Close();

        // Encrypt
        byte[] encryptedByte = Encrypt.AESHelper.Encrypt(byteSave);

        // Save
        FileStream fileStream = File.Create(Application.dataPath + "/Saves" + "/Save.carta");
        bf.Serialize(fileStream, encryptedByte);
        fileStream.Close();

        return File.Exists(Application.dataPath + "/Saves" + "/Save.carta");
    }
    public static Save Load()
    {
        if (File.Exists(Application.dataPath + "/Saves" + "/Save.carta"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            // Load
            FileStream fileStream = File.Open(Application.dataPath + "/Saves" + "/Save.carta", FileMode.Open);
            byte[] encryptedByte = (byte[])bf.Deserialize(fileStream);
            fileStream.Close();

            // Dncrypt
            byte[] byteSave = Encrypt.AESHelper.Decrypt(encryptedByte);

            // Convert bytes to class
            MemoryStream ms = new MemoryStream(byteSave);
            Save save = (Save)bf.Deserialize(ms);
            ms.Close();

            return save;
        }
        return null;
    }
}

namespace Encrypt
{
    public class AESHelper
    {
        /// <summary>
        /// 默认密钥-密钥的长度必须是32
        /// </summary>
        private const string PublicKey = "1234567890123456";

        /// <summary>
        /// 默认向量
        /// </summary>
        private const string Iv = "abcdefghijklmnop";
        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="str">需要加密字符串</param>  
        /// <returns>加密后字符串</returns>  
        public static Byte[] Encrypt(Byte[] data)
        {
            return Encrypt(data, PublicKey);
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="str">需要解密字符串</param>  
        /// <returns>解密后字符串</returns>  
        public static Byte[] Decrypt(Byte[] data)
        {
            return Decrypt(data, PublicKey);
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="key">32位密钥</param>
        /// <returns>加密后的字符串</returns>
        public static Byte[] Encrypt(Byte[] data, string key)
        {
            Byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(key);
            Byte[] toEncryptArray = data;
            var rijndael = new System.Security.Cryptography.RijndaelManaged();
            rijndael.Key = keyArray;
            rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
            rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            rijndael.IV = System.Text.Encoding.UTF8.GetBytes(Iv);
            System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return resultArray;
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <param name="key">32位密钥</param>
        /// <returns>解密后的字符串</returns>
        public static Byte[] Decrypt(Byte[] data, string key)
        {
            Byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(key);
            Byte[] toEncryptArray = data;// Convert.FromBase64String(str);
            var rijndael = new System.Security.Cryptography.RijndaelManaged();
            rijndael.Key = keyArray;
            rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
            rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            rijndael.IV = System.Text.Encoding.UTF8.GetBytes(Iv);
            System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return resultArray;
        }
    }
}
