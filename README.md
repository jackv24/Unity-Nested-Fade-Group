# Nested Fade Group for Unity
A generic fade group system to mimic the standard Canvas Group alpha control functionality for other, non-canvas things. Currently the only fade target is SpriteRenderer, but it can be easily extended to support new fade targets such as TextMeshPro, MeshRenderer, etc.

## How do I add new fade targets?
Create a new class that extends NestedFadeGroupBase, and remember to give it the [ExecuteInEditMode] attribute so that it can update live in edit mode.

If your class is supposed to be a bridge to an existing class (such as with SpriteRenderer) then you can also add some logic to the AddMissingBridgeComponents method of the NestedFadeGroup class.

Since classes like SpriteRenderer can't be extended I've had to do it this way, but someone knows of a cleaner way of accomplishing this then please, let me know!
