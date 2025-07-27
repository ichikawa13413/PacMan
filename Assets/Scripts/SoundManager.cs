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
        //attackaudioClipをSEとして１度再生
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(attackAudioClip);
        }
    }

    public void StartSceneSound()
    {
        //startAudioClipをSEとして１度再生
        if (_audioSource != null)
        {
            PlayBGM(startAudioClip,false);
            StartCoroutine(StartSceneSetBgm());
        }
    }

    public void EatCookieSound()
    {
        //EatCookieSoundをSEとして１度再生
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(eatCookieSound);
        }
    }

    /// <summary>
    /// meinシーンで流すBGMを切り替える
    /// </summary>
    /// <param name="newBGM">再生するAudioClip</param>
    /// <param name="isloop">ループ再生するか<</param>
    public void PlayBGM(AudioClip newBGM, bool isloop = true)
    {
        // 現在再生中のBGMと同じなら何もしない
        if (_audioSource.clip == newBGM && _audioSource.isPlaying)
        {
            return;
        }

        _audioSource.clip = newBGM;
        _audioSource.loop = isloop;
        _audioSource.Play();
    }

    /// <summary>
    /// スタートシーンでのみ使用、StartSceneSound()が終わったらBGMにセット
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
