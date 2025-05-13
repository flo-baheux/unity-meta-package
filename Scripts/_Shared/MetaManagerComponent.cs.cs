using UnityEngine;

namespace MetaPackage
{
  public abstract class MetaManagerComponent : MonoBehaviour
  {
    private bool isInitialized = false;

    public void Initialize()
    {
      if (isInitialized)
        return;

      Setup();

      isInitialized = true;
    }

    protected abstract void Setup();

#if UNITY_EDITOR
    public void Reset() => isInitialized = false;
#endif
  }
}