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
                GetMissingReferences();

                return spriteRenderer.color;
            }
            set
            {
                GetMissingReferences();

                AlphaSelf = value.a;
                BaseColor = value;
            }
        }

        public Color BaseColor
        {
            get
            {
                GetMissingReferences();

                var color = spriteRenderer.color;
                color.a = 1.0f;

                return color;
            }
            set
            {
                if (spriteRenderer)
                    spriteRenderer.color = new Color(value.r, value.g, value.b, AlphaTotal);
            }
        }

        public Sprite Sprite
        {
            get
            {
                GetMissingReferences();

                return spriteRenderer.sprite;
            }
            set
            {
                GetMissingReferences();

                spriteRenderer.sprite = value;
            }
        }

        protected override void GetMissingReferences()
        {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void OnComponentAdded()
        {
            if (spriteRenderer)
                AlphaSelf = spriteRenderer.color.a;
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
