using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip attackAudioClip;
    [SerializeField] private AudioClip startAudioClip;
    [SerializeField] private AudioClip eatCookieSound;
    [SerializeField] private AudioClip startBgmSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SoundStart()
    {
        _audioSource.Play();
    }

    public void SoundStop()
    {
        _audioSource.Stop();
    }

    public void Attack()
    {
        //attackaudioClip��SE�Ƃ��ĂP�x�Đ�
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(attackAudioClip);
        }
    }

    public void StartSceneSound()
    {
        //startAudioClip��SE�Ƃ��ĂP�x�Đ�
        if (_audioSource != null)
        {
            PlayBGM(startAudioClip,false);
            StartCoroutine(StartSceneSetBgm());
        }
    }

    public void EatCookieSound()
    {
        //EatCookieSound��SE�Ƃ��ĂP�x�Đ�
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(eatCookieSound);
        }
    }

    /// <summary>
    /// mein�V�[���ŗ���BGM��؂�ւ���
    /// </summary>
    /// <param name="newBGM">�Đ�����AudioClip</param>
    /// <param name="isloop">���[�v�Đ����邩<</param>
    public void PlayBGM(AudioClip newBGM, bool isloop = true)
    {
        // ���ݍĐ�����BGM�Ɠ����Ȃ牽�����Ȃ�
        if (_audioSource.clip == newBGM && _audioSource.isPlaying)
        {
            return;
        }

        _audioSource.clip = newBGM;
        _audioSource.loop = isloop;
        _audioSource.Play();
    }

    /// <summary>
    /// �X�^�[�g�V�[���ł̂ݎg�p�AStartSceneSound()���I�������BGM�ɃZ�b�g
    /// </summary>
    private IEnumerator StartSceneSetBgm()
    {
        var currentScene = SceneManager.GetActiveScene().name;

        yield return new WaitForSeconds(startAudioClip.length);
      
        if (currentScene == SceneManager.GetActiveScene().name)
        {
            PlayBGM(startBgmSound);
        }
    }
}
