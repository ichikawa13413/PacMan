using System.Collections;
using TMPro;
using UnityEngine;
using VContainer;

public class PlayTimer : MonoBehaviour
{
    private float time;
    private TextMeshProUGUI timeText;
    private Coroutine slowCoroutine;
    private ReadyText _readyText;
    public bool isSlow {  get; private set; }
    
    [SerializeField] private float slowScale;
    [SerializeField] private float slowWait;

    [Inject]
    public void Construct(ReadyText readyText)
    {
        _readyText = readyText;
    }

    private void Awake()
    {
        Time.timeScale = 1.0f;
        timeText = GetComponent<TextMeshProUGUI>();
        isSlow = false;
    }

    private void Update()
    {
        // _readyTextが注入されるまで処理をスキップする
        if (_readyText == null)
        {
            return;
        }

        if (_readyText.isReady)
        {
            return;
        }

        time += Time.deltaTime;

        //経過時間を整数秒に丸める←これをしないと正確時間を計れない
        int totalSeconds = Mathf.FloorToInt(time);

        var min = (totalSeconds / 60).ToString("00");
        var sec = (totalSeconds % 60).ToString("00");

        timeText.text = min + ":" + sec;
    }

    public void StartSlow()
    {
        slowCoroutine = StartCoroutine(OnSlow());
    }
    
    public IEnumerator OnSlow()
    {
        Time.timeScale = slowScale;

        yield return new WaitForSecondsRealtime(slowWait);

        Time.timeScale = 1.0f;

        StopSlow();
    }

    private void StopSlow()
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }
    }

    public float GetslowWait()
    {
        return slowWait;
    }

    public void StopTimer()
    {
        Time.timeScale = 0;
    }

    public void StartTimer()
    {
        Time.timeScale = 1.0f;
    }
}
