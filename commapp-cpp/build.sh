#!/bin/bash

mkdir build
pushd build
cmake .. --fresh
make -j
popd