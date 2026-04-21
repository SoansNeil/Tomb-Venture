using TMPro;
using UnityEngine;

public class WinScreenUI : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void SaveScore()
    {
        if (DatabaseManager.Instance == null || GameOverManager.Instance == null) return;

        string playerName = string.IsNullOrWhiteSpace(nameInput.text) ? "Player" : nameInput.text.Trim();
        DatabaseManager.Instance.SaveScore(playerName, GameOverManager.Instance.FinalCoins, GameOverManager.Instance.FinalTime);
        Debug.Log($"Score saved — Name: {playerName}, Coins: {GameOverManager.Instance.FinalCoins}, Time: {GameOverManager.Instance.FinalTime:F2}s");
    }
}
