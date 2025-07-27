using System.Collections;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

using VContainer;
using VContainer.Unity;

public class FloatingTextManager : MonoBehaviour
{
    /*
     * �G�l�~�[���j���ɃG�l�~�[�̓���ɃX�R�A��\��
     * �X�R�A�͕ς�����悤�ɂ���
     * �\�����Ԃ�_playTimer.StartSlow()�̃X���[���o��
     */
    private Camera _camera;
    private Canvas _canvas;

    [SerializeField] private FloatingText enemyScoreText;
    [SerializeField] private int yOffset;//�e�L�X�g��ݒu���鍂����ݒ�
    private Vector3 pos = Vector3.zero;

    private ObjectPool<FloatingText> enemyScorePool;

    private IObjectResolver _container;
    private PlayTimer _playTimer;

    [Inject] 
    public void Construct(IObjectResolver objectResolver, PlayTimer playTimer,Camera camera, Canvas canvas)
    {
        _container = objectResolver;
        _playTimer = playTimer;
        _camera = camera;
        _canvas = canvas;
    }

    private void Awake()
    {
        enemyScorePool = new ObjectPool<FloatingText>(CreateText, GetText, ReleaseText);
    }

    private Vector3 GetScreenPosition(Vector3 enemyPos)
    {
        Vector2 pos = enemyPos;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, pos);
        screenPoint.y += yOffset;
        return screenPoint;
    }

    private FloatingText CreateText()
    {
        return _container.Instantiate(enemyScoreText, pos, Quaternion.identity, _canvas.transform);
    }

    private void GetText(FloatingText floatingText)
    {
        floatingText.gameObject.SetActive(true);
    }

    private void ReleaseText(FloatingText floatingText)
    {
        floatingText.gameObject.SetActive(false);
    }

    public void SetFloatingText(Vector3 pos)
    {
        Vector3 setPosition = GetScreenPosition(pos);
        var floatingText = enemyScorePool.Get();
        RectTransform UI = floatingText.gameObject.GetComponent<RectTransform>();
        UI.position = setPosition;
        StartCoroutine(CloseFloatingText(floatingText));
    }

    private IEnumerator CloseFloatingText(FloatingText floatingText)
    {
        yield return new WaitForSecondsRealtime(_playTimer.GetslowWait());
        enemyScorePool.Release(floatingText);
    }
}
