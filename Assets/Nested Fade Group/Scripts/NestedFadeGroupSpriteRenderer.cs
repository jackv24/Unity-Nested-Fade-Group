using UnityEngine;

namespace NestedFadeGroup
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(SpriteRenderer))]
	[NestedFadeGroupBridge(typeof(SpriteRenderer))]
	public class NestedFadeGroupSpriteRenderer : NestedFadeGroupBase
	{
		private SpriteRenderer spriteRenderer;

		public Color Color
		{
			get
			{
				return spriteRenderer.color;
			}
			set
			{
				AlphaSelf = value.a;

				if (spriteRenderer)
					spriteRenderer.color = new Color(value.r, value.g, value.b, AlphaTotal);
			}
		}

		public Sprite Sprite
		{
			get
			{
				return spriteRenderer.sprite;
			}
			set
			{
				spriteRenderer.sprite = value;
			}
		}

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

		protected override void LateUpdate()
		{
			base.LateUpdate();

			// Ensure alpha is correct when something else changes sprite renderer color directly
			float alpha = AlphaTotal;
			Color color = spriteRenderer.color;
			if (color.a != alpha)
			{
				color.a = alpha;
				spriteRenderer.color = color;
			}
		}
	}
}
