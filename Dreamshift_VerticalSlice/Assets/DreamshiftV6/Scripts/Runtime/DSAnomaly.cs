using UnityEngine;

// Base de las 3 anomalías (Espejo, Maleta y Reloj)
// Cada anomalía tiene un Box Collider con Is Trigger alrededor todo esto visto en Clase 2
// Si el jugador está dentro del trigger y mantiene E durante 2 segundos, la anomalía se estabiliza


public class DSAnomaly : MonoBehaviour
{
    public string displayName = "Anomalía";  // nombre que se muestra en el juego
    public int order;                        // orden en de la lista de anomalias del GameManager (Clase 3)

    public Light glowLight;                  // luz magenta de la anomalía
    public Renderer[] glowRenderers;         // arreglo de piezas que brillan en magenta
    public Color glowColor = new Color(1f, 0.25f, 0.64f); // color magenta (rojo, verde, azul de 0 a 1)
    public float glowLightIntensity = 2.5f; // que tan fuerte brilla la luz

    public Animator animator;                // Animator Controller AC_Anomalia parámetro "Estabilizada" esto visto en Clase 3
    public ParticleSystem sparks;            // chispas que flotan alrededor - visto en clase 3
    public ParticleSystem burst;             // estallido al estabilizar - Clase 3

    public float tiempoParaEstabilizar = 2f; // segundos que hay que mantener E

    public bool despierta = false;  // el jugador ya entró a su habitación
    public bool estabilizada = false; // ya se resolvió esta anomalia
    public float progreso = 0f; // de 0 a 1 mientras se mantiene E

    protected AudioSource sonido;   // sonidos generados con apoyo de IA

    // Utilice IA - relacionado a audio donde "virtual" permite que una clase hija reescriba
    // Start() con "override" si necesita hacer algo extra al iniciar
    protected virtual void Start() 
    {
        sonido = gameObject.AddComponent<AudioSource>();
        sonido.playOnAwake = false; // no suena sola al iniciar el juego
        sonido.spatialBlend = 1f; // sonido 3D, se oye más fuerte de cerca
        sonido.maxDistance = 14f; // a más de 14 metros ya casi no se oye
    }

    // La llama DSRoomZone cuando el jugador entra a la habitación
    // Despierta la anomalía, empieza a brillar fuerte y a echar más chispas.
    public void Despertar()
    {
        if (despierta || estabilizada) return; // Si ya despertó o ya se resolvió no hace nada (evita repetir el efecto)
        despierta = true;

        //  IA = PlayOneShot reproduce un sonido una sola vez
        // Whoosh() es un soplido generado por código en DSAudioSynth
        sonido.PlayOneShot(DSAudioSynth.Whoosh());

        // Más chispas cuando la anomalía despierta
        if (sparks != null)
        {
            // IA - En clase las partículas se configuraron en el Inspector
            // Aquí se entra por código al módulo Emission del Particle System y se multiplica por 3 la cantidad de chispas por segundo
            var emision = sparks.emission;
            emision.rateOverTimeMultiplier = emision.rateOverTimeMultiplier * 3f;
        }
    }

    //  Trigger (Clase 2) 

    void OnTriggerStay(Collider other)
    {
        if (estabilizada) return; // ya resuelta, no hacer nada
        if (!other.CompareTag("Player")) return;  // solo reacciona al jugador (Clase 2)

        // En clase buscábamos el gestor con FindFirstObjectByType
        // Aquí el GameManager se guarda a sí mismo en una variable static "Instance" patrón Singleton para no buscarlo en cada frame
        DSGameManager gestor = DSGameManager.Instance;
        if (gestor == null || !gestor.Jugando) return;

        gestor.anomaliaCercana = this; // avisa al gestor cual anomalia está cerca
        Despertar();  // por si el jugador llegó sin pasar por la zona

        // Mantener E para estabilizar
        if (Input.GetKey(KeyCode.E) || Input.GetMouseButton(0))
        {
            // Time.deltaTime = segundos desde el frame anterior (Lab 3)
            // Al dividir entre tiempoParaEstabilizar (2 s), el progreso llega a 1 en 2 segundos
            progreso += Time.deltaTime / tiempoParaEstabilizar;
            if (progreso >= 1f) Estabilizar();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // OnTriggerExit en Unity lo llama UNA vez cuando el jugador sale  del trigger
        // Si el jugador se aleja entonces el progreso se pierde y vuelve a 0%
        if (!other.CompareTag("Player")) return;
        progreso = 0f;
        DSGameManager gestor = DSGameManager.Instance;
        // Si esta era la anomalía cercana se borra para que se quite el mensaje "Mantén E"
        if (gestor != null && gestor.anomaliaCercana == this) gestor.anomaliaCercana = null;
    }

    void Estabilizar() // Se llama cuando el progreso llega al 100%
    {
        estabilizada = true;
        progreso = 0f;
        sonido.PlayOneShot(DSAudioSynth.Chime()); // Sonido de campanita generado por IA (DSAudioSynth)

        // C# - Animator Controller (Clase 3): la gema crece y desaparece
        // cambia el parámetro bool Estabilizada a true y el Animator pasa del estado Indicador_Activo a Indicador_Estabilizado
        if (animator != null) animator.SetBool("Estabilizada", true);

        // Partículas (Clase 3) se apagan las chispas y sale el estallido
        if (sparks != null) sparks.Stop();
        if (burst != null) burst.Play();

        AlEstabilizar(); // efecto propio de cada anomalía
        // en este caso la maleta cae al piso

        DSGameManager gestor = DSGameManager.Instance;
        // Avisa al GameManager (patrón GestorPartida, Clase 2) porque suma 1 al contador y si ya van las 3 abre la puerta de salida
        if (gestor != null)
        {
            if (gestor.anomaliaCercana == this) gestor.anomaliaCercana = null;
            gestor.Estabilizar(this);
        }

        }

    // Brillo y animación por código

    //Esta parte fue creada con Inteligencia Artificial (Claude) 

    // El jugador necesita saber, con solo ver, cuál objeto es una anomalía y en qué estado está
    // Sin esto los objetos se verían como muebles normales del hotel por ello el brillo es fundamental y señal de visualización
    // 3 estados:
    //   - Dormida (jugador aún no entra):  brillo tenue (0.3)
    //   - Despierta (jugador en el cuarto): brillo fuerte que "late" como pulso
    //   - Estabilizada (ya resuelta):       brillo apagado (0)

    protected virtual void Update()
    {
        // Brillo tenue dormida, fuerte despierta, apagado al estabilizar
        float brillo = 0.3f;
        if (despierta) brillo = 0.75f + 0.25f * Mathf.Sin(Time.time * 3f);
        if (estabilizada) brillo = 0f;

        if (glowLight != null) glowLight.intensity = glowLightIntensity * brillo;
        // Cambia la intensidad de la luz magenta según el brillo
        if (glowRenderers != null)
        {
            foreach (Renderer r in glowRenderers)
            {
                //  cambia por código el color de emisión del material (lo que hace que brille solo)
                if (r != null) r.material.SetColor("_EmissionColor", glowColor * (brillo * 5f));
                 
            }
        }
        // Cada anomalía hija decide qué hacer en cada estado
        if (estabilizada) Calmada();
        else Rara();
    }

    // Cada anomalía decide cómo se ve rara y cómo se ve calmada (Metodos virtuales vacios (herencia)
    // La anomalía hija (Espejo, Maleta o Reloj) reescribe estos métodos con override para hacer algo propio
    protected virtual void Rara() { }
    protected virtual void Calmada() { }
    protected virtual void AlEstabilizar() { }

}
// Justificacion con IA:

//Porque en mis pruebas (playtest) el jugador necesitaba identificar rápido qué objetos eran las anomalías
// El brillo magenta le dice dónde está, si ya despertó y si ya la resolvió
// Lo hice con IA porque en clase solo cambiamos los colores de los materiales a mano en el Inspector
// Por lo tanto, yo necesitaba que el brillo cambiara mientras se juega, según el estado de la anomalía
// Además quería que el brillo latiera como un pulso, y para eso se usa Mathf.Sin
// El cual es na función matemática que sube y baja con el tiempo