// 管理者 菅沼
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SgLibUnite
{
    enum LoggingMode
    {
        Normal,
        Warning,
        Error,
    }
    /// <summary> 渡された文字列をただDebugログへ出力する。 </summary>
    public class DummyLogger : MonoBehaviour
    {
        [SerializeField] LoggingMode mode;
        public void DummyLoggerOutputLog(string message)
        {
            switch (mode)
            {
                case LoggingMode.Warning:
                    Debug.LogWarning(message);
                    break;
                case LoggingMode.Error:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }
    }
}