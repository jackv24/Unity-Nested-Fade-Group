using UnityEngine;

namespace NestedFadeGroup
{
	[ExecuteInEditMode]
	public abstract class NestedFadeGroupBase : MonoBehaviour
	{
		[SerializeField]
		[Range(0, 1.0f)]
		private float alpha = 1.0f;

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

		private void OnValidate()
		{
			// Make sure alpha is updated when field is edited in inspector
			AlphaSelf = alpha;
		}

		protected virtual void Awake()
		{
			// Updates self when script starts, also handle case where component is added to parent of existing children
			NestedFadeGroupBase[] fadeGroupBases = GetComponentsInChildren<NestedFadeGroupBase>();
			foreach (var groupBase in fadeGroupBases)
				groupBase.UpdateParent();
		}

		private void OnTransformParentChanged()
		{
			UpdateParent();
		}

		private void OnDestroy()
		{
			if (ParentGroup)
				ParentGroup.AlphaChanged -= UpdateAlpha;
		}

		private void UpdateParent()
		{
			if (ParentGroup)
				ParentGroup.AlphaChanged -= UpdateAlpha;

			ParentGroup = transform.parent ? transform.parent.GetComponentInParent<NestedFadeGroup>() : null;

			if (ParentGroup)
				ParentGroup.AlphaChanged += UpdateAlpha;

			// Trigger alpha change logic when reparented
			AlphaSelf = alpha;
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
