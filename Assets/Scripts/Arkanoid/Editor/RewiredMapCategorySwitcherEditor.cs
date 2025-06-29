using UnityEditor;
using UnityEngine;
using Rewired;

[CustomEditor(typeof(RewiredMapCategorySwitcher))]
public class RewiredMapCategorySwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var switcher = (RewiredMapCategorySwitcher)target;

        // PlayerId
        switcher.GetType().GetField("m_PlayerId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .SetValue(switcher, EditorGUILayout.IntField("Player Id", (int)switcher.GetType().GetField("m_PlayerId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(switcher)));

        // Sprawdź, czy Rewired jest zainicjalizowany
        if (!ReInput.isReady)
        {
            EditorGUILayout.HelpBox("Rewired is not initialized. Add a Rewired Input Manager to the scene.", MessageType.Warning);
            return;
        }

        // Pole do ręcznego wpisania indexu
        int currentIndex = (int)switcher.GetType().GetField("m_MapCategoryIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(switcher);
        int newIndex = EditorGUILayout.IntField("Map Category Index", currentIndex);
        if (newIndex != currentIndex)
        {
            switcher.SetCategoryByIndex(newIndex, "");
            EditorUtility.SetDirty(switcher);
        }
    }
} 