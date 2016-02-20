from myro import *
import math
import searchPath
import sys
init("/dev/tty.Fluke2-0AC8-Fluke2")

vertexCoor = searchPath.findPath(sys.argv[1])





# move from v to _v
def move(v, _v, alpha):
    #first turn
    difx = _v[0] - v[0]
    dify = _v[1] - v[1]
    tmp = 0
    print difx
    print dify
    if difx == 0:
        if dify > 0:
            tmp = 90
            angle = 90 - alpha
        elif dify < 0:
            tmp = -90
            angle = -90 - alpha
    elif dify == 0:
        if difx > 0:
            tmp = 0
            angle = (360 - alpha) % 360
        elif difx < 0:
            tmp = 180
            angle = 180 - alpha
    else:
        angle = math.atan(dify / difx) / math.pi * 180
        tmp = angle
        if dify > 0 and difx < 0:
            angle += 180
            tmp = angle
            angle -= alpha
        elif dify < 0 and difx > 0:
            angle -= alpha
        elif dify > 0 and difx > 0:
            angle -= alpha
        elif dify < 0 and difx < 0:
            angle += 180
            tmp = angle
            angle -= alpha



    
    print "alpha is: {}, turning angle from alpha: {}".format(alpha, angle)
    #angle = angle % 360
    #print "turning angle after modulation is: {}".format(angle)
    turnBy(int(angle), "deg")

    #then translate
    l = math.sqrt(difx ** 2 + dify ** 2)
    forward(0.5, l)
    return tmp


alpha = 0
for i in range(len(vertexCoor) - 1):
    alpha = move(vertexCoor[i], vertexCoor[i + 1], alpha)

# a = move((0.0, 0.0), (2.0, 2.0), 0)
# a = move((2.0, 2.0), (1.0, 3.0), a)
# a = move((1.0, 3.0), (0.0, 2.0), a)