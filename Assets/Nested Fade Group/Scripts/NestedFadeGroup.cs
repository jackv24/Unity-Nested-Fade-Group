using UnityEngine;
using System;
using TMPro;

namespace NestedFadeGroup
{
	[ExecuteInEditMode]
	public class NestedFadeGroup : NestedFadeGroupBase
	{
		/// <summary>
		/// Called when the alpha of this fade group is set.
		/// </summary>
		public Action<float> AlphaChanged;

		protected override void OnAlphaChanged(float alpha)
		{
			if (AlphaChanged != null)
				AlphaChanged(AlphaTotal);
		}

		/// <summary>
		/// Adds corresponding bridge components for any existing components on children GameObjects.
		/// </summary>
		[ContextMenu("Add Missing Bridge Components")]
		public void AddMissingBridgeComponents()
		{
			// SpriteRenderer can not be inherited from, so an additional component must be used instead.
			var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (var spriteRenderer in spriteRenderers)
			{
				if (!spriteRenderer.GetComponent<NestedFadeGroupSpriteRenderer>())
					spriteRenderer.gameObject.AddComponent<NestedFadeGroupSpriteRenderer>();
			}
		}
	}
}
