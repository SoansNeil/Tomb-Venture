using System;
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
    public int coinsToWin = 10;
    private int currentHealth;
    public int CoinCount { get; private set; }

    public event Action OnAllCoinsCollected;
    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnCoinCollected;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPoint == null) return;
        StartCoroutine(SpawnPlayer(spawnPoint.transform.position));
    }

    private System.Collections.IEnumerator SpawnPlayer(Vector3 position)
    {
        float timeout = 5f;
        float elapsed = 0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        while (player == null && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null)
            player.transform.position = position;
    }

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
            SceneManager.LoadScene("GameOver");
    }

    public void CollectCoin()
    {
        CoinCount++;
        OnCoinCollected?.Invoke(CoinCount);
        if (CoinCount >= coinsToWin)
            OnAllCoinsCollected?.Invoke();
    }

    public void DamageEnemy(IDamageable enemy, int damage)
    {
        enemy.TakeDamage(damage);
    }
}
