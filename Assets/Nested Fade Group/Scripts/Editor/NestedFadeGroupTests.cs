using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

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

		private void AutoAddsBridgingComponentsWhenAddedToExistingGroup<T, U>()
			where T : Component
			where U : NestedFadeGroupBase
		{
			GameObject root = new GameObject("Root", typeof(NestedFadeGroup));
			GameObject child = new GameObject("Child", typeof(T));

			child.transform.SetParent(root.transform);

			Assert.IsNotNull(child.GetComponent<U>());

			Object.DestroyImmediate(root.gameObject);
		}

		private void AutoAddsBridgingComponentsWhenGroupAddedToExistingParent<T, U>()
			where T : Component
			where U : NestedFadeGroupBase
		{
			GameObject root = new GameObject("Root");
			GameObject child = new GameObject("Child", typeof(T));

			child.transform.SetParent(root.transform);

			root.AddComponent<NestedFadeGroup>();

			Assert.IsNotNull(child.GetComponent<U>());

			Object.DestroyImmediate(root.gameObject);
		}

		[Test]
		public void AutoAddsBridgingComponentsSpriteRendererWhenAddedToExistingGroup()
		{
			AutoAddsBridgingComponentsWhenAddedToExistingGroup<SpriteRenderer, NestedFadeGroupSpriteRenderer>();
		}

		[Test]
		public void AutoAddsBridgingComponentsSpriteRendererWhenGroupAddedToExistingParent()
		{
			AutoAddsBridgingComponentsWhenGroupAddedToExistingParent<SpriteRenderer, NestedFadeGroupSpriteRenderer>();
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
	}
}
