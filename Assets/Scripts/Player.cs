
using System.Collections;

using UnityEngine;

using VContainer;

public class Player : MonoBehaviour
{
    [SerializeField] private float nextInterval;
    [SerializeField] private float nextTimer;
    [SerializeField] private float nextTime;//�����ړ��p
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float animSpeed;
    [SerializeField] private MapManager _mapManager;
    private float invincibleTime;//���񂾎��̃N�[���^�C��
    [SerializeField] private float invincibleInterval;
    [SerializeField] private float godModeTime;

    [SerializeField] private int life;
    public bool die { get; private set; }
    private Coroutine blinkCoroutine;//�_�ł̃R���[�`��
    public bool isGodMode {get; private set;}
    private Coroutine godModeCoroutine;


    private Transform _transform;
    public Vector3 _previousPosition;//�L�[�����͂��ꂽ���̈ʒu
    private Vector3Int _currentCell;
    private Vector3 myPosition;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    public bool OnTeleport = false;
    private KeyCode _lastKey;
    private PlayTimer _playTimer;
    private ReadyText _readyText;
    private BGMSetter _bgmSetter;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");//StringToHash���Ȃ킿�n�b�V���l�͐�΂�int�^�ɂȂ�
    //�����Ă��Ȃ����̓A�j���[�V������~���Q�[���������Ă��邩������Ȃ��Ȃ邩��I�~�b�g��_animator.speed = 1.0f;�ŃX���[�ɂ���

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
        // _readyText�����������܂ŏ������X�L�b�v����
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
            //�Q�[���I�[�o�[
            return;
        }
        else if (_mapManager.cookieCounter <= 0)
        {
            //�N���A
            return;
        }

        PlayerInput();
        MovePersistently();
        _previousPosition = _transform.position;

        //�e���|�[�g������l�����Z�b�g
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

    //�G�l�~�[�Ƃ̏Փˏ���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        PowerCookie powerCookie = collision.GetComponent<PowerCookie>();

        if (enemy != null && Time.time > invincibleTime && !isGodMode)
        {
            life--;
            die = true;
            invincibleTime = Time.time + invincibleInterval;
            blinkCoroutine = StartCoroutine(BlinkSprite());//���G���Ԓ��͓_�ł���
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
        //�ړ����backGround�̃^�C��������AWall�̃^�C�����Ȃ��ꍇ
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, 90);
            _animator.speed = _speed;
        }//�ړ����backGround�̃^�C��������AWall�̃^�C��������ꍇ
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
        //�ړ����backGround�̃^�C��������AWall�̃^�C�����Ȃ��ꍇ
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, -90);
            _animator.speed = _speed;
        }//�ړ����backGround�̃^�C��������AWall�̃^�C��������ꍇ
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
        //�ړ����backGround�̃^�C��������AWall�̃^�C�����Ȃ��ꍇ
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, -180);
            _animator.speed = _speed;
        }//�ړ����backGround�̃^�C��������AWall�̃^�C��������ꍇ
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
        //�ړ����backGround�̃^�C��������AWall�̃^�C�����Ȃ��ꍇ
        if (_mapManager.backGround.HasTile(_currentCell) && !_mapManager.wall.HasTile(_currentCell))
        {
            myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);
            _transform.position = myPosition;
            _transform.rotation = Quaternion.Euler(0, 0, 0);
            _animator.speed = _speed;
        }//�ړ����backGround�̃^�C��������AWall�̃^�C��������ꍇ
        else if (_mapManager.backGround.HasTile(_currentCell) && _mapManager.wall.HasTile(_currentCell))
        {
            _currentCell += new Vector3Int(-1, 0, 0);
            _transform.rotation = Quaternion.Euler(0, 0, 0);
            _animator.speed = animSpeed;
        }
    }

    //�v���C���[�������Ă��Ȃ�������Ō�ɓ��͂��������ֈړ���������A�X�|�[�����͉������͂�������ΉE�ֈړ���������
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

    //�v���C���[�������Ă��Ȃ��������m����
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

    //�e���|�[�g��ɒ�������ړ��n�̒l�����Z�b�g
    private void ResetMovemen()
    {
        _currentCell = _mapManager.backGround.WorldToCell(_transform.position);//�����̈ʒu�Ɉ�ԋ߂��Z�����擾
        myPosition = _mapManager.backGround.GetCellCenterWorld(_currentCell);//�����̈�ԋ߂��Z���̒��S���W���擾
        _transform.position = myPosition;
    }

    //���G���Ԃ��I������܂ŃX�v���C�g�����_���[��t������������肷��
    private IEnumerator BlinkSprite()
    {
        while (Time.time < invincibleTime)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;// �\����Ԃ𔽓]
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
