using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 150;
    public int damageToPlayer = 20;
    public int phase2Threshold = 75;

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 4f;

    [Header("Charge")]
    public float chargeSpeed = 9f;
    public float chargeDuration = 0.7f;

    [Header("Leap")]
    public float leapForceX = 5f;
    public float leapForceY = 13f;

    [Header("Slam")]
    public float slamRiseForce = 15f;
    public float slamFallForce = 30f;
    public float slamRiseDuration = 0.4f;

    [Header("Timing")]
    public float minInterval = 1.2f;
    public float maxInterval = 2.8f;
    public float phase2SpeedMultiplier = 1.5f;

    private int currentHealth;
    private bool isPhase2 = false;
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 startPosition;
    private bool movingRight = true;

    private readonly WaitForSeconds waitLeap = new(1f);
    private readonly WaitForSeconds waitSlamFall = new(0.8f);
    private WaitForSeconds waitSlamRise;

    private enum Attack { Patrol, Charge, Leap, Slam }

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        waitSlamRise = new WaitForSeconds(slamRiseDuration);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        if (!isPhase2 && currentHealth <= phase2Threshold)
            EnterPhase2();
    }

    private void EnterPhase2()
    {
        isPhase2 = true;
        patrolSpeed *= phase2SpeedMultiplier;
        chargeSpeed *= phase2SpeedMultiplier;
        minInterval *= 0.6f;
        maxInterval *= 0.6f;
        Debug.Log("Boss: Phase 2!");
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            Attack chosen = PickAttack();
            switch (chosen)
            {
                case Attack.Patrol: yield return StartCoroutine(DoPatrol()); break;
                case Attack.Charge: yield return StartCoroutine(DoCharge()); break;
                case Attack.Leap:   yield return StartCoroutine(DoLeap());   break;
                case Attack.Slam:   yield return StartCoroutine(DoSlam());   break;
            }
        }
    }

    private Attack PickAttack()
    {
        Attack[] pool = isPhase2
            ? new[] { Attack.Charge, Attack.Charge, Attack.Leap, Attack.Slam, Attack.Patrol }
            : new[] { Attack.Patrol, Attack.Charge, Attack.Leap };
        return pool[Random.Range(0, pool.Length)];
    }

    private IEnumerator DoPatrol()
    {
        float elapsed = 0f;
        float duration = Random.Range(2f, 4f);
        while (elapsed < duration)
        {
            float dir = movingRight ? 1f : -1f;
            rb.velocity = new Vector2(dir * patrolSpeed, rb.velocity.y);

            float dist = transform.position.x - startPosition.x;
            if (dist >= patrolDistance) movingRight = false;
            else if (dist <= -patrolDistance) movingRight = true;

            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private IEnumerator DoCharge()
    {
        if (player == null) yield break;

        float dir = player.position.x > transform.position.x ? 1f : -1f;
        float elapsed = 0f;
        while (elapsed < chargeDuration)
        {
            rb.velocity = new Vector2(dir * chargeSpeed, rb.velocity.y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private IEnumerator DoLeap()
    {
        if (player == null) yield break;

        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * leapForceX, leapForceY);
        yield return waitLeap;
    }

    private IEnumerator DoSlam()
    {
        if (player == null) yield break;

        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * patrolSpeed, slamRiseForce);
        yield return waitSlamRise;
        rb.velocity = new Vector2(0f, -slamFallForce);
        yield return waitSlamFall;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.Instance != null)
            GameManager.Instance.TakeDamage(damageToPlayer);
    }
}
