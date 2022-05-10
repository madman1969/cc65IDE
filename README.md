# cc65IDE

An IDE for C development using CC65

## Overview

For some time I've been playing around with the wonderful CC65 8-bit compiler, but setting up a development environment to my liking has been a PITA. I use Visual Studio all day, so I wanted a similar development experience, i.e.:

* An editor with line numbers & C syntax colouring
* The ability to edit and save a file (Ctrl+S)
* Support for a Visual Studio-style solution file
* No fiddling with Make files !
* One-click building of a project (Ctrl+Shift+B)
* Launch the app (F5)

The issue is CC65 is fundamentally a command line tool. I've tried integrating it into Geany, Notepad++, Visual Studio Code & Visual Studio itself, but in all cases there were some jarring issues.

So in the best developer tradition I've built a simple WinForms CC65 IDE, in the style of Visual Studio.

It's currently a mainly a PoC, but I intend to extend it out to cover missing features over time.
