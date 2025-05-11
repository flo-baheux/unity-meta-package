
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MetaPackage;
using System.Linq;

public class UI_InfiniteOneStepTrack : MonoBehaviour
{
  [SerializeField] private Slider slider;
  [SerializeField] private RectTransform sliderTransform, scrollRectContent, currentValueTracker;
  [SerializeField] private ScrollRect scrollRect;

  [SerializeField] private Transform rewardContainer;
  [SerializeField] private GameObject rewardPrefab;

  [SerializeField] private TextMeshProUGUI currentValueText;
  [SerializeField] private Image rewardPointsIcon;

  [SerializeField] private int spaceBetweenRewards = 500;
  [SerializeField] private TrackKind trackKind;

  private IBaseTrack track;
  private List<MilestoneBundle> milestoneBundles;
  private Dictionary<MilestoneBundle, UI_InfiniteOneStepTrackBundle> uiRewardByRewardBundle = new();

  public void LockScroll(bool locked) => scrollRect.enabled = !locked;

  private bool IsInitialized = false;

  public void Init()
  {
    if (IsInitialized)
      return;

    track = MetaManager.Instance.GetTrack(trackKind);
    rewardPointsIcon.sprite = track.RewardPointsIcon;
    track.OnProgressPointsChanged += values => Display();

    milestoneBundles = track.GetAllMilestoneBundles();

    slider.minValue = 0;
    slider.maxValue = 1;
    sliderTransform.sizeDelta = new Vector2(sliderTransform.sizeDelta.x, milestoneBundles.Count * spaceBetweenRewards);

    Vector2 nextRewardPos = new(0, spaceBetweenRewards);
    foreach (MilestoneBundle bundle in milestoneBundles)
    {
      GameObject rewardObject = Instantiate(rewardPrefab.gameObject, rewardContainer);
      rewardObject.GetComponent<RectTransform>().anchoredPosition = nextRewardPos;
      UI_InfiniteOneStepTrackBundle UIInfiniteOneStepTrackBundle = rewardObject.GetComponent<UI_InfiniteOneStepTrackBundle>();
      UIInfiniteOneStepTrackBundle.Init(bundle);
      uiRewardByRewardBundle.Add(bundle, UIInfiniteOneStepTrackBundle);
      nextRewardPos.y += spaceBetweenRewards;
    }

    IsInitialized = true;

    Display();
  }

  public void OnEnable() => Display();

  public void Display()
  {
    if (!IsInitialized)
      return;
    slider.value = GetNormalizedSliderValue(track.RewardPoints);
    currentValueTracker.anchoredPosition = new(currentValueTracker.anchoredPosition.x, GetYPosition(track.RewardPoints));
    currentValueText.text = $"{track.RewardPoints}";

    var firstAvailable = milestoneBundles.FirstOrDefault(x => x.state == RewardBundleStateEnum.Available);
    RectTransform target = firstAvailable == null ? currentValueTracker : uiRewardByRewardBundle[firstAvailable].GetComponent<RectTransform>();

    ScrollTo(target);
  }

  public float GetYPosition(int points)
  {
    MilestoneBundle lowerReward = track.GetLowerboundMilestoneBundle();
    MilestoneBundle upperReward = track.GetUpperboundMilestoneBundle();

    int lowerRewardIndex = milestoneBundles.IndexOf(lowerReward);
    int upperRewardIndex = milestoneBundles.IndexOf(upperReward);

    int lowerRewardPos = spaceBetweenRewards * (lowerRewardIndex == -1 ? 0 : lowerRewardIndex + 1);
    int upperRewardPos = spaceBetweenRewards * (upperRewardIndex == -1 ? milestoneBundles.Count : upperRewardIndex + 1);

    float lowerThreshold = lowerRewardIndex == -1 ? 0 : lowerReward.settings.pointsRequired;
    float upperThreshold = upperRewardIndex == -1 ? milestoneBundles[^1].settings.pointsRequired : upperReward.settings.pointsRequired;

    float pointsRatio = (points - lowerThreshold) / (upperThreshold - lowerThreshold);

    return Mathf.Lerp(lowerRewardPos, upperRewardPos, pointsRatio);
  }

  public float GetNormalizedSliderValue(int points)
    => Mathf.InverseLerp(0, sliderTransform.sizeDelta.y, GetYPosition(points));

  public void ScrollTo(RectTransform target)
  {
    if (!scrollRect.enabled) return;

    float anchorPosY = scrollRect.transform.InverseTransformPoint(scrollRectContent.position).y
      - scrollRect.transform.InverseTransformPoint(target.position).y
      - Screen.height / 2;

    scrollRectContent.DOKill();
    scrollRectContent.DOAnchorPosY(anchorPosY, 1f)
    .SetDelay(0.3f)
    .SetEase(Ease.OutQuad)
    .SetUpdate(true);

    // No DOTWeen version
    // var anchorPos = scrollRectContent.anchoredPosition;
    // anchorPos.y = anchorPosY;
    // scrollRectContent.anchoredPosition = anchorPos;
  }

  public void CancelScroll()
  {
    scrollRectContent.DOKill();
  }
}
