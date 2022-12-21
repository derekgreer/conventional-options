#!/bin/bash

SUFFIX=$1

if [ ! -d ./node_modules ]
then
  npm ci
fi

if [[ "${SUFFIX}" == "" ]]
then
  npm run build
else

  if ! command -v standard-version &> /dev/null
  then
    echo "Please install the latest version of standard-version (see https://github.com/conventional-changelog/standard-version)"
    exit -1
  fi

  PRE_RELEASE_VERSION=$(standard-version --dry-run | grep "tagging" | sed 's/.* v//g')

  if [[ "${PRE_RELEASE_VERSION}" == "" ]]
  then
    PRE_RELEASE_VERSION="0.0.1"
  fi

  echo Building Prelease Version: ${PRE_RELEASE_VERSION}-${SUFFIX}
  npm run build --version-prefix=${PRE_RELEASE_VERSION} --version-suffix=${SUFFIX}

fi
