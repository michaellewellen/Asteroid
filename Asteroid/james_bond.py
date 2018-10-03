import socket
import sys
import json
import math

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM);
sock.connect(("127.0.0.1", int(sys.argv[1])));
command = {"shoot": True,
           "accel": True,
           "left": False,
           "right": False
           }

while(True):
    sock.send(json.dumps(command));
    msg = sock.recv(10000);
    data = json.loads(msg);
    mx = data['myShip']['pos']['x'];
    my = data['myShip']['pos']['y'];
    ex = data['enemy']['pos']['x'];
    ey = data['enemy']['pos']['y'];
    dirToEnemy = math.atan2(ey-my, ex-mx);
    heading = data['myShip']['heading'];
    if(heading > dirToEnemy):
        command['right'] = True;
        command['left'] = False;
    else:
        command['right'] = False;
        command['left'] = True;
        

    print data