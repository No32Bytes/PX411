using UnityEngine;

public class LeverButton : MonoBehaviour
{
    public FuseBoxPuzzle mainCabinet;
    public string leverColor;

    private int currentPosition = 0;

    private void Start()
    {
        // Startzustand: Hebel zeigt im 35-Grad-Winkel nach oben
        transform.localEulerAngles = new Vector3(-35, 0, 0);
    }

    public void ToggleLever()
    {
        if (mainCabinet == null)
        {
            //Debug.LogError("[Hebel-System] Fehler auf '" + gameObject.name + "': 'Main Cabinet' ist im Inspector nicht zugewiesen!");
            return;
        }

        currentPosition = (currentPosition + 1) % 3;

        //string positionName = "";
        if (currentPosition == 0)
        {
            // Zurück nach oben (35 Grad)
            transform.localEulerAngles = new Vector3(-35, 0, 0);
            //positionName = "Schräg nach oben (35°)";
        }
        else if (currentPosition == 1)
        {
            // 1. Klick: Zeigt gerade nach vorne zum Spieler (0 Grad)
            transform.localEulerAngles = new Vector3(0, 0, 0);
            //positionName = "Gerade nach vorne zum Spieler (0°)";
        }
        else if (currentPosition == 2)
        {
            // 2. Klick: Klappt nach unten weg (35 Grad)
            transform.localEulerAngles = new Vector3(35, 0, 0);
            //positionName = "Schräg nach unten (35°)";
        }

        //Debug.Log("<color=cyan>[Hebel-System]</color> Hebel <b>" + leverColor + "</b> rotiert auf Zustand: <b>" + positionName + "</b> (Index: " + currentPosition + ")");
        mainCabinet.UpdateLeverPosition(leverColor, currentPosition);
    }
}