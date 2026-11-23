using UnityEngine;

// Zona de una habitación con anomalía
// Tiene un Box Collider con Is Trigger del tamaño del cuarto (Clase 2)
// Cuando el jugador entra, despierta la anomalía del cuarto
public class DSRoomZone : MonoBehaviour
{
    public string roomName = "Habitación"; // nombre del cuarto solo para identificar en el Inspector
    public Vector3 size = new Vector3(7.8f, 3.2f, 7.8f);   // tamaño del trigger
    public DSAnomaly[] anomalies;                          // anomalías de este cuarto

    // OnTriggerEnter (Clase 2) donde unity lo llama una sola vez cuando algo entra al Box Collider con Is Trigger
    void OnTriggerEnter(Collider other)
    {
        // si lo que entro no es el jugador no hace nada (Clase 2)
        if (!other.CompareTag("Player")) return;

         // foreach recorre todas las anomalias del cuarto y las despierta
        foreach (DSAnomaly anomalia in anomalies)
        {
             // revisa que no este vacio por si falto arrastrar una en el Inspector
            if (anomalia != null) anomalia.Despertar();
        }
    }
}

// Justificacion:  todo este script es de la Clase 2 (Box Collider con Is Trigger,
// OnTriggerEnter y CompareTag("Player")), al igual que los arreglos y foreach
