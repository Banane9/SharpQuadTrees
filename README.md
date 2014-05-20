SharpQuadTrees
==============

Generic quad tree implementation in C#.

[Wikipedia Article on Quad Trees](http://en.wikipedia.org/wiki/Quadtree)

[Inspiration for this Project](https://github.com/fogleman/Quads)  
Maybe I'll make my own, using this.

------------------------------------------------------------------------------------------------------------------

##Usage##

To use, reference the assembly and create a controller class that implements `SharpQuadTrees.IQuadTreeController`.

Then just create a `SharpQuadTrees.QuadTreeLeaf` with your controller and content and type for the average.

When you call any `Split` method on it, it will return a `SharpQuadTrees.QuadTreeBranch`, if it was split.

------------------------------------------------------------------------------------------------------------------