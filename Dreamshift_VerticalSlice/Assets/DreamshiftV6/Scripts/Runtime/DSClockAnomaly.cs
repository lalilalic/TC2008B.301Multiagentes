using UnityEngine;

// generado con IA CODEX -  DSAnomaly significa que hereda de DSAnomaly osea ya tiene el brillo, el trigger, mantener E, Animator y particulas
// y aqui solo agrego lo propio del reloj
public class DSClockAnomaly : DSAnomaly
{
    public Transform hourHand;      // manecilla de la hora, se arrastra en el Inspector
    public Transform minuteHand;    // manecilla de los minutos
    public Transform pendulum;      // el pendulo

    private float anguloHora = 200f;    // en que angulo empieza la manecilla de la hora
    private float anguloMinuto = 90f;   // en que angulo empieza la manecilla de los minutos
    private float siguienteTic = 0f;    // en que segundo suena el siguiente tic

    // DSAnomaly llama a Rara() en cada frame mientras no esta estabilizada - Codex
    protected override void Rara()
    {
        // dormido gira lento y despierto gira muy rapido
        float velocidad = 30f;
        if (despierta) velocidad = 240f;

        // sumar angulo en Z hace que giren hacia atras vistas de frente
        // la de la hora va 12 veces mas lento que la de los minutos como un reloj de verdad
        // Time.deltaTime (Lab 3) para que gire igual en cualquier compu

        anguloMinuto += velocidad * Time.deltaTime;
        anguloHora += velocidad / 12f * Time.deltaTime;
        MoverManecillas();

        // el pendulo se queda congelado chueco a 22 grados, como si el tiempo se detuviera
        if (pendulum != null) pendulum.localRotation = Quaternion.Euler(0f, 0f, 22f);

        // tic-tac a destiempo y grave
        if (Time.time > siguienteTic)
        {
            // Random.Range (Lab 6): el siguiente tic suena entre 0.25 y 1.2 segundos, osea sin ritmo
            siguienteTic = Time.time + Random.Range(0.25f, 1.2f);

            // pitch menor a 1 hace el sonido mas grave y raro - apoyo con IA
            sonido.pitch = Random.Range(0.45f, 0.8f);
            sonido.PlayOneShot(DSAudioSynth.Tick());   // el tic se genera por codigo en DSAudioSynth
        }
    }

    // Uso de la IA DSAnomaly llama a Calmada() en cada frame cuando ya esta estabilizada
    protected override void Calmada()
    {
        // las manecillas ya no se mueven, se quedan donde estaban

        // el pendulo se mueve de lado a lado
        // Mathf.Sin sube y baja entre -1 y 1, por 14 queda entre -14 y 14 grados
        if (pendulum != null) pendulum.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * Mathf.PI) * 14f);

        // tic-tac normal: cada 1 segundo exacto, tono normal y bajito
        if (Time.time > siguienteTic)
        {
            siguienteTic = Time.time + 1f;
            sonido.pitch = 1f;                               // tono normal
            sonido.PlayOneShot(DSAudioSynth.Tick(), 0.4f);   // 0.4 = mas bajito
        }
    }

    // pone las manecillas en su angulo
    // Quaternion.Euler convierte grados a una rotacion de unity - uso de la IA
    void MoverManecillas()
    {
        if (minuteHand != null) minuteHand.localRotation = Quaternion.Euler(0f, 0f, anguloMinuto);
        if (hourHand != null) hourHand.localRotation = Quaternion.Euler(0f, 0f, anguloHora);
    }
}

// justificacion de la IA: donde las manecillas giran hacia atras, el pendulo esta congelado chueco
// y el tic-tac suena grave y a destiempo y cuando esta calmado calmado las manecillas se quedan quietas, el pendulo se mueve normal
// de lado a lado y el tic-tac suena tranquilo cada segundo

// visto en clase: Transform (rotacion), Time.deltaTime (Lab 3), Random.Range (Lab 6), no visto en clase donde se uso IA: herencia con override, Quaternion.Euler, Mathf.Sin y el sonido (pitch y PlayOneShot)

