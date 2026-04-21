using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Health")]
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("Coins")]
    public TMP_Text coinText;

    [Header("Boss")]
    public Slider bossHealthBar;
    public TMP_Text bossHealthText;

    [Header("Pause")]
    public GameObject pauseMenu;

    [Header("Save/Load")]
    public TMP_Text saveLoadFeedbackText;
    public string mainMenuScene = "Menu";

    private BossMovement boss;
    private bool isPaused = false;

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
        RefreshReferences();
        if (GameManager.Instance != null)
            GameManager.Instance.RefreshUI();
    }

    private void RefreshReferences()
    {
        GameObject hb = GameObject.FindGameObjectWithTag("HealthBar");
        if (hb != null) healthBar = hb.GetComponent<Slider>();

        GameObject ht = GameObject.FindGameObjectWithTag("HealthText");
        if (ht != null) healthText = ht.GetComponent<TMP_Text>();

        GameObject ct = GameObject.FindGameObjectWithTag("CoinText");
        if (ct != null) coinText = ct.GetComponent<TMP_Text>();

        GameObject bhb = GameObject.FindGameObjectWithTag("BossHealthBar");
        if (bhb != null) bossHealthBar = bhb.GetComponent<Slider>();

        GameObject bht = GameObject.FindGameObjectWithTag("BossHealthText");
        if (bht != null) bossHealthText = bht.GetComponent<TMP_Text>();

        foreach (var obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.scene.IsValid() && obj.CompareTag("PauseMenu"))
            {
                pauseMenu = obj;
                break;
            }
        }

        SetBossUIVisible(false);
    }

    public void RegisterBoss(BossMovement bossMovement)
    {
        if (boss != null)
            boss.OnHealthChanged -= UpdateBossHealth;

        boss = bossMovement;
        boss.OnHealthChanged += UpdateBossHealth;
        SetBossUIVisible(true);
    }

    private void SetBossUIVisible(bool visible)
    {
        if (bossHealthBar != null) bossHealthBar.gameObject.SetActive(visible);
        if (bossHealthText != null) bossHealthText.gameObject.SetActive(visible);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pauseMenu != null) pauseMenu.SetActive(isPaused);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    public void SaveGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Resume();
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    public void LoadGame()
    {
        if (GameManager.Instance != null && SaveSystem.HasSave())
        {
            Resume();
            GameManager.Instance.LoadGame();
        }
        else
        {
            ShowFeedback("No save found.");
        }
    }

    private void ShowFeedback(string message)
    {
        if (saveLoadFeedbackText != null)
        {
            saveLoadFeedbackText.text = message;
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    private void ClearFeedback()
    {
        if (saveLoadFeedbackText != null)
            saveLoadFeedbackText.text = "";
    }

    void OnEnable()
    {
        GameManager.Instance.OnHealthChanged += UpdateHealth;
        GameManager.Instance.OnCoinCollected += UpdateCoins;
    }

    void OnDisable()
    {
        GameManager.Instance.OnHealthChanged -= UpdateHealth;
        GameManager.Instance.OnCoinCollected -= UpdateCoins;
    }

    private void UpdateHealth(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
        if (healthText != null)
            healthText.text = $"Player Health: {current} / {max}";
    }

    private void UpdateCoins(int count)
    {
        if (coinText != null)
            coinText.text = $"Coins: {count} / {GameManager.Instance.coinsToWin}";
    }

    private void UpdateBossHealth(int current, int max)
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = max;
            bossHealthBar.value = current;
        }
        if (bossHealthText != null)
            bossHealthText.text = $"Boss Health: {current} / {max}";
    }
}
