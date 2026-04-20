using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float riseHeight = 3f;
    public float riseSpeed = 2f;

    void OnEnable()
    {
        GameManager.Instance.OnAllCoinsCollected += Open;
    }

    void OnDisable()
    {
        GameManager.Instance.OnAllCoinsCollected -= Open;
    }

    private void Open()
    {
        StartCoroutine(Rise());
    }

    private IEnumerator Rise()
    {
        Vector2 target = (Vector2)transform.position + Vector2.up * riseHeight;
        while ((Vector2)transform.position != target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, riseSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
