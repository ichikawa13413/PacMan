using UnityEngine;

using UnityEngine.UI;
/*
 ---life�̃��W�b�N---
player�Őݒ肳�ꂽ���C�t���̕��������C�t�C���[�W�����
���C�t�̐��ɉ����ă��C�t�C���[�W���ϓ��ȊԊu�Ŕz�u�i�}�b�N�X5�܂Łj
player�����񂾂烉�C�t����
���C�t���������烉�C�t�C���[�W�����E�ΏۂɂȂ�悤�ɍĔz�u
*/
public class LifePanel : MonoBehaviour
{
    [SerializeField] private Image _image;
    private Image[] LifeImage;
    [SerializeField] private Player _player;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
        LifeImage = new Image[_player.GetPlayerLife()];
        CreateLifeImage();
    }

    private void Update()
    {
        if (_player.die == true)
        {
            playerDie();
        }
    }

    private void CreateLifeImage()
    {
        for (int i = 0; i < _player.GetPlayerLife(); i++)
        {
            LifeImage[i] = Instantiate(_image,SetLife(i),Quaternion.identity,transform);
        }
    }

    //player�̃��C�t����������i���񂾂�j��ԉE�̃��C�t�������ăp�l���̈ʒu�����炷
    private void playerDie()
    {
        if (_player.GetPlayerLife() > 0)
        {
            LifeImage[_player.GetPlayerLife()].gameObject.SetActive(false);

            for (int i = 0; i < _player.GetPlayerLife(); i++)
            {
                LifeImage[i].transform.position = SetLife(i);
            }
        }
        else 
        {
            LifeImage[0].gameObject.SetActive(false);
        }
    }

    //���Ԋu�ɔz�u����ׂ̍��W�v�Z
    private Vector3 SetLife(int i)
    {
        float x = new();
        switch (_player.GetPlayerLife())
        {
            case 1:
                x = _transform.position.x;//���C�t���P��
                break;
            case 2:
                x = 874.5f + (i) * 180;//���C�t���Q��
                break;
            case 3:
                x = 834.5f + (i) * 130;//���C�t���R��
                break;
            case 4:
                x = 829.5f + (i) * 90;//���C�t���S��
                break;
            case 5:
                x = 814.5f + (i) * 75;//���C�t���T�� �����������Ȃ̂̓��C�t�����E�ϓ��Ɍ�����悤�ɂ��邽��
                break;
            default:
                break;
        }
        float y = _transform.position.y;
        Vector3 position = new Vector3(x, y, 0);
        return position;
    }
}
