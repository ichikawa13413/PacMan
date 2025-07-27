using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VContainer;

//�[�[�G�l�~�[�̈ړ��Ɋւ���l�����[�[
//�X�^�[�g���͉E�֓�����
//point�𓥂񂾂�ړ��\�ȕ����փ����_���Ɉړ�������
//��{�I�ɍŌ�Ɉړ����������ֈړ���������
//�����ړ����������������ǂ�s���~�܂肾������ړ��\�ȕ�����T���Ĉړ�����

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
    [SerializeField] private int cCost;//�ړ��R�X�g
    private FindGoal _findGoal;

    private Vector3Int _currentCell;
    private Vector3 myPosition;
    private Vector3 _previousPosition;

    private bool onPoint;
    public bool onTeleport = false;
    public bool death {  get; private set; }

    public string lastDirection;//�Ō�Ɉړ������������L�^

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
        lastDirection = "enemy:Right";//�X�^�[�g���͉E�֓�����
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
        randamMove();//�X�^�[�g���͉E�֓�����,�����E�ɕǂ��������烉���_���ɓ�����
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

            //�e���|�[�g������l�����Z�b�g
            if (onTeleport)
            {
                ResetMovement();
                onTeleport = false;
            }
        }

        //���S���Ƀ{�f�B�̃X�v���C�g������
        if (death)
        {
            _spriteRenderer.enabled = false;
        }
        else
        {
            _spriteRenderer.enabled = true;
        }

        //player��godmode��true�ɂȂ�����X�v���C�g��؂�ւ���
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

        //checkDirection�ňړ����Wall���Ȃ����m�F�A�����������烉���_���ŕʕ����Ɉړ�
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

        //�ړ��\�ȕ����������_���őI�o
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

    // ���͂̃Z��wall���Ȃ����m�F
    private List<Vector3Int> checkWall(Vector3Int position)
    {
        List<Vector3Int> possibleMoveDirections = new();

        Vector3Int[] directionsToCheck = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),  // ��
            new Vector3Int(0, -1, 0), // ��
            new Vector3Int(-1, 0, 0), // ��
            new Vector3Int(1, 0, 0)   // �E
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

    //checkDirection�ňړ����Wall���Ȃ����m�F�A�����������烉���_���ŕʕ����Ɉړ��A�����Ȃ������炻�̂܂ܓ����������p��
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

    //�e���|�[�g��ɒ�������ړ��n�̒l�����Z�b�g
    private void ResetMovement()
    {
        _currentCell = _mapManager.backGround.WorldToCell(_transform.position);//�����̈ʒu�Ɉ�ԋ߂��Z�����擾
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);//�����̈�ԋ߂��Z���̒��S���W���擾
        _transform.position = myPosition;
    }

    //player�ɐH��ꂽ�猈�߂�ꂽ�C���^�[�o���ňړ�
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
�Ȃ������_���ŕǂɖ��܂����̂��������randomMove�Œ���Up�Ƃ��ɃA�N�Z�X���Ă�������
�����_���œ������āA���̂��ƂɃ��^�[���ŕǕ�����nextdirection���������܂ܓ������Ă�������ǂɖ��܂���
*/