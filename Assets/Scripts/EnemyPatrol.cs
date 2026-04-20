using UnityEngine;

public class EnemyPatrol : MonoBehaviour, IDamageable
{
    public float moveSpeed = 2f;
    public float patrolDistance = 4f;
    public int maxHealth = 30;
    public int damageToPlayer = 10;

    private int currentHealth;
    private Vector2 startPosition;
    private bool movingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
    }

    void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        float distanceTravelled = transform.position.x - startPosition.x;
        if (distanceTravelled >= patrolDistance) movingRight = false;
        else if (distanceTravelled <= -patrolDistance) movingRight = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        GameManager.Instance.TakeDamage(damageToPlayer);
        Debug.Log("Player Damaged: " + damageToPlayer);
    }
}
