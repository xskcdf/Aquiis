#!/usr/bin/env bash
set -euo pipefail
# Usage: ./kudu-cleanup-backups.sh /path/to/publish_profile.xml APP_NAME RETAIN_DAYS
PUBLISH_PROFILE_FILE=${1:-}
APP_NAME=${2:-}
RETAIN_DAYS=${3:-30}
if [ -z "$PUBLISH_PROFILE_FILE" ] || [ -z "$APP_NAME" ]; then
  echo "Usage: $0 <publish-profile-xml> <webapp-name> [retain-days]" >&2
  exit 2
fi

KUDU_USER=$(grep -o 'userName="[^"]*"' "$PUBLISH_PROFILE_FILE" | head -1 | sed -E 's/userName="([^"]+)"/\1/')
KUDU_PWD=$(grep -o 'userPWD="[^"]*"' "$PUBLISH_PROFILE_FILE" | head -1 | sed -E 's/userPWD="([^"]+)"/\1/')


API="https://${APP_NAME}.scm.azurewebsites.net/api/command"
CMD="find /home/data/Backups -type f -mtime +$RETAIN_DAYS -print -delete || true && ls -l /home/data/Backups"

echo "Cleaning up backups older than $RETAIN_DAYS days on ${APP_NAME} via Kudu..."
curl -s -X POST -u "$KUDU_USER:$KUDU_PWD" -H "Content-Type: application/json" -d "{\"command\":\"$CMD\"}" "$API" | jq .

echo "Cleanup complete."
