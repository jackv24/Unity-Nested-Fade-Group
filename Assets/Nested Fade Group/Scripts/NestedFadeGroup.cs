using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace NestedFadeGroup
{
	[ExecuteInEditMode]
	public class NestedFadeGroup : NestedFadeGroupBase
	{
		public Action<float> AlphaChanged;
		public Action<NestedFadeGroup> Reparent;

		private struct Bridge
		{
			public Type SourceType;
			public Type DestinationType;
		}

		private static List<Bridge> bridges;

		protected override void OnAlphaChanged(float alpha)
		{
			if (AlphaChanged != null)
				AlphaChanged(AlphaTotal);
		}

		protected override void OnEnable()
		{
			AddMissingBridgeComponents();

			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			// When this group is disabled make sure children reparent to this groups parent
			if (Reparent != null)
				Reparent(ParentGroup);
		}

		private void OnTransformChildrenChanged()
		{
			AddMissingBridgeComponents();
		}

		public void AddMissingBridgeComponents()
		{
			// Only run this once, since the types available in the assembly should not change at runtime
			if (bridges == null)
			{
				bridges = new List<Bridge>();

				Type[] types = Assembly.GetExecutingAssembly().GetTypes();
				foreach (Type type in types)
				{
					var attributes = type.GetCustomAttributes(typeof(NestedFadeGroupBridgeAttribute), true);
					foreach (var attribute in attributes)
					{
						var attr = (NestedFadeGroupBridgeAttribute)attribute;

						bridges.Add(new Bridge { SourceType = attr.TargetType, DestinationType = type });
					}
				}
			}

			foreach (var bridge in bridges)
				AddMissingBridgeComponents(bridge.SourceType, bridge.DestinationType);
		}

		private void AddMissingBridgeComponents(Type sourceType, Type destinationType)
		{
			var components = GetComponentsInChildren(sourceType, true);
			foreach (var component in components)
			{
				if (!component.GetComponent(destinationType))
					component.gameObject.AddComponent(destinationType);
			}
		}
	}
}
