from myro import *
init("/dev/tty.Fluke2-0AC8-Fluke2")
translate(1)
abc = 1
sensorThreshold = 1500
while (abc == 1):
	if (wall(threshold = sensorThreshold)):
		stop()
		turnRight(0.61,1)
		abc = 0
abc = 1
while (abc == 1):
	forward(1,1)
	turnLeft(0.61,1)
	if (wall(threshold = sensorThreshold)):
		turnRight(0.61,1)
	else:
		abc = 0
turnRight(0.61,1)
stop()
