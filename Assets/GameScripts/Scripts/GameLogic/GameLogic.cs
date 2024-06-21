using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using SgLibUnite.Systems;
using SgLibUnite.Singleton;

/* インゲームシーンとボスシーン間でガウス差分の
 ポスプロのボリュームの参照を保持したいのでシングルトン */

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : SingletonBaseClass<GameLogic>
        , IBossDieNotifiable
{
    #region Acitons

    /// <summary> パリィ成功時のイベント </summary>
    public Action EParrySucceed;

    /// <summary> 一時停止処理が走った時の処理 </summary>
    public Action EPause;

    /// <summary> 一時停止から抜けるときに走る処理 </summary>
    public Action EResume;

    /// <summary> ゾーンに入るときに走る処理 </summary>
    public Action EDiveZone;

    /// <summary> ゾーンから出るときに走る処理 </summary>
    public Action EGetoutZone;

    #endregion

    /// <summary> ガウス差分クラス </summary>
    private DifferenceOfGaussian _dog;

    /// <summary> ポスプロをかけるために必要なVolumeクラス </summary>
    private Volume _volume;

    /// <summary> シーン遷移クラス </summary>
    private SceneLoader _sceneLoader;

    /// <summary> シーンのインデックス </summary>
    private int _selectedSceneIndex;

    /// <summary> 敵のトランスフォーム </summary>
    private List<Transform> _enemies = new List<Transform>();

    public event Action TaskOnBossDefeated;

    private InGameUIManager _ingameUI;

    protected override void ToDoAtAwakeSingleton()
    {
    }

    private void Start()
    {
        Initialize();

        _ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>();
    }

    private void Update()
    {
        _dog.center.Override(Vector2.one * .5f);
    }

    /// <summary>
    /// 敵のトランスフォームを登録
    /// </summary>
    public void ApplyEnemyTransform(Transform enemy)
    {
        _enemies.Add(enemy);
    }

    /// <summary>
    /// 敵を取得する
    /// </summary>
    public List<Transform> GetEnemies()
    {
        return _enemies;
    }

    public void StartPostProDoG()
    {
        Debug.Log($"ガウス さっぶーーーーん");

        DOTween.To((_) => { _dog.elapsedTime.Override(_); },
            0f, 1f, .75f);
    }

    /// <summary>
    /// 一時停止を開始する
    /// </summary>
    public void StartPause()
    {
        EPause();
        
        _ingameUI.DisplayPausingPanel();
    }

    /// <summary>
    /// 一時停止を終了する
    /// </summary>
    public void StartResume()
    {
        EResume();
        
        _ingameUI.ClosePausingPanel();
    }

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        EDiveZone();

        StartPostProDoG();
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        EGetoutZone();

        DOTween.To((_) => { _dog.elapsedTime.Override(_); },
            1f, 0f, .75f);
    }

    public void NotifyBossIsDeath()
    {
        TaskOnBossDefeated();
    }

    public void Initialize()
    {
        // SceneLoader が Nullである場合には生成。
        if (GameObject.FindFirstObjectByType<SceneLoader>() is null)
        {
            var sl = Resources.Load<GameObject>("Prefabs/GameSystem/SceneLoader");

            var obj = GameObject.Instantiate(sl);
            _sceneLoader = obj.GetComponent<SceneLoader>();
        }

        if (GameObject.FindAnyObjectByType<Volume>() is not null)
        {
            _volume = GameObject.FindFirstObjectByType<Volume>();
            if (_volume.profile.TryGet(out _dog))
            {
                Debug.Log($"ガウス差分クラスないんだけど");
            }
        }
        else
        {
            Debug.Log($"ボリュームねぇんだけど");
        }
    }
}
