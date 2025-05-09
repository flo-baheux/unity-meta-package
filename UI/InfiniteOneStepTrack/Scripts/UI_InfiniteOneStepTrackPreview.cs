using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MetaPackage;

namespace HitCraft
{
  public class UI_InfiniteOneStepTrackPreview : MonoBehaviour
  {
    [SerializeField] protected Slider _progressSlider;
    [SerializeField] protected Image containerBackground, nextRewardIcon, sliderFill, progressIcon;
    [SerializeField] protected TextMeshProUGUI progressText;
    [SerializeField] protected Sprite normalBackground, highlightBackground;
    [SerializeField] protected Color highlightSliderColor;
    [SerializeField] protected TrackKind trackKind;

    protected IBaseTrack track;
    protected MilestoneBundle nextAvailableReward;
    protected int previousPointAmount = -1;

    public virtual void Init()
    {
      track = MetaManager.Instance.GetTrack(trackKind);

      track.OnRewardBundleClaimed += (_) => Refresh();
      track.OnRewardBundleAvailable += (_) => Refresh();
      Refresh();
    }

    public virtual void Refresh()
    {
      DOTween.Kill(gameObject);
      nextAvailableReward = track.GetFirstMilestoneBundleWithStatus(RewardBundleStateEnum.Available);
      progressText.text = $"{track.RewardPoints}";
      progressIcon.sprite = track.ProgressPointsIcon;
      if (nextAvailableReward != null)
      {
        _progressSlider.value = _progressSlider.maxValue;
        nextRewardIcon.sprite = nextAvailableReward.rewards[0].GetSprite();
        containerBackground.sprite = highlightBackground;
        sliderFill.color = highlightSliderColor;

        nextRewardIcon.transform.DOScale(Vector3.one * 1.2f, 0.4f).From(1).SetLoops(-1, LoopType.Yoyo).SetId(gameObject);
      }
      else
      {
        MilestoneBundle lowerReward = track.GetLastMilestoneBundleWithStatus(RewardBundleStateEnum.Claimed);
        MilestoneBundle upperReward = track.GetFirstMilestoneBundleWithStatus(RewardBundleStateEnum.Locked);

        nextRewardIcon.sprite = upperReward.rewards[0].GetSprite();
        _progressSlider.minValue = lowerReward == null ? 0 : lowerReward.settings.pointsRequired;
        _progressSlider.maxValue = upperReward == null ? 1 : upperReward.settings.pointsRequired;

        bool shouldPlayTweening = previousPointAmount != -1 && previousPointAmount != track.RewardPoints;
        _progressSlider.DOValue(
          track.RewardPoints,
          shouldPlayTweening ? 0 : 1
        );
        previousPointAmount = track.RewardPoints;
        containerBackground.sprite = normalBackground;
        sliderFill.color = Color.white;
      }
    }
  }
}