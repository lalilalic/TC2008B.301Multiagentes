using UnityEngine;

// Ambiente sonoro (generado con apoyo de IA):
// zumbido eléctrico constante y, de vez en cuando, crujidos o golpes lejanos

public class DSAmbience : MonoBehaviour
{
    // 0 = tranquilo, 1 = máxima tensión (lo controla DSTimePressureFX)
    public static float pressure = 0f;
    // Volumen base del zumbido (0 = silencio, 1 = máximo) igual se puede ajustar en el Inspector
    public float volumenZumbido = 0.16f;

    // AudioSource = componente que reproduce sonido
    private AudioSource zumbido; // bocina del zumbido constante
    private AudioSource lejanos; // bocina de los sonidos lejanos
    private float siguienteSonido; // bocina de los crujidos y toquidos lejanos

    void Start()
    {
        // Bocina 1: zumbido constante
        // Agregamos por código un AudioSource al GameManager
        zumbido = gameObject.AddComponent<AudioSource>();
        // El zumbido es un sonido generado por código (no un clip de audio)
        zumbido.clip = DSAudioSynth.Hum(); 
        zumbido.loop = true; // al terminar vuelve a empezar y suena toda la partida
        zumbido.volume = volumenZumbido; // volumen bajito
        zumbido.Play(); // comienza a sonar

        // Bocina 2: sonidos lejanos
        // Se crea un objeto vacío aparte porque esta bocina se moverá por el hotel 
        GameObject objeto = new GameObject("SonidosLejanos");
        objeto.transform.SetParent(transform); // lo hacemos hijo del GameManager para que quede ordenado
        lejanos = objeto.AddComponent<AudioSource>();
        lejanos.playOnAwake = false; // no suena al iniciar, solo cuando se le indica
        lejanos.spatialBlend = 1f;
        // spatialBlend = 1, es el sonido 3D y se oye según dónde esté la bocina
        lejanos.maxDistance = 25f; // a partir de 25 metros ya no se oye
        siguienteSonido = Time.time + Random.Range(5f, 10f);
        // El primer sonido lejano será entre 5 y 10 segundos después de empezar (Random.Range, Lab 6)
    }

    void Update()
    {
        // Con más tensión el zumbido sube y se hace más grave
        zumbido.volume = volumenZumbido + 0.22f * pressure;
        zumbido.pitch = 1f - 0.08f * pressure;

        // return = salir del método sin hacer lo de abajo
        // Si no se está jugando (menú, pausa o fin de partida) no suenan sonidos lejanos
        DSGameManager gestor = DSGameManager.Instance;
        if (gestor != null && !gestor.Jugando) return;
        // Si todavía no llega la hora del siguiente sonido, no hacemos nada
        if (Time.time < siguienteSonido) return;

        // Programamos el próximo sonido: entre 6 y 14 s al azar.
        // Con tensión la espera se acorta a la mitad (entre 3 y 7 s con pressure = 1).
        siguienteSonido = Time.time + Random.Range(6f, 14f) * (1f - 0.5f * pressure);

        // El sonido aparece en un punto al azar cerca de la cámara
        Vector3 posicion = Vector3.zero;
        // Camera.main = la cámara principal (la del jugador)
        if (Camera.main != null) posicion = Camera.main.transform.position;
        // Random.insideUnitSphere = punto al azar dentro de una esfera de radio 1 (× 9 = radio 9 m)
        posicion += Random.insideUnitSphere * 9f;
        posicion.y = 1.5f; // a la altura de la cabeza (ni bajo el piso ni en el techo)
        lejanos.transform.position = posicion; // movemos la bocina 3D a ese punto
        lejanos.pitch = Random.Range(0.8f, 1.15f); // variamos un poco la altura del sonido para que no suene siempre igual

        // 70 % de las veces un crujido, 30 % unos toquidos (Random.value = número al azar entre 0 y 1)
        // IA - PlayOneShot = reproduce un sonido UNA vez (sonidos creados en DSAudioSynth)
        if (Random.value < 0.7f) lejanos.PlayOneShot(DSAudioSynth.Creak(), 0.7f);
        else lejanos.PlayOneShot(DSAudioSynth.Knock(), 0.7f);
    }
}

// Justificacion

// En un juego de terror psicológico, el silencio total rompe la sensación de estar en un hotel  abandonado
// Por eso agregué un zumbido eléctrico constante (música de fondo) y los crujidos y golpes  que estan de manera aleatoria en 3D para que suenen cerca del jugador
// Ademas estos sonidos ponen al usuario en tensión mientras busca las anomalías, tambien el zumbido sube de volumen cuando se te esta acabando el tiempo (pressure)

// Uso de IA: el manejo de AudioSource (loop, spatialBlend, pitch, PlayOneShot) no se vio en clase
// Esta parte esta generado con apoyo de Claude, Codex y GitHub Copilot asimismo confirme con la información de Unity 
// AudioSource.PlayOneShot, spatialBlend y pitch
