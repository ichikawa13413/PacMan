using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LifeTimeScope : LifetimeScope
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private Player player;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private PlayTimer playTimer;
    [SerializeField] private FloatingTextManager floatingTextManager;
    [SerializeField] private FloatingText floatingText;
    [SerializeField] private Camera camera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private HighScore highScore;
    [SerializeField] private ReadyText readyText;
    [SerializeField] private BGMSetter bgmSetter;
    [SerializeField] private ClearAndOverManager clearAndOverManager;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(mapManager);
        builder.RegisterInstance(player);
        builder.RegisterInstance(scoreManager);
        builder.RegisterInstance(playTimer);
        builder.RegisterInstance(floatingTextManager);
        builder.RegisterInstance(floatingText);
        builder.RegisterInstance(camera);
        builder.RegisterInstance(canvas);
        builder.RegisterInstance(highScore);
        builder.RegisterInstance(readyText);
        builder.RegisterInstance(bgmSetter);
        builder.RegisterInstance(clearAndOverManager);

        builder.Register<FindGoal>(Lifetime.Singleton);
        builder.Register<ISaveData, SaveFile>(Lifetime.Singleton);
    }
}
/*����
 * Auto Inject GameObjects�͊��ɃV�[����ɑ��݂���I�u�W�F�N�g��ΏۂƂ��āA�ˑ����̒���������B�ŏ��ɃV�[����ɂȂ����͈̂ˑ����̒����͏o���Ȃ��B
 * �ŏ�����V�[����ɂȂ��ꍇ�́A��������X�N���v�g��IObjectResolver�𒍓����ďグ��B��Fpublic void Construct(IObjectResolver container)
 * --Enemy�i�J�n���͐�������Ă��Ȃ��j��FindGoal�����Ă��闬--
 *   LifeTimeScope��Auto Inject Game Objects�Ƀ}�b�v�}�l�[�W���[��o�^
 *   �}�b�v�}�l�[�W���[��public void Construct(IObjectResolver container)��IObjectResolver���쐬
 *   IObjectResolver�iVContainer�j��Instantiate�i_container.Instantiate�j��Enemy�𐶐�������Ő��������ꍇ�A��������VContainer��Enemy�N���X��[Inject] �������t����ꂽ���\�b�h��T���āA�K�v�Ƃ��Ă��镨�������Œ������Ă����
 *   �������ꂽEnemy��FindGoal����������āA�K�v�Ȏ���FindGoal����AStar���\�b�h���Ă΂��
 */
