using UnityEngine;

public class Floor : MonoBehaviour{
    private void OnCollisionEnter2D(Collision2D other) {
        GameManager.Instance.GameOver();
    }
}
