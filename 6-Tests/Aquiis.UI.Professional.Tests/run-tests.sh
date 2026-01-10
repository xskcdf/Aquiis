#!/bin/bash
# Run Playwright tests normally (headless mode, no inspector)

unset PWDEBUG
export DISPLAY=:0

dotnet test "$@"
