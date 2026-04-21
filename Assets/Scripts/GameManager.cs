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
    public int CurrentHealth => currentHealth;
    public int CoinCount { get; private set; }
    public float ElapsedTime { get; private set; }
    private bool timerRunning = false;

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

    public string[] gameplayScenes = { "Room1", "BossRoom" };

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (System.Array.IndexOf(gameplayScenes, scene.name) >= 0)
            StartTimer();
        else
            StopTimer();

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
        ResetGame();
    }

    public void ResetGame()
    {
        currentHealth = maxHealth;
        CoinCount = 0;
        ElapsedTime = 0f;
        timerRunning = false;
        RefreshUI();
    }

    public void StartTimer()
    {
        timerRunning = true;
        Debug.Log($"[GameManager] Timer started in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
    }

    public void StopTimer()
    {
        timerRunning = false;
        Debug.Log($"[GameManager] Timer stopped. ElapsedTime: {ElapsedTime:F2}s");
    }

    public void RefreshUI()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnCoinCollected?.Invoke(CoinCount);
    }

    void Update()
    {
        if (timerRunning)
            ElapsedTime += Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            if (GameOverManager.Instance != null)
                GameOverManager.Instance.TriggerGameOver();
            else
                SceneManager.LoadScene("GameOver");
        }
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

    public void BossDefeated()
    {
        if (GameOverManager.Instance != null)
            GameOverManager.Instance.TriggerWin();
        else
            SceneManager.LoadScene("WinScreen");
    }
}
