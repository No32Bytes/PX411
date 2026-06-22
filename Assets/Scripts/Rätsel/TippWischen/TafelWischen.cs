using UnityEngine;

public class TafelWischer : BaseEntity
{
    [Header("Wisch-Einstellungen")]
    public float wischSchritt = 0.08f;

    private Material dreckMaterial;
    private float aktuelleDeckkraft = 1.0f;
    private bool istSauber = false;

    private void Start()
    {
        if (TryGetComponent<Renderer>(out var rend))
        {
            dreckMaterial = rend.material;

            SetzeSichtbarkeit(1.0f);
        }
    }

    public override void EntityInteraction()
    {
        if (istSauber) return;
        WischeEinStueck();
    }

    private void WischeEinStueck()
    {
        aktuelleDeckkraft -= wischSchritt;
        aktuelleDeckkraft = Mathf.Clamp01(aktuelleDeckkraft);

        SetzeSichtbarkeit(aktuelleDeckkraft);

        if (aktuelleDeckkraft <= 0.02f)
        {
            TafelIstSauber();
        }
    }

    private void SetzeSichtbarkeit(float deckkraft)
    {
        if (dreckMaterial == null) return;

        if (dreckMaterial.HasProperty("_BaseColor"))
        {
            Color farbe = dreckMaterial.GetColor("_BaseColor");
            farbe.a = deckkraft;
            dreckMaterial.SetColor("_BaseColor", farbe);
        }
        else if (dreckMaterial.HasProperty("_Color"))
        {
            Color farbe = dreckMaterial.GetColor("_Color");
            farbe.a = deckkraft;
            dreckMaterial.SetColor("_Color", farbe);
        }

        float clipWert = 1.0f - deckkraft;
        string[] clipEigenschaften = { "_Cutoff", "_AlphaClip", "_AlphaCutoff", "_AlphaClipThreshold" };

        foreach (string prop in clipEigenschaften)
        {
            if (dreckMaterial.HasProperty(prop))
            {
                dreckMaterial.SetFloat(prop, clipWert);
            }
        }
    }

    private void TafelIstSauber()
    {
        istSauber = true;
        //Debug.Log("Tafel erfolgreich freigewischt!");
        gameObject.SetActive(false);
    }
}