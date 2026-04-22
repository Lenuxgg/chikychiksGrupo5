using UnityEngine;

public class Personaje : MonoBehaviour
{
    [SerializeField] private float velocidad;


    private Rigidbody2D rig;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();

        if (Juego.I == null)
        {
            GameObject obj = new GameObject("Juego");
            obj.AddComponent<Juego>();
        }
    }
    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rig.velocity = new Vector2(horizontal, vertical) * velocidad;

        if (Juego.I != null)
        {
            Vector2 minBounds = Juego.I.Min;
            Vector2 maxBounds = Juego.I.Max;
            rig.position = new Vector2(
                Mathf.Clamp(rig.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(rig.position.y, minBounds.y, maxBounds.y)
            );
        }
    }
}
