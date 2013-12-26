Outlander
==========

A Front End for Dragonrealms.  A work in progress.

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
* label:
* match
* matchre
* matchwait
* goto
* pause 0.5
* waitfor
* put collect rock

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
* $roomdesc
* $roomobjs
* $roomplayers
* $roomexits
* $roomextra
* $roomtitle
* $roundtime
* $prompt
