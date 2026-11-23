using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// la partida solo puede estar en uno de estos estados a la vez
public enum EstadoPartida { Menu, Jugando, Pausa, Victoria, Derrota }

public class DSGameManager : MonoBehaviour
{
    public float duration = 120f;       // duracion de la partida en segundos (2 minutos)
    public DSExitDoor salida;           // la puerta que se abre al cumplir la meta, se arrastra en el Inspector

    //  en clase buscabamos el gestor con FindFirstObjectByType
    // aqui el gestor se guarda solo en esta variable static para que los otros scripts
    // lo usen directo con DSGameManager.Instance sin buscarlo cada frame
    public static DSGameManager Instance;

    public EstadoPartida estado = EstadoPartida.Menu;   // en que estado esta la partida, empieza en el menu
    public float tiempoRestante;                        // cuantos segundos quedan
    public int estabilizadas = 0;                       // cuantas anomalias ya resolvio el jugador
    public List<DSAnomaly> anomalias = new List<DSAnomaly>();   // lista con las 3 anomalias (List visto en Lab 6)

    // la anomalia que el jugador tiene cerca, la usa el Canvas para escribir "Mantén E"
    public DSAnomaly anomaliaCercana;

    //  IA static hace que el valor no se borre al recargar la escena
    // sirve para que al dar "Reintentar" se juegue directo sin pasar por el menu
    private static bool saltarMenu = false;

    // mensaje corto que aparece unos segundos en pantalla
    private string aviso = "";
    private float avisoHasta = 0f;      // hasta que segundo se sigue viendo el mensaje

    // IA - propiedad con get: se usa como variable pero calcula el valor al momento
    // regresa true cuando ya se estabilizaron todas, la salida lo revisa antes de dejarte ganar
    public bool MisionCumplida
    {
        get { return estabilizadas >= anomalias.Count; }
    }

    //  IA propiedad con get donde true si la partida esta en modo Jugando
    public bool Jugando
    {
        get { return estado == EstadoPartida.Jugando; }
    }

    // Awake se ejecuta antes que Start, asi el gestor ya esta listo cuando los demas lo buscan
    void Awake()
    {
        Instance = this;            // se guarda a si mismo para que los otros scripts lo encuentren
        tiempoRestante = duration;  // empieza con los 2 minutos completos

        // FindObjectsByType busca TODAS las anomalias de la escena
        // (en clase usamos FindFirstObjectByType que busca solo una) y AddRange las mete a la lista
        anomalias.AddRange(FindObjectsByType<DSAnomaly>());

        // IA Sort ordena la lista segun el numero "order" de cada anomalia
        // asi en el Canvas siempre salen en orden: 1 Espejo, 2 Maleta, 3 Reloj
        anomalias.Sort((a, b) => a.order.CompareTo(b.order));
    }

    void Start()
    {
        // si venimos de "Reintentar" se juega directo, si no se muestra el menu
        if (saltarMenu)
        {
            saltarMenu = false;
            Jugar();
        }
        else
        {
            CambiarEstado(EstadoPartida.Menu);
        }
    }

    void Update()
    {
        if (estado == EstadoPartida.Jugando)
        {
            // temporizador (Lab 3): restamos el tiempo de cada frame
            tiempoRestante -= Time.deltaTime;

            // si se acaba el tiempo pierdes
            if (tiempoRestante <= 0f)
            {
                tiempoRestante = 0f;
                Perder();
            }

            // Esc pausa el juego
            if (Input.GetKeyDown(KeyCode.Escape)) Pausar();
        }
        else if (estado == EstadoPartida.Pausa)
        {
            // si esta en pausa Esc lo quita
            if (Input.GetKeyDown(KeyCode.Escape)) Continuar();
        }
    }

    //  Botones del Canvas (se asignan en el OnClick del boton, Labs 2 y 3)

    // boton "Jugar" del menu
    public void Jugar()
    {
        tiempoRestante = duration;
        CambiarEstado(EstadoPartida.Jugando);
        MostrarAviso("Encuentra y estabiliza las 3 anomalías");
    }

    // se llama con Esc
    public void Pausar()
    {
        CambiarEstado(EstadoPartida.Pausa);
    }

    // boton "Continuar" de la pausa
    public void Continuar()
    {
        CambiarEstado(EstadoPartida.Jugando);
    }

    // botones "Reiniciar", "Jugar de nuevo" y "Reintentar"
    public void Reiniciar()
    {
        saltarMenu = true;  // al recargar se juega directo
        Time.timeScale = 1f;  // por si estaba en pausa, que el tiempo corra normal
        // recarga la misma escena para empezar de cero (Clase 2)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // botones "Menú principal"
    public void MenuPrincipal()
    {
        saltarMenu = false;   // al recargar sale el menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // boton "Salir" del menu
    // Uso de la IA para Application.Quit cierra el juego
    public void Salir()
    {
        Application.Quit();
    }

    // ¿Reglas del juego 

    // la llama cada anomalia cuando el jugador la estabiliza
    public void Estabilizar(DSAnomaly anomalia)
    {
        if (estado != EstadoPartida.Jugando) return;   // si no esta jugando no cuenta
        estabilizadas++;                               // suma 1 al contador

        if (MisionCumplida)
        {
            // ya estan las 3, se abre la puerta de salida (como en la Clase 2)
            MostrarAviso("La salida se ha abierto. ¡Corre al final del pasillo!");
            if (salida != null) salida.Abrir();
        }
        else
        {
            // todavia faltan: avisa cual se estabilizo y cuantas lleva
            MostrarAviso("Anomalía estabilizada: " + anomalia.displayName + " (" + estabilizadas + "/" + anomalias.Count + ")");
        }
    }

    // la llama la puerta de salida cuando el jugador entra con la mision cumplida
    public void Ganar()
    {
        if (estado != EstadoPartida.Jugando) return;
        CambiarEstado(EstadoPartida.Victoria);
    }

    // se llama cuando se acaba el tiempo
    public void Perder()
    {
        if (estado != EstadoPartida.Jugando) return;
        CambiarEstado(EstadoPartida.Derrota);
    }

    // guarda un mensaje para que el Canvas lo muestre 3.5 segundos
    public void MostrarAviso(string texto)
    {
        aviso = texto;
        // Uso de la IA Time.unscaledTime es el tiempo real que sigue corriendo
        // aunque el juego este en pausa (timeScale = 0)
        avisoHasta = Time.unscaledTime + 3.5f;
    }

    // regresa el mensaje solo mientras no hayan pasado los 3.5 segundos si no regresa vacio
    public string AvisoActual()
    {
        if (Time.unscaledTime < avisoHasta) return aviso;
        return "";
    }

    // cambia el estado de la partida y ajusta el tiempo y el raton
    void CambiarEstado(EstadoPartida nuevo)
    {
        estado = nuevo;

        // Generado con IA Time.timeScale controla que tan rapido corre el juego
        // 0 = todo se congela (menu y pausa), 1 = velocidad normal
        if (estado == EstadoPartida.Menu || estado == EstadoPartida.Pausa) Time.timeScale = 0f;
        else Time.timeScale = 1f;

        // Implemnetacion de la iA mientras juegas el raton queda fijo en el centro y escondido
        // para poder girar la camara, en los menus aparece para poder dar clic a los botones
        if (estado == EstadoPartida.Jugando)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

// Justificacion: el juego necesitaba un menu, una pausa y pantallas de ganar
// y perder para que el loop quede cerrado (objetivo, ganar, perder y reiniciar)
// la base es el GestorPartida de la Clase 2 pero para pausar el juego, esconder el
// raton mientras juegas y manejar los estados use apoyo de IA (Claude)

