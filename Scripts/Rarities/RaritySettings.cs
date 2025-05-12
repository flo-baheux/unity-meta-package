using UnityEngine;
using System;

namespace MetaPackage
{
    [Serializable]
    public class RaritySettings : Referenceable<RarityReference>
    {
        public string displayName;
        public Color color;

        public override RarityReference GenerateReference() => new(ID, displayName);

        public void Refresh()
        {
            if (string.IsNullOrEmpty(_ID))
                ResetID();
            name = displayName;
        }
    }
}