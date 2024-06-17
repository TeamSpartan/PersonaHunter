using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

///<summary>プレイヤーHP</summary>
public class PlayerHpView : MonoBehaviour
{
	[SerializeField, Header("HPが減る速度")] private float _duration;
	[SerializeField, Header("赤ゲージが残る時間")] private float _waitTime = .2f;
	[SerializeField] private Image healthImage;
	[SerializeField] private Image burnImage;
	[SerializeField, Header("死亡時パネル")] private GameObject _deadPanel;
	private Tween _burnEffect;

	private void Start()
	{
		_deadPanel.SetActive(false);
		healthImage.fillAmount = 1f;
		burnImage.fillAmount = 1f;
	}

	///<summary>受ダメージ時にゲージを変化させる</summary>
	///<param name="InitialHp">初期HP</param>
	///<param name="CurrentHp"	>現在のHP</param>
	public void SetGauge(float InitialHp, float CurrentHp)
	{
		_burnEffect?.Kill();
		healthImage.DOFillAmount(CurrentHp / InitialHp, _duration).OnComplete(() =>
		{
			_burnEffect = burnImage.DOFillAmount(CurrentHp / InitialHp, _duration * 0.5f).SetDelay(_waitTime);
		});
	}

	///<summary>リジェネ時にゲージを変化させる</summary>
	///<param name="InitialHp">初期HP</param>
	///<param name="CurrentHp"	>現在のHP</param>
	public void SetRegenerate(float InitialHp, float CurrentHp)
	{
		healthImage.fillAmount = CurrentHp / InitialHp;
		burnImage.fillAmount = CurrentHp / InitialHp;
	}

	public void DisplayDeadPanel()
	{
		_deadPanel.SetActive(true);
	}
}
