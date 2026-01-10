#!/bin/bash
# Run Playwright tests in debug mode with visible browser and inspector

export PWDEBUG=1
export DISPLAY=:0

dotnet test "$@"
