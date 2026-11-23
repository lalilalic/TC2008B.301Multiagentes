using UnityEngine;
using TMPro;

public class DSCanvasHud : MonoBehaviour
{
    // Generado con IA para el titulo
    [Header("Textos del juego")]
    public TextMeshProUGUI textoAnomalias;     // "Anomalías 1/3" y la lista
    public TextMeshProUGUI textoTiempo;        // el reloj "01:45"
    public TextMeshProUGUI textoInteraccion;   // "Mantén E para estabilizar: Espejo 50%"
    public TextMeshProUGUI textoAviso;         // mensajes cortos por ejemplo "Anomalía estabilizada"
    public TextMeshProUGUI textoSalida;        // "La salida está abierta"

    [Header("Paneles")]
    public GameObject panelJuego;              // todo lo que se ve mientras juegas
    public GameObject panelMenu;               // menu principal
    public GameObject panelPausa;              // pausa con Esc
    public GameObject panelVictoria;           // pantalla de ganaste
    public GameObject panelDerrota;            // pantalla de perdiste
    public TextMeshProUGUI textoVictoria;      // cuanto tiempo te sobro
    public TextMeshProUGUI textoDerrota;       // cuantas anomalias lograste

    void Update()
    {
        // agarra el GameManager, si no existe no hace nada
        DSGameManager gestor = DSGameManager.Instance;
        if (gestor == null) return;

        // solo se ve el panel del estado actual (SetActive visto en clase)
        // Apoyo con Claude: EstadoPartida es un enum que viene del GameManager es la lista de estados: Menu, Jugando, Pausa, Victoria, Derrota
        panelMenu.SetActive(gestor.estado == EstadoPartida.Menu);
        panelJuego.SetActive(gestor.estado == EstadoPartida.Jugando || gestor.estado == EstadoPartida.Pausa);
        panelPausa.SetActive(gestor.estado == EstadoPartida.Pausa);
        panelVictoria.SetActive(gestor.estado == EstadoPartida.Victoria);
        panelDerrota.SetActive(gestor.estado == EstadoPartida.Derrota);

        // contador de anomalias, por ejemplo "Anomalías 1/3"
        string lista = "Anomalías  " + gestor.estabilizadas + "/" + gestor.anomalias.Count;

        // foreach recorre las 3 anomalias y agrega una linea por cada una con su estado
        foreach (DSAnomaly a in gestor.anomalias)
        {
            string estado = "sin encontrar";
            if (a.despierta) estado = "activa";

            // Sugerencia de la IA acerca del color de la palabra verde
            // TextMeshPro entiende estas etiquetas dentro del texto
            if (a.estabilizada) estado = "<color=#2BFF88>estabilizada</color>";

            // \n es salto de linea y <size=80%> hace la letra mas chiquita
            lista += "\n• " + a.displayName + "  <size=80%>" + estado + "</size>";
        }
        textoAnomalias.text = lista;

        // tiempo restante como reloj (mm:ss)
        textoTiempo.text = FormatoTiempo(gestor.tiempoRestante);

        // si queda menos de la cuarta parte del tiempo (30 de 120 segundos) se pone rojo
        if (gestor.tiempoRestante < gestor.duration * 0.25f) textoTiempo.color = Color.red;
        else textoTiempo.color = Color.white;

        // "Mantén E" solo cuando hay una anomalia cerca
        if (gestor.anomaliaCercana != null)
        {
            // Uso de la IA progreso va de 0 a 1, por 100 da el porcentaje
            // y Mathf.RoundToInt lo redondea para que salga 50% y no 49.87%
            int porcentaje = Mathf.RoundToInt(gestor.anomaliaCercana.progreso * 100f);

            // Muestra el texto de interacción con el porcentaje
            textoInteraccion.text = "Mantén E para estabilizar: " + gestor.anomaliaCercana.displayName + " " + porcentaje + "%";
                
        }
        else
        {
            textoInteraccion.text = "";   // si no hay anomalia cerca no se muestra nada
        }

        // mensajes cortos que manda el GameManager
        textoAviso.text = gestor.AvisoActual();

        // Apoyo con IA: ? : es un if en una sola linea
        // si ya se cumplio la mision dice "La salida está abierta", si no queda vacio
        textoSalida.text = gestor.MisionCumplida ? "La salida está abierta" : "";

        // pantallas finales
        textoVictoria.text = "Tiempo restante: " + FormatoTiempo(gestor.tiempoRestante);
        textoDerrota.text = "Anomalías estabilizadas: " + gestor.estabilizadas + "/" + gestor.anomalias.Count;
    }

    // Apoyo con IA: convierte segundos en formato de reloj, por ejemplo 105 → "01:45"
    string FormatoTiempo(float segundos)
    {
        int t = Mathf.CeilToInt(segundos);   // redondea para arriba, asi 0.3 segundos se ve como 1 y no como 0
        int min = t / 60;                    // minutos completos (105 / 60 = 1)
        int seg = t % 60;                    // lo que sobra son los segundos (105 % 60 = 45)
        return min.ToString("00") + ":" + seg.ToString("00");   // "00" pone el cero de adelante: 1 → "01"
    }
}

// Justificación con IA: el jugador necesita saber cuanto tiempo le queda, cuantas anomalías lleva y
// cuanto le falta para estabilizar, la base del Canvas es igual
// a la de los labs pero para que el tiempo salga como reloj (01:45) y la lista se
// vea mas clara con colores use apoyo de IA 