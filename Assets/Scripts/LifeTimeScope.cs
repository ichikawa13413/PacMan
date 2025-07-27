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
/*メモ
 * Auto Inject GameObjectsは既にシーン上に存在するオブジェクトを対象として、依存性の注入をする。最初にシーン上にないものは依存性の注入は出来ない。
 * 最初からシーン上にない場合は、生成するスクリプトでIObjectResolverを注入して上げる。例：public void Construct(IObjectResolver container)
 * --Enemy（開始時は生成されていない）にFindGoalを入れている流--
 *   LifeTimeScopeのAuto Inject Game Objectsにマップマネージャーを登録
 *   マップマネージャーでpublic void Construct(IObjectResolver container)でIObjectResolverを作成
 *   IObjectResolver（VContainer）のInstantiate（_container.Instantiate）でEnemyを生成←これで生成した場合、生成時にVContainerがEnemyクラスに[Inject] 属性が付けられたメソッドを探して、必要としている物を自動で注入してくれる
 *   生成されたEnemyにFindGoalが注入されて、必要な時にFindGoal内のAStarメソッドが呼ばれる
 */
