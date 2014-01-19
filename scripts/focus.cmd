debuglevel 5

FOCUS:
	pause .5
	put focus my rune
	waitfor Roundtime
	goto CheckEXP

CheckEXP:
	if ($Sorcery.LearningRate >= 34) then goto END
	goto FOCUS

END:
	pause 1
	put #parse FOCUS DONE