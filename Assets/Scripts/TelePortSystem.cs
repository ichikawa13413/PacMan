using UnityEngine;

using VContainer;

/*
 mapmanger�Ɏ������悤�Ɖ����A���炭�^�C���}�b�v�ł̓I�u�W�F�N�g�������ė������ǂ������ʂł��Ȃ��H�ׁA
�V�����X�N���v�g�Ŏ�������B�܂��Amapmanager�Ŏ�������ƂȂ�ǐ��������Ȃ�̂ŐV�K�쐬����B
 */
public class TelePortSystem : MonoBehaviour
{
    [SerializeField] private GameObject teleportPoint;//�G�ꂽ�e���|�[�g����I�u�W�F�N�g
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
        // �Փ˂����I�u�W�F�N�g��Transform���擾
        Player player = collision.GetComponent<Player>();
        Enemy enemy = collision.GetComponent<Enemy>();

        if (Time.time > nextTimer && (player != null || enemy != null))
        {
            Transform target = player != null ? player.transform : enemy.transform;
            if (_transform.position.x > 0)//�E��������čs������-1
            {
                Vector3Int getCellPosition = _mapManager.backGround.WorldToCell
                    (new Vector3(-1 * (_transform.position.x - 1),target.transform.position.y,target.transform.position.z));
                Vector3 getCellCenter = _mapManager.backGround.GetCellCenterWorld(getCellPosition);
                target.transform.position = getCellCenter;
            }
            else if (_transform.position.x < 0)//����������čs������+1
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
//����A���̂��e���|�[�g��ɂ�������͂��󂯕t���Ȃ��B�����A�߂낤�Ƃ�����͎͂󂯕t���鎖���o����
//��player�������Ă���l�i_currentCell�Ȃǁj���Z�b�g���鎖�ŉ����ł����B�����ł������R�͋��炭�A
//_currentCell�Ȃǂ��e���|�[�g�����ʒu�̏�񂪎c�����܂܂���������A�Ǝv����B
//���e���|�[�g�n�͈ړ�������A���W�b�N�Ȃǂ����Z�b�g���Ȃ��ƃ_��