# Random notes

The intention here is to write some code that makes a simple RPG video game map. For now, I'm just focusing on pokemon style maps,
and the further refine, I'm focusing just on Pokemon Diamond and Pearl (gen 4) maps from my childhood.

There are a few map design ideas that I draw from my underlying inspiration:

- Looped map: The map constitutes a simple loop the player traverses clockwise. The loop features several offshoot branches away from the loop, and maybe a cut-through in the middle
- Squiggle shape: The player does a lose rough M shape. The tops and bottoms of the squiggle may be connected with cut-throughs. Offshoots appear at various points (top bottom left right edge)
- Overlapping rectangles: Make a single rectangle to be on top. Create 1-3 other smaller rectangles to go under the main one. Most 3-way intersection points are cities. Add extrusions.

Width = 30 tiles
height = 25 tiles

Looped map params

- loop width ~= 13 tiles
- loop height ~= 8 tiles

Alternate loop properties

- width: 9
- height: 9

Squiggle properties:

- Squiggle 1 up height: 10     over (at top) 4
- Squiggle 2 down height: 7    over 0
- Total width of squiggles: 18
- Total height box of squiggles: 11

=====
