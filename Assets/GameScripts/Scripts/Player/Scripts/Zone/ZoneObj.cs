using System;
using System.Collections.Generic;
using Player.Action;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace Player.Zone
{
	/// <summary>SlowTime用のゲージ</summary>
	public class ZoneObj : MonoBehaviour
	{
		[SerializeField] private GameObject zoneObj;

		[SerializeField, Header("アニメーションスピード")]
		private float duration = 5f;

		[SerializeField]
		private int _maxZoneObj = 5;
		
		private float _animationTime = 0.3f;

		private bool _canIncreaseGaugeValue = true; // ゲージのValueを増加できるかのフラグ

		private float _currentZoneGaugeValue;

		public int ActiveZoneObjCount => _splineAnimates.Count;
			
		private ZoneTimeController _zoneTimeController;
		private SplineContainer _spline;
		private PlayerAvoid _playerAvoid;
		private List<SplineAnimate> _splineAnimates = new();
		private List<SplineAnimate> _evacuationZoneObj = new();
		

		/// <summary>ゲージのValueを増加できるかのフラグを変更します</summary>
		public void SetCanIncreaseGaugeValue(bool flag) => _canIncreaseGaugeValue = flag;

		private void Start()
		{
			_zoneTimeController = GetComponent<ZoneTimeController>();
			_spline = GetComponent<SplineContainer>();
			InitializedGauge();
		}

		private void OnEnable()
		{
			_playerAvoid = GetComponentInParent<PlayerAvoid>();
			_playerAvoid.OnJustAvoidSuccess += IncreaseGaugeValue;
		}

		private void OnDisable()
		{
			_playerAvoid.OnJustAvoidSuccess -= IncreaseGaugeValue;
		}

		/// <summary>呼ばれたときにゲージのValueを増加させます</summary>
		public void IncreaseGaugeValue(float value)
		{
			Debug.Log($"ゾーン ふやしたお");
			if (!_canIncreaseGaugeValue)
			{
				Debug.Log($"ゾーン？ 増やせねぇぞばーかばーか");
				return;
			}

			_currentZoneGaugeValue += value;
			if (_currentZoneGaugeValue >= _maxZoneObj)
				_currentZoneGaugeValue = _maxZoneObj;
			ZoneIncreaseUpdate(_currentZoneGaugeValue);
		}

		void InitializedGauge()
		{
			_currentZoneGaugeValue = 0;

			NotEnoughObj(Mathf.FloorToInt(_maxZoneObj));
		}

		public void StartDull()
		{
			SetCanIncreaseGaugeValue(false);
			// ゲージの最大値を制限時間の最大値と同じにする
		}

		public void EndDull()
		{
			// ゲージの最大値を元に戻す
			SetCanIncreaseGaugeValue(true);
			Debug.Log($"ゾーン おーわり");
		}

		///<summary>ZoneGaugeが減った時に更新</summary>
		public void ZoneDecreaseUpdate(float value)
		{
			if (_splineAnimates.Count < 1) return;

			// for (int i = Mathf.FloorToInt(value); i < _splineAnimates.Count; i++)
			// {
			SplineAnimate splineAnimate = _splineAnimates[0];
			splineAnimate.gameObject.SetActive(false);
			_splineAnimates.Remove(splineAnimate);
			_evacuationZoneObj.Add(splineAnimate);
			AnimateUpdate();
			//}
		}

		///<summary>ZoneGaugeが 増えた時に更新</summary>
		public void ZoneIncreaseUpdate(float currentValue)
		{
			if (_evacuationZoneObj.Count < 1) return;

			for (int i = _splineAnimates.Count; i < Mathf.FloorToInt(currentValue); i++)
			{
				SplineAnimate splineAnim = _evacuationZoneObj[0];
				splineAnim.Duration = duration;
				splineAnim.gameObject.SetActive(true);
				_splineAnimates.Add(splineAnim);
				_evacuationZoneObj.Remove(splineAnim);
				AnimateUpdate();
			}
		}

		//数珠の動きの更新。
		void AnimateUpdate()
		{
			for (int i = 0; i < _splineAnimates.Count; i++)
			{
				_splineAnimates[i].Pause();
				_splineAnimates[i].StartOffset = 1f / _splineAnimates.Count * i;
				_splineAnimates[i].Restart(true);
				//_splineAnimates[i].Play();
			}
		}

		//オブジェクトが足りない場合、現在の値を入れる
		void NotEnoughObj(int value)
		{
			for (int i = _splineAnimates.Count; i < value; i++)
			{
				
				SplineAnimate splineAnimate = Instantiate(zoneObj, transform).GetComponent<SplineAnimate>();
				splineAnimate.Container = _spline;
				_evacuationZoneObj.Add(splineAnimate);
				splineAnimate.gameObject.SetActive(false);
			}
		}
	}
}
