
using System.Collections;

using UnityEngine;

using VContainer;

public class Player : MonoBehaviour
{
    [SerializeField] private float nextInterval;
    [SerializeField] private float nextTimer;
    [SerializeField] private float nextTime;//自動移動用
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float animSpeed;
    [SerializeField] private MapManager _mapManager;
    private float invincibleTime;//死んだ時のクールタイム
    [SerializeField] private float invincibleInterval;
    [SerializeField] private float godModeTime;

    [SerializeField] private int life;
    public bool die { get; private set; }
    private Coroutine blinkCoroutine;//点滅のコルーチン
    public bool isGodMode {get; private set;}
    private Coroutine godModeCoroutine;


    private Transform _transform;
    public Vector3 _previousPosition;//キーが入力された時の位置
    private Vector3Int _currentCell;
    private Vector3 myPosition;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    public bool OnTeleport = false;
    private KeyCode _lastKey;
    private PlayTimer _playTimer;
    private ReadyText _readyText;
    private BGMSetter _bgmSetter;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");//StringToHashすなわちハッシュ値は絶対にint型になる
    //動いていない時はアニメーション停止←ゲームが動いているか分からなくなるからオミット←_animator.speed = 1.0f;でスローにする

    [Inject]
    public void Construct(ReadyText readyText, PlayTimer playTimer, BGMSetter bGMSetter)
    {
        _readyText = readyText;
        _playTimer = playTimer;
        _bgmSetter = bGMSetter;
    }

    private void Awake()
    {
        _transform = transform;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        die = false;
        isGodMode = false;
    }
    private void Start()
    {
        nextTimer = Time.time + nextInterval;
        nextTime = nextTimer;

        _lastKey = KeyCode.None;
        ResetMovemen();
        _mapManager.getPlayerPosition(_transform.position);
        invincibleInterval = 3.0f;
        invincibleTime =Time.time + invincibleInterval;
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

        if (life == 0)
        {
            //ゲームオーバー
            return;
        }
        else if (_mapManager.cookieCounter <= 0)
        {
            //クリア
            return;
        }

        PlayerInput();
        MovePersistently();
        _previousPosition = _transform.position;

        //テレポートしたら値をリセット
        if (OnTeleport)
        {
            ResetMovemen();
            OnTeleport = false;
        }
    }

    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.W) && (Time.time > nextTimer))
        {
            nextTimer = Time.time + nextInterval;
            _lastKey = KeyCode.W;
            _previousPosition = _transform.position;
            Debug.Log("W");

        }
        else if (Input.GetKey(KeyCode.S) && (Time.time > nextTimer))
        {
            nextTimer = Time.time + nextInterval;
            _lastKey = KeyCode.S;
            _previousPosition = _transform.position;
            Debug.Log("S");
        }
        else if (Input.GetKey(KeyCode.D) && (Time.time > nextTimer))
        {
            nextTimer = Time.time + nextInterval;
            _lastKey = KeyCode.D;
            _previousPosition = _transform.position;
            Debug.Log("D");
        }
        else if (Input.GetKey(KeyCode.A) && (Time.time > nextTimer))
        {
            nextTimer = Time.time + nextInterval;
            _lastKey = KeyCode.A;
            _previousPosition = _transform.position;
            Debug.Log("A");
        }
    }

    //エネミーとの衝突処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        PowerCookie powerCookie = collision.GetComponent<PowerCookie>();

        if (enemy != null && Time.time > invincibleTime && !isGodMode)
        {
            life--;
            die = true;
            invincibleTime = Time.time + invincibleInterval;
            blinkCoroutine = StartCoroutine(BlinkSprite());//無敵時間中は点滅する
        }

        if (powerCookie != null)
        {
            Debug.Log("godmodeOn");
            isGodMode = true;
            _bgmSetter.OnGodModeBGM();
            godModeCoroutine = StartCoroutine(GodMode());
        }
    }

    public void moveUp()
    {
        _currentCell += new Vector3Int(0, 1, 0);
        //移動先にbackGroundのタイルがあり、Wallのタイルがない場合
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, 90);
            _animator.speed = _speed;
        }//移動先にbackGroundのタイルがあり、Wallのタイルもある場合
        else if(_mapManager.backGround.HasTile(_currentCell) && _mapManager.wall.HasTile(_currentCell))
        {
            _currentCell += new Vector3Int(0, -1, 0);
            _transform.rotation = Quaternion.Euler(0, 0, 90);
            _animator.speed = animSpeed;
        }
    }

    public void moveDown()
    {
        _currentCell += new Vector3Int(0, -1, 0);
        //移動先にbackGroundのタイルがあり、Wallのタイルがない場合
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, -90);
            _animator.speed = _speed;
        }//移動先にbackGroundのタイルがあり、Wallのタイルもある場合
        else if (_mapManager.backGround.HasTile(_currentCell) && _mapManager.wall.HasTile(_currentCell))
        {
            _currentCell += new Vector3Int(0, 1, 0);
            _transform.rotation = Quaternion.Euler(0, 0, -90);
            _animator.speed = animSpeed;
        }
    }

    public void moveLeft()
    {
        _currentCell += new Vector3Int(-1, 0, 0);
        //移動先にbackGroundのタイルがあり、Wallのタイルがない場合
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, -180);
            _animator.speed = _speed;
        }//移動先にbackGroundのタイルがあり、Wallのタイルもある場合
        else if (_mapManager.backGround.HasTile(_currentCell) && _mapManager.wall.HasTile(_currentCell))
        {
            _currentCell += new Vector3Int(1, 0, 0);
            _transform.rotation = Quaternion.Euler(0, 0, -180);
            _animator.speed = animSpeed;
        }
    }

    public void moveRight()
    {
        _currentCell += new Vector3Int(1, 0, 0);
        //移動先にbackGroundのタイルがあり、Wallのタイルがない場合
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, 0);
            _animator.speed = _speed;
        }//移動先にbackGroundのタイルがあり、Wallのタイルもある場合
        else if (_mapManager.backGround.HasTile(_currentCell) && _mapManager.wall.HasTile(_currentCell))
        {
            _currentCell += new Vector3Int(-1, 0, 0);
            _transform.rotation = Quaternion.Euler(0, 0, 0);
            _animator.speed = animSpeed;
        }
    }

    //プレイヤーが動いていなかったら最後に入力した方向へ移動し続ける、スポーン時は何も入力が無ければ右へ移動し続ける
    private void MovePersistently()
    {
        float Interval = nextInterval;

        if (Time.time > nextTime && !IsPlayerMoving())
        {
            switch (_lastKey)
            {
                case KeyCode.W:
                    moveUp();
                    break;
                case KeyCode.S:
                    moveDown();
                    break;
                case KeyCode.D:
                    moveRight();
                    break;
                case KeyCode.A:
                    moveLeft();
                    break;
                default:
                    moveRight();
                    break;
            }
            nextTime = Time.time + Interval;
        }
    }

    //プレイヤーが動いていないかを検知する
    private bool IsPlayerMoving()
    {
        float distance = Vector3.Distance(_transform.position, _previousPosition);
        if (distance == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //テレポート先に着いたら移動系の値をリセット
    private void ResetMovemen()
    {
        _currentCell = _mapManager.backGround.WorldToCell(_transform.position);//自分の位置に一番近いセルを取得
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);//自分の一番近いセルの中心座標を取得
        _transform.position = myPosition;
    }

    //無敵時間が終了するまでスプライトレンダラーを付けたり消したりする
    private IEnumerator BlinkSprite()
    {
        while (Time.time < invincibleTime)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;// 表示状態を反転
            yield return new WaitForSeconds(nextInterval);
        }

        StopBlinking();
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        _spriteRenderer.enabled = true;
        die = false;
    }


    private void StopGodMode()
    {
        if (godModeCoroutine != null)
        {
            StopCoroutine(godModeCoroutine);
            blinkCoroutine = null;
        }
    }

    private IEnumerator GodMode()
    {
        yield return new WaitForSeconds(godModeTime);

        isGodMode = false;
        Debug.Log("godmodeFin");
        _bgmSetter.OnMeinBGM();
        StopGodMode();
    }

    public int GetPlayerLife()
    {
        return life;
    }
}
