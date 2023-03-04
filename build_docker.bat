@echo off

set /p Build=<version.txt

docker build . -t pokerit:%Build%
docker save pokerit:%Build% > .\pokerit-%Build%.tar
