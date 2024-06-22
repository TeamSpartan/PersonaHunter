using System;
using UnityEngine;

// 各モジュール間のインターフェイスはここに集約

/// <summary>
/// 敵AI 本体のコンポーネントが継承すべきインターフェイス
/// </summary>
public interface IEnemiesParameter
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

    /// <summary>
    /// 左のロックオン対象を選択する時のイベント
    /// </summary>
    public Action EvtCamLeftTarget { get; set; }

    /// <summary>
    /// 右のロックオン対象を選択する時のイベント
    /// </summary>
    public Action EvtCamRightTarget { get; set; }
}

/// <summary>
/// 入力を受け付けるクラスが継承すべきインターフェース
/// </summary>
public interface IInputValueReferencable
{
    /// <summary>
    /// キャラ移動 の値を返す
    /// </summary>
    public Vector2 GetMoveValue();

    /// <summary>
    /// マウス入力 の値を返す
    /// </summary>
    public Vector2 GetCamMoveValue();
}

/// <summary>
/// プレイヤがガードをしている間に呼びだすメソッドが格納されている。GameLogicが継承すべきインターフェイス
/// </summary>
public interface IAbleToParry
{
    /// <summary>
    /// プレイヤがガード中かのコンディションを返す
    /// </summary>
    public bool NotifyPlayerIsGuarding();

    /// <summary>
    /// パリィ成功時に呼ぶ
    /// </summary>
    public void ParrySuccess();
}

/// <summary>
/// 敵を倒した時に呼びだすメソッドの実装を強制する。
/// </summary>
public interface IEnemyDieNotifiable
{
    public enum EnemyType
    {
        Komashira,
        Nue
    }
    
    /// <summary>
    /// BOSS撃破時に呼び出す。
    /// </summary>
    void NotifyEnemyIsDeath(EnemyType type, GameObject enemy);
}

/// <summary>
/// プレイヤのデータを格納しているクラスに継承してほしいインターフェース
/// </summary>
public interface IPlayerDataContainable
{
    /// <summary>
    /// プロローグを再生したかのフラグを返す
    /// </summary>
    /// <returns></returns>
    public bool PlayedPrologue { get; }

    /// <summary>
    /// プロローグを再生したことを通知する
    /// </summary>
    public void NotifyPlayedPrologue();

    /// <summary>
    /// データのセーブをする
    /// </summary>
    public void SaveData();

    /// <summary>
    /// データの読み込みをする
    /// </summary>
    public void LoadData();
}
