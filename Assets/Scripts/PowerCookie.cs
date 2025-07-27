using UnityEngine;
using VContainer;

public class PowerCookie : MonoBehaviour
{
    private MapManager _mapManager;

    [Inject]
    public void Construct(MapManager mapManager)
    {
        _mapManager = mapManager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            SoundManager.instance.EatCookieSound();
            this.gameObject.SetActive(false);
            _mapManager.cookieCounter--;
        }
    }
}
