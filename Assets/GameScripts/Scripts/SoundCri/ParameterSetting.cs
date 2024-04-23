using CriWare;
using UnityEngine;

namespace Sound.PlayOption
{
	///<summary>サウンドのパラメーター</summary>
	public class ParameterSetting : MonoBehaviour
	{
		public class VolumeParam : IAudioPlayOption
		{
			private float _volume;

			public VolumeParam(float volume)
			{
				_volume = volume;
			}

			public CriAtomSource ApplySetting(CriAtomSource target)
			{
				target.volume = _volume;
				return target;
			}
		}

		/// <summary>ピッチの調整</summary>
		public class PitchParam : IAudioPlayOption
		{
			private float _pitch;

			public PitchParam(float pitch)
			{
				_pitch = pitch;
			}

			public CriAtomSource ApplySetting(CriAtomSource target)
			{
				target.pitch = _pitch;
				return target;
			}
		}
		
		public class LoopSet : IAudioPlayOption
		{
			private bool _isLoop = false;

			public LoopSet(bool value)
			{
				_isLoop = value;
			}
			public CriAtomSource ApplySetting(CriAtomSource target)
			{
				target.loop = _isLoop;
				return target;
			}
		}

		/// <summary>エンジン音のように徐々に音が大きくなっていく感じ？？</summary>
		//https://game.criware.jp/manual/unity_plugin/latest/contents/atom4u_keys_aisac.html
		public class AisacParam : IAudioPlayOption
		{
			private uint _aisacTargetId;
			private string _aisacTargetName;
			private float _aisacControlvalue;
			private bool _isSearchById;

			public AisacParam(uint aisacTargetId, float aisacControlvalue)
			{
				_aisacTargetId = aisacTargetId;
				_aisacControlvalue = Mathf.Clamp01(aisacControlvalue);
				_isSearchById = true;
			}

			public AisacParam(string targetName, float aisacControlvalue)
			{
				_aisacTargetName = targetName;
				_aisacControlvalue = Mathf.Clamp01(aisacControlvalue);
				_isSearchById = false;
			}

			public CriAtomSource ApplySetting(CriAtomSource target)
			{
				if (_isSearchById)
					target.SetAisacControl(_aisacTargetId, _aisacControlvalue);
				else
					target.SetAisacControl(_aisacTargetName, _aisacControlvalue);
				return target;
			}
		}
	}
}
