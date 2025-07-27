using UnityEngine;
using VContainer;

public class EscSystem : MonoBehaviour
{
    private ClearAndOverManager _clearAndOverManager;
    private PlayTimer _playTimer;
    private bool isPaused;
    private float nextTimer;

    [SerializeField] private float interval;

    [Inject]
    public void Construct(ClearAndOverManager clearAndOverManager,PlayTimer playTimer)
    {
        _clearAndOverManager = clearAndOverManager;
        _playTimer = playTimer;
    }

    private void Start()
    {
        isPaused = false;
        nextTimer = Time.time + interval;
    }

    private void Update()
    {
        if (_clearAndOverManager == null && _playTimer == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused();
        }
    }

    private void Paused()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            _clearAndOverManager.gameFinButton.gameObject.SetActive(true);
            _clearAndOverManager.ReTryButton.gameObject.SetActive(true);
            SoundManager.instance.SoundStop();
            _playTimer.StopTimer();
        }
        else
        {
            _clearAndOverManager.gameFinButton.gameObject.SetActive(false);
            _clearAndOverManager.ReTryButton.gameObject.SetActive(false);
            SoundManager.instance.SoundStart();
            _playTimer.StartTimer();
        }
    }
}
