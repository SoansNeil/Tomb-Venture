using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    public int stompDamage = 10;
    public float bounceForce = 8f;

    private IDamageable enemy;

    void Start()
    {
        enemy = GetComponentInParent<IDamageable>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (enemy == null) return;

        GameManager.Instance.DamageEnemy(enemy, stompDamage);
        Debug.Log("Enemy Damaged: " + stompDamage);

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);
    }
}
