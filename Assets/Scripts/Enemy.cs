using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VContainer;

//ーーエネミーの移動に関する考え方ーー
//スタート時は右へ動かす
//pointを踏んだら移動可能な方向へランダムに移動させる
//基本的に最後に移動した方向へ移動し続ける
//もし移動し続けた方向が壁や行き止まりだったら移動可能な方向を探して移動する

public class Enemy : MonoBehaviour
{
    private Transform _transform;

    private MapManager _mapManager;
    private Player _player;
    private ScoreManager _scoreManager;
    private PlayTimer _playTimer;
    private FloatingTextManager _floatingTextManager;
    private ReadyText _readyText;
   
    [SerializeField] private float nextInterval;
    [SerializeField] private float nextTimer;
    [SerializeField] private AStarNode _starNode;
    [SerializeField] private Vector3 goal;
    [SerializeField] private int cCost;//移動コスト
    private FindGoal _findGoal;

    private Vector3Int _currentCell;
    private Vector3 myPosition;
    private Vector3 _previousPosition;

    private bool onPoint;
    public bool onTeleport = false;
    public bool death {  get; private set; }

    public string lastDirection;//最後に移動した方向を記録

    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject normalEye;
    [SerializeField] private GameObject whiteEye;
    private Color memoryColor;

    [Inject]
    public void Construct(FindGoal findGoal,MapManager mapManager, Player player, ScoreManager scoreManager,
        PlayTimer playTimer,FloatingTextManager floatingTextManager, ReadyText readyText)
    {
        _findGoal = findGoal;
        _mapManager = mapManager;
        _player = player;
        _scoreManager = scoreManager;
        _playTimer = playTimer;
        _floatingTextManager = floatingTextManager;
        _readyText = readyText;
    }

    private void Awake()
    {
        _transform = transform;
        onPoint = false;
        lastDirection = "enemy:Right";//スタート時は右へ動かす
        _spriteRenderer = GetComponent<SpriteRenderer>();
        whiteEye.SetActive(false);

    }

    private void Start()
    {
        _spriteRenderer.color = RandomColor();
        memoryColor = _spriteRenderer.color;
        nextTimer = Time.time + nextInterval;
        ResetMovement();
        _previousPosition = _transform.position;
        randamMove();//スタート時は右へ動かす,もし右に壁があったらランダムに動かす
        death = false;
    }

    private void Update()
    {
        if (_readyText.isReady)
        {
            return;
        }

        if (Time.time > nextTimer && !death)
        {
            if (onPoint)
            {
                lastDirection = randamMove();
                onPoint = false;
            }
            EnemyMove();
            nextTimer = Time.time + nextInterval;

            //テレポートしたら値をリセット
            if (onTeleport)
            {
                ResetMovement();
                onTeleport = false;
            }
        }

        //死亡時にボディのスプライトを消す
        if (death)
        {
            _spriteRenderer.enabled = false;
        }
        else
        {
            _spriteRenderer.enabled = true;
        }

        //playerのgodmodeがtrueになったらスプライトを切り替える
        if (_player.isGodMode)
        {
            _spriteRenderer.color = Color.blue;

            normalEye.SetActive(false);
            whiteEye.SetActive(true);
        }
        else if (!_player.isGodMode)
        {
            _spriteRenderer.color = memoryColor;

            normalEye.SetActive(true);
            whiteEye.SetActive(false);
        }
    }

    public void EnemyMove()
    {
        _currentCell = _mapManager.backGround.WorldToCell(_transform.position);

        //checkDirectionで移動先にWallがないか確認、もしあったらランダムで別方向に移動
        lastDirection = checkDirection(_currentCell);
        switch (lastDirection)
        {
            case "enemy:Up":
                Up();
                break;
            case "enemy:Down":
                Down();
                break;
            case "enemy:Left":
                Left();
                break;
            case "enemy:Right":
                Right();
                break;
        }
    }

    private void Up()
    {
        _currentCell += new Vector3Int(0, 1, 0);
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
        _transform.position = myPosition;
        lastDirection = "enemy:Up";
    }

    private void Down()
    {
        _currentCell += new Vector3Int(0, -1, 0);
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
        _transform.position = myPosition;
        lastDirection = "enemy:Down";
    }

    private void Left()
    {
        _currentCell += new Vector3Int(-1, 0, 0);
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
        _transform.position = myPosition;
        lastDirection = "enemy:Left";
    }

    private void Right()
    {
        _currentCell += new Vector3Int(1, 0, 0);
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
        _transform.position = myPosition;
        lastDirection = "enemy:Right";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PointObject point = collision.GetComponent<PointObject>();
        Player player = collision.gameObject.GetComponent<Player>();

        if (point != null)
        {
            onPoint =true;
        }

        if (player != null && _player.isGodMode && !death)
        {
            SoundManager.instance.Attack();
            _scoreManager.EnemyScore();
            _floatingTextManager.SetFloatingText(_transform.position);
            _playTimer.StartSlow();
            StartCoroutine(waitMove(_findGoal.AStar(_transform.position, goal, cCost)));
            death = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onPoint = false;
    }

    private string randamMove()
    {
        List<Vector3Int> possibleMoveDirections = checkWall(_currentCell);

        //移動可能な方向をランダムで選出
        if (possibleMoveDirections.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleMoveDirections.Count);

            Vector3Int chosenDirection = possibleMoveDirections[randomIndex];

            string nextDirection = "enemy:Down";
            if (chosenDirection == new Vector3Int(0, 1, 0))
            {
                nextDirection = "enemy:Up";
                return nextDirection;
            }
            else if (chosenDirection == new Vector3Int(0, -1, 0))
            {
                nextDirection = "enemy:Down";
                return nextDirection;
            }
            else if (chosenDirection == new Vector3Int(-1, 0, 0))
            {
                nextDirection = "enemy:Left";
                return nextDirection;
            }
            else if (chosenDirection == new Vector3Int(1, 0, 0))
            {
                nextDirection = "enemy:Right";
                return nextDirection;
            }
        }
        return "";
    }

    // 周囲のセルwallがないか確認
    private List<Vector3Int> checkWall(Vector3Int position)
    {
        List<Vector3Int> possibleMoveDirections = new();

        Vector3Int[] directionsToCheck = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),  // 上
            new Vector3Int(0, -1, 0), // 下
            new Vector3Int(-1, 0, 0), // 左
            new Vector3Int(1, 0, 0)   // 右
        };

        foreach (Vector3Int direction in directionsToCheck)
        {
            Vector3Int targetCell = position + direction;
            if (_mapManager.backGround.HasTile(targetCell) && !_mapManager.wall.HasTile(targetCell))
            {
                possibleMoveDirections.Add(direction);
            }
        }
        return possibleMoveDirections;
    }

    //checkDirectionで移動先にWallがないか確認、もしあったらランダムで別方向に移動、もしなかったらそのまま同じ方向を継続
    private string checkDirection(Vector3Int currentCell)
    {
        string nextdirection = lastDirection;
        if (lastDirection == "enemy:Up")
        {
            currentCell += new Vector3Int(0, 1, 0);
            if (_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = randamMove();
                return nextdirection;

            }
            else if (!_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = "enemy:Up";
                return nextdirection;
            }
        }
        else if (lastDirection == "enemy:Down")
        {
            currentCell += new Vector3Int(0, -1, 0);
            if (_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = randamMove();
                return nextdirection;
            }
            else if (!_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = "enemy:Down";
                return nextdirection;
            }
        }
        else if (lastDirection == "enemy:Left")
        {
            currentCell += new Vector3Int(-1, 0, 0);
            if (_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = randamMove();
                return nextdirection;

            }
            else if (!_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = "enemy:Left";
                return nextdirection;
            }
        }
        else if (lastDirection == "enemy:Right")
        {
            currentCell += new Vector3Int(1, 0, 0);
            if (_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = randamMove();
                return nextdirection;
            }
            else if (!_mapManager.wall.HasTile(currentCell))
            {
                nextdirection = "enemy:Right";
                return nextdirection;
            }
        }
        return "";
    }

    //テレポート先に着いたら移動系の値をリセット
    private void ResetMovement()
    {
        _currentCell = _mapManager.backGround.WorldToCell(_transform.position);//自分の位置に一番近いセルを取得
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);//自分の一番近いセルの中心座標を取得
        _transform.position = myPosition;
    }

    //playerに食われたら決められたインターバルで移動
    private IEnumerator waitMove(List<AStarNode> path)
    {
        foreach (AStarNode node in path)
        {
            _transform.position = node.position;
            yield return new WaitForSeconds(nextInterval);
        }

        death = false;
    }

    private Color RandomColor()
    {
        Color enemyColor = new Color();

        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                enemyColor = Color.red;
                break;
            case 1:
                enemyColor = Color.green;
                break;
            case 2:
                enemyColor = Color.yellow;
                break;
            case 3:
                enemyColor = Color.cyan;
                break;
        }

        return enemyColor;
    }

    //private IEnumerator 
}
/*
なぜランダムで壁に埋まったのか→それはrandomMoveで直接Upとかにアクセスしていたから
ランダムで動かして、そのあとにリターンで壁方向のnextdirectionが入ったまま動かしていたから壁に埋まった
*/