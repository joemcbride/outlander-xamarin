Outlander
==========

A Front End for [Dragonrealms](http://www.play.net/dr).  A work in progress.

![v0 4](https://f.cloud.github.com/assets/255007/1845013/a631271c-7567-11e3-947c-036270a62bb0.png)

Download
====

[Version 0.4](https://github.com/joemcbride/outlander/releases/tag/v0.4)

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
	#send <command> - queues the command to send after roundtime
	#var myvar value - sets a global variable
	#parse <text> - sends the text to be parsed by the scripting engine, as if sent by the game

Commands

* var myvar value
	* Creates a local variable that can be referenced later in the script as %myvar
* setvariable myvar value
	* acts the same as var
* #var myvar value
	* acts the same as var, except creates a global variable
* unvar
	* removes the variable
* hasvar
	* hasvar &lt;var to check&gt; &lt;result variable&gt;
	* hasvar item hasitem
	* assigns to variable hasitem True or False if item variable exists
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
* IF_N
	* IF_1, IF_2, etc.
	* checks that the script argument exists
* action
	* action &lt;command&gt; when &lt;pattern&gt;
* send
	* send &lt;command&gt;
	* same as the put command, though will wait for roundtime
* parse &lt;text&gt;
	* sends the text to be parsed by the scripting engine, as if sent by the game
* gosub
	* gosub &lt;label&gt; &lt;argument1, argument2, etc.&gt;
	* Move to a label with the supplied arguments.  Arguments are referenced by $1, $2, etc.  Use $0 to reference all arguments.
	* Use 'return' to return the line directly after the gosub.
* debuglevel
	* Sets the debug level of the script.  Currently anything above 0 wil display script debug information.
* exit
	* immediately exists the script 


Global Variables
====

Global variables are prefixed with a $.

* $charactername - your character name
* $preparedspell - currently prepared spell, 'None' when there isn't one.
* $spelltime - amount of time in seconds the spell has been prepared
* $game - what game you are connected to, ex: 'DR'
* $gametime - the time in game you are playing, a Unix timestamp, ex: 1388858263
* $health - the percentange of your health
* $mana - the percentange of your mana
* $stamina - the percentange of your stamina
* $spirit - the percentange of your spirit
* $concentration - the percentange of your concentration
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
* $monstercount - the number of monsters in the room you are currently in; requires monsterbold to be set
* $monsterlist - the names of the monsters in the room you are currently in; requires monsterbold to be set
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

Configuration
====

Configuration settings can be found in the 'config' folder.  Add additional profiles through the profiles dialog accessed via command-P.

Global variables can be defined in variables.cfg for each profile.  variables.cfg will be saved to disk whenever a global value gets updated through gameplay or the command prompt.

	#var {primary.container} {backpack}

Highlights can be defined in highlights.cfg.  Note that only hexadecimal color codes are supported.

	#highlight {#296B00} {Weapon Master}

To highlight an entire line use the folowing regex pattern:

	#highlight {#FFFF00} {^.*is facing you at melee range.*$}

Credits
====

Icons provided by [game-icons.net](http://game-icons.net), [Ravenmore Icon Pack](http://opengameart.org/content/fantasy-icon-pack-by-ravenmore-20), [Moik Mellah](http://opengameart.org/content/mv-platformer-skeleton)
