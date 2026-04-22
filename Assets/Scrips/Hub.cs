using UnityEngine;
using UnityEngine.SceneManagement;

public class Hub : MonoBehaviour
{
    private const string EscenaHub = "Hub";
    private const string EscenaJuego = "Primera escena";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Cargar()
    {
        if (SceneManager.GetActiveScene().name != EscenaHub)
        {
            return;
        }

        if (FindObjectOfType<Hub>() != null)
        {
            return;
        }

        GameObject obj = new GameObject("Hub");
        obj.AddComponent<Hub>();
    }

    private void OnGUI()
    {
        Rect p = new Rect((Screen.width - 320f) * 0.5f, (Screen.height - 220f) * 0.5f, 320f, 220f);
        GUI.Box(p, "CHIKY CHIKS");

        GUIStyle c = new GUIStyle(GUI.skin.label);
        c.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(p.x + 20f, p.y + 40f, 280f, 30f), "Recoge 10 huevos en 1 minuto", c);

        if (GUI.Button(new Rect(p.x + 70f, p.y + 90f, 180f, 40f), "Jugar"))
        {
            SceneManager.LoadScene(EscenaJuego);
        }

        if (GUI.Button(new Rect(p.x + 70f, p.y + 145f, 180f, 35f), "Salir"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
