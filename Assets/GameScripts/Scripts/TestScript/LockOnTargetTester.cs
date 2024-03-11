using UnityEngine;

namespace GameScripts.Scripts.TestScript
{
    public class LockOnTargetTester
    : MonoBehaviour
    ,IPlayerCamLockable
    {
        public Transform GetLockableObjectTransform()
        {
            return this.transform;
        }
    }
}
