# Nested Fade Group for Unity
A generic fade group system to mimic the standard Canvas Group alpha control functionality for other, non-canvas things. Currently the only fade target is SpriteRenderer, but it can be easily extended to support new fade targets such as TextMeshPro, MeshRenderer, etc.

## How do I add new fade targets?
New "bridging components" can be created to allow existing sealed Unity components (such as SpriteRenderer) to be used with the fade groups. Simply extend NestedFadeGroupBase and use the attribute NestedFadeGroupBridge like so:

```C#
[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
[NestedFadeGroupBridge(typeof(SpriteRenderer))]
public class NestedFadeGroupSpriteRenderer : NestedFadeGroupBase
{
    private SpriteRenderer spriteRenderer;
```
