using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        // Host's callback fires before this scene loads, so check immediately on Start
        if (TryAssignLocalPlayer()) return;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        TryAssignLocalPlayer();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private bool TryAssignLocalPlayer()
    {
        foreach (var player in FindObjectsOfType<PlayerMovement>())
        {
            if (!player.IsOwner) continue;
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
            return true;
        }
        return false;
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
}
