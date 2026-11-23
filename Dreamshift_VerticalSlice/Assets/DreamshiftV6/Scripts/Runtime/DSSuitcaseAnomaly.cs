using UnityEngine;

// Anomalía 2 — La maleta.
// Rara: flota sobre la cama, sube y baja y gira despacio.
// Calmada: cae a su lugar sobre la cama.
public class DSSuitcaseAnomaly : DSAnomaly
{
    public Transform body;      // el cuerpo de la maleta (lo que se mueve)

    private Vector3 posicionInicial; 
    private Quaternion rotacionInicial; // guarda la posición y rotación inicial de la maleta para regresar a ella
    private float giro = 0f; // cuanto ha girado la maleta sobre sí misma

    protected override void Start() // Generado con IA override reescribe el Start() de DSAnomaly
    {
        base.Start(); // base.Start() primero hace el Start() de DSAnomaly (crea el AudioSource)
        if (body != null)  // guarda donde esta la maleta para luego regresarla a ese lugar
        {
            posicionInicial = body.localPosition;
            rotacionInicial = body.localRotation;
        }
    }

    protected override void Rara()
    {
        if (body == null) return;
        float altura = 0.05f; // dormida: flota poquito y gira lento
        float velocidadGiro = 6f;
        if (despierta) // despierta (el jugador entro al cuarto) entonces sube mas alto y gira mas rapido
        {
            altura = 0.14f;
            velocidadGiro = 28f;
        }
        giro += velocidadGiro * Time.deltaTime;

        // Flota: sube y baja con Mathf.Sin y gira sobre sí misma
        body.localPosition = posicionInicial + new Vector3(0f, altura + Mathf.Sin(Time.time * 1.7f) * 0.03f, 0f);
        body.localRotation = rotacionInicial * Quaternion.Euler(0f, giro, 0f);
    }

    protected override void Calmada()
    // Gerado con IA: DSAnomaly llama a Calmada() cada frame cuando se estabiliza
    {
        if (body == null) return;
        // Cae suavemente a su lugar
        // Lerp acerca la posicion poquito a poquito cada frame hacia la posicion inicial
        // Slerp hace lo mismo pero con la rotacion, asi no cae de golpe
        body.localPosition = Vector3.Lerp(body.localPosition, posicionInicial, Time.deltaTime * 8f);
        body.localRotation = Quaternion.Slerp(body.localRotation, rotacionInicial, Time.deltaTime * 8f);
    }

    protected override void AlEstabilizar() // Gerado con IA: DSAnomaly llama a AlEstabilizar() una vez cuando se estabiliza
    {
         // sonido de golpe sordo como si la maleta cayera en la cama (generado en DSAudioSynth)
        sonido.PlayOneShot(DSAudioSynth.Thump());
    }
}

// Justificación: Este script es una clase hija de DSAnomaly que hereda sus propiedades y métodos
// Se utiliza para controlar el comportamiento de la anomalía de la maleta en el juego. Se utilizo Codex para generar el código

// La clase sobrescribe los métodos Start(), Rara(), Calmada() y AlEstabilizar() para definir cómo se comportara la maleta
// en los diferentes estados (dormida, despierta, estabilizada)

// Ademas se uso técnicas de interpolación (Lerp y Slerp) para suavizar sus movimientos y rotaciones de la maleta, 
// y estos reproducen los sonidos que son generados por IA para mejorar la experiencia del jugador