using UnityEngine;

public class PauseMenuRelay : MonoBehaviour
{
    public void Resume() => UIManager.Instance.Resume();
    public void SaveGame() => UIManager.Instance.SaveGame();
    public void LoadGame() => UIManager.Instance.LoadGame();
}
