using UnityEngine;
using UnityEngine.SceneManagement;

public class Juego : MonoBehaviour
{
    private const string EscenaJuego = "Primera escena";
    private const string EscenaHub = "Hub";

    public static Juego I { get; private set; }

    [SerializeField] private float tiempoMax = 60f;
    [SerializeField] private int meta = 10;
    [SerializeField] private int huevos = 10;
    [SerializeField] private Vector2 min = new Vector2(-14f, -8f);
    [SerializeField] private Vector2 max = new Vector2(14f, 8f);

    private float tiempo;
    private int total;
    private bool fin;
    private bool gana;
    private Personaje p;
    private static Sprite s;

    public bool PuedeTomar
    {
        get { return !fin; }
    }

    public Vector2 Min
    {
        get { return min; }
    }

    public Vector2 Max
    {
        get { return max; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Cargar()
    {
        if (SceneManager.GetActiveScene().name != EscenaJuego)
        {
            return;
        }

        if (FindObjectOfType<Juego>() != null)
        {
            return;
        }

        GameObject obj = new GameObject("Juego");
        obj.AddComponent<Juego>();
    }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        tiempo = tiempoMax;
        p = FindObjectOfType<Personaje>();
    }

    private void Start()
    {
        CrearHuevos();
    }

    private void Update()
    {
        if (fin)
        {
            return;
        }

        tiempo -= Time.deltaTime;
        if (tiempo <= 0f)
        {
            tiempo = 0f;
            Terminar();
        }
    }

    public void Sumar()
    {
        if (fin)
        {
            return;
        }

        total++;
        if (total >= meta)
        {
            tiempo = 0f;
            Terminar();
        }
    }

    private void Terminar()
    {
        fin = true;
        gana = total >= meta;

        if (p != null)
        {
            p.enabled = false;
            Rigidbody2D b = p.GetComponent<Rigidbody2D>();
            if (b != null)
            {
                b.velocity = Vector2.zero;
            }
        }
    }

    private void CrearHuevos()
    {
        Vector2 basePos = p != null ? (Vector2)p.transform.position : Vector2.zero;
        Vector2[] puntos = new Vector2[]
        {
            new Vector2(-12f, 6f),
            new Vector2(-4f, 7f),
            new Vector2(4f, 7f),
            new Vector2(12f, 6f),
            new Vector2(-12f, 1f),
            new Vector2(-4f, 2f),
            new Vector2(4f, 2f),
            new Vector2(12f, 1f),
            new Vector2(-8f, -5f),
            new Vector2(8f, -5f)
        };

        int cant = Mathf.Min(huevos, puntos.Length);
        for (int i = 0; i < cant; i++)
        {
            Vector2 pos = AjustarLejos(puntos[i], basePos);
            pos = new Vector2(
                Mathf.Clamp(pos.x, min.x, max.x),
                Mathf.Clamp(pos.y, min.y, max.y)
            );

            GameObject h = new GameObject("Huevo_" + i);
            h.transform.position = new Vector3(pos.x, pos.y, 0f);

            SpriteRenderer r = h.AddComponent<SpriteRenderer>();
            r.sprite = SpriteHuevo();
            r.color = Color.white;
            r.sortingOrder = 100;
            h.transform.localScale = new Vector3(1f, 1f, 1f);

            CircleCollider2D c = h.AddComponent<CircleCollider2D>();
            c.isTrigger = true;
            c.radius = 0.5f;

            h.AddComponent<Huevo>();
        }
    }

    private Vector2 AjustarLejos(Vector2 pos, Vector2 jugador)
    {
        if (Vector2.Distance(pos, jugador) >= 4.5f)
        {
            return pos;
        }

        Vector2 dir = pos - jugador;
        if (dir.sqrMagnitude < 0.01f)
        {
            dir = Vector2.right;
        }

        dir.Normalize();
        Vector2 nuevo = jugador + dir * 4.5f;
        return new Vector2(
            Mathf.Clamp(nuevo.x, min.x, max.x),
            Mathf.Clamp(nuevo.y, min.y, max.y)
        );
    }

    private static Sprite SpriteHuevo()
    {
        if (s != null)
        {
            return s;
        }

        int w = 24;
        int h = 32;
        Texture2D t = new Texture2D(w, h, TextureFormat.RGBA32, false);
        t.filterMode = FilterMode.Point;
        t.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color c1 = new Color(1f, 0.96f, 0.85f, 1f);
        Color c2 = new Color(0.97f, 0.86f, 0.65f, 1f);
        Color c3 = new Color(1f, 1f, 0.95f, 1f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                t.SetPixel(x, y, clear);
            }
        }

        float cx = (w - 1) * 0.5f;
        float cy = (h - 1) * 0.53f;
        float rx = w * 0.33f;
        float ry = h * 0.44f;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = (x - cx) / rx;
                float dy = (y - cy) / ry;
                float d = dx * dx + dy * dy;
                if (d > 1f)
                {
                    continue;
                }

                Color c = y < cy ? c2 : c1;
                if (x < cx - 2f && y > cy + 2f)
                {
                    c = c3;
                }

                t.SetPixel(x, y, c);
            }
        }

        t.Apply();
        s = Sprite.Create(t, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), 24f);
        return s;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 20f, 250f, 30f), "Tiempo: " + Mathf.CeilToInt(tiempo) + " s");
        GUI.Label(new Rect(20f, 45f, 250f, 30f), "Huevos: " + total + "/" + meta);

        if (!fin)
        {
            return;
        }

        string txt = gana ? "Ganaste" : "Perdiste";
        Rect p = new Rect((Screen.width - 320f) * 0.5f, (Screen.height - 180f) * 0.5f, 320f, 180f);
        GUI.Box(p, "Fin de partida");
        GUI.Label(new Rect(p.x + 20f, p.y + 45f, 280f, 30f), txt + " - Huevos: " + total + "/" + meta);

        if (GUI.Button(new Rect(p.x + 70f, p.y + 85f, 180f, 35f), "Reintentar"))
        {
            SceneManager.LoadScene(EscenaJuego);
        }

        if (GUI.Button(new Rect(p.x + 70f, p.y + 130f, 180f, 30f), "Volver al HUB"))
        {
            SceneManager.LoadScene(EscenaHub);
        }
    }
}
