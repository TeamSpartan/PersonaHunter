using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

#region 参考文献
// https://qiita.com/sune2/items/cf9ef9d197b47b2d7a10
// https://www.nttpc.co.jp/technology/number_algorithm.html
#endregion

namespace SgLibUnite.UI
{
    /// <summary> 
    /// <para> ボタンのような機能を提供する。 多角形に対応</para>
    /// This Component Works As Button
    /// </summary>
    public class ClicableObject : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasRaycastFilter
    {
        [SerializeField]
        UnityEvent OnClick;

        Image _image;

        List<Vector2> _verts = new();

        void Press()
        {
            if (OnClick != null)
                OnClick.Invoke();
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            var img = _image;
            if (img == null) { _image = GetComponent<Image>(); }
            Vector2 local;
            var rectT = (RectTransform)transform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectT, sp, eventCamera, out local))
            {
                // 画面内のRectTransformの平面にrayがヒットすればtrue ここではヒットしなかった場合
                return false;
            }
            var rect = rectT.rect;
            // スプライト内に当たった場合、その座標を求める
            var pivot = rectT.pivot;
            var sprite = _image.sprite;
            var x = (local.x / rect.width + pivot.x - .5f) * sprite.rect.width / sprite.pixelsPerUnit;
            var y = (local.y / rect.height + pivot.y - .5f) * sprite.rect.height / sprite.pixelsPerUnit;
            var p = new Vector2(x, y);
            // 内外判定
            var physicShapeCnt = sprite.GetPhysicsShapeCount();
            for (int i = 0; i < physicShapeCnt; i++)
            {
                sprite.GetPhysicsShape(i, _verts);
                if (IsInside(_verts, p))
                {
                    return true;
                }
            }
            return false;
        }
        float Cross2D(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        // 頂点と点ｐを渡して内外判定をする
        bool IsInside(List<Vector2> verts, Vector2 point)
        {
            #region だめだったアルゴリズム
#if false
            int cnt = 0;
            for (int i = 0; i < verts.Count - 1; i++)
            {
                // 上向きの辺 ー 点ｐがｙ軸方向について、始点と終点の間にあり終点を含んでいない
                if (((verts[i].y <= point.y) && (verts[i + 1].y > point.y))
                    // 下向きの辺 ー 点ｐがｙ軸方向について、始点と終点の間にあり、始点を含んでいない
                    || (verts[i].y > point.y) && (verts[i + 1].y <= point.y))
                {
                    var vt = (point.y - verts[i].y) / (verts[i + 1].y - verts[i].y);
                    // 辺は点ｐよりも右側にあるが重ならない
                    // 辺が点ｐと同じ高さになる位置を特定、その時のｘの値と点ｐのｘの値を比較
                    // もし小さい場合（左側にある）
                    if (point.x < (verts[i].x + (vt * verts[i + 1].x - verts[i].x)))
                    {
                        ++cnt;
                    }
                }
            }
            return !(cnt % 2 == 0); // 交差回数が偶数の場合には内部にはない
#endif
            #endregion
            var n = verts.Count;
            var isInside = false;
            for (int i = 0; i < n; i++)
            {
                var nxt = i + 1;
                if (nxt >= n) nxt = 0;

                var A = verts[i] - point;
                var B = verts[nxt] - point;

                if (A.y > B.y)
                    (A.y, B.y) = (B.y, A.y);    // swap

                if (A.y <= 0 && 0 < B.y && Cross2D(A, B) > 0)
                    isInside = !isInside;
            }
            return isInside;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (PointerEventData.InputButton.Left == eventData.button)
            {
                Debug.Log("Click");
                Press();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Debug.Log("Submit");
        }
    }
}
