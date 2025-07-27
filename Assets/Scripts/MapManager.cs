using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;
using VContainer.Unity;

public class MapManager : MonoBehaviour
{
    private Transform _transform;
    public Tilemap wall;
    public Tilemap backGround;

    public List<Vector3Int> possiblePositions;//移動可能なbackGroundのリスト
    public HashSet<Vector3Int> occupiedPositions;//既にpointが設置されている座標を保存
    [SerializeField] public GameObject _point;
    [SerializeField] public Enemy _enemy;
    [SerializeField] private GameObject teleportPoint;//触れたテレポートするオブジェクト
    [SerializeField] private Cookie _cookie;
    [SerializeField] private PowerCookie _powerCookie;
    private Vector3 playerTransform;

    [SerializeField] private float[] X = new float[2];//[0]が左、[1]が右 テレポート用
    [SerializeField] private float[] Y = new float[2];//[0]が上、[1]が下 テレポート用
    [SerializeField] private int enemyCount;
    [SerializeField] private int powerCookieCount;
    public int cookieCounter;

    private IObjectResolver _container;

    [Inject]
    public void Construct(IObjectResolver container)
    {
        _container = container;
    }

    private void Awake()
    {
        _transform = transform;
    }
    private void Start()
    {
        possiblePositions = new List<Vector3Int>();
        occupiedPositions = new HashSet<Vector3Int>();
        searchWallPosition();
        CreateCookie();
        CreateEnemies();
        CreatePoint();
        CreateteleportPoint();
    }

    public void searchWallPosition()
    {
        //backGroundの範囲を取得
        BoundsInt bounds = backGround.cellBounds;

        //backGroundの上にwallがない場所を検索
        foreach (var position in bounds.allPositionsWithin)
        {
            //backGroundでwallがなければ　true
            if (backGround.HasTile(position) && !wall.HasTile(position))
            {
                possiblePositions.Add(position);
            }
        }
    }

    private void CreatePoint()
    {
        //teleportPointを生成する場所と左右の場所をpossiblePositionsから除外
        for (int x = 0; x <= 1; x++ )
        {
            for (int y = 0; y <= 1; y++)
            {
                Vector3Int position = backGround.WorldToCell(new Vector3(X[x], Y[y], 0));
                Vector3Int[] around = CheckAround(position);

                possiblePositions.Remove(position);//テレポートの生成位置
                possiblePositions.Remove(around[2]);//生成位置のXに-1した座標
                possiblePositions.Remove(around[3]);//生成位置からXに+１した座標
            }
        }
        //ランダムに1/5の位置を選択してpointを生成
        var count = possiblePositions.Count / 5;
        for (int i = 0; i < count; i++)
        {
            int randam = Random.Range(0, possiblePositions.Count);

            //実際にpointを設置する位置
            Vector3Int selectedPosition = possiblePositions[randam];

            //周囲のセルにpointがないか確認
            Vector3Int[] around = CheckAround(selectedPosition);

            if (!occupiedPositions.Contains(around[0]) && !occupiedPositions.Contains(around[1])
                && !occupiedPositions.Contains(around[2]) && !occupiedPositions.Contains(around[3]))
            {
                occupiedPositions.Add(selectedPosition);

                //実際にpointを設置する位置のtransform上の正確な位置を取得
                Vector3 worldPosition = backGround.GetCellCenterWorld(selectedPosition);

                //pointを生成
                Instantiate(_point, worldPosition, Quaternion.identity, _transform);

            }

            //既に生成した位置に生成しないように排除する
            possiblePositions.RemoveAt(randam);
        }
    }

    private void CreateEnemies()
    {
        Vector3 playerPosition = playerTransform;

        //playerの位置とその周辺のタイルを除外するリストを作成
        Vector3Int playerCellPosition = backGround.WorldToCell(playerPosition);
        List<Vector3Int> playerZone = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                playerZone.Add(playerCellPosition + new Vector3Int(x, y, 0));
            }
        }

        Enemy[] enemies = new Enemy[enemyCount];

        //ランダムな位置に敵をenemycountの数だけ生成
        for (int i = 0; i < enemyCount; i++)
        {
            int randam = Random.Range(0, possiblePositions.Count);

            //実際に敵を設置する位置
            Vector3Int selectedPosition = possiblePositions[randam];

            //周囲のセルにpointがないか確認
            Vector3Int[] around = CheckAround(selectedPosition);

            //上下左右かつプレイヤーの付近でない所に敵を設置
            if (!occupiedPositions.Contains(around[0]) && !occupiedPositions.Contains(around[1])
                && !occupiedPositions.Contains(around[2]) && !occupiedPositions.Contains(around[3]) 
                && !playerZone.Contains(selectedPosition))
            {
                occupiedPositions.Add(selectedPosition);

                //実際にenemyを設置する位置のtransform上の正確な位置を取得
                Vector3 worldPosition = backGround.GetCellCenterWorld(selectedPosition);

                //enemyを生成
                enemies[i] = _container.Instantiate(_enemy, worldPosition, Quaternion.identity);
            }

            //既に生成した位置に生成しないように排除する
            possiblePositions.RemoveAt(randam);
        }
    }

    //プレイヤーの座標取得　
    public void getPlayerPosition(Vector3 playerPosition)
    {
        //現状プレイヤーの座標を取得出来る方法がこれしか思いつかない為、このメソットを作成
        //CreateEnemiesの引数でプレイヤーの座標を直接入れる事が出来なかった理由、
        //恐らくpossiblePositionsの値が生成される前にCreateEnemiesが呼ばれいるから
        playerTransform = playerPosition;
    }

    //触れたらテレポートするpointを設置
    private void CreateteleportPoint()
    {
        //teleportPointを指定座標に設置し、セルのセンターに設置する
        for (int x = 0; x <= 1; x++)
        {
            for (int y = 0; y <= 1; y++)
            {
                Vector3Int position = backGround.WorldToCell(new Vector3(X[x], Y[y], 0));
                Vector3 CellCenter = backGround.GetCellCenterWorld(position);
                _container.Instantiate(teleportPoint, CellCenter, Quaternion.identity, transform);
            }
        }
    }

    //上下左右確認メソット
    private Vector3Int[] CheckAround(Vector3Int position)
    {
        Vector3Int up = position + Vector3Int.up;
        Vector3Int down = position + Vector3Int.down;
        Vector3Int left = position + Vector3Int.left;
        Vector3Int right = position + Vector3Int.right;

        Vector3Int[] around = new Vector3Int[4] {up,down,left,right};
        return around;
    }

    private void CreateCookie()
    {
        List<Vector3Int> cookieList = possiblePositions;
        //最初にパワークッキーを生成
        for (int i = 0; i < powerCookieCount; i++)
        {
            //cookieListからランダムで選ぶ
            int value = Random.Range(0, cookieList.Count);
            Vector3Int selectPosition = cookieList[value];

            //_powerCookieパワークッキーを生成
            Vector3 cellCenter = backGround.GetCellCenterWorld(selectPosition);
            _container.Instantiate(_powerCookie, cellCenter, Quaternion.identity, transform);
            cookieList.RemoveAt(value);
            cookieCounter++;
        }
        
        //普通のクッキーを生成
        foreach (Vector3Int position in cookieList)
        {
            Vector3 center = backGround.GetCellCenterWorld(position);
            _container.Instantiate(_cookie, center, Quaternion.identity, transform);
            cookieCounter++;
        }
    }
}
