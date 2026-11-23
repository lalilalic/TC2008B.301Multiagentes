using UnityEngine;

// Anomalía 1 — El espejo.
// Rara: la superficie late en magenta y la cruza un destello.
// Calmada: vuelve a ser un espejo plateado.
public class DSMirrorAnomaly : DSAnomaly
{
    public Renderer surface;    // la superficie del espejo
    public Transform streak;    // el destello diagonal

    protected override void Rara()
    {
        // Mathf.Sin sube y baja entre -1 y 1, con el 0.5 + 0.5 queda entre 0 y 1
        // asi "onda" va de 0 a 1 y de regreso todo el tiempo como un latido
        float onda = 0.5f + 0.5f * Mathf.Sin(Time.time * 3f);
        if (surface != null)
        {
            // Color.Lerp mezcla dos colores segun la onda
            // con 0 queda morado oscuro y con 1 queda magenta, asi el espejo late
            surface.material.color = Color.Lerp(new Color(0.16f, 0.03f, 0.13f), new Color(0.42f, 0.07f, 0.32f), onda);
            // cambia el brillo propio del vidrio (emision) para que tambien lata
            surface.material.SetColor("_EmissionColor", glowColor * (0.4f + onda));
        }

        // El destello sube en diagonal y vuelve a empezar (Mathf.Repeat da un valor de 0 a 1 que se repite)
        if (streak != null)
        {
            float t = Mathf.Repeat(Time.time * 0.4f, 1f);
             // Mathf.Repeat da un valor de 0 a 1 que cuando llega a 1 regresa a 0 y se repite
            streak.localPosition = new Vector3(Mathf.Lerp(-0.34f, 0.34f, t), Mathf.Lerp(0.55f, 1.8f, t), streak.localPosition.z);
        }
    }

    protected override void AlEstabilizar() // Gerado con IA: DSAnomaly llama a AlEstabilizar() una vez cuando se estabiliza
    {
        if (streak != null) streak.gameObject.SetActive(false);  // esconde el destello (SetActive visto en clase)
        if (surface != null)
        {
            surface.material.color = new Color(0.78f, 0.8f, 0.82f); // el espejo vuelve a ser gris plateado normal
            surface.material.SetColor("_EmissionColor", Color.black);  // Color.black en la emision = ya no brilla
        }
    }
}

// Justificacion: queria que el espejo se viera embrujado como si algo estuviera del otro lado por eso late en magenta
// y lo cruza un destello, cambiar el color del material mientras se juega no lo vimos en clase y lo hice con apoyo de IA (Claude)
