#!/bin/bash
directory=$PWD

echo Setting up your Service pipeline...

echo Whats the relative path to internal provisioning?
read keystore

cd $keystore
./CiPipelineCreationViaTcApi.sh

cd $directory