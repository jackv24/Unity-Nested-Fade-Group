using UnityEngine;

namespace NestedFadeGroup
{
    [ExecuteInEditMode]
    public abstract class NestedFadeGroupBase : MonoBehaviour
    {
        [SerializeField, Range(0, 1.0f)]
        private float alpha = 1.0f;
        private float? previousAlpha;

        [SerializeField]
        private bool exclude;
        private bool? previousExclude;

        /// <summary>
        /// The alpha of this fade object, not multiplied with parents.
        /// </summary>
        public float AlphaSelf
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
                RefreshAlpha();
            }
        }

        protected virtual float ExtraAlpha
        {
            get
            {
                return 1.0f;
            }
        }

        public bool Exclude
        {
            get
            {
                return exclude;
            }
            set
            {
                exclude = value;

                // Updates alpha with new exclude value
                AlphaSelf = alpha;
            }
        }

        /// <summary>
        /// The total alpha value of this object after applying all parent group alphas.
        /// </summary>
        public float AlphaTotal
        {
            get;
            private set;
        }

        /// <summary>
        /// The parent group that this group gets it's alpha from. Will be null if top level group.
        /// </summary>
        public NestedFadeGroup ParentGroup
        {
            get;
            private set;
        }

        // Use static variable for this so it can be set before component is added
        public static bool QueuedOnComponentAdded
        {
            get;
            protected set;
        }

        protected virtual void OnEnable()
        {
            previousAlpha = null;

            UpdateParent();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromParent();
        }

        protected virtual void LateUpdate()
        {
            // Keep values updated when edited through inspector or animation
            if (previousAlpha == null || alpha != previousAlpha)
            {
                previousAlpha = alpha;
                AlphaSelf = alpha;
            }
            if (previousExclude == null || exclude != previousExclude)
            {
                previousExclude = exclude;
                Exclude = exclude;
            }
        }

        protected virtual void OnTransformParentChanged()
        {
            UpdateParent();
        }

        public void UpdateParent()
        {
            if (!enabled)
                return;

            UnsubscribeFromParent();
            ParentGroup = GetParentRecursively(transform);
            SubscribeToParent();

            // Trigger alpha change logic when reparented
            AlphaSelf = alpha;
        }

        private NestedFadeGroup GetParentRecursively(Transform transform)
        {
            if (transform == null)
                return null;

            // Get components in parents, skipping any that are disabled
            // Also don't get self (bridging component can be on same object as group)
            var group = transform.GetComponent<NestedFadeGroup>();
            if (group != null && group.enabled && group != this)
                return group;
            else if (transform.parent)
                return GetParentRecursively(transform.parent);
            else
                return null;
        }

        private void SetParent(NestedFadeGroup parentGroup)
        {
            UnsubscribeFromParent();
            ParentGroup = parentGroup;
            SubscribeToParent();

            // Trigger alpha change logic when reparented
            AlphaSelf = alpha;
        }

        private void SubscribeToParent()
        {
            if (ParentGroup)
            {
                ParentGroup.AlphaChanged += UpdateAlpha;
                ParentGroup.Reparent += SetParent;
            }
        }

        private void UnsubscribeFromParent()
        {
            if (ParentGroup)
            {
                ParentGroup.AlphaChanged -= UpdateAlpha;
                ParentGroup.Reparent -= SetParent;
            }
        }

        protected void RefreshAlpha()
        {
            UpdateAlpha(ParentGroup ? ParentGroup.AlphaTotal : 1.0f);
        }

        private void UpdateAlpha(float parentAlpha)
        {
            // Function can still be subscribed to event when underlying native object has been destroyed
            if (this == null)
            {
                //Debug.LogError("NestedFadeGroupBase Object missing!");
                return;
            }

            GetMissingReferences();

            // While adding component values need to be updated first
            if (QueuedOnComponentAdded)
            {
                QueuedOnComponentAdded = false;

                // Will probably call UpdateAlpha again but that's fine
                OnComponentAdded();
            }

            if (exclude)
                AlphaTotal = alpha;
            else
                AlphaTotal = alpha * parentAlpha;

            AlphaTotal *= ExtraAlpha;

            OnAlphaChanged(AlphaTotal);
        }

        protected virtual void GetMissingReferences() { }
        protected virtual void OnComponentAdded() { }

        protected abstract void OnAlphaChanged(float alpha);
    }
}
