using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.CollectCoin();
        AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.coinSound);
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}
