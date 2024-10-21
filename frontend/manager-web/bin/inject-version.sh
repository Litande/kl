#!/usr/bin/env bash

VERSION=$(node -pe "require('./package.json')['version']")
echo "Version package:$VERSION"

TIME=$(date +%y%m%d).$(date +%H%M)
echo "Time build:$TIME"

FULL_VERSION="$VERSION $TIME"
echo "Version build:$FULL_VERSION"

if [[ "$OSTYPE" == "linux-gnu"* ]]; then
  # LINUX
  sed -i "s/%version%/$FULL_VERSION/g" build/index.html
elif [[ "$OSTYPE" == "darwin"* ]]; then
  # MACOS
  sed "s/%version%/$FULL_VERSION/g" build/index.html > build/index2.html
  mv build/index2.html build/index.html
else
  echo 'This OS is not supported'
fi
