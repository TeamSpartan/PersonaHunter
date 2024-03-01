using UnityEngine;

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
    public void InitializeThisComp();

    /// <summary>
    /// リセットの時に呼び出される
    /// </summary>
    public void FinalizeThisComp();
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