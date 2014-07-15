import usb
import sys

SIFTEO_VID  = 0x22fa
BASE_PID    = 0x0105

IN_EP       = 0x81
OUT_EP      = 0x01
INTF        = 0x0
MAX_PACKET  = 64

USER_SUBSYS = 7

def find_and_open():
    dev = usb.core.find(idVendor = SIFTEO_VID, idProduct = BASE_PID)
    #if dev is not None:
        #print str(dev) 
    while dev is None:
        sys.stderr.write('Device is not connected\n')
        #sys.exit(1)
        dev = usb.core.find(idVendor = SIFTEO_VID, idProduct = BASE_PID)

    # set the active configuration. With no arguments, the first
    # configuration will be the active one
    dev.set_configuration()

    return dev

def send(dev, bytes, timeout = 1000):
    """
    Write a byte array to the device.
    """

    # Ensure that our message will be dispatched appropriately by the base.
    # Highest 4 bits specify the subsystem, user subsystem is 7.
    USER_HDR = USER_SUBSYS << 4
    msg = [0, 0, 0, USER_HDR]
    msg.extend(bytes)

    if len(msg) > MAX_PACKET:
        raise ValueError("msg is too long")

    return dev.write(OUT_EP, msg, INTF, timeout)

dev = find_and_open()
for arg in sys.argv:
    #b = arg.encode('utf-8')
    b = [1]
    if arg is 'ok':
        b = [2]
    send(dev, b)
