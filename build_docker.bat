@echo off

set /p Build=<version.txt

docker build . -t noa-sector-mapper:%Build%
docker save noa-sector-mapper:%Build% > .\noa-sector-mapper-%Build%.tar
