using UnityEngine;

// Puerta de salida (como el script Salida de la Clase 2)
// Letrero rojo mientras falten anomalías. Al cumplir la meta el gestor llama a Abrir():
// el letrero se pone verde, el Animator abre la puerta y salen partículas verdes
// Detrás de la puerta hay un Box Collider con Is Trigger: si el jugador entra, gana
public class DSExitDoor : MonoBehaviour
{
    public Animator animator;               // Animator Controller AC_Puerta_Salida (parámetro "Abierta")
    public ParticleSystem openParticles;    // partículas verdes al abrir
    public Transform leftLeaf;              // hoja izquierda (la mueve el Animator)
    public Transform rightLeaf;             // hoja derecha (la mueve el Animator)
    public Renderer[] signRenderers;        // el letrero de SALIDA
    public Light signLight;                 // luz del letrero
    public Vector3 victoryZoneCenter = new Vector3(0f, 1.2f, 46.8f);  // trigger de victoria
    public Vector3 victoryZoneSize = new Vector3(2.8f, 3f, 2.4f);

    private bool abierta = false;
    private AudioSource sonido;  // sonido generado con apoyo de IA

    void Start()
    {
        sonido = gameObject.AddComponent<AudioSource>();
        sonido.playOnAwake = false;
        PintarLetrero(Color.red);
    }

    public void Abrir()
    {
        if (abierta) return;
        abierta = true;
        PintarLetrero(Color.green);

        // C# → Animator Controller (Clase 3)
        if (animator != null) animator.SetBool("Abierta", true);
        if (openParticles != null) openParticles.Play();
        sonido.PlayOneShot(DSAudioSynth.Unlock());
    }

    // OnTriggerEnter (Clase 2): unity lo llama una sola vez cuando algo entra
    // al Box Collider con Is Trigger que esta detras de la puerta
    void OnTriggerEnter(Collider other)
    {
        // si lo que entro no es el jugador no hace nada (Clase 2)
        if (!other.CompareTag("Player")) return;

        // busca el GameManager en la escena igual que en la Clase 2 (FindAnyObjectByType)
        DSGameManager gestor = Object.FindAnyObjectByType<DSGameManager>();
        if (gestor == null) return;   // si no lo encuentra no hace nada para que no de error

        // solo se gana si ya se estabilizaron todas las anomalias
        // si el jugador llega antes no pasa nada, la puerta sigue cerrada
        if (gestor.MisionCumplida) gestor.Ganar();
    }

    //  IA cambia el color de la luz y del brillo del letrero de SALIDA
    // rojo = todavia faltan anomalias, verde = ya puedes salir
    // en clase los colores se ponian a mano en el Inspector y aqui cambia mientras se juega
    void PintarLetrero(Color color)
    {
        // cambia el color de la luz del letrero
        if (signLight != null) signLight.color = color;

        // si no hay piezas del letrero ya no sigue
        if (signRenderers == null) return;

        // foreach recorre cada pieza del letrero
        foreach (Renderer r in signRenderers)
        {
            // SetColor cambia el color de emision (lo que hace que brille solo)
            // por 3.5 para que brille fuerte y se vea desde lejos en el pasillo oscuro
            if (r != null) r.material.SetColor("_EmissionColor", color * 3.5f);
        }
    }
}

// justificacion con IA: el jugador tiene que saber cuando ya puede salir, por eso
// el letrero esta en rojo mientras faltan anomalias y cambia a verde cuando ya las
// estabilizo todas, ademas suena la cerradura y salen particulas verdes, asi queda
// claro que se cumplio el objetiv