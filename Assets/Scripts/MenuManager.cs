using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button loadGameButton;

    void Start()
    {
        if (loadGameButton != null)
            loadGameButton.gameObject.SetActive(SaveSystem.HasSave());
    }

    public void StartHost()
    {
        GameManager.Instance.ResetGame();
        NetworkManagerSetup.Instance.StartHost();
    }

    public void StartClient()
    {
        NetworkManagerSetup.Instance.StartClient();
    }

    public void LoadGame()
    {
        if (SaveSystem.HasSave() && GameManager.Instance != null)
            GameManager.Instance.LoadGame();
    }
}
