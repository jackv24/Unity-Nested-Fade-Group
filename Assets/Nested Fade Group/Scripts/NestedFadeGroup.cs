using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace NestedFadeGroup
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
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

            // Handle case where new group is added as parent of existing bases (since OnTransformParentChanged is not called for the whole heirarchy)
            var children = GetChildrenUntilNextFadeGroup(transform);
            foreach (var child in children)
                child.UpdateParent();
        }

        private List<NestedFadeGroupBase> GetChildrenUntilNextFadeGroup(Transform parent)
        {
            var runningList = new List<NestedFadeGroupBase>();

            foreach (Transform child in parent)
            {
                var childBase = child.GetComponent<NestedFadeGroupBase>();
                if (childBase)
                {
                    runningList.Add(childBase);

                    // Don't recurse if child is an active group (each NestedFadeGroup will find it's own children)
                    if (childBase is NestedFadeGroup && childBase.enabled)
                        continue;
                }

                runningList.AddRange(GetChildrenUntilNextFadeGroup(child));
            }

            return runningList;
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

                        foreach (var sourceType in attr.TargetTypes)
                            bridges.Add(new Bridge { SourceType = sourceType, DestinationType = type });
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
                {
                    // When auto-adding components make sure existing component alphas are kept
                    QueuedOnComponentAdded = true;
                    component.gameObject.AddComponent(destinationType);
                    QueuedOnComponentAdded = false;
                }
            }
        }
    }
}
