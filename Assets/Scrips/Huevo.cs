using UnityEngine;

public class Huevo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Juego.I == null || !Juego.I.PuedeTomar)
        {
            return;
        }

        if (other.GetComponent<Personaje>() == null)
        {
            return;
        }

        Juego.I.Sumar();
        Destroy(gameObject);
    }
}
