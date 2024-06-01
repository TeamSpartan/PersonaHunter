using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

#region 3DTransform To 2DScreenPosition

/// <summary> 与えられた３次元の方向から２次元の方向の正規化したベクトルを返す </summary>
/// <param name="direction"></param>
/// <returns></returns>
//Vector2 Calculate3DPosTo2DViewPos(Vector3 target, Vector3 origin, Vector3 cameraForward, Vector3 cameraRight)
//{
/// 考え方
/// 3d での目標とプレイヤーのベクトルをD
/// 3d でのプレイヤーの正面ベクトルをF
/// 2d での正面ベクトルF1は(0,1)
/// F から何度回転した方向にDがあるか知りたいので F ・ D を求める
/// これを2d 上で再現すればよいかも

#region Cos_

//// the vector player -> target
//Vector3 t = (target - origin).normalized, f = cameraForward.normalized;
//// calculate dotFT-product
//var dotP = Vector3.Dot(f, t);
//dotP = Mathf.Clamp(dotP, -1.0f, 1.0f);
//// calculate euler angle
//var angle = Mathf.Acos(dotP) * Mathf.Rad2Deg;
//// calculate vector rotated
//var vec = cameraForward;
//var result = Quaternion.Euler(0, angle, 0) * vec;
//var rvec = new Vector2(result.z, result.x);
//// get resolutions 
//var res = _canvasS.referenceResolution;
//rvec.Normalize();
//var posX = rvec.x * (res.x / 2.0f) + (res.x / 2.0f);
//var posY = rvec.y * (res.y / 2.0f) + (res.y / 2.0f);
//return new Vector2(posX, posY);
//Debug.Log($"vector res : {result.ToString()}");

#endregion

#region Atan2

// XZ平面 → forward-right平面
// calculate angle
//Vector3 forward = target - cameraForward;
//Vector3 right = target - cameraRight;
//forward.Normalize();
//right.Normalize();
//float deg = Mathf.Atan2(forward.magnitude, right.magnitude);
//Vector3 dir = target - cameraForward;
//dir.Normalize();

//float deg = Mathf.Atan2(dir.z, dir.x);
//// calculate vector rotated
//var vec = cameraForward;
//var result = Quaternion.Euler(0, (deg < 0) ? deg + 360.0f : deg, 0) * vec;
//var rvec = new Vector2(result.x, result.z);
//// get resolutions 
//var res = _canvasS.referenceResolution;
//rvec.Normalize();
//var posX = rvec.x * (res.x / 2.0f) + (res.x / 2.0f);
//var posY = rvec.y * (res.y / 2.0f) + (res.y / 2.0f);
//return new Vector2(posX, posY);
//Debug.Log($"vector res : {result.ToString()}");

#endregion

#region DotPAcos-Atan2

//Vector3 f = cameraForward;
//Vector3 r = cameraRight;
//Vector3 t = target - origin;

//// calculate Dot
//float dotFT = Vector3.Dot(f, t);
//dotFT = Mathf.Clamp(dotFT, -1.0f, 1.0f);
//// calculate euler angle Acos
//var angleAcos = Mathf.Acos(dotFT) * Mathf.Rad2Deg;
//Vector3 resVec1 = Quaternion.Euler(0, angleAcos, 0) * cameraForward;

//// calculate angle Atan2
//// 射影ベクトルを求める Atanのx
//Vector3 pjT = r.normalized * (t.magnitude * Mathf.Cos((90.0f - angleAcos)));
//// 高さのヘクトル Atanのy
//Vector3 h = t.normalized * Mathf.Sin((90.0f - angleAcos));
//float atan2 = Mathf.Atan2(h.magnitude, pjT.magnitude);
//Vector3 resVec2 = Quaternion.Euler(0, atan2, 0) * cameraForward;

//var result = resVec1 + resVec2;

//var rvec = new Vector2(result.x, result.z);
//// get resolutions 
//var res = _canvasS.referenceResolution;
//rvec.Normalize();
//// set screen position
//var posX = rvec.x * (res.x / 2.0f) + (res.x / 2.0f);
//var posY = rvec.y * (res.y / 2.0f) + (res.y / 2.0f);
//// return result

//Debug.Log($"Acos = {angleAcos}, Atan2 = {atan2}");

//return new Vector2(posX, posY);

#endregion

//}

#endregion

namespace SgLibUnite.UI
{
    /// <summary> マップ内に目標を強調表示 </summary>
    public class ObjectiveMapIndicator : MonoBehaviour
    {
        [SerializeField, Header("Indicator Sprite Image")]
        Image ImageIcon;

        [SerializeField, Header("Player Tag")] string PlayerTag;
        [SerializeField, Header("Camera Tag")] string CameraTag;

        [SerializeField, Header("Objective Tag")]
        string ObjTag;

        Transform _target;

        public Transform Target
        {
            get { return _target; }
        }

        Camera _mainCam;
        RectTransform _rect;

        public void SetTarget(Transform target)
        {
            _target = target;
            ImageIcon.color = (_target == null) ? Color.clear : Color.white;

            if (GameObject.FindGameObjectWithTag(CameraTag).GetComponent<Camera>() != null)
            {
                _mainCam = GameObject.FindGameObjectWithTag(CameraTag).GetComponent<Camera>();
            }
            else
            {
                _mainCam = GameObject.FindGameObjectWithTag(CameraTag).GetComponentInChildren<Camera>();
            }
        }

        private void Start()
        {
            if (GameObject.FindGameObjectWithTag(CameraTag).GetComponent<Camera>() != null)
            {
                _mainCam = GameObject.FindGameObjectWithTag(CameraTag).GetComponent<Camera>();
            }
            else
            {
                _mainCam = GameObject.FindGameObjectWithTag(CameraTag).GetComponentInChildren<Camera>();
            }

            _rect = ImageIcon.gameObject.GetComponent<RectTransform>();
            ImageIcon.color = (_target == null) ? Color.clear : Color.white;
        }

        private void Update()
        {
            if (GameObject.FindAnyObjectByType<GameInfo>().GetSceneStatus !=
                GameInfo.SceneTransitStatus.WentToInGameScene)
            {
                return;
            }

            _rect = ImageIcon.gameObject.GetComponent<RectTransform>();

            float canvasScale = transform.root.localScale.z;
            var center = 0.5f * new Vector3(Screen.width, Screen.height);

            var pos = _mainCam.WorldToScreenPoint(_target.position) - center;
            if (pos.z < 0f)
            {
                pos.x = -pos.x;
                pos.y = -pos.y;

                if (Mathf.Approximately(pos.y, 0f))
                {
                    pos.y = -center.y;
                }
            }

            var halfSize = 0.5f * canvasScale * _rect.sizeDelta;
            float d = Mathf.Max(
                Mathf.Abs(pos.x / (center.x - halfSize.x)),
                Mathf.Abs(pos.y / (center.y - halfSize.y))
            );


            bool isOffscreen = (pos.z < 0f || d > 1f);
            if (isOffscreen)
            {
                pos.x /= d;
                pos.y /= d;
            }

            _rect.anchoredPosition = pos / canvasScale;
        }
    }
}
