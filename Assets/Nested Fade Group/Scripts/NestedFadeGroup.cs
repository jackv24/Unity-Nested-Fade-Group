using UnityEngine;
using System;
using TMPro;

namespace NestedFadeGroup
{
	[ExecuteInEditMode]
	public class NestedFadeGroup : NestedFadeGroupBase
	{
		public Action<float> AlphaChanged;

		protected override void OnAlphaChanged(float alpha)
		{
			if (AlphaChanged != null)
				AlphaChanged(AlphaTotal);
		}

		[ContextMenu("Add Missing Bridge Components")]
		public void AddMissingBridgeComponents()
		{
			var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (var spriteRenderer in spriteRenderers)
			{
				if (!spriteRenderer.GetComponent<NestedFadeGroupSpriteRenderer>())
					spriteRenderer.gameObject.AddComponent<NestedFadeGroupSpriteRenderer>();
			}
		}
	}
}
