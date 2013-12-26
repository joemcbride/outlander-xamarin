#
# Collect & kick items
#

var item %1

Collect:
	match Kick You manage to collect
	match Wait1 ...wait
	put collect %item
	matchwait 3
	goto Collect

Wait1:
	pause 1
	goto Collect

Wait2:
	pause 1
	goto Kick

Kick:
	matchre CheckEXP You take a step back|Now what did the|I could not find
	match Wait2 ...wait
	put kick %item
	matchwait 20

CheckEXP:
	pause 0.2
	if ($Outdoorsmanship.LearningRate >= 18) then goto END
	goto Collect

END:
	pause 1
	put stop play