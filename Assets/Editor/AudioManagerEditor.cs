using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AudioManager))]
class AudioManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate SoundTrack content"))
            GenerateContent();
    }
    private void GenerateContent()
    {
        List<SoundTrack> soundTrackStore = (target as AudioManager).InternalGetSoundTrackStore;
        EditorHelper.FindAssetsAndSaveToList(soundTrackStore, target);
    }
}