using UnityEngine;

public class RunPacMan : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform _transform;

    [SerializeField] private Vector3 upStartPosition;
    [SerializeField] private Vector3 upEndPosition;
    [SerializeField] private Vector3 downStartPosition;
    [SerializeField] private Vector3 downEndPosition;
    [SerializeField] private float speed;
    private Vector3 direction;

    //現在の状態を管理するenum
    private enum State
    {
        MovingToUpper,
        MovingToLower,
    }

    private State currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = transform;
    }

    private void Start()
    {
        _transform.position = upStartPosition;
        currentState = State.MovingToUpper;
    }

    private void FixedUpdate()
    {
        switch(currentState)
        {
            case State.MovingToUpper:
                MoveToTarget(upEndPosition, Vector2.left, State.MovingToLower, downStartPosition, Quaternion.Euler(180, 0, 0));
                break;
            case State.MovingToLower:
                MoveToTarget(downEndPosition, Vector2.right, State.MovingToUpper, upStartPosition, Quaternion.Euler(0, 0, 180));
                break;
        }
    }

    private void MoveToTarget(Vector3 targetPos, Vector2 direction, State nextState, 
        Vector3 nextStartPos, Quaternion nextQuaternion)
    {
        //ターゲットまでの距離を計算
        float distance = Vector3.Distance(_transform.position, targetPos);

        if (distance < 0.1f)
        {
            //次の開始位置にテレポート
            _transform.rotation = nextQuaternion;
            rb.linearVelocity = Vector2.zero;
            currentState = nextState;
            _transform.position = nextStartPos;
        }
        else
        {
            rb.linearVelocity = direction * speed;
        }
    }
}
