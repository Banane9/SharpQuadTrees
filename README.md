SharpQuadTrees
==============

Generic quad tree implementation in C# that's supposed to work on (according to the portable DLL selector):

* Silverlight 5+
* Windows Phone 8+
* Windows Phone Silverlight 8+
* Xamarin.Android
* Xamarin.iOS

[Wikipedia Article on Quad Trees](http://en.wikipedia.org/wiki/Quadtree)

[Inspiration for this Project](https://github.com/fogleman/Quads)  
Maybe I'll make my own, using this. -> I made my own, linked below, in the Usage section.

------------------------------------------------------------------------------------------------------------------

##Usage##

To use, reference the assembly and create a controller class that implements `SharpQuadTrees.IQuadTreeController`.

Then just create a `SharpQuadTrees.QuadTreeLeaf` with your controller and content and type for the average.

When you call any `Split` method on it, it will return a `SharpQuadTrees.QuadTreeBranch`, if it was split.

For a more detailed example, check the [Quads](https://github.com/Banane9/Quads) project.

------------------------------------------------------------------------------------------------------------------