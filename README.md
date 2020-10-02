# Unity object outlines

## Built-in render pipeline:
The effect requires the `post-processing stack (v2)` package. To add the effect, click `Add effect...`
on a `Post-Process Volume` component and select `Custom > Outline`, or add a `OutlineQuickVolume`
component on an empty game object. All the objects with a specific tag (default tag is "Outline")
will have the effect applied;

## Universal render pipeline:
To add the effect, click `Add Render Feature` and select `Outline Feature` on the `Forward Renderer Data`
asset. All the objects on a specific layers (chosen in the 
outline feature settings) will have the effect applied.

![Screenshot](https://user-images.githubusercontent.com/71973715/94941544-af48cf00-04dd-11eb-9e38-611864473ee2.png)
	
###### Limitations:
All the objects are drawn into a single buffer, as a result all the properties
of the outline are global. This means that you cannot have objects with different
color, size outlines at the same time.

For outlines with different properties multiple buffers can be drawn, or in the
case of Universal render pipeline, multiple passes can be added, one for each set
of properties.
