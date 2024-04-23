using System;
using UnityEngine;
using UnityEngine.AI;

// 各モジュール間のインターフェイスはここに集約

/// <summary>
/// ゲームシーンに配置されるオブジェクトにアタッチされるユーザー定義コンポーネント
/// はこれを継承すべき
/// </summary>
public interface IInitializableComponent
{
    /// <summary>
    /// 起動の時に呼び出される
    /// </summary>
    public void InitializeThisComponent();

    /// <summary>
    /// リセットの時に呼び出される
    /// </summary>
    public void FinalizeThisComponent();
}

/// <summary>
/// セーブデータに必要なプレイヤのコンポーネントが継承すべきインターフェイス
/// </summary>
public interface ISavablePlayerInfo
{
    /// <summary>
    /// プレイヤーのトランスフォーム情報を設定
    /// </summary>
    public void SetPlayerTransform(Transform transform);

    /// <summary>
    /// プレイヤーのトランスフォーム情報を返す 
    /// </summary>
    public Transform GetPlayerTransform(); // データセーバから呼ばれる
}

/// <summary>
/// 集中モードの時に鈍化するオブジェクトが継承すべきインターフェイス
/// </summary>
public interface IDulledTarget
{
    /// <summary>
    /// 集中モード開始時に呼ばれる
    /// </summary>
    public void StartDull();

    /// <summary>
    /// 集中モード終了時に呼ばれる
    /// </summary>
    public void EndDull();
}

/// <summary>
/// 敵AI 本体のコンポーネントが継承すべきインターフェイス
/// </summary>
public interface IMobBehaviourParameter
{
    /// <summary>
    /// 体力値を返す
    /// </summary>
    public float GetHealth();

    /// <summary>
    /// 体力値を初期化する
    /// </summary>
    public void SetHealth(float val);
}

/// <summary>
/// 敵AIのステートが継承すべきインターフェイス
/// </summary>
public interface IEnemyState
{
    /// <summary>
    /// ステートを更新。これがないと毎フレーム処理が期待した通りに動作しない。
    /// </summary>
    public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent);
}

/// <summary>
/// ダメージを加えられるコンポーネントが継承すべき
/// </summary>
public interface IDamagedComponent
{
    /// <summary>
    /// ダメージを加える
    /// </summary>
    public void AddDamage(float dmg);

    /// <summary>
    /// 即死させる
    /// </summary>
    public void Kill();
}

/// <summary>
/// プレイヤーカメラのロック対象が継承すべきインタフェイス
/// </summary>
public interface IPlayerCamLockable
{
    /// <summary>
    /// 自分のトランスフォームを返す
    /// </summary>
    public Transform GetLockableObjectTransform();
}

/// <summary>
/// プレイヤカメラに追跡されることのできるコンポーネントが継承すべきインタフェイス
/// </summary>
public interface IPlayerCameraTrasable
{
    /// <summary>
    /// 自分のトランスフォームを返す
    /// </summary>
    public Transform GetPlayerCamTrasableTransform();
}

/// <summary>
/// ロックオン入力がされたときにイベント発火処理をするコンポーネントが継承すべきインタフェイス
/// </summary>
public interface ILockOnEventFirable
{
    /// <summary>
    /// ロックオン入力があったとき
    /// </summary>
    public event Action ELockOnTriggered;
}

/// <summary>
/// 入力を受け付けるクラスが継承すべきインターフェース
/// </summary>
public interface IInputValueReferencable
{
    /// <summary>
    /// キャラ移動 水平 の値を返す
    /// </summary>
    public float GetHorizontalMoveValue();
    
    /// <summary>
    /// キャラ移動 垂直 の値を返す
    /// </summary>
    public float GetVerticalMoveValue();
    
    /// <summary>
    /// マウス入力 水平 の値を返す
    /// </summary>
    public float GetHorizontalMouseMoveValue();
    
    /// <summary>
    /// マウス入力 垂直 の値を返す
    /// </summary>
    public float GetVerticalMouseMoveValue();
}

/// <summary>
/// 敵クラスが継承すべきインターフェイス
/// </summary>
public interface IEnemy
{
    public void NotifyEnemyHasStartedAttack(GameObject instance, bool condition);
}

/// <summary>
/// プレイヤがガードをしている間に呼びだすメソッドが格納されている。GameLogicが継承すべきインターフェイス
/// </summary>
public interface IGuardEventHandler
{
    public void NotifyPlayerIsGuarding(GameObject enemy);
    public Action EParrySucceed { get; set; }
}
