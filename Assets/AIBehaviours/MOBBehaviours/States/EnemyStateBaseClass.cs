using UnityEngine;

namespace MOBState
{
    public abstract class EnemyStateBaseClass
    {
        /// <summary>
        /// 毎フレームのタスク
        /// </summary>
        public abstract void TaskOnUpdate(Transform position, Transform destination);
    }
}