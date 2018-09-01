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

			Debug.Assert(root.AlphaTotal == 0.5f);
			Debug.Assert(parent1.AlphaTotal == 0.25f);
			Debug.Assert(parent1Child1.AlphaTotal == 0.125f);

			CleanupTestHeirarchy();
		}

		[Test]
		public void RespondsToChangesInParentsAlpha()
		{
			SetupTestHeirarchy();

			root.AlphaSelf = 1.0f;
			parent1.AlphaSelf = 1.0f;
			parent1Child1.AlphaSelf = 1.0f;

			Debug.Assert(root.AlphaTotal == 1.0f);
			Debug.Assert(parent1.AlphaTotal == 1.0f);
			Debug.Assert(parent1Child1.AlphaTotal == 1.0f);

			root.AlphaSelf = 0.5f;

			Debug.Assert(root.AlphaTotal == 0.5f);
			Debug.Assert(parent1.AlphaTotal == 0.5f);
			Debug.Assert(parent1Child1.AlphaTotal == 0.5f);

			parent1.AlphaSelf = 0.25f;

			Debug.Assert(root.AlphaTotal == 0.5f);
			Debug.Assert(parent1.AlphaTotal == 0.125f);
			Debug.Assert(parent1Child1.AlphaTotal == 0.125f);

			CleanupTestHeirarchy();
		}

		[Test]
		public void UpdatesParentGroupAutomatically()
		{
			SetupTestHeirarchy();

			Debug.Assert(root.ParentGroup == null);
			Debug.Assert(parent1.ParentGroup == root);
			Debug.Assert(parent1Child1.ParentGroup == parent1);

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

			Debug.Assert(group1.ParentGroup == group2);
		}

		[Test]
		public void UpdatesAlphaWhenReparented()
		{
			SetupTestHeirarchy();

			root.AlphaSelf = 0.5f;
			parent1.AlphaSelf = 0.5f;
			parent1Child1.AlphaSelf = 0.5f;

			parent1Child1.transform.SetParent(root.transform);

			Debug.Assert(parent1Child1.AlphaTotal == 0.25f);

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

			Debug.Assert(group1.ParentGroup == emptyParentAddedGroup);
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

			Debug.Assert(emptyParentAddedGroup.AlphaTotal == 0.5f);
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

			Debug.Assert(emptyParentAddedGroup.ParentGroup == root);
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

			Debug.Assert(parent1Child1.AlphaTotal == 0.5f);

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

			Debug.Assert(spriteRenderer.color == new Color(1.0f, 1.0f, 1.0f, 0.5f));
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

			Debug.Assert(spriteRenderer.color == new Color(1.0f, 1.0f, 1.0f, 0.0625f));
			Object.DestroyImmediate(spriteObj);

			CleanupTestHeirarchy();
		}

		[Test]
		public void AutoAddsBridgeSpriteRendererWhenFadeGroupAddedToParent()
		{
			GameObject root = new GameObject("Root");
			GameObject sprite = new GameObject("Sprite", typeof(SpriteRenderer));

			sprite.transform.SetParent(root.transform);

			root.AddComponent<NestedFadeGroup>();

			Debug.Assert(sprite.GetComponent<NestedFadeGroupSpriteRenderer>());

			Object.DestroyImmediate(root);
		}

		[Test]
		public void AutoAddsBridgeSpriteRendererWhenParentedToFadeGroup()
		{
			GameObject root = new GameObject("Root", typeof(NestedFadeGroup));
			GameObject sprite = new GameObject("Sprite", typeof(SpriteRenderer));

			sprite.transform.SetParent(root.transform);

			Debug.Assert(sprite.GetComponent<NestedFadeGroupSpriteRenderer>());

			Object.DestroyImmediate(root);
		}
	}
}
