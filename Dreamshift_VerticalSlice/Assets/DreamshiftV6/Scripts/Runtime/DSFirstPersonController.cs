using UnityEngine;

// Jugador 
// Se mueve con Rigidbody + MovePosition en FixedUpdate, igual que el bombero de la Clase 1, y gira la cámara con el ratón
[RequireComponent(typeof(Rigidbody))]
public class DSFirstPersonController : MonoBehaviour
{
    public Transform cameraPivot;       // la "Cabeza", donde está la cámara
    public float velocidad = 2.2f;      // caminar
    public float velocidadCorrer = 3.6f; // con Shift
    public float sensibilidad = 2f;     // giro del ratón

    private Rigidbody rb; // el Rigidbody del jugador (Clase 1)
    private Vector3 movimiento;  // hacia donde y que tan rapido se mueve, se calcula en Update y se usa en FixedUpdate
    private float giroHorizontal;  // cuanto ha girado el cuerpo a la izquierda o derecha
    private float giroVertical;    // cuanto ha girado la cabeza hacia arriba o abajo
    private Vector3 posicionCabeza; // la posición de la cámara, para que no suba ni baje al mirar arriba o abajo

    // Pasos (sonido generado con apoyo de IA)
    private AudioSource pasos;  // la "bocina" que suena los pasos
    private float distanciaPaso = 0f; // cuanto ha caminado desde el ultimo paso

    void Start()
    {
        rb = GetComponent<Rigidbody>();   // agarra el Rigidbody del jugador (Clase 1)
        rb.freezeRotation = true;       // que el jugador no se caiga de lado
        giroHorizontal = transform.eulerAngles.y;   // empieza mirando hacia donde ya esta volteado el jugador en la escena
        if (cameraPivot != null) posicionCabeza = cameraPivot.localPosition;   // guarda la posicion de la cabeza para dejar la camara siempre a esa altura

        // Sonido de pasos (generado con apoyo de IA)
        pasos = gameObject.AddComponent<AudioSource>();
        pasos.playOnAwake = false;  // que no suene solo al iniciar
        pasos.volume = 0.35f;  // volumen bajito para que no moleste
    }

    void Update()
    {
        DSGameManager gestor = DSGameManager.Instance;
         // si el juego no esta en modo Jugando (menu, pausa, gano o perdio) el jugador no se mueve
        if (gestor != null && !gestor.Jugando)
        {
            movimiento = Vector3.zero;  // se queda quieto
            return;  // y no lee el teclado ni el raton
        }

        // Teclado: igual que en la Clase 1
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float vel = velocidad; // si se aprieta Shift corre, si no camina
        if (Input.GetKey(KeyCode.LeftShift)) vel = velocidadCorrer;
         // transform.right y transform.forward son derecha y adelante del jugador
        movimiento = (transform.right * x + transform.forward * z).normalized * vel;

        // Ratón: giro horizontal del cuerpo y vertical de la cabeza
        // Generado con apoyo de IA (Claude) porque en clase solo se giraba la cámara, no el cuerpo del jugador
        giroHorizontal += Input.GetAxis("Mouse X") * sensibilidad;
        giroVertical -= Input.GetAxis("Mouse Y") * sensibilidad;
        giroVertical = Mathf.Clamp(giroVertical, -80f, 80f);

        if (cameraPivot != null)
        {
            // No visto en clases IA, Quaternion.Euler convierte grados a una rotacion de unity
            cameraPivot.localRotation = Quaternion.Euler(giroVertical, 0f, 0f);
            cameraPivot.localPosition = posicionCabeza;   // la cámara se queda a la altura de la cabeza
        }

        // Sonido de pasos cada cierta distancia recorrida
        // Hecho con IA (Claude) porque en clase solo se reproducía un clip de audio, no se generaba el paso por código
        distanciaPaso += movimiento.magnitude * Time.deltaTime;
        if (distanciaPaso > 0.75f)  // cada 0.75 metros suena un paso
        {
            distanciaPaso = 0f;
              // Random.Range (Lab 6) cambia un poquito el tono para que no todos los pasos suenen igual
            pasos.pitch = Random.Range(0.9f, 1.1f);
            pasos.PlayOneShot(DSAudioSynth.Step());   // el paso se genera por codigo en DSAudioSynth
        }
    }

    void FixedUpdate()
    {
        // Física: el Rigidbody choca con las paredes (Clase 1)
        // Time.fixedDeltaTime es el tiempo de cada paso de fisica
        rb.MovePosition(rb.position + movimiento * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(0f, giroHorizontal, 0f));
    }
}

// Justificacion con IA: mi juego es en primera persona entonces el jugador tiene que poder voltear para todos lados con el raton 
// para buscar las anomalias en los distintos cuartios creados, ya que en los labs de clase el bombero se movia solo con teclado y la camara estaba fija
// por eso la parte del raton la hice con apoyo de IA