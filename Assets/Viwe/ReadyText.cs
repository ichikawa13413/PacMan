using System.Collections;
using TMPro;
using UnityEngine;


public class ReadyText : MonoBehaviour
{
    [SerializeField] private float openTime;

    public bool isReady { get; private set; }

    private TextMeshProUGUI readyText;

    private Coroutine ReadyTextCoroutine;

    

    private void Start()
    {
        readyText = GetComponent<TextMeshProUGUI>();
        isReady = true;
        ReadyTextCoroutine = StartCoroutine(OpenReadyText());
    }

    private IEnumerator OpenReadyText()
    {
        yield return new WaitForSeconds(openTime);

        isReady = false;
        StopReadyText();
    }

    private void StopReadyText()
    {
        if (ReadyTextCoroutine != null)
        {
            StopCoroutine(ReadyTextCoroutine);
            ReadyTextCoroutine = null;
        }

        Destroy(readyText);
    }
}
