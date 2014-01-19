#braid %item/vines for mech

debuglevel 5

var totaltime $time
var item vine
if_1 var item %1

var container1 backpack

var maxexp $Mechanical_Lore.LearningRate
var maxexp 34
#math maxexp add 25
if (%maxexp >= 34) then
	var maxexp 34

getbraided:
 MATCH braid You get
 MATCH braid already holding
 MATCH getmaterial referring
 PUT get my brai %item
 MATCHWAIT 30
 goto errorhandler

getmaterial:
 MATCH braid You get
 MATCH forage referring
 PUT get my %item from my %container1
 MATCHWAIT 30
 goto errorhandler

braidP:
 PAUSE
braid:
 MATCH braidP ...wait
 MATCH braidP Sorry,
 match pull nothing more than wasted effort
 match exp lead rope
 match exp bundling rope
 match pull heavy rope
 match exp You begin
 match exp for anything yet
 match forage need to have more
 #put tend my left arm
 PUT braid my %item
 MATCHWAIT 30
 goto errorhandler

exp:
 if ($Mechanical_Lore.LearningRate >= %maxexp) then goto done
 goto braid

pullP:
 PAUSE
pull:
 MATCH pullP ...wait
 MATCH pullP Sorry,
 match forage ruined pieces
 match braid what you have left
 MATCH drop examine your new
 MATCH braid Roundtime
 PUT pull my %item
 MATCHWAIT 30
 goto errorhandler

drop:
 pause 1
 PUT empty right hand
 PUT empty left hand
 GOTO getbraided

forage:
 match braid you find
 MATCH braid manage to find
 match novine wondering what you might find
 match novine are sure you knew what you were looking
 MATCH novine unable to find
 MATCH novine futile
 MATCH novine dragon's egg
 PUT forage %item
 MATCHWAIT 30
 goto errorhandler

novine:
 SETVARIABLE item grass
 goto forage

errorhandler:
 echo "Error"

done:
 put drop my %item
 pause 1
 send #parse BRAID DONE
 EXIT