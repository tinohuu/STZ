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

        //����һ�������Ƹ�ʽ������
        BinaryFormatter bf = new BinaryFormatter();
        //����һ���ļ���
        FileStream fileStream = File.Create(Application.dataPath + "/Saves" + "/Save.carta");
        //�ö����Ƹ�ʽ����������л����������л�Save����,�������������ļ�������Ҫ���л��Ķ���
        bf.Serialize(fileStream, save);
        //�ر���
        fileStream.Close();

        //����ļ����ڣ�����ʾ����ɹ�
        return File.Exists(Application.dataPath + "/Saves" + "/Save.carta");
    }
    public static Save Load()
    {
        if (File.Exists(Application.dataPath + "/Saves" + "/Save.carta"))
        {
            //�����л�����
            //����һ�������Ƹ�ʽ������
            BinaryFormatter bf = new BinaryFormatter();
            //��һ���ļ���
            FileStream fileStream = File.Open(Application.dataPath + "/Saves" + "/Save.carta", FileMode.Open);
            //���ø�ʽ������ķ����л����������ļ���ת��Ϊһ��Save����
            Save save = (Save)bf.Deserialize(fileStream);
            //�ر��ļ���
            fileStream.Close();
            return save;
            //SetGame(save);
            //UIManager._instance.ShowMessage("");

        }
        return null;
    }
}
