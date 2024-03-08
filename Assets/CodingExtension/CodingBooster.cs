using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SgLibUnite.CodingBooster
{
    public class CBooster
    {
        /// <summary>
        /// 型パラメータに対応するクラスを継承するコンポーネントを返す
        /// </summary>
        public List<T> GetDerivedComponents<T>()
        {
            var obj = GameObject.FindObjectsOfType<GameObject>()
                .Where(_ => _.GetComponent<T>() != null)
                .Select(_ => _.GetComponent<T>()).ToList();
            return obj;
        }
    }
}
