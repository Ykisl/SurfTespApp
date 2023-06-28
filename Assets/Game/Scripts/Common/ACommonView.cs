using UnityEngine;

namespace Game.Common
{
    public abstract class ACommonView : MonoBehaviour
    {
        public abstract void SetModel(ICommonModel model);
    }
}
