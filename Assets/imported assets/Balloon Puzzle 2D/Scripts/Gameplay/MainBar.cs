using UnityEngine;
namespace BalloonPuzzle2D
{
public class MainBar : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.mass = 5f; 
    }
}
}