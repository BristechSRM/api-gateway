#!/bin/bash -euv
echo -e '\033]2;'Gateway'\007'
./setup.sh
./build.sh
./run.sh
