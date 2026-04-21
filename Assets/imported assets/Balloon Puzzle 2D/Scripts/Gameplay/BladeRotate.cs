
using UnityEngine;
namespace BalloonPuzzle2D
{
public class BladeRotate : MonoBehaviour
{
    public float rotationSpeed = 200f; // Degrees per second

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Cube obj = col.gameObject.GetComponent<Cube>();
            if (obj != null)
            {
                obj.Smash();
            }
        }

        else if (col.gameObject.CompareTag("Balloon"))
        {

            Balloon balloon = col.gameObject.GetComponent<Balloon>();
            balloon.Kill();
        }
    }
}
}

