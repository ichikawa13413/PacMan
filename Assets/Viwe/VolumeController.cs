using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    private Slider VolumeSlider;
    private TextMeshProUGUI volumeText;

    [SerializeField] private AudioSource soundManager;

    private void Start()
    {
        volumeText = GetComponent<TextMeshProUGUI>();

        GameObject ob = gameObject.transform.GetChild(0).gameObject;
        VolumeSlider = ob.GetComponent<Slider>();

        //初期値をAudioSourceの現在のボリュームに設定
        VolumeSlider.value = soundManager.volume;

        // スライダーの値が変更されたときに呼び出すメソッドを設定
        VolumeSlider.onValueChanged.AddListener(SetVolume);

        UpdateVolumeText(soundManager.volume);
    }

    /// <summary>
    /// スライダーの値に基づいてsoundManagerのボリュームを設定する
    /// </summary>
    /// <param name="volume">スライダーの値</param>
    private void SetVolume(float volume)
    {
        if (soundManager != null)
        {
            soundManager.volume = volume;
            UpdateVolumeText(volume);
        }
    }

    //ボリューム値をテキストに表示する
    private void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            volumeText.text = $"{(int)(volume * 100)}";
        }
    }
}
