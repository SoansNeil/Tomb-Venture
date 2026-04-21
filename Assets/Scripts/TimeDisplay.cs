using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    void Start()
    {
        if (GameOverManager.Instance == null) return;

        TMP_Text label = GetComponent<TMP_Text>();
        if (label != null)
            GameOverManager.Instance.RegisterTimeDisplay(label);
    }
}
