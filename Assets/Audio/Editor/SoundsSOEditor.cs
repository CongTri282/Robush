using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundsSO))]
public class SoundsSOEditor : Editor
{
    private SerializedProperty soundLists;

    private void OnEnable()
    {
        soundLists = serializedObject.FindProperty("soundLists");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Sound Groups", EditorStyles.boldLabel);

        for (int i = 0; i < soundLists.arraySize; i++)
        {
            var group = soundLists.GetArrayElementAtIndex(i);
            var type = group.FindPropertyRelative("type");
            var sounds = group.FindPropertyRelative("sounds");
            var volume = group.FindPropertyRelative("volume");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            type.enumValueIndex = (int)(SoundType)EditorGUILayout.EnumPopup("Type", (SoundType)type.enumValueIndex);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                soundLists.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(sounds, new GUIContent("Audio Clips"), true);
            volume.floatValue = EditorGUILayout.Slider("Volume", volume.floatValue, 0f, 1f);

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Sound Group"))
        {
            soundLists.InsertArrayElementAtIndex(soundLists.arraySize);
            var newGroup = soundLists.GetArrayElementAtIndex(soundLists.arraySize - 1);
            newGroup.FindPropertyRelative("type").enumValueIndex = 0;
            newGroup.FindPropertyRelative("volume").floatValue = 1f;
        }

        serializedObject.ApplyModifiedProperties();
    }
}