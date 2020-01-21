using UnityEngine;
using TMPro;

namespace NestedFadeGroup
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshPro))]
    [NestedFadeGroupBridge(typeof(TextMeshPro))]
    public class NestedFadeGroupTextMeshPro : NestedFadeGroupBase
    {
        private TextMeshPro textMesh;

        public Color Color
        {
            get
            {
                GetMissingReferences();

                return textMesh ? textMesh.color : Color.black;
            }
            set
            {
                GetMissingReferences();

                AlphaSelf = value.a;

                var totalColor = value;
                totalColor.a = AlphaTotal;

                if (textMesh)
                    textMesh.color = totalColor;
            }
        }

        public string Text
        {
            get
            {
                GetMissingReferences();

                return textMesh ? textMesh.text : string.Empty;
            }
            set
            {
                GetMissingReferences();

                if (textMesh)
                    textMesh.text = value;
            }
        }

        protected override void GetMissingReferences()
        {
            if (!textMesh)
                textMesh = GetComponent<TextMeshPro>();
        }

        protected override void OnComponentAdded()
        {
            if (textMesh)
                AlphaSelf = textMesh.color.a;
        }

        protected override void OnAlphaChanged(float alpha)
        {
            Color color = textMesh.color;
            color.a = alpha;
            textMesh.color = color;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            float alpha = AlphaTotal;
            Color color = textMesh.color;
            if (color.a != alpha)
            {
                color.a = alpha;
                textMesh.color = color;
            }
        }
    }
}
