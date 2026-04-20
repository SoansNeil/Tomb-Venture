using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerSetup : MonoBehaviour
{
    public static NetworkManagerSetup Instance { get; private set; }

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

    private void OnNetworkSceneLoaded(string sceneName, LoadSceneMode mode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPoint == null) return;

        int index = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null) continue;
            Vector3 pos = spawnPoint.transform.position + new Vector3(index * clientSpawnOffset, 0f, 0f);
            client.PlayerObject.transform.position = pos;
            index++;
        }
    }

    public Vector2 hostSpawnPoint;
    public float clientSpawnOffset = 2f;

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApproveConnection;
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnNetworkSceneLoaded;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnNetworkSceneLoaded;
        NetworkManager.Singleton.SceneManager.LoadScene("Room1", LoadSceneMode.Single);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.StartClient();
    }

    private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        bool isHost = request.ClientNetworkId == NetworkManager.Singleton.LocalClientId;
        response.Position = isHost
            ? hostSpawnPoint
            : new Vector2(hostSpawnPoint.x + clientSpawnOffset, hostSpawnPoint.y);
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
        response.Approved = true;
    }
}
