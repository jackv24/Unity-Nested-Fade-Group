using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using System;
using Object = UnityEngine.Object;

namespace NestedFadeGroup
{
    public class NestedFadeGroupTests
    {
        private NestedFadeGroup root;
        private NestedFadeGroup parent1;
        private NestedFadeGroup parent1Child1;

        private void SetupTestHeirarchy()
        {
            root = new GameObject("Root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            parent1 = new GameObject("Parent 1", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            parent1Child1 = new GameObject("Parent 1 Child 1", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();

            parent1Child1.transform.SetParent(parent1.transform);
            parent1.transform.SetParent(root.transform);
        }

        private void CleanupTestHeirarchy()
        {
            Object.DestroyImmediate(parent1Child1);
            Object.DestroyImmediate(parent1);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void InheritsParentAlphaRecursively()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 0.5f;
            parent1.AlphaSelf = 0.5f;
            parent1Child1.AlphaSelf = 0.5f;

            Assert.AreEqual(0.5f, root.AlphaTotal);
            Assert.AreEqual(0.25f, parent1.AlphaTotal);
            Assert.AreEqual(0.125f, parent1Child1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void RespondsToChangesInParentsAlpha()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 1.0f;
            parent1.AlphaSelf = 1.0f;
            parent1Child1.AlphaSelf = 1.0f;

            Assert.AreEqual(1.0f, root.AlphaTotal);
            Assert.AreEqual(1.0f, parent1.AlphaTotal);
            Assert.AreEqual(1.0f, parent1Child1.AlphaTotal);

            root.AlphaSelf = 0.5f;

            Assert.AreEqual(0.5f, root.AlphaTotal);
            Assert.AreEqual(0.5f, parent1.AlphaTotal);
            Assert.AreEqual(0.5f, parent1Child1.AlphaTotal);

            parent1.AlphaSelf = 0.25f;

            Assert.AreEqual(0.5f, root.AlphaTotal);
            Assert.AreEqual(0.125f, parent1.AlphaTotal);
            Assert.AreEqual(0.125f, parent1Child1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void UpdatesParentGroupAutomatically()
        {
            SetupTestHeirarchy();

            Assert.IsNull(root.ParentGroup);
            Assert.AreSame(root, parent1.ParentGroup);
            Assert.AreSame(parent1, parent1Child1.ParentGroup);

            CleanupTestHeirarchy();
        }

        [Test]
        public void UpdatesParentGroupAutomaticallyWhenHigherParentIsReparented()
        {
            NestedFadeGroup root = new GameObject("Root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            Transform emptyParent = new GameObject("Empty Parent").transform;
            NestedFadeGroup group1 = new GameObject("Group 1", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            NestedFadeGroup group2 = new GameObject("Group 2", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();

            emptyParent.SetParent(root.transform);
            group1.transform.SetParent(emptyParent);
            group2.transform.SetParent(root.transform);

            emptyParent.SetParent(group2.transform);

            Assert.AreSame(group2, group1.ParentGroup);
        }

        [Test]
        public void UpdatesAlphaWhenReparented()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 0.5f;
            parent1.AlphaSelf = 0.5f;
            parent1Child1.AlphaSelf = 0.5f;

            parent1Child1.transform.SetParent(root.transform);

            Assert.AreEqual(0.25f, parent1Child1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void UpdatesExistingChildrenWhenGroupAdded()
        {
            NestedFadeGroup root = new GameObject("Root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            Transform emptyParent = new GameObject("Empty Parent").transform;
            NestedFadeGroup group1 = new GameObject("Group 1", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();

            emptyParent.SetParent(root.transform);
            group1.transform.SetParent(emptyParent);

            var emptyParentAddedGroup = emptyParent.gameObject.AddComponent<NestedFadeGroup>();

            Assert.AreSame(emptyParentAddedGroup, group1.ParentGroup);
        }

        [Test]
        public void UpdatesAlphaWhenGroupAdded()
        {
            NestedFadeGroup root = new GameObject("Root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            Transform emptyParent = new GameObject("Empty Parent").transform;
            NestedFadeGroup group1 = new GameObject("Group 1", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();

            emptyParent.SetParent(root.transform);
            group1.transform.SetParent(emptyParent);

            root.AlphaSelf = 0.5f;

            var emptyParentAddedGroup = emptyParent.gameObject.AddComponent<NestedFadeGroup>();

            Assert.AreEqual(0.5f, emptyParentAddedGroup.AlphaTotal);
        }

        [Test]
        public void UpdatesParentGroupWhenGroupAdded()
        {
            NestedFadeGroup root = new GameObject("Root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            Transform emptyParent = new GameObject("Empty Parent").transform;
            NestedFadeGroup group1 = new GameObject("Group 1", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();

            emptyParent.SetParent(root.transform);
            group1.transform.SetParent(emptyParent);

            var emptyParentAddedGroup = emptyParent.gameObject.AddComponent<NestedFadeGroup>();

            Assert.AreSame(root, emptyParentAddedGroup.ParentGroup);
        }

        [Test]
        public void UpdatesAlphaWhenChangedWhileDisabledAndThenEnabled()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 1.0f;
            parent1.AlphaSelf = 1.0f;
            parent1Child1.AlphaSelf = 1.0f;

            parent1.gameObject.SetActive(false);
            parent1.AlphaSelf = 0.5f;
            parent1.gameObject.SetActive(true);

            Assert.AreEqual(0.5f, parent1Child1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void BridgeSpriteRendererAdjustSelfAlpha()
        {
            GameObject spriteObj = new GameObject("Sprite", typeof(SpriteRenderer), typeof(NestedFadeGroupSpriteRenderer));
            var spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
            var spriteRendererBridge = spriteObj.GetComponent<NestedFadeGroupSpriteRenderer>();

            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            spriteRendererBridge.AlphaSelf = 0.5f;

            Assert.AreEqual(new Color(1.0f, 1.0f, 1.0f, 0.5f), spriteRenderer.color);
            Object.DestroyImmediate(spriteObj);
        }

        [Test]
        public void BridgeSpriteRendererInheritsParentsAlphaRecursively()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 0.5f;
            parent1.AlphaSelf = 0.5f;
            parent1Child1.AlphaSelf = 0.5f;

            GameObject spriteObj = new GameObject("Sprite", typeof(SpriteRenderer), typeof(NestedFadeGroupSpriteRenderer));
            var spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
            var spriteRendererBridge = spriteObj.GetComponent<NestedFadeGroupSpriteRenderer>();

            spriteObj.transform.SetParent(parent1Child1.transform);

            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            spriteRendererBridge.AlphaSelf = 0.5f;

            Assert.AreEqual(new Color(1.0f, 1.0f, 1.0f, 0.0625f), spriteRenderer.color);
            Object.DestroyImmediate(spriteObj);

            CleanupTestHeirarchy();
        }

        [Test]
        public void BridgeTextMeshProAdjustSelfAlpha()
        {
            GameObject textObj = new GameObject("Text", typeof(TextMeshPro), typeof(NestedFadeGroupTextMeshPro));
            var textMesh = textObj.GetComponent<TextMeshPro>();
            var textMeshBridge = textObj.GetComponent<NestedFadeGroupTextMeshPro>();

            textMesh.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            textMeshBridge.AlphaSelf = 0.5f;

            Assert.AreEqual(new Color(1.0f, 1.0f, 1.0f, 0.5f), textMesh.color);
            Object.DestroyImmediate(textObj);
        }

        [Test]
        public void BridgeTextMeshInheritsParentsAlphaRecursively()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 0.5f;
            parent1.AlphaSelf = 0.5f;
            parent1Child1.AlphaSelf = 0.5f;

            GameObject textObj = new GameObject("Text", typeof(TextMeshPro), typeof(NestedFadeGroupTextMeshPro));
            var textMesh = textObj.GetComponent<TextMeshPro>();
            var textMeshBridge = textObj.GetComponent<NestedFadeGroupTextMeshPro>();

            textObj.transform.SetParent(parent1Child1.transform);

            textMesh.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            textMeshBridge.AlphaSelf = 0.5f;

            Assert.AreEqual(new Color(1.0f, 1.0f, 1.0f, 0.0625f), textMesh.color);
            Object.DestroyImmediate(textObj);

            CleanupTestHeirarchy();
        }

        private static IEnumerable<TestCaseData> BridgingComponentCases()
        {
            yield return new TestCaseData(typeof(SpriteRenderer), typeof(NestedFadeGroupSpriteRenderer));
            yield return new TestCaseData(typeof(TextMeshPro), typeof(NestedFadeGroupTextMeshPro));
            yield return new TestCaseData(typeof(ParticleSystem), typeof(NestedFadeGroupParticleSystem));
        }

        [Test, TestCaseSource("BridgingComponentCases")]
        public void AutoAddsBridgingComponentsWhenAddedToExistingGroup(Type sourceType, Type destinationType)
        {
            GameObject root = new GameObject("Root", typeof(NestedFadeGroup));
            GameObject child = new GameObject("Child", sourceType);

            child.transform.SetParent(root.transform);

            Assert.IsNotNull(child.GetComponent(destinationType));

            Object.DestroyImmediate(root.gameObject);
        }

        [Test, TestCaseSource("BridgingComponentCases")]
        public void AutoAddsBridgingComponentsWhenGroupAddedToExistingParent(Type sourceType, Type destinationType)
        {
            GameObject root = new GameObject("Root");
            GameObject child = new GameObject("Child", sourceType);

            child.transform.SetParent(root.transform);

            root.AddComponent<NestedFadeGroup>();

            Assert.IsNotNull(child.GetComponent(destinationType));

            Object.DestroyImmediate(root.gameObject);
        }

        [Test]
        public void ReparentsWhenMiddleParentIsDestroyed()
        {
            SetupTestHeirarchy();
            Object.DestroyImmediate(parent1);

            Assert.AreSame(root, parent1Child1.ParentGroup);

            CleanupTestHeirarchy();
        }

        [Test]
        public void ReparentsWhenMiddleParentIsDisabled()
        {
            SetupTestHeirarchy();
            parent1.enabled = false;

            Assert.AreSame(root, parent1Child1.ParentGroup);

            CleanupTestHeirarchy();
        }

        [Test]
        public void ReparentsWhenMiddleParentIsEnabled()
        {
            SetupTestHeirarchy();
            parent1.enabled = false;
            parent1.enabled = true;

            Assert.AreSame(parent1, parent1Child1.ParentGroup);

            CleanupTestHeirarchy();
        }

        [Test]
        public void UpdatesAlphaWhenParentDisabled()
        {
            SetupTestHeirarchy();
            root.AlphaSelf = 0.5f;
            root.enabled = false;

            Assert.AreEqual(1.0f, parent1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void UpdatesAlphaWhenParentEnabled()
        {
            SetupTestHeirarchy();
            root.AlphaSelf = 0.5f;
            root.enabled = false;
            root.enabled = true;

            Assert.AreEqual(0.5f, parent1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void SkipsDisabledFadeGroupsWhenFindingParent()
        {
            SetupTestHeirarchy();
            root.enabled = false;
            parent1.enabled = false;
            root.enabled = true;

            Assert.AreEqual(root, parent1Child1.ParentGroup);

            CleanupTestHeirarchy();
        }

        [Test, TestCaseSource("BridgingComponentCases")]
        public void BridgingComponentUnsubscribesWhenDestroyed(Type sourceType, Type destinationType)
        {
            var group = new GameObject("Group", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            var child = new GameObject("Child", sourceType, destinationType);

            child.transform.SetParent(group.transform);

            Object.DestroyImmediate(child);

            Assert.DoesNotThrow(() => group.AlphaSelf = 1.0f);
        }

        [Test]
        public void ExcludeToggleNestedFadeGroupUsesOnlySelfAlpha()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 0.5f;
            parent1.Exclude = true;
            parent1.AlphaSelf = 0.5f;
            parent1Child1.AlphaSelf = 0.5f;
            parent1Child1.Exclude = true;

            Assert.AreEqual(0.5f, parent1.AlphaTotal);
            Assert.AreEqual(0.5f, parent1Child1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void ExcludeToggleNestedFadeGroupUsesOnlySelfAlphaWhenParentUpdated()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 0.5f;
            parent1.Exclude = true;
            parent1.AlphaSelf = 0.5f;
            parent1Child1.AlphaSelf = 0.5f;
            parent1Child1.Exclude = true;

            root.AlphaSelf = 0.25f;

            Assert.AreEqual(0.5f, parent1.AlphaTotal);
            Assert.AreEqual(0.5f, parent1Child1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test, TestCaseSource("BridgingComponentCases")]
        public void BridgeUsesGroupOnSameGameObjectAsParent(Type sourceType, Type destinationType)
        {
            var obj = new GameObject();
            var source = obj.AddComponent(sourceType);
            var group = obj.AddComponent<NestedFadeGroup>();
            var bridge = (NestedFadeGroupBase)obj.GetComponent(destinationType);

            Assert.AreSame(group, bridge.ParentGroup);

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void UpdatesAlphaWhenGroupComponentDisabled()
        {
            SetupTestHeirarchy();

            root.AlphaSelf = 0.5f;
            root.enabled = false;

            Assert.AreEqual(1.0f, parent1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test]
        public void UpdatesAlphaWhenDisabledGroupComponentEnabled()
        {
            SetupTestHeirarchy();

            root.enabled = false;
            root.AlphaSelf = 0.5f;
            root.enabled = true;

            Assert.AreEqual(0.5f, parent1.AlphaTotal);

            CleanupTestHeirarchy();
        }

        [Test, TestCaseSource("BridgingComponentCases")]
        public void UpdatesBridgeAlphaWhenGroupComponentDisabled(Type sourceType, Type destinationType)
        {
            var root = new GameObject("root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            var child = new GameObject("child", sourceType, destinationType).GetComponent(destinationType) as NestedFadeGroupBase;
            child.transform.SetParent(root.transform);

            root.AlphaSelf = 0.5f;
            root.enabled = false;

            Assert.AreEqual(1.0f, child.AlphaTotal);

            Object.DestroyImmediate(root);
        }

        [Test, TestCaseSource("BridgingComponentCases")]
        public void UpdatesBridgeAlphaWhenDisabledGroupComponentEnabled(Type sourceType, Type destinationType)
        {
            var root = new GameObject("root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            var child = new GameObject("child", sourceType, destinationType).GetComponent(destinationType) as NestedFadeGroupBase;
            child.transform.SetParent(root.transform);

            root.enabled = false;
            root.AlphaSelf = 0.5f;
            root.enabled = true;

            Assert.AreEqual(0.5f, child.AlphaTotal);

            Object.DestroyImmediate(root);
        }

        [Test, TestCaseSource("BridgingComponentCases")]
        public void UpdatesBridgeAlphaWhenDisabledGroupComponentEnabledAndNotDirectChildOfGroup(Type sourceType, Type destinationType)
        {
            var root = new GameObject("root", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();
            var midChild = new GameObject("midchild");
            var child = new GameObject("child", sourceType, destinationType).GetComponent(destinationType) as NestedFadeGroupBase;
            midChild.transform.SetParent(root.transform);
            child.transform.SetParent(midChild.transform);

            root.enabled = false;
            root.AlphaSelf = 0.5f;
            root.enabled = true;

            Assert.AreEqual(0.5f, child.AlphaTotal);

            Object.DestroyImmediate(root);
        }

        [Test]
        public void RetargetSimpleWorks()
        {
            var orig = new GameObject("Original", typeof(NestedFadeGroupRetarget)).GetComponent<NestedFadeGroupRetarget>();
            var target = new GameObject("Target", typeof(NestedFadeGroup)).GetComponent<NestedFadeGroup>();

            orig.AlphaSelf = 0.5f;
            target.AlphaSelf = 1.0f;

            orig.Target = target;

            Assert.AreEqual(0.5f, target.AlphaSelf);
        }

        [Test]
        public void KeepsSpriteRendererAlphaWhenGroupAdded()
        {
            var obj = new GameObject("Sprite");
            var sprite = obj.AddComponent<SpriteRenderer>();

            sprite.color = new Color(1, 1, 1, 0.5f);

            obj.AddComponent<NestedFadeGroup>();

            Assert.AreEqual(0.5f, sprite.color.a);

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void KeepsTextMeshProAlphaWhenGroupAdded()
        {
            var obj = new GameObject("Sprite");
            var text = obj.AddComponent<TextMeshPro>();

            text.color = new Color(1, 1, 1, 0.5f);

            obj.AddComponent<NestedFadeGroup>();

            Assert.AreEqual(0.5f, text.color.a);

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void KeepsTextMeshAlphaWhenGroupAdded()
        {
            var obj = new GameObject("Sprite");
            var text = obj.AddComponent<TextMesh>();

            text.color = new Color(1, 1, 1, 0.5f);

            obj.AddComponent<NestedFadeGroup>();

            // For some reason TextMesh color has precision issues, so we'll just round them out here so the test doesn't fail
            const float roundError = 0.05f;
            float textAlpha = Mathf.Round(text.color.a / roundError) * roundError;

            Assert.AreEqual(0.5f, textAlpha);

            Object.DestroyImmediate(obj);
        }
    }
}