using UnityEngine;
using System.Collections;
using System;

namespace NestedFadeGroup
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NestedFadeGroupBridgeAttribute : Attribute
	{
		public Type TargetType { get; private set; }

		public NestedFadeGroupBridgeAttribute(Type targetType)
		{
			TargetType = targetType;
		}
	}
}
