Outlander
==========

A Front End for [Dragonrealms](http://www.play.net/dr).  A work in progress.

[Screenshot](https://github.com/joemcbride/outlander/blob/master/releases/ss.png?raw=true)

Download
====

[Version 0.1](https://github.com/joemcbride/outlander/blob/master/releases/Outlander.0.1.app.zip?raw=true)

Requirements
====

* OSX 10.6 or higher
* [Mono 3.2.4](http://www.go-mono.com/mono-downloads/download.html) or higher - [developer runtime](http://download.xamarin.com/MonoFrameworkMDK/Macx86/MonoFramework-MDK-3.2.5.macos10.xamarin.x86.pkg)

Scripting
====

	.myscript one two "three four"

Script arguments become local variables:
	
	%0 = one two three four
	%1 = one
	%2 = two
	%3 = three four

Command line commands

	#script abort <name>
	#script vars <name>

Commands

* var myvar value
	* Creates a local variable that can be refereced later in the script as %myvar
* label:
* match
	* match <label> <text>
	* match start You see
* matchre
	* matchre <label> <text>
	* matchre start first|second|third|fourth
* matchwait
* goto
* pause 0.5
* waitfor
* put collect rock
* if () then
* else if
* else
	* if ($Outdoorsmanship.LearningRate >= 18) then goto END
	* else goto START

Global Variables
====

Global variables are prefixed with a $.

* $charactername
* $spell
* $game
* $gametime
* $health
* $mana
* $stamina
* $spirit
* $concentration
* $&lt;skill_name&gt;.Ranks (ex: Shield_Usage.Ranks, Outdoorsmanship.Ranks, etc.)
* $&lt;skill_name&gt;.LearningRate
* $&lt;skill_name&gt;.LearningRateName
* $bleeding (0/1)
* $kneeling (0/1)
* $prone (0/1)
* $sitting (0/1)
* $standing (0/1)
* $stunned (0/1)
* $hidden (0/1)
* $invisible (0/1)
* $dead (0/1)
* $webbed (0/1)
* $joined (0/1)
* $lefthand
* $lefthandnoun
* $lefthandnounid
* $righthand
* $righthandnoun
* $righthandnounid
* $roomtitle
* $roomdesc
* $roomobjs
* $roomplayers
* $roomexits
* $roomextra
* $roundtime
* $prompt
