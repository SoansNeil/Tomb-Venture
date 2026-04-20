using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    public int stompDamage = 10;
    public float bounceForce = 8f;

    private EnemyPatrol enemy;

    void Start()
    {
        enemy = GetComponentInParent<EnemyPatrol>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        GameManager.Instance.DamageEnemy(enemy, stompDamage);
        Debug.Log("Enemy Damaged: " + stompDamage);

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);
    }
}
