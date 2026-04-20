using UnityEngine;
using UnityEngine.SceneManagement;

public interface IDamageable
{
    void TakeDamage(int damage);
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxHealth = 100;
    private int currentHealth;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            SceneManager.LoadScene("GameOver");
    }

    public void DamageEnemy(IDamageable enemy, int damage)
    {
        enemy.TakeDamage(damage);
    }
}
