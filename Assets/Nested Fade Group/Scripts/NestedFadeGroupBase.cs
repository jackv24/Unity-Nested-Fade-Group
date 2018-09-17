using UnityEngine;

namespace NestedFadeGroup
{
	[ExecuteInEditMode]
	public abstract class NestedFadeGroupBase : MonoBehaviour
	{
		[SerializeField]
		[Range(0, 1.0f)]
		private float alpha = 1.0f;
		private float previousAlpha;

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
				UpdateAlpha(ParentGroup ? ParentGroup.AlphaTotal : 1.0f);
			}
		}

		/// <summary>
		/// The total alpha value of this object after applying all parent group alphas.
		/// </summary>
		public float AlphaTotal { get; private set; }

		/// <summary>
		/// The parent group that this group gets it's alpha from. Will be null if top level group.
		/// </summary>
		public NestedFadeGroup ParentGroup { get; private set; }

		protected virtual void LateUpdate()
		{
			// Keep value updated when edited through inspector or animation
			if (alpha != previousAlpha)
			{
				previousAlpha = alpha;
				AlphaSelf = alpha;
			}
		}

		protected virtual void OnEnable()
		{
			// Updates self when script starts, also handle case where component is added to parent of existing children
			NestedFadeGroupBase[] fadeGroupBases = GetComponentsInChildren<NestedFadeGroupBase>();
			foreach (var groupBase in fadeGroupBases)
				groupBase.UpdateParent();
		}

		protected virtual void OnTransformParentChanged()
		{
			UpdateParent();
		}

		protected virtual void OnDisable()
		{
			UnsubscribeFromParent();
		}

		private void UpdateParent()
		{
			UnsubscribeFromParent();
			ParentGroup = GetParentRecursively(transform.parent);
			SubscribeToParent();

			// Trigger alpha change logic when reparented
			AlphaSelf = alpha;
		}

		private NestedFadeGroup GetParentRecursively(Transform parent)
		{
			if (parent == null)
				return null;

			// Get components in parents, skipping any that are disabled
			var group = parent.GetComponent<NestedFadeGroup>();
			if (group != null && group.enabled)
				return group;
			else if (parent.parent)
				return GetParentRecursively(parent.parent);
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

		private void UpdateAlpha(float parentAlpha)
		{
			GetMissingReferences();

			AlphaTotal = alpha * parentAlpha;

			OnAlphaChanged(AlphaTotal);
		}

		protected virtual void GetMissingReferences() { }

		protected abstract void OnAlphaChanged(float alpha);
	}
}
