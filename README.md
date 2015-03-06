Team 4089 Scouting App for FRC 2015 season
=================

A bunch of programs to accelerate and expand data collection to help determine ideal alliance partners. This is done by observing matches, tracking every goal and penalty of each team in each match, and then analyzing that data in extreme detail.

Normal models and z-scores are included!

Frameworks and Designs used:
-----------------
* WPF with C# .NET 4.5 (All apps)
* Elysium SDK (All apps)
* Json.NET, aka Newtonsoft.Json (All apps)
* Elysium Extra (Scouting App)
* Scouting Data structural code (All apps)
* MVVM (Scouting IO)
* Event-driven architecture (Scouting App, Scouting Stats)

Code Map of Everything (seriously):
![Really Complicated Code Map][codemap]
Each blue box is a class/struct/interface/delegate/namespace, and each white tag is a method, UIElement, event, or property.

[codemap]: http://i.imgur.com/mCjlNRm.png "Code Map of 2015 Scouting Apps. Extremely zoomed out."
