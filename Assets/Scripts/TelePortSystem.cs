using UnityEngine;

using VContainer;

/*
 mapmangerに実装しようと下が、恐らくタイルマップではオブジェクトが入って来たかどうか判別できない？為、
新しいスクリプトで実装する。また、mapmanagerで実装するとなる可読性が悪くなるので新規作成する。
 */
public class TelePortSystem : MonoBehaviour
{
    [SerializeField] private GameObject teleportPoint;//触れたテレポートするオブジェクト
    [SerializeField] private float nextTimer;
    [SerializeField] private float teleportInterval;

    private MapManager _mapManager;
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;

    [Inject]
    public void Construct(MapManager mapManager)
    {
        _mapManager = mapManager;
    }

    private void Awake()
    {
        _transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        nextTimer = Time.time + teleportInterval;
        _spriteRenderer.enabled = false;
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 衝突したオブジェクトのTransformを取得
        Player player = collision.GetComponent<Player>();
        Enemy enemy = collision.GetComponent<Enemy>();

        if (Time.time > nextTimer && (player != null || enemy != null))
        {
            Transform target = player != null ? player.transform : enemy.transform;
            if (_transform.position.x > 0)//右から入って行ったら-1
            {
                Vector3Int getCellPosition = _mapManager.backGround.WorldToCell
                    (new Vector3(-1 * (_transform.position.x - 1),target.transform.position.y,target.transform.position.z));
                Vector3 getCellCenter = _mapManager.backGround.GetCellCenterWorld(getCellPosition);
                target.transform.position = getCellCenter;
            }
            else if (_transform.position.x < 0)//左から入って行ったら+1
            {
                Vector3Int getCellPosition = _mapManager.backGround.WorldToCell
                    (new Vector3(-1 * (_transform.position.x + 1), target.transform.position.y, target.transform.position.z));
                Vector3 getCellCenter = _mapManager.backGround.GetCellCenterWorld(getCellPosition);
                target.transform.position = getCellCenter;
            }
        }

        if (player != null)
        {
            player.OnTeleport = true;
        }
        else if (enemy != null)
        {
            enemy.onTeleport = true;
        }
    }
}
//現状、何故かテレポート先についたら入力を受け付けない。だが、戻ろうとする入力は受け付ける事が出来る
//→playerが持っている値（_currentCellなど）リセットする事で解決できた。解決できた理由は恐らく、
//_currentCellなどがテレポートした位置の情報が残ったままだったから、と思われる。
//※テレポート系は移動したら、ロジックなどをリセットしないとダメ