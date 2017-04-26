#! /usr/bin/env python

import hashlib

def makesum():
    fd = open("/dev/urandom", "rb")
    buf = fd.read(64)
    if len(buf) != 64:
	raise Exception("Unable to get random data")
    hash = hashlib.md5()
    hash.update(buf)
    return hash.hexdigest()

print "Change-Id: I%s" % makesum()
