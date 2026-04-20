using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string bossRoomScene = "BossRoom";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.SceneManager.LoadScene(bossRoomScene, LoadSceneMode.Single);
    }
}
