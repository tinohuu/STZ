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
        save.Deck = CardManager.Instance.Deck;
        // Save skin data
        save.DeckSkinDatas = SkinManager.Instance.DeckSkinDatas;
        save.BackSkinDatas = SkinManager.Instance.BackSkinDatas;
        save.CurDeckSkinId = SkinManager.Instance.CurDeckSkinId;
        save.CurBackSkinId = SkinManager.Instance.CurBackSkinId;
        // Save undo data
        save.Undos = UndoManager.Instance.Undos;
        // Save mission data
        save.MissionUpdateTime = MissionManager.Instance.MissionUpdateDate;
        List<MissionData> missionDatas = new List<MissionData>();
        foreach (Mission mission in MissionManager.Instance.CurMissions) missionDatas.Add(mission.MissionData);
        save.MissionDatas = missionDatas;
        List<ExclusiveOffer> exclusiveOffers = new List<ExclusiveOffer>();
        foreach (EOView view in MissionViewManager.Instance.EOViews) exclusiveOffers.Add(view.Offer);
        save.ExclusiveOffers = exclusiveOffers;
        save.ChallengeManagerData = ChallengeManager.Instance.Data;

        BinaryFormatter bf = new BinaryFormatter();

        // Convert class to bytes
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, save);
        byte[] byteSave = ms.GetBuffer();
        ms.Close();

        // Encrypt
        byte[] encryptedByte = Encrypt.AESHelper.Encrypt(byteSave);

        // Save
        FileStream fileStream = File.Create(Application.persistentDataPath + "/Saves" + "/Save.carta");
        bf.Serialize(fileStream, encryptedByte);
        fileStream.Close();

        return File.Exists(Application.persistentDataPath + "/Saves" + "/Save.carta");
    }
    public static Save Load()
    {
        if (File.Exists(Application.persistentDataPath + "/Saves" + "/Save.carta"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            // Load
            FileStream fileStream = File.Open(Application.persistentDataPath + "/Saves" + "/Save.carta", FileMode.Open);
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
        /// ????????-????????????????32
        /// </summary>
        private const string PublicKey = "1234567890123456";

        /// <summary>
        /// ????????
        /// </summary>
        private const string Iv = "abcdefghijklmnop";
        /// <summary>  
        /// AES????  
        /// </summary>  
        /// <param name="str">??????????????</param>  
        /// <returns>????????????</returns>  
        public static Byte[] Encrypt(Byte[] data)
        {
            return Encrypt(data, PublicKey);
        }

        /// <summary>  
        /// AES????  
        /// </summary>  
        /// <param name="str">??????????????</param>  
        /// <returns>????????????</returns>  
        public static Byte[] Decrypt(Byte[] data)
        {
            return Decrypt(data, PublicKey);
        }
        /// <summary>
        /// AES????
        /// </summary>
        /// <param name="str">????????????????</param>
        /// <param name="key">32??????</param>
        /// <returns>??????????????</returns>
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
        /// AES????
        /// </summary>
        /// <param name="str">????????????????</param>
        /// <param name="key">32??????</param>
        /// <returns>??????????????</returns>
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
