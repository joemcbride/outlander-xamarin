Outlander
==========

A Front End for [Dragonrealms](http://www.play.net/dr).  A work in progress.

[Screenshot](/releases/ss.png?raw=true)

Download
====

[Version 0.1](https://github.com/joemcbride/outlander/releases/tag/v0.1)

Requirements
====

* OSX 10.6 or higher
* [Mono 3.2.4](http://www.go-mono.com/mono-downloads/download.html) or higher - [developer runtime](http://download.xamarin.com/MonoFrameworkMDK/Macx86/MonoFramework-MDK-3.2.5.macos10.xamarin.x86.pkg)

Scripting
====

[Sample Scripts] (/scripts)

	.myscript one two "three four"

Script arguments become local variables:
	
	%0 = one two three four
	%1 = one
	%2 = two
	%3 = three four

Command line commands

	#script abort <name> - stop the script
	#script vars <name> - display script variables

Commands

* var myvar value
	* Creates a local variable that can be referenced later in the script as %myvar
* setvariable myvar value
	* acts the same as var
* label:
* match
	* match &lt;label&gt; &lt;text&gt;
	* match start You see
* matchre
	* matchre &lt;label&gt; &lt;text&gt;
	* matchre start first|second|third|fourth
* matchwait
* goto
* pause 0.5
* waitfor
* waitforre
	* a regex enabled waitfor
* put
	* put &lt;command&gt;
	* put collect rock
* if () then
	* Note that an equals comparison requires double ==, instead of just a single =
	* if ( "%guild" == "Ranger" ) then goto WIN
* else if ()
* else
	* if ($Outdoorsmanship.LearningRate >= 18) then goto END
	* else goto START
* echo
	* echo &lt;data&gt;
	* echo $charactername - echos the current value of $charactername to the game window
* save
	* save my.label
	* saves the value my.label to the %s variable
* move
	* move northeast
	* sends the northeast command and pauses until the player moves to another room
* nextroom
	* pauses until the player moves to another room


Planned Commands
====

* #parse
* action
* containsre
* gosub

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
