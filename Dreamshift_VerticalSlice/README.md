# Dreamshift: El Hotel del Umbral

**Vertical Slice — Actividad de Medio Término, TC2008B Modelación de sistemas multiagentes con gráficas computacionales**

Autor(a): <Tu nombre> (<Tu matrícula>)

Juego de exploración y misterio psicológico en primera persona. Despiertas en un hotel surreal y tienes **2 minutos** para encontrar y estabilizar **3 anomalías**: el espejo, la maleta y el reloj. Después debes escapar por la puerta de salida antes de que el hotel te atrape.

## Cómo abrirlo y jugar

1. Abre el proyecto con **Unity 6.6 (6000.6.10f1)**. Usa Universal Render Pipeline (URP).
2. Abre la escena `Assets/DreamshiftV6/Scenes/Hotel_Dreamshift.unity`.
3. Presiona **▶ Play** y luego **Jugar**.

## Controles

| Acción | Tecla |
| --- | --- |
| Moverse | W A S D |
| Mirar | Ratón |
| Caminar rápido | Shift |
| Estabilizar una anomalía | Mirarla de cerca y mantener **E** (o clic izquierdo) 2 segundos |
| Pausa | Esc |

## Play loop

- **Objetivo:** estabilizar las 3 anomalías. El contador está arriba a la izquierda.
  - Espejo: habitación 2.
  - Maleta: habitación 6.
  - Reloj: habitación 10, al fondo del pasillo.
- **Victoria:** con las 3 estabilizadas, la puerta de salida cambia de rojo a verde y se abre. Al cruzarla aparece "Escapaste".
- **Derrota:** si el tiempo llega a 0:00 aparece "El hotel te atrapó".
- **Reinicio:** botones "Reintentar" / "Jugar de nuevo", sin salir de Play.
- **Tensión:** el temporizador se pone rojo en los últimos 30 segundos, algunas luces del hotel fallan y se escuchan crujidos y toquidos lejanos.

## Estructura del proyecto

| Carpeta | Contenido |
| --- | --- |
| `Assets/DreamshiftV6/Scenes` | Escena del juego |
| `Assets/DreamshiftV6/Scripts/Runtime` | Scripts del juego (GameManager, jugador, anomalías, salida, Canvas, efectos) |
| `Assets/DreamshiftV6/Prefabs` | Prefabs: anomalías, jugador, salida, luces y muebles |
| `Assets/DreamshiftV6/Animacion` | Animator Controllers `AC_Anomalia` (bool `Estabilizada`) y `AC_Puerta_Salida` (bool `Abierta`) con sus clips |
| `Assets/DreamshiftV6/Materials` | Materiales URP |
| `Assets/DreamshiftV6/Generated` | Texturas y mallas generadas |
| `Assets/DreamshiftV6/Settings` | Perfil de post-procesado e iluminación |

## Créditos y assets

- **Modelos, texturas y materiales:** generados por una herramienta de editor hecha con IA (ya retirada del proyecto, ver "Uso de inteligencia artificial"). No se usaron assets de terceros.
- **Sonidos:** generados por código (`DSAudioSynth.cs`).
- **Fuente:** Liberation Sans, incluida con TextMesh Pro de Unity (licencia SIL Open Font License).
- **Motor:** Unity 6.6, Universal Render Pipeline, Input System y uGUI.

## Uso de inteligencia artificial

Este proyecto se desarrolló con apoyo de **Claude (Anthropic)**, conforme a la política de IA del curso:

- **Qué hizo la IA:** generó el código de los scripts a partir de mis instrucciones (idea, plano del hotel, anomalías, tiempo y reglas) y después lo adaptó al estilo visto en clase (Rigidbody, Is Trigger + OnTriggerEnter, GestorPartida, Animator, Particle System, Canvas con TextMeshPro). También generó con IA: la herramienta de editor que construyó la escena y las texturas, y los sonidos por código (`DSAudioSynth`, `DSAmbience`). Cada script indica con comentarios `[NO VISTO EN CLASE - IA]` qué partes no se vieron en clase.
- **Qué hice yo:** <describe aquí lo que hiciste tú: definir la idea y el diseño, probar y pedir cambios (por ejemplo, reducir el tiempo a 2 minutos y agregar más habitaciones), crear los prefabs, ajustar la configuración de sombras y luces del proyecto, hacer el playtest, etc.>
- Cada script tiene un encabezado que indica qué hace y que fue generado con IA.
- El detalle está en el reporte técnico, sección 4.

## Enlaces

- Video de demostración: <URL del video>
- Reporte técnico (PDF): <URL o "incluido en el repositorio">
