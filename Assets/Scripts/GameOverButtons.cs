using UnityEngine;

public class GameOverButtons : MonoBehaviour
{
    public void Retry() => GameOverManager.Instance.Retry();
    public void BackToMenu() => GameOverManager.Instance.BackToMenu();
}
