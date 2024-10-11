using System.Collections;
using DG.Tweening;
using SgLibUnite.CodingBooster;
using SgLibUnite.Systems;
using UnityEngine;
using UnityEngine.UI;

///<summary>
/// HPゲージを表示させる機能を提供する
/// </summary>
public class PlayerHpView : MonoBehaviour
{
	[SerializeField, Header("死亡時のパネル")] private GameObject _diePanel;
	[SerializeField] private AudioSource _audioSource;
	[SerializeField, Header("DieME")] private AudioClip _dieME;
	[SerializeField, Header("HPが減る速度")] private float _duration;
	[SerializeField, Header("赤ゲージが残る時間")] private float _waitTime = .2f;

	[SerializeField, Header("死亡後何秒後にシーン移動するか")]
	private float _dieWaitTime = 3f;
	[SerializeField] private Image healthImage;
	[SerializeField] private Image burnImage;
	private Animator _animator;
	private SceneLoader _sceneLoader;

	private Tween _burnEffect;

	private void Start()
	{
		_animator = GetComponent<Animator>();
		healthImage.fillAmount = 1f;
		burnImage.fillAmount = 1f;
		_diePanel?.SetActive(false);
	}

	public void SetIdleAnimate()
	{
		_animator.Play("Idle");
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

	public void DisplayDiePanel()
	{
		_diePanel.SetActive(true);
		StartCoroutine(WaitDie());
	}

	IEnumerator WaitDie()
	{
		_audioSource.clip = _dieME;
		_audioSource.Play();
		yield return new WaitForSeconds(_dieWaitTime);
		_diePanel.SetActive(false);
		var validator = GameObject.FindAnyObjectByType<MainLoopValidator>(FindObjectsInactive.Include);
		validator.Dispose_InGameObject();
		_sceneLoader = FindAnyObjectByType<SceneLoader>(FindObjectsInactive.Include);
		_audioSource.Stop();
		_sceneLoader.LoadSceneByName("Title");
	}
}
