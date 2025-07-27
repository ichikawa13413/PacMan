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

        //�����l��AudioSource�̌��݂̃{�����[���ɐݒ�
        VolumeSlider.value = soundManager.volume;

        // �X���C�_�[�̒l���ύX���ꂽ�Ƃ��ɌĂяo�����\�b�h��ݒ�
        VolumeSlider.onValueChanged.AddListener(SetVolume);

        UpdateVolumeText(soundManager.volume);
    }

    /// <summary>
    /// �X���C�_�[�̒l�Ɋ�Â���soundManager�̃{�����[����ݒ肷��
    /// </summary>
    /// <param name="volume">�X���C�_�[�̒l</param>
    private void SetVolume(float volume)
    {
        if (soundManager != null)
        {
            soundManager.volume = volume;
            UpdateVolumeText(volume);
        }
    }

    //�{�����[���l���e�L�X�g�ɕ\������
    private void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            volumeText.text = $"{(int)(volume * 100)}";
        }
    }
}
