#
# Collect & kick items
#

debuglevel 5

setvariable item %1
var maxexp 34

Collect:
	save collect
	match Kick You manage to collect
	match Wait ...wait
	put play $play.song $play.style
	put collect %item
	matchwait 3
	goto Collect

Wait:
	pause 1
	goto %s

Kick:
	save Kick
	matchre CheckEXP You take a step back|Now what did the|I could not find
	match Wait ...wait
	put kick %item
	matchwait 20

CheckEXP:
	pause 0.2
	if ($Outdoorsmanship.LearningRate >= %maxexp) then goto END
	goto Collect

END:
	pause 1
	put stop play
	put #parse COLLECT DONE