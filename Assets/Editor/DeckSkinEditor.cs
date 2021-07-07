using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeckSkin))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DeckSkin deckSkin = (DeckSkin)target;

        if (GUILayout.Button("Find Assets"))
        {
            string rootPath = "DeckSkins/" + deckSkin.Id + "/";
            string spriteName = "default";
            Sprite sprite = null;

            spriteName = "DeckSkin_" + deckSkin.Id + "_Back";
            sprite = Resources.Load<Sprite>(rootPath + spriteName);
            if (!sprite) Debug.LogWarning("No back sprite.");
            else deckSkin.BackSprite = sprite;

            spriteName = "DeckSkin_" + deckSkin.Id + "_Face";
            sprite = Resources.Load<Sprite>(rootPath + spriteName);
            if (!sprite) Debug.LogWarning("No face sprite.");
            else deckSkin.FaceSprite = sprite;

            spriteName = "DeckSkin_" + deckSkin.Id + "_Cover";
            sprite = Resources.Load<Sprite>(rootPath + spriteName);
            if (!sprite) Debug.LogWarning("No cover sprite.");
            else deckSkin.CoverSprite = sprite;

            spriteName = "DeckSkin_" + deckSkin.Id + "_Box";
            sprite = Resources.Load<Sprite>(rootPath + spriteName);
            if (!sprite) Debug.LogWarning("No box sprite.");
            else deckSkin.BoxSprite = sprite;

            deckSkin.NumberSprites.Clear();
            for (int i = 0; i < 13; i++)
            {
                string name = "DeckSkin_" + deckSkin.Id + "_" + (i + 1).ToString();
                Sprite spt = Resources.Load<Sprite>(rootPath + name);
                if (!spt)
                {
                    Debug.LogWarning("No number sprite of " + (i + 1).ToString());
                    break;
                }
                deckSkin.NumberSprites.Add(spt);
            }

            deckSkin.SmallSuitSprites.Clear();
            for (int i = 0; i < 4; i++)
            {

                string name = "DeckSkin_" + deckSkin.Id + "_Small_" + ((Card.SuitType)i).ToString();
                Sprite spt = Resources.Load<Sprite>(rootPath + name);
                if (!spt)
                {
                    Debug.LogWarning("No small suit sprite of " + ((Card.SuitType)i).ToString());
                    break;
                }
                deckSkin.SmallSuitSprites.Add(spt);
            }


            foreach (List<Sprite> sprites in deckSkin.BigSuitSprites) sprites.Clear();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    string name = "DeckSkin_" + deckSkin.Id + "_Big_" + ((Card.SuitType)i).ToString() + "_" + (j + 1).ToString();
                    Sprite spt = Resources.Load<Sprite>(rootPath + name);
                    if (!spt)
                    {
                        name = "DeckSkin_" + deckSkin.Id + "_Big_" + ((Card.SuitType)i).ToString();
                        spt = Resources.Load<Sprite>(rootPath + name);

                        deckSkin.BigSuitSprites[i].Add(spt);
                        if (!spt)
                        {
                            Debug.LogWarning("No big suit sprite of " + ((Card.SuitType)i).ToString());
                            break;
                        }
                    }
                    else deckSkin.BigSuitSprites[i].Add(spt);
                }
            }

            for (int i = 0; i < 13; i++)
            {
                string name = "DeckSkin_" + deckSkin.Id + "_Big_Any_" + (i + 1).ToString();
                Sprite spt = Resources.Load<Sprite>(rootPath + name);
                if (spt) foreach (List<Sprite> sprites in deckSkin.BigSuitSprites) sprites[i] = spt;
            }
            EditorUtility.SetDirty(target);
        }
        if (deckSkin.NumberSprites.Count == 13 && deckSkin.SmallSuitSprites.Count == 4 &&
            deckSkin.BigClubSprites.Count == 13 && deckSkin.BigDiamondSprites.Count == 13 && deckSkin.BigHeartSprites.Count == 13 && deckSkin.BigSpadeSprites.Count == 13)
            EditorGUILayout.HelpBox("Deck skin ready.", MessageType.Info);
        else EditorGUILayout.HelpBox("Sprites missing.", MessageType.Error);

    }
}
