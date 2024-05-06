using System.Collections.Generic;
using CriWare;
using Sound.PlayOption;
using UnityEngine;

namespace Sound
{
	public class ADXPlayer : MonoBehaviour
	{
		private static ADXPlayer _instance;
		static public ADXPlayer Instance => _instance;
		
		[SerializeField] private CriAtomSource _BGMAtomSource;
		[SerializeField] private CriAtomSource _MEAtomSource;
		[SerializeField] private CriAtomSource _SEAtomSource;
		[SerializeField] private CriAtomSource _VoiceAtomSouce;
		[SerializeField] private List<BGMSoundData> _bgmList = new();
		[SerializeField] private List<SESoundData> _seList = new();
		[SerializeField] private List<MESoundData> _meList = new();
		[SerializeField] private List<VoiceSoundData> _voiceList = new();
		private CriAtomExPlayer _exPlayer;
		private CriAtomExAcb _cueAcb;
		
		const string BGMSHEETNAME = "BGM";
		const string SESHEETNAME = "SE";
		const string MESHEETNAME = "ME";
		const string VoiceSHEETNAME = "Voice";

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			Initialize();
		}

		void Initialize()
		{
			_BGMAtomSource.player.ResetParameters();
			_MEAtomSource.player.ResetParameters();
			_SEAtomSource.player.ResetParameters();
			_exPlayer = new();
		}

		///<summary>BGMを流す用の関数</summary>
		public void PlayBGM(BGMSoundData.BGM bgm, params IAudioPlayOption[] options)
		{
			//キューシートの設定
			_cueAcb = CriAtom.GetAcb(BGMSHEETNAME);
			_BGMAtomSource.cueSheet = BGMSHEETNAME;

			//キューの取得
			BGMSoundData data = _bgmList.Find(data => data._BGM == bgm);
			
			//キューの有無
			if (!_cueAcb.Exists(data?.AudioClipName))
			{
				Debug.Log("Sound Not Found!");
				return;
			}

			//キューの設定
			_BGMAtomSource.cueName = data.AudioClipName;
			_BGMAtomSource.volume = data.Volume;
			_BGMAtomSource.pitch = data.Pitch;

			//オプションの設定
			foreach (var option in options)
			{
				option.ApplySetting(_BGMAtomSource);
			}

			//プレイ
			_BGMAtomSource.Play();
		}
		
		public void PlayBGM(BGMSoundData.BGM bgm, float volume = 1f, float pitch = 0f, bool isLoop = true)
		{
			//キューシートの設定
			_cueAcb = CriAtom.GetAcb(BGMSHEETNAME);

			//キューの取得
			BGMSoundData data = _bgmList.Find(data => data._BGM == bgm);
			
			//キューの有無
			if (!_cueAcb.Exists(data?.AudioClipName))
			{
				Debug.Log("Sound Not Found!");
				return;
			}
			_exPlayer.ResetParameters();

			//キューの設定
			_exPlayer.SetCue(_cueAcb, data.AudioClipName);
			_exPlayer.Loop(isLoop);
			_exPlayer.SetVolume(volume);
			_exPlayer.SetPitch(pitch);

			//プレイ
			_exPlayer.Start();
		}

		///<summary>SEを流す用の関数</summary>
		public void PlaySE(SESoundData.SE se, params IAudioPlayOption[] options)
		{
			//キューシートの設定
			_cueAcb = CriAtom.GetAcb(SESHEETNAME);
			_SEAtomSource.cueSheet = SESHEETNAME;

			//キューの取得
			SESoundData data = _seList.Find(data => data._SE == se);
			
			//キューの有無
			if (!_cueAcb.Exists(data?.AudioClipName))
			{
				Debug.Log("Sound Not Found!");
				return;
			}
			
			//キューの設定
			_SEAtomSource.cueName = data.AudioClipName;
			_SEAtomSource.volume = data.Volume;
			_SEAtomSource.pitch = data.Pitch;

			//オプションの設定
			foreach (var option in options)
			{
				option.ApplySetting(_SEAtomSource);
			}

			//プレイ
			_SEAtomSource.Play();
		}

		///<summary>3D減衰用の関数</summary>
		/// <summary>CriAtomSourceをアタッチして使って下さい</summary>
		public void PlaySE(CriAtomSource _source, SESoundData.SE se, params IAudioPlayOption[] options)
		{
			//キューシートの設定
			_cueAcb = CriAtom.GetAcb(SESHEETNAME);
			_source.cueSheet = SESHEETNAME;

			//キューの取得
			SESoundData data = _seList.Find(data => data._SE == se);
			
			//キューの有無
			if (!_cueAcb.Exists(data?.AudioClipName))
			{
				Debug.Log("Sound Not Found!");
				return;
			}
			
			//キューの設定
			_source.cueName = data.AudioClipName;
			_source.volume = data.Volume;
			_source.pitch = data.Pitch;

			//オプションの設定
			foreach (var option in options)
			{
				option.ApplySetting(_source);
			}

			//プレイ
			_source.Play();
		}

		
		///<summary>MEを流す用の関数</summary>
		public void PlayME(MESoundData.ME me, params IAudioPlayOption[] options)
		{
			//キューシートの設定
			_cueAcb = CriAtom.GetAcb(MESHEETNAME);
			_MEAtomSource.cueSheet = MESHEETNAME;

			//キューの取得
			MESoundData data = _meList.Find(data => data._ME == me);
			
			//キューの有無
			if (!_cueAcb.Exists(data.AudioClipName))
			{
				Debug.Log("Sound Not Found!");
				return;
			}
			
			//キューの設定
			_MEAtomSource.cueName = data.AudioClipName;
			_MEAtomSource.volume = data.Volume;
			_MEAtomSource.pitch = data.Pitch;

			//オプションの設定
			foreach (var option in options)
			{
				option.ApplySetting(_MEAtomSource);
			}

			//プレイ
			_MEAtomSource.Play();
		}

		///<summary>Voiceを流す用の関数</summary>
		public void PlayVoice(VoiceSoundData.Voice voice, params IAudioPlayOption[] options)
		{
			//キューシートの設定
			_cueAcb = CriAtom.GetAcb(VoiceSHEETNAME);
			_VoiceAtomSouce.cueSheet = VoiceSHEETNAME;

			//キューの取得
			VoiceSoundData data = _voiceList.Find(data => data._Voice == voice);
			
			//キューの有無
			if (!_cueAcb.Exists(data.AudioClipName))
			{
				Debug.Log("Sound Not Found!");
				return;
			}
			
			//キューの設定
			_VoiceAtomSouce.cueName = data.AudioClipName;
			_VoiceAtomSouce.volume = data.Volume;
			_VoiceAtomSouce.pitch = data.Pitch;

			//オプションの設定
			foreach (var option in options)
			{
				option.ApplySetting(_VoiceAtomSouce);
			}

			//プレイ
			_VoiceAtomSouce.Play();
		}
		
		void OnDestroy()
		{
			CriAtom.RemoveCueSheet(BGMSHEETNAME);
			CriAtom.RemoveCueSheet(SESHEETNAME);
			CriAtom.RemoveCueSheet(MESHEETNAME);
			CriAtom.RemoveCueSheet(VoiceSHEETNAME);
			_exPlayer.Dispose();
		}
	}
	
	
	[System.Serializable]
	public class BGMSoundData
	{
		public enum BGM
		{
			Title,
			Prologue,
			Field,
			InBattle,
			InZone,
			Shrine,
			BossBattle,
			Ending
		}

		public BGM _BGM;
		public string AudioClipName;
		[Range(0, 1)] public float Volume = 1;
		[Range(-120, 120)] public float Pitch = 0;
	}


	[System.Serializable]
	public class SESoundData
	{
		public enum SE
		{
			Decision,
			StartButton,
			
			//プレイヤー
			PlayerDirtWalk,
			PlayerGravelWalk,
			PlayerGrassWalk,
			PlayerDirtRun,
			PlayerGravelRun,
			PlayerGrassRun,
			PlayerAttackRight,
			PlayerAttackLeft,
			PlayerBigAttack,
			PlayerAvoid,
			PlayerParry,
			PlayerDamage,
			PlayerDIe,
			
			//エネミー
			EnemyDirtWalk,
			EnemyGraveWalk,
			EnemyGrassWalk,
			EnemyDirtRun,
			EnemyGraveRun,
			EnemyGrassRun,
			EnemyAttack,
			EnemyBigAttack,
			EnemyDamage,
			EnemyInterval,
			EnemyDie,
			
			//ボス
			BossDirtWalk,
			BossGraveWalk,
			BossGrassWalk,
			BossDirtRun,
			BossGraveRun,
			BossGrassRun,
			BossScratchAttack,
			BossTailAttack,
			BossRushAttack,
			BossParry,
			BossParryReceived,
			BossDown,
			BossDamage,
			BossDie,
			
			//環境音
			FieldWind,
			FieldSparrow,
			FieldWarbler
		}

		public SE _SE;
		public string AudioClipName;
		[Range(0, 1)] public float Volume = 1;
		[Range(-120, 120)] public float Pitch = 0;
	}
	
	[System.Serializable]
	public class MESoundData
	{
		public enum ME
		{
			InZone,
			GameOver
		}

		public ME _ME;
		public string AudioClipName;
		[Range(0, 1)] public float Volume = 1;
		[Range(-120, 120)] public float Pitch = 0;
	}


	[System.Serializable]
	public class VoiceSoundData
	{
		public enum Voice
		{
			//プレイヤー
			PlayerRightAttack,
			PlayerLeftAttack,
			PlayerBigAttack,
			PlayerAvoid,
			PlayerDamage,
			PlayerBigDamage,
			PlayerDie,
			
			//エネミー
			EnemyAttack,
			EnemyBigAttack,
			EnemyDamage,
			EnemyBigDamage,
			EnemyDie,
			
			//ボス
			BossScratchAttack,
			BossTailAttack,
			BossRushAttack,
			BossBigAttack,
			BossDamage,
			BossBigDamage,
			BossDie,
		}

		public Voice _Voice;
		public string AudioClipName;
		[Range(0, 1)] public float Volume = 1;
		[Range(-120, 120)] public float Pitch = 0;
	}

	
}
