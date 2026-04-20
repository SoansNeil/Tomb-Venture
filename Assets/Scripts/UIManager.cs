using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Health")]
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("Coins")]
    public TMP_Text coinText;

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
            healthText.text = $"{current} / {max}";
    }

    private void UpdateCoins(int count)
    {
        if (coinText != null)
            coinText.text = $"Coins: {count} / {GameManager.Instance.coinsToWin}";
    }
}
