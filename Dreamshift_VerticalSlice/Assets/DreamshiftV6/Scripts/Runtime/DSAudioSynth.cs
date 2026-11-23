using UnityEngine;

// es "static" porque no se pone en ningun objeto de la escena
// los otros scripts lo llaman directo por ejemplo DSAudioSynth.Chime()
public static class DSAudioSynth
{
    // cuantas muestras (numeros) hay en un segundo de sonido
    // 44100 es la calidad normal de un cd
    const int SR = 44100;

    // generador de numeros al azar para hacer ruido
    // el 1977 es la "semilla" asi el ruido sale igual cada vez que se juega
    static System.Random azar = new System.Random(1977);

    // aqui se guarda cada sonido ya creado para no crearlo otra vez
    // la primera vez se arma y las siguientes nomas se regresa el que ya estaba
    static AudioClip hum, tick, chime, whoosh, unlock, heart, step, creak, thump, knock, countHit, boom;

    // regresa un numero al azar entre -1 y 1
    // muchos numeros al azar seguidos suenan como "shhhh" (ruido)
    // sirve para pasos golpes y cosas que no tienen un tono fijo
    static float Ruido() { return (float)(azar.NextDouble() * 2.0 - 1.0); }

    // regresa una onda seno osea un tono limpio como un "uuuu"
    // hz es la frecuencia: numero bajo = sonido grave, numero alto = sonido agudo
    // t es el tiempo en segundos
    static float Seno(float hz, float t) { return Mathf.Sin(2f * Mathf.PI * hz * t); }

    // convierte la lista de numeros en un AudioClip que unity si puede reproducir
    static AudioClip Crear(string nombre, float[] muestras, float volumenMaximo)
    {
        // primero busca el numero mas grande de la lista
        float maximo = 0.0001f;
        foreach (float m in muestras) maximo = Mathf.Max(maximo, Mathf.Abs(m));

        // luego divide todo entre ese maximo para que nada pase de 1
        // si pasa de 1 el sonido se escucha roto o saturado
        // y despues lo multiplica por el volumen que quiero para ese sonido
        for (int i = 0; i < muestras.Length; i++) muestras[i] = muestras[i] / maximo * volumenMaximo;

        // crea el clip vacio: nombre, cuantas muestras, 1 canal (mono), 44100 por segundo
        // el false es para que no se genere en streaming, se crea todo de una vez
        AudioClip clip = AudioClip.Create(nombre, muestras.Length, 1, SR, false);

        // mete los numeros dentro del clip y listo ya es un sonido
        clip.SetData(muestras, 0);
        return clip;
    }

    // zumbido electrico de fondo, dura 4 segundos y DSAmbience lo repite en loop
    // es la "musica de fondo" del hotel
    public static AudioClip Hum()
    {
        if (hum != null) return hum;              // si ya existe lo regresa y ya
        float[] d = new float[SR * 4];            // espacio para 4 segundos
        float suave = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;              // convierte el numero de muestra a segundos

            // ruido suavizado: en vez de "shhh" fuerte queda un ruido grave y apagado
            suave += (Ruido() - suave) * 0.02f;

            // se suman varios tonos graves (50 100 150 y 55 hz) como el zumbido
            // de los focos y cables viejos, mas el ruido suave de fondo
            d[i] = Seno(50f, t) * 0.35f + Seno(100f, t) * 0.22f + Seno(150f, t) * 0.07f + Seno(55f, t) * 0.08f + suave * 0.6f;
        }
        hum = Crear("DS_Zumbido", d, 0.5f);
        return hum;
    }

    // el tic del reloj (sonido muy cortito de 0.07 segundos)
    public static AudioClip Tick()
    {
        if (tick != null) return tick;
        float[] d = new float[(int)(SR * 0.07f)];
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;

            // Mathf.Exp(-t * numero) hace que el sonido empiece fuerte y se apague rapido
            // mientras mas grande el numero mas rapido se apaga
            // se junta un golpecito de ruido con dos tonos agudos y sale el "tic"
            d[i] = Ruido() * Mathf.Exp(-t * 140f) * 0.6f + Seno(2300f, t) * Mathf.Exp(-t * 90f) * 0.5f + Seno(900f, t) * Mathf.Exp(-t * 60f) * 0.3f;
        }
        tick = Crear("DS_Tic", d, 0.8f);
        return tick;
    }

    // campanita cuando estabilizas una anomalia (es el efecto de cumplir objetivo)
    public static AudioClip Chime()
    {
        if (chime != null) return chime;
        float[] d = new float[(int)(SR * 1.8f)];

        // las notas do mi sol do (acorde de do mayor) suena alegre como de logro
        float[] notas = { 523.25f, 659.25f, 783.99f, 1046.5f };
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;
            float s = 0f;
            for (int j = 0; j < notas.Length; j++)
            {
                // cada nota empieza 0.09 segundos despues de la anterior
                // por eso suena como "tilin" y no todas de golpe
                float tt = t - j * 0.09f;

                // Exp hace que se apague poco a poco y Min evita un click feo al inicio
                if (tt > 0f) s += Seno(notas[j], tt) * Mathf.Exp(-tt * 2.2f) * Mathf.Min(1f, tt * 200f);
            }
            d[i] = s;
        }
        chime = Crear("DS_Campanita", d, 0.6f);
        return chime;
    }

    // sonido grave tipo "wuuuush" cuando una anomalia despierta
    public static AudioClip Whoosh()
    {
        if (whoosh != null) return whoosh;
        float[] d = new float[(int)(SR * 1.4f)];
        float suave = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;

            // envolvente: el volumen sube y luego baja (empieza en 0 sube y termina en 0)
            float envolvente = Mathf.Sin(Mathf.PI * t / 1.4f);

            // Mathf.Lerp va de un valor a otro poco a poco
            // aqui el ruido se va haciendo mas aspero conforme pasa el tiempo
            suave += (Ruido() - suave) * Mathf.Lerp(0.02f, 0.12f, t / 1.4f);

            // el tono baja de 90 hz a 42 hz osea se va haciendo mas grave
            float f = Mathf.Lerp(90f, 42f, t / 1.4f);
            d[i] = (suave * 0.8f + Seno(f, t) * 0.5f + Seno(f * 1.5f + 3f, t) * 0.25f) * envolvente;
        }
        whoosh = Crear("DS_Despertar", d, 0.6f);
        return whoosh;
    }

    // cuando la puerta de salida se abre: dos clicks de cerradura y luego un acorde
    public static AudioClip Unlock()
    {
        if (unlock != null) return unlock;
        float[] d = new float[(int)(SR * 2.2f)];
        float[] acorde = { 392f, 493.88f, 587.33f, 783.99f };   // sol si re sol
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;
            float s = 0f;

            // primer click de la cerradura (al inicio)
            if (t < 0.06f) s += Ruido() * Mathf.Exp(-t * 80f);

            // segundo click 0.16 segundos despues
            float t2 = t - 0.16f;
            if (t2 > 0f && t2 < 0.06f) s += Ruido() * Mathf.Exp(-t2 * 80f) * 0.8f;

            // a los 0.35 segundos entra el acorde que avisa que ya puedes salir
            float t3 = t - 0.35f;
            if (t3 > 0f)
            {
                foreach (float f in acorde) s += Seno(f, t3) * Mathf.Exp(-t3 * 1.4f) * 0.35f * Mathf.Min(1f, t3 * 30f);
            }
            d[i] = s;
        }
        unlock = Crear("DS_Desbloqueo", d, 0.7f);
        return unlock;
    }

    // latido doble "pum pum" que suena cuando queda poco tiempo
    public static AudioClip Heartbeat()
    {
        if (heart != null) return heart;
        float[] d = new float[(int)(SR * 0.6f)];
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;

            // primer "pum" un tono muy grave que se apaga rapido
            float s = Seno(52f, t) * Mathf.Exp(-t * 18f);

            // segundo "pum" 0.24 segundos despues un poquito mas bajito
            float t2 = t - 0.24f;
            if (t2 > 0f) s += Seno(46f, t2) * Mathf.Exp(-t2 * 20f) * 0.7f;
            d[i] = s;
        }
        heart = Crear("DS_Latido", d, 0.9f);
        return heart;
    }

    // un paso sobre el piso (lo usa el jugador al caminar)
    public static AudioClip Step()
    {
        if (step != null) return step;
        float[] d = new float[(int)(SR * 0.13f)];
        float suave = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;
            suave += (Ruido() - suave) * 0.18f;

            // ruido corto que se apaga rapido mas un tono grave = pisada en alfombra
            d[i] = suave * Mathf.Exp(-t * 38f) + Seno(80f, t) * Mathf.Exp(-t * 45f) * 0.4f;
        }
        step = Crear("DS_Paso", d, 0.6f);
        return step;
    }

    // crujido de madera vieja (suena al azar en el ambiente para dar miedo)
    public static AudioClip Creak()
    {
        if (creak != null) return creak;
        float[] d = new float[(int)(SR * 1.1f)];
        float fase = 0f, suave = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;

            // el tono se mueve para arriba y para abajo como madera que se dobla
            float f = 170f + 70f * Mathf.Sin(t * 6.5f) + 20f * Mathf.Sin(t * 31f);
            fase += f / SR;

            // onda "sierra" sube recto y cae de golpe, suena aspera como rechinido
            float sierra = (fase % 1f) * 2f - 1f;

            // vibrato hace que el sonido tiemble 18 veces por segundo
            float vibrato = 0.5f + 0.5f * Mathf.Sin(t * 2f * Mathf.PI * 18f);
            suave += (sierra * vibrato - suave) * 0.08f;

            // al final se multiplica para que entre suave y salga suave
            d[i] = suave * Mathf.Sin(Mathf.PI * t / 1.1f);
        }
        creak = Crear("DS_Crujido", d, 0.45f);
        return creak;
    }

    // golpe sordo cuando la maleta cae al piso al estabilizarla
    public static AudioClip Thump()
    {
        if (thump != null) return thump;
        float[] d = new float[(int)(SR * 0.4f)];
        float suave = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;
            suave += (Ruido() - suave) * 0.1f;

            // tono grave de 68 hz + un poco de ruido = "pum" de algo pesado
            d[i] = Seno(68f, t) * Mathf.Exp(-t * 16f) + suave * Mathf.Exp(-t * 30f) * 0.6f;
        }
        thump = Crear("DS_Golpe", d, 0.8f);
        return thump;
    }

    // tres toquidos en una puerta lejana "toc toc toc" (ambiente de miedo)
    public static AudioClip Knock()
    {
        if (knock != null) return knock;
        float[] d = new float[(int)(SR * 0.9f)];
        float suave = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;
            suave += (Ruido() - suave) * 0.12f;
            float s = 0f;

            // se repite 3 veces el mismo golpe separado 0.22 segundos
            for (int k = 0; k < 3; k++)
            {
                float tk = t - k * 0.22f;
                if (tk > 0f) s += (Seno(110f, tk) * 0.6f + suave) * Mathf.Exp(-tk * 30f);
            }
            d[i] = s;
        }
        knock = Crear("DS_Toquidos", d, 0.55f);
        return knock;
    }

    // golpe fuerte que suena cada segundo en el modo panico (ultimos 10 segundos)
    public static AudioClip CountHit()
    {
        if (countHit != null) return countHit;
        float[] d = new float[(int)(SR * 0.7f)];
        float suave = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;
            suave += (Ruido() - suave) * 0.25f;

            // un tono muy grave (48 hz) para el golpe + dos tonos agudos que
            // suenan como metal + un poco de ruido al inicio para el impacto
            d[i] = Seno(48f, t) * Mathf.Exp(-t * 9f) * 0.9f
                 + Seno(1380f, t) * Mathf.Exp(-t * 14f) * 0.18f
                 + Seno(1913f, t) * Mathf.Exp(-t * 11f) * 0.12f
                 + suave * Mathf.Exp(-t * 60f) * 0.6f;
        }
        countHit = Crear("DS_GolpeCuenta", d, 0.95f);
        return countHit;
    }

    // estruendo final cuando pierdes (se acaba el tiempo)
    public static AudioClip Boom()
    {
        if (boom != null) return boom;
        float[] d = new float[(int)(SR * 3.2f)];
        float suave = 0f, fase = 0f;
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SR;

            // Clamp01 deja el valor entre 0 y 1 para que Lerp no se pase
            // el ruido empieza aspero (explosion) y se va apagando a un retumbe
            suave += (Ruido() - suave) * Mathf.Lerp(0.3f, 0.01f, Mathf.Clamp01(t / 1.5f));

            // el tono baja de 70 hz a 26 hz osea se va hasta muy grave
            float f = Mathf.Lerp(70f, 26f, Mathf.Clamp01(t / 1.2f));
            fase += f / SR;

            // tono grave que baja + ruido, los dos se apagan lento (dura 3.2 segundos)
            d[i] = Mathf.Sin(fase * 2f * Mathf.PI) * Mathf.Exp(-t * 1.6f) + suave * Mathf.Exp(-t * 2.2f) * 1.4f;
        }
        boom = Crear("DS_Estruendo", d, 1f);
        return boom;
    }
}

// Para su documentacion, aclaracion y entendimiento en este archivo complete todo con Inteligencia Artificial uno de los 
// retos mas grandes de la creacion de los videojuegos son los sonidos la forma en como esta programado para cada accion por mas 
// minima que sea son demasiados calculos, una manera especifica de como desarrollarlo. Todo esre script se utilizo tres inteligencias
// artificiales para su comprobacion el cual hubo demasiados cambios, por ello no existen archivos dre audo no se descargan ni nada,
// esta loquisimo e impresionante que cada sonido se arma con ondas seno y ruida a traves de las matematicas. Igual el no tener mucsica
// de un determinado autor hace que el codigo pese menos y esto klo revise en a documentacuion de Unity AudioClip.Create y AudioClip.SetData
