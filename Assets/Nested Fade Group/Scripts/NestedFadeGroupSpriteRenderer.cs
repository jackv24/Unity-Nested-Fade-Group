using UnityEngine;

namespace NestedFadeGroup
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(SpriteRenderer))]
	public class NestedFadeGroupSpriteRenderer : NestedFadeGroupBase
	{
		private SpriteRenderer spriteRenderer;

		protected override void GetMissingReferences()
		{
			if (!spriteRenderer)
				spriteRenderer = GetComponent<SpriteRenderer>();
		}

		protected override void OnAlphaChanged(float alpha)
		{
			Color color = spriteRenderer.color;
			color.a = alpha;
			spriteRenderer.color = color;
		}
	}
}
