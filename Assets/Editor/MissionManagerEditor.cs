using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionManager))]
public class MissionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MissionManager mm = (MissionManager)target;

        if (mm.PresetMissionsCount > 0)
            EditorGUILayout.HelpBox(mm.PresetMissionsCount + " preset missions.", MessageType.Info);
        else EditorGUILayout.HelpBox("No preset missions.", MessageType.Error);

        if (GUILayout.Button("Load Preset Missions"))
        {
            mm.LoadPresetMissions();
            EditorUtility.SetDirty(mm);
        }

        if (mm.PresetRewardsCount > 0)
            EditorGUILayout.HelpBox(mm.PresetRewardsCount + " preset rewards.", MessageType.Info);
        else EditorGUILayout.HelpBox("No preset rewards.", MessageType.Error);

        if (GUILayout.Button("Load Preset Rewards"))
        {
            mm.LoadPresetRewards();
            EditorUtility.SetDirty(mm);
        }
    }
}
