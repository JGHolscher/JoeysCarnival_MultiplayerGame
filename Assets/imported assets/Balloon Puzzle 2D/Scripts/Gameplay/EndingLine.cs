using UnityEngine;

namespace BalloonPuzzle2D
{
    public class EndingLine : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (GameControl.Current != null)
                {
                    GameControl.Current.HandleWin();
                }
            }
        }
    }
}
