using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    public float FinalTime { get; private set; }
    public int FinalCoins { get; private set; }
    public bool Won { get; private set; }

    public string gameOverScene = "GameOver";
    public string winScene = "WinScreen";
    public string menu = "Menu";
    public string retry = "Room1";

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    public void RegisterTimeDisplay(TMP_Text label)
    {
        int minutes = (int)(FinalTime / 60);
        int seconds = (int)(FinalTime % 60);
        label.text = $"Time: {minutes:00}:{seconds:00}";
    }

    public void TriggerGameOver()
    {
        SaveStats();
        Won = false;
        ShutdownNetwork();
        SceneManager.LoadScene(gameOverScene);
    }

    public void TriggerWin()
    {
        SaveStats();
        Won = true;
        ShutdownNetwork();
        SceneManager.LoadScene(winScene);
    }

    private void ShutdownNetwork()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();
    }
    public void BackToMenu()
    {
        Won = false;
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(menu);
    }

    public void Retry()
    {
        Won = false;
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();
        if (NetworkManagerSetup.Instance != null)
            NetworkManagerSetup.Instance.StartHost();
        else
            SceneManager.LoadScene(retry);
    }

    private void SaveStats()
    {
        Debug.Log($"[GameOverManager] SaveStats called. GameManager null: {GameManager.Instance == null}");
        if (GameManager.Instance == null) return;
        Debug.Log($"[GameOverManager] ElapsedTime before stop: {GameManager.Instance.ElapsedTime:F2}s");
        GameManager.Instance.StopTimer();
        FinalTime = GameManager.Instance.ElapsedTime;
        FinalCoins = GameManager.Instance.CoinCount;
        Debug.Log($"[GameOverManager] SaveStats — FinalTime: {FinalTime:F2}s, FinalCoins: {FinalCoins}");
    }
}
