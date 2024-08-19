using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SgLibUnite.Devicies;
// 作成 ： 菅沼
namespace SgLibUnite
{
    namespace Devicies
    {
        /// <summary> ゲームパッドの振動をさせる機能を提供する </summary>
        public class GamepadRumbler : MonoBehaviour
        {
            Gamepad _gamepad;
            /// <summary> 振動プリセット 左から 左の振動 右の振動 振動の時間 </summary>
            [SerializeField] GamepadRamblePreset rumblePreset;

            private void Start()
            {
                _gamepad = Gamepad.current;
            }

            public void Rumble(float leftSpeed, float rightSpeed)
            {
                _gamepad.SetMotorSpeeds(leftSpeed, rightSpeed);
            }

            public IEnumerator RumbleRoutine(float leftSpeed, float rightSpeed, float rumbleTime)
            {
                _gamepad.SetMotorSpeeds(leftSpeed, rightSpeed);

                yield return new WaitForSeconds(rumbleTime);

                _gamepad.SetMotorSpeeds(0f, 0f);
            }

            public IEnumerator RumbleRoutine(float leftSpeed, float rightSpeed, float rumbleTime, int repeatTimes)
            {
                for (int i = 0; i < repeatTimes; i++)
                {
                    _gamepad.SetMotorSpeeds(leftSpeed, rightSpeed);

                    yield return new WaitForSeconds(rumbleTime);

                    _gamepad.SetMotorSpeeds(0f, 0f);
                }
            }

            public IEnumerator RumbleByTable()
            {
                foreach (var table in rumblePreset.Rumbles)
                {
                    _gamepad.SetMotorSpeeds(table.LeftStrength, table.RightStrength);

                    yield return new WaitForSeconds(table.Time);

                    _gamepad.SetMotorSpeeds(0f, 0f);
                }
            }
        }

        [Serializable]
        public class RumbleInfo<T, T1, T2>
        {
            [SerializeField] T _lowFreqL;
            public T LeftStrength { get { return _lowFreqL; } }
            [SerializeField] T1 _highFreqR;
            public T1 RightStrength { get { return _highFreqR; } }
            [SerializeField] T2 _rumblingTime;
            public T2 Time { get { return _rumblingTime; } }

            public RumbleInfo(T leftStrength, T1 rightStrength, T2 rumbleTime)
            {
                _lowFreqL = leftStrength;
                _highFreqR = rightStrength;
                _rumblingTime = rumbleTime;
            }
        }
    }
}