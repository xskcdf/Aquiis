#!/usr/bin/env bash
set -euo pipefail
# Usage: ./kudu-create-data-dir.sh /path/to/publish_profile.xml APP_NAME
PUBLISH_PROFILE_FILE=${1:-}
APP_NAME=${2:-}
if [ -z "$PUBLISH_PROFILE_FILE" ] || [ -z "$APP_NAME" ]; then
  echo "Usage: $0 <publish-profile-xml> <webapp-name>" >&2
  exit 2
fi

KUDU_USER=$(grep -o 'userName="[^"]*"' "$PUBLISH_PROFILE_FILE" | head -1 | sed -E 's/userName="([^"]+)"/\1/')
KUDU_PWD=$(grep -o 'userPWD="[^"]*"' "$PUBLISH_PROFILE_FILE" | head -1 | sed -E 's/userPWD="([^"]+)"/\1/')

API="https://${APP_NAME}.scm.azurewebsites.net/api/command"
CMD='mkdir -p /home/data && chmod 775 /home/data'

echo "Creating /home/data on ${APP_NAME} via Kudu..."
curl -s -X POST -u "$KUDU_USER:$KUDU_PWD" -H "Content-Type: application/json" -d "{\"command\":\"$CMD\"}" "$API" | jq .

echo "Done."
