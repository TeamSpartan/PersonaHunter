using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// 指定されたカメラの内容をキャプチャするスクリーンショット
/// </summary>
public class CameraScreenShotCapturer : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    public void CaptureScreenShot()
    {
        ////ファイルの有無を確認
        ////同じファイル名があったら処理を行わない
        //if(File.Exists(filePath))
        //{
        //    Debug.Log("その名前のファイルは使われています");
        //    return;
        //}

        //描画範囲を指定
        var rt = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
        var prev = _camera.targetTexture;
        _camera.targetTexture = rt;
        //カメラをレンダリングする
        _camera.Render();
        _camera.targetTexture = prev;
        RenderTexture.active = rt;
        var screenShot = new Texture2D(
            _camera.pixelWidth,
            _camera.pixelHeight,
            TextureFormat.RGB24,
            false);
        //スクリーンショットからピクセルデータに変換
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
        //スクリーンショットを反映させる
        screenShot.Apply();
        //PNG形式にエンコード
        var bytes = screenShot.EncodeToPNG();
        //Destroy(screenShot);

        var path = System.DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".png";
        File.WriteAllBytes(Application.dataPath + "/" + path, bytes);
    }
}
