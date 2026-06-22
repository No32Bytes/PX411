using UnityEngine;

public class SafeButton : MonoBehaviour
{
    public KeypadSafePuzzle safeMainScript;

    public bool isColorButton;
    public string colorName;

    public bool isNumberButton;
    public int buttonValue;

    public void TriggerButton()
    {
        if (safeMainScript == null)
        {
            return;
        }

        if (isColorButton)
        {
            safeMainScript.SelectColor(colorName);
        }
        else if (isNumberButton)
        {
            safeMainScript.PressNumberButton(buttonValue);
        }
    }
}