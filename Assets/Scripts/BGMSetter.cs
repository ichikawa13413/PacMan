using UnityEngine;

public class BGMSetter : MonoBehaviour
{
    [SerializeField] private AudioClip meinBgm;
    [SerializeField] private AudioClip godModeBgm;

    private void Start()
    {
        SoundManager.instance.SoundStop();
        OnMeinBGM();
    }

    public void OnGodModeBGM()
    {
        if (SoundManager.instance != null && godModeBgm != null)
        {
            SoundManager.instance.PlayBGM(godModeBgm);
        }
    }

    public void OnMeinBGM()
    {
        if (SoundManager.instance != null && meinBgm != null)
        {
            SoundManager.instance.PlayBGM(meinBgm);
        }
    }
}
