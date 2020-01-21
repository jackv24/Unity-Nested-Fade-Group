using UnityEngine;

namespace NestedFadeGroup
{
    [ExecuteInEditMode]
    public class NestedFadeGroupRetarget : NestedFadeGroupBase
    {
        [SerializeField]
        private NestedFadeGroupBase target;

        public NestedFadeGroupBase Target
        {
            get { return target; }
            set
            {
                target = value;
                RefreshAlpha();
            }
        }

        protected override void OnAlphaChanged(float alpha)
        {
            if (!target)
                return;

            target.AlphaSelf = alpha;
        }
    }
}
