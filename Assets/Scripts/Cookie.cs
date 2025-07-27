using UnityEngine;
using VContainer;

public class Cookie : MonoBehaviour
{
    private ScoreManager _scoreManager;
    private MapManager _mapManager;

    [Inject]
    public void Construct(ScoreManager scoreManager, MapManager mapManager)
    {
        _scoreManager = scoreManager;
        _mapManager = mapManager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            SoundManager.instance.EatCookieSound();
            _scoreManager.CookieScore();
            this.gameObject.SetActive(false);
            _mapManager.cookieCounter--;
        }
    }
}
