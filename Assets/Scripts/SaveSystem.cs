using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
        save.GameData = GameManager.Instance.GameData;
        save.SettingsData = GameManager.Instance.SettingsData;
        save.StatisticsData = GameManager.Instance.StatisticsData;
        // Save skin data
        save.DeckSkinDatas = SkinManager.Instance.DeckSkinDatas;
        save.BackSkinDatas = SkinManager.Instance.BackSkinDatas;
        save.CurDeckSkinId = SkinManager.Instance.CurDeckSkinId;
        save.CurBackSkinId = SkinManager.Instance.CurBackSkinId;

        //创建一个二进制格式化程序
        BinaryFormatter bf = new BinaryFormatter();
        //创建一个文件流
        FileStream fileStream = File.Create(Application.dataPath + "/Saves" + "/Save.carta");
        //用二进制格式化程序的序列化方法来序列化Save对象,参数：创建的文件流和需要序列化的对象
        bf.Serialize(fileStream, save);
        //关闭流
        fileStream.Close();

        //如果文件存在，则显示保存成功
        return File.Exists(Application.dataPath + "/Saves" + "/Save.carta");
    }
    public static Save Load()
    {
        if (File.Exists(Application.dataPath + "/Saves" + "/Save.carta"))
        {
            //反序列化过程
            //创建一个二进制格式化程序
            BinaryFormatter bf = new BinaryFormatter();
            //打开一个文件流
            FileStream fileStream = File.Open(Application.dataPath + "/Saves" + "/Save.carta", FileMode.Open);
            //调用格式化程序的反序列化方法，将文件流转换为一个Save对象
            Save save = (Save)bf.Deserialize(fileStream);
            //关闭文件流
            fileStream.Close();
            return save;
            //SetGame(save);
            //UIManager._instance.ShowMessage("");

        }
        return null;
    }
}
