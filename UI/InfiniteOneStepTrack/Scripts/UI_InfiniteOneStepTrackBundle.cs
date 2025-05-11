using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MetaPackage;

public class UI_InfiniteOneStepTrackBundle : MonoBehaviour
{
  [SerializeField] protected TextMeshProUGUI valueText, rewardText;
  [SerializeField] protected GameObject claimedOverlay, godRays, checkmarkIcon, lockIcon;
  [SerializeField] protected Button claimButton;
  [SerializeField] protected Image background, rewardIcon;
  [SerializeField] protected CanvasGroup canvasGroup;

  [SerializeField] protected Sprite lockedBackgroundSprite, availableBackgroundSprite, claimedBackgroundSprite;
  [SerializeField] protected Material UIGrayscaleMaterial;

  protected MilestoneBundle rewardBundle;

  public virtual void Init(MilestoneBundle rewardBundle)
  {
    this.rewardBundle = rewardBundle;

    // For now, assumption rewardBundle has only one reward.
    var reward = rewardBundle.rewards[0];
    ; valueText.text = rewardBundle.settings.pointsRequired.ToString();
    rewardText.text = reward.GetText();
    rewardIcon.sprite = reward.GetSprite();

    claimButton.onClick.AddListener(() => rewardBundle.TryClaim());
    rewardBundle.OnClaimed += _ => Refresh();
    rewardBundle.OnAvailable += _ => Refresh();

    Refresh();
  }

  public virtual void Refresh()
  {
    lockIcon.SetActive(rewardBundle.state == RewardBundleStateEnum.Locked);
    rewardIcon.material = rewardBundle.state == RewardBundleStateEnum.Locked ? UIGrayscaleMaterial : null;

    godRays.SetActive(rewardBundle.state == RewardBundleStateEnum.Available);
    canvasGroup.blocksRaycasts = rewardBundle.state == RewardBundleStateEnum.Available;

    claimedOverlay.SetActive(rewardBundle.state == RewardBundleStateEnum.Claimed);
    checkmarkIcon.SetActive(rewardBundle.state == RewardBundleStateEnum.Claimed);

    switch (rewardBundle.state)
    {
      default:
      case RewardBundleStateEnum.Locked:
        background.sprite = lockedBackgroundSprite;
        break;

      case RewardBundleStateEnum.Available:
        background.sprite = availableBackgroundSprite;
        break;

      case RewardBundleStateEnum.Claimed:
        background.sprite = claimedBackgroundSprite;
        break;
    }
  }

  public RectTransform GetButtonTransform() => (RectTransform)claimButton.transform;
}
