using TMPro;
using UnityEngine;

public class CheatScreen : MonoBehaviour
{
    [SerializeField] private Light lightTarget;
    [SerializeField] private GameObject teacher;
    [SerializeField] private TMP_Text lightTextState;
    [SerializeField] private TMP_Text disableTeacherState;
    [SerializeField] private float lightCheatOn;
    [SerializeField] private float lightCheatOff;
    private bool lightCheatEnabled = false;
    private bool teacherCheatEnabled = false;
    public void OnLightCheatClick()
    {
        lightCheatEnabled = !lightCheatEnabled;
        if (lightCheatEnabled)
            lightTarget.intensity = lightCheatOn;
        else
            lightTarget.intensity = lightCheatOff;

        lightTextState.text = lightCheatEnabled.ToString();
    }

    public void OnToggleTeacherClick()
    {
        teacherCheatEnabled = !teacherCheatEnabled;
        teacher.SetActive(!teacherCheatEnabled);
        disableTeacherState.text = teacherCheatEnabled.ToString();
    }
}