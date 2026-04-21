using UnityEngine;
namespace BalloonPuzzle2D
{
public class Cube : MonoBehaviour
{
    public GameObject m_SmashPrefab;
    void Start()
    {

    }

    public void Smash()
    {
        GameObject obj = Instantiate(m_SmashPrefab);
        obj.transform.position = transform.position + new Vector3(0, 0, -.1f);
        obj.transform.rotation = transform.rotation;
        Destroy(obj, 5);

        Destroy(gameObject);
        GameControl.Current.HandleLose();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Smash();
            GameControl.Current.HandleLose();
        }
    }
}
}