using UnityEngine;

using UnityEngine.UI;
/*
 ---lifeのロジック---
playerで設定されたライフ数の分だけライフイメージを作る
ライフの数に応じてライフイメージを均等な間隔で配置（マックス5個まで）
playerが死んだらライフ消す
ライフを消したらライフイメージを左右対象になるように再配置
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

    //playerのライフが減ったら（死んだら）一番右のライフを消してパネルの位置をずらす
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

    //等間隔に配置する為の座標計算
    private Vector3 SetLife(int i)
    {
        float x = new();
        switch (_player.GetPlayerLife())
        {
            case 1:
                x = _transform.position.x;//ライフが１つ
                break;
            case 2:
                x = 874.5f + (i) * 180;//ライフが２つ
                break;
            case 3:
                x = 834.5f + (i) * 130;//ライフが３つ
                break;
            case 4:
                x = 829.5f + (i) * 90;//ライフが４つ
                break;
            case 5:
                x = 814.5f + (i) * 75;//ライフが５つ 初項が少数なのはライフが左右均等に見えるようにするため
                break;
            default:
                break;
        }
        float y = _transform.position.y;
        Vector3 position = new Vector3(x, y, 0);
        return position;
    }
}
