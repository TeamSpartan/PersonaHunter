using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/*
参考資料：https://qiita.com/dj_kusuha/items/13a68474edfd78e41b82
          https://cg-method.com/unity-editor-extensions/#index_id4
*/

/// <summary>
/// エディタ拡張でUnityエディタ上からGameビューのスクリーンショットを撮る機能を追加
/// </summary>
public class CaptureScreenshotFromEditor : Editor
{
    /// <summary>
    /// ゲームビューのキャプチャを撮る
    /// </summary>
    /// <remarks>
    /// Edit > CaptureScreenshot に追加。
    /// HotKeyは Ctrl + Alt + F12。
    /// </remarks>
    [MenuItem("Editor/CaptureScreenshot %&F12")]
    private static void CaptureScrennShot()
    {
        //ダイアログで保存先を指定して、その場所に画像を保存する
        //SaveFilePanelの第一引数は、タイトル、第二引数は最初に表示するフォルダこの場合は、Asset/ 第三引数は、デフォルトのファイル名、第四引数は、拡張子
        string filePath = EditorUtility.SaveFilePanel("Save GameView", Application.dataPath, System.DateTime.Now.ToString("yyyyMMdd-HHmmss"), "png");
        //スクリーンショットを撮る
        ScreenCapture.CaptureScreenshot(filePath);
        //GameViewを取得してくる
        var assembly = typeof(EditorWindow).Assembly;
        //スクリーンショットする画面
        var type = assembly.GetType("UnityEditor.GameView");
        //画面上にあるゲームビューウィンドウを返す
        var gameview = EditorWindow.GetWindow(type);
        //GameViewを再描画
        gameview.Repaint();

        Debug.Log("ScreenShot: " + filePath);
    }
}
