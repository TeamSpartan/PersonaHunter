using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 担当 菅沼
// ディスプレイのデバイス名とリフレッシュレートの変更はできたっぽい ← ここまでは動作確認できている
// アクティブなディスプレイ切り替え機能 実装 OK ← 動作確認 OK
// Player設定 ＞ FullScreen モード 、 Default Is Native Resolution = false この設定は必ずすること

namespace SgLibUnite
{
    namespace UI
    {
        public class DisplaySettingsManager : MonoBehaviour
        {
            [SerializeField] Dropdown displaysDD;
            [SerializeField] Dropdown resolutionsDD;
            [SerializeField] Dropdown refreshRateDD;

            int _resolutionIndex;
            public int ResolutionIndex => _resolutionIndex;
            int _refreshRateIndex;
            public int RefreshRateIndex => _refreshRateIndex;
            int _displayIndex;
            public int DisplayIndex => _displayIndex;

            private DisplaySettingDataContainer dispDatas;
            
            #region ScriptFunctions

            /// <summary> DropDownOnValueChanged </summary>
            /// <param name="dropdown"></param>
            /// <returns></returns>
            int DDOnValueChanged(Dropdown dropdown)
            {
                return dropdown.value;
            }

            public string GetResolution()
            {
                return $"{Screen.currentResolution.ToString()}";
            }

            public string GetDisplayName(int index)
            {
                List<DisplayInfo> list = new List<DisplayInfo>();
                Screen.GetDisplayLayout(list);
                return list[index].name;
            }

            public List<DisplayInfo> GetDisplays()
            {
                List<DisplayInfo> list = new List<DisplayInfo>();
                Screen.GetDisplayLayout(list);
                return list;
            }

            /// <summary> 空白区切りでピクセル数の指定をする </summary>
            /// <param name="resolution"></param>
            public void SetDisplayResolutions(string resolution)
            {
                int width;
                int height;
                width = int.Parse(resolution.Split()[0]);
                height = int.Parse(resolution.Split()[1]);

                Screen.SetResolution(width, height, true);
            }
            public void SetDisplayResolutions(Tuple<int, int> resolution)
            {
                int width = resolution.Item1;
                int height = resolution.Item2;
                
                Screen.SetResolution(width, height, true);
            }

            public int GetRefreshRate()
            {
                return Application.targetFrameRate;
            }

            public void SetRefreshRate(int rate)
            {
                Application.targetFrameRate = rate;
            }

            #endregion

            public void ChangeGameDisplay(Dropdown dropdown)
            {
                int displayIndex = DDOnValueChanged(dropdown);
                var displays = GetDisplays();
                var display = displays[displayIndex];

                _displayIndex = displayIndex;
                Screen.MoveMainWindowTo(display, display.workArea.position);
            }

            public void ChangeGameResolutions(Dropdown dropdown)
            {
                int resolutionIndex = DDOnValueChanged(dropdown);
                var resolutionRaw = dispDatas.GetResolutionList.Values.ToList();
                var resolution = resolutionRaw[resolutionIndex];

                _resolutionIndex = resolutionIndex;
                SetDisplayResolutions(resolution);
            }

            public void ChangeGameRefreshRate(Dropdown dropdown)
            {
                int rateIndex = DDOnValueChanged(dropdown);
                var rateRaw = dispDatas.GetRefreshRateList;
                var rate = rateRaw[rateIndex];

                _refreshRateIndex = rateIndex;
                SetRefreshRate(rate);
            }

            void SetupActiveDisplaysDropdown() // アクティブなディスプレイをドロップダウンへ名前のみ渡す
            {
                displaysDD.options.Clear();
                var displays = GetDisplays();
                List<Dropdown.OptionData> optionData = new();
                foreach (var display in displays)
                {
                    var data = new Dropdown.OptionData();
                    data.text = display.name;
                    optionData.Add(data);
                }

                displaysDD.options = optionData;
            }

            void SetupResolutionsDropDown()
            {
                resolutionsDD.options.Clear();
                List<Dropdown.OptionData> optionData = new();
                var resolutions = dispDatas.GetResolutionList.Keys.ToList();
                foreach (var resolution in resolutions)
                {
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = resolution;
                    optionData.Add(data);
                }

                resolutionsDD.options = optionData;
            }

            void SetupRefreshRateDropDown()
            {
                refreshRateDD.options.Clear();
                List<Dropdown.OptionData> optionData = new();
                var rates = dispDatas.GetRefreshRateList;
                foreach (var rate in rates)
                {
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = rate.ToString();
                    optionData.Add(data);
                }

                refreshRateDD.options = optionData;
            }

            void Setup()
            {
                SetupActiveDisplaysDropdown();
                SetupResolutionsDropDown();
                SetupRefreshRateDropDown();
            }

            private void Start()
            {
                Setup();
            }
        }
    }
}