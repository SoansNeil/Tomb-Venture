using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
        public void StartHost()
    {
        NetworkManagerSetup.Instance.StartHost();
    }

    public void StartClient()
    {
        NetworkManagerSetup.Instance.StartClient();
    }
}
