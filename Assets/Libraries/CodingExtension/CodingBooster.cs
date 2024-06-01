using System;
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
            var list = GameObject.FindObjectsOfType<GameObject>()
                .Where(_ => _.GetComponent<T>() != null)
                .Select(_ => _.GetComponent<T>()).ToList();
            return list;
        }

        /// <summary>
        /// 子オブジェクト親オブジェクトかそのオブジェクトに望むコンポーネントがあればそれを返す
        /// </summary>
        public T GetComponentInHierarchie<T>(GameObject target) where T : Component
        {
            if (target.GetComponent<T>() != null)
            {
                return target.GetComponent<T>();
            }
            else if(target.GetComponentInChildren<T>() != null)
            {
                return target.GetComponentInChildren<T>();
            }
            else if(target.GetComponentInParent<T>() != null)
            {
                return target.GetComponentInParent<T>();
            }
            else
            {
                throw new Exception("Component Does Not Found");
            }

            return null;
        }
    }
}
