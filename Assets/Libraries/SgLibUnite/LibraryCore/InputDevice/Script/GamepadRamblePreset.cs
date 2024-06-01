using SgLibUnite.Devicies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 作成 菅沼
namespace SgLibUnite
{
    namespace Devicies
    {
        [CreateAssetMenu(fileName = "PadRamblingPreset", menuName = "GamepadRamblingPreset", order = 1)]
        public class GamepadRamblePreset : ScriptableObject
        {
            /// <summary> RumbleInfo 左から 左の振動 右の振動 振動の時間 </summary>
            [SerializeField] List<RumbleInfo<float, float, float>> rumbleTable;
            public List<RumbleInfo<float, float, float>> Rumbles { get { return rumbleTable; } }
        }
    }
}