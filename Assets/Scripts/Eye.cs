using UnityEngine;

public class Eye : MonoBehaviour
{
    private Enemy _enemy;
    private SpriteRenderer _sprite;

    [SerializeField] private Sprite eye_up;
    [SerializeField] private Sprite eye_down;
    [SerializeField] private Sprite eye_left;
    [SerializeField] private Sprite eye_right;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _enemy = GetComponentInParent<Enemy>();
        _sprite.sprite = eye_right;
    }

    private void Update()
    {
        if (_enemy.lastDirection == "enemy:Up")
        {
            _sprite.sprite = eye_up;
        }
        else if (_enemy.lastDirection == "enemy:Down")
        {
            _sprite.sprite= eye_down;
        }
        else if (_enemy.lastDirection == "enemy:Left")
        {
            _sprite.sprite = eye_left;
        }
        else if (_enemy.lastDirection == "enemy:Right")
        {
            _sprite.sprite = eye_right;
        }
    }
}
