using UnityEngine;

// IA - RequireComponent hace que el objeto siempre tenga una Light
[RequireComponent(typeof(Light))]
public class DSLightFlicker : MonoBehaviour
{
    public Renderer emissiveRenderer;   // el vidrio del aplique que tambien brilla
    public bool faulty = false;         // si esta en true esta luz falla y se apaga a ratos

    private Light luz;                  // la luz de este objeto
    private float intensidadBase;       // que tan fuerte brilla al inicio
    private Color emisionBase;          // color del brillo del vidrio al inicio
    private float apagadaHasta = 0f;    // hasta que segundo se queda apagada

    void Start()
    {
        luz = GetComponent<Light>();     // agarra la luz (GetComponent visto en clase)
        intensidadBase = luz.intensity;  // guarda la intensidad original

        // IA - guarda el color de emision original del vidrio
        if (emissiveRenderer != null) emisionBase = emissiveRenderer.material.GetColor("_EmissionColor");
    }

    void Update()
    {
        // temblor pequeño de vela: cada frame la luz brilla entre 94% y 100%
        float k = Random.Range(0.94f, 1f);

        // Uso de la Inteligencia Artificial si es una luz que falla a veces se apaga un instante
        // Random.value da un numero al azar entre 0 y 1, si sale menor a 0.0027f (muy poquitas veces) se apaga entre 0.05 y 0.3 segundos
        if (faulty && Time.time > apagadaHasta && Random.value < 0.0027f)
        {
            apagadaHasta = Time.time + Random.Range(0.05f, 0.3f);
        }
        if (Time.time < apagadaHasta) k = 0.12f;   // mientras esta "apagada" brilla casi nada

        // Generado con IA (Codex) -  cambia la intensidad de la luz y el brillo del vidrio por codigo
        luz.intensity = intensidadBase * k;
        if (emissiveRenderer != null) emissiveRenderer.material.SetColor("_EmissionColor", emisionBase * k);
    }
}

// justificacion con IA: el juego es de un hotel abandonado y unas luces que fallan hacen que se sienta viejo y de miedo, 
//  esto dando una experiencia mas realista para el usuario. En clase las luces se ponian fijas en el Inspector
// por eso el parpadeo por codigo lo hice con apoyo de IA (Codex)