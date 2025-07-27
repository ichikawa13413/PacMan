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

    public List<Vector3Int> possiblePositions;//�ړ��\��backGround�̃��X�g
    public HashSet<Vector3Int> occupiedPositions;//����point���ݒu����Ă�����W��ۑ�
    [SerializeField] public GameObject _point;
    [SerializeField] public Enemy _enemy;
    [SerializeField] private GameObject teleportPoint;//�G�ꂽ�e���|�[�g����I�u�W�F�N�g
    [SerializeField] private Cookie _cookie;
    [SerializeField] private PowerCookie _powerCookie;
    private Vector3 playerTransform;

    [SerializeField] private float[] X = new float[2];//[0]�����A[1]���E �e���|�[�g�p
    [SerializeField] private float[] Y = new float[2];//[0]����A[1]���� �e���|�[�g�p
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
        //backGround�͈̔͂��擾
        BoundsInt bounds = backGround.cellBounds;

        //backGround�̏��wall���Ȃ��ꏊ������
        foreach (var position in bounds.allPositionsWithin)
        {
            //backGround��wall���Ȃ���΁@true
            if (backGround.HasTile(position) && !wall.HasTile(position))
            {
                possiblePositions.Add(position);
            }
        }
    }

    private void CreatePoint()
    {
        //teleportPoint�𐶐�����ꏊ�ƍ��E�̏ꏊ��possiblePositions���珜�O
        for (int x = 0; x <= 1; x++ )
        {
            for (int y = 0; y <= 1; y++)
            {
                Vector3Int position = backGround.WorldToCell(new Vector3(X[x], Y[y], 0));
                Vector3Int[] around = CheckAround(position);

                possiblePositions.Remove(position);//�e���|�[�g�̐����ʒu
                possiblePositions.Remove(around[2]);//�����ʒu��X��-1�������W
                possiblePositions.Remove(around[3]);//�����ʒu����X��+�P�������W
            }
        }
        //�����_����1/5�̈ʒu��I������point�𐶐�
        var count = possiblePositions.Count / 5;
        for (int i = 0; i < count; i++)
        {
            int randam = Random.Range(0, possiblePositions.Count);

            //���ۂ�point��ݒu����ʒu
            Vector3Int selectedPosition = possiblePositions[randam];

            //���͂̃Z����point���Ȃ����m�F
            Vector3Int[] around = CheckAround(selectedPosition);

            if (!occupiedPositions.Contains(around[0]) && !occupiedPositions.Contains(around[1])
                && !occupiedPositions.Contains(around[2]) && !occupiedPositions.Contains(around[3]))
            {
                occupiedPositions.Add(selectedPosition);

                //���ۂ�point��ݒu����ʒu��transform��̐��m�Ȉʒu���擾
                Vector3 worldPosition = backGround.GetCellCenterWorld(selectedPosition);

                //point�𐶐�
                Instantiate(_point, worldPosition, Quaternion.identity, _transform);

            }

            //���ɐ��������ʒu�ɐ������Ȃ��悤�ɔr������
            possiblePositions.RemoveAt(randam);
        }
    }

    private void CreateEnemies()
    {
        Vector3 playerPosition = playerTransform;

        //player�̈ʒu�Ƃ��̎��ӂ̃^�C�������O���郊�X�g���쐬
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

        //�����_���Ȉʒu�ɓG��enemycount�̐���������
        for (int i = 0; i < enemyCount; i++)
        {
            int randam = Random.Range(0, possiblePositions.Count);

            //���ۂɓG��ݒu����ʒu
            Vector3Int selectedPosition = possiblePositions[randam];

            //���͂̃Z����point���Ȃ����m�F
            Vector3Int[] around = CheckAround(selectedPosition);

            //�㉺���E���v���C���[�̕t�߂łȂ����ɓG��ݒu
            if (!occupiedPositions.Contains(around[0]) && !occupiedPositions.Contains(around[1])
                && !occupiedPositions.Contains(around[2]) && !occupiedPositions.Contains(around[3]) 
                && !playerZone.Contains(selectedPosition))
            {
                occupiedPositions.Add(selectedPosition);

                //���ۂ�enemy��ݒu����ʒu��transform��̐��m�Ȉʒu���擾
                Vector3 worldPosition = backGround.GetCellCenterWorld(selectedPosition);

                //enemy�𐶐�
                enemies[i] = _container.Instantiate(_enemy, worldPosition, Quaternion.identity);
            }

            //���ɐ��������ʒu�ɐ������Ȃ��悤�ɔr������
            possiblePositions.RemoveAt(randam);
        }
    }

    //�v���C���[�̍��W�擾�@
    public void getPlayerPosition(Vector3 playerPosition)
    {
        //����v���C���[�̍��W���擾�o������@�����ꂵ���v�����Ȃ��ׁA���̃��\�b�g���쐬
        //CreateEnemies�̈����Ńv���C���[�̍��W�𒼐ړ���鎖���o���Ȃ��������R�A
        //���炭possiblePositions�̒l�����������O��CreateEnemies���Ă΂ꂢ�邩��
        playerTransform = playerPosition;
    }

    //�G�ꂽ��e���|�[�g����point��ݒu
    private void CreateteleportPoint()
    {
        //teleportPoint���w����W�ɐݒu���A�Z���̃Z���^�[�ɐݒu����
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

    //�㉺���E�m�F���\�b�g
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
        //�ŏ��Ƀp���[�N�b�L�[�𐶐�
        for (int i = 0; i < powerCookieCount; i++)
        {
            //cookieList���烉���_���őI��
            int value = Random.Range(0, cookieList.Count);
            Vector3Int selectPosition = cookieList[value];

            //_powerCookie�p���[�N�b�L�[�𐶐�
            Vector3 cellCenter = backGround.GetCellCenterWorld(selectPosition);
            _container.Instantiate(_powerCookie, cellCenter, Quaternion.identity, transform);
            cookieList.RemoveAt(value);
            cookieCounter++;
        }
        
        //���ʂ̃N�b�L�[�𐶐�
        foreach (Vector3Int position in cookieList)
        {
            Vector3 center = backGround.GetCellCenterWorld(position);
            _container.Instantiate(_cookie, center, Quaternion.identity, transform);
            cookieCounter++;
        }
    }
}
