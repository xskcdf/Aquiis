#!/usr/bin/env bash
set -euo pipefail
# Usage: ./kudu-backup-db.sh /path/to/publish_profile.xml APP_NAME [/home/data/app.db]
PUBLISH_PROFILE_FILE=${1:-}
APP_NAME=${2:-}
SRC_DB_PATH=${3:-/home/data/app_v0.0.0.db}
if [ -z "$PUBLISH_PROFILE_FILE" ] || [ -z "$APP_NAME" ]; then
  echo "Usage: $0 <publish-profile-xml> <webapp-name> [src-db-path]" >&2
  exit 2
fi

KUDU_USER=$(grep -o 'userName="[^"]*"' "$PUBLISH_PROFILE_FILE" | head -1 | sed -E 's/userName="([^"]+)"/\1/')
KUDU_PWD=$(grep -o 'userPWD="[^"]*"' "$PUBLISH_PROFILE_FILE" | head -1 | sed -E 's/userPWD="([^"]+)"/\1/')


TS=$(date +%Y%m%d%H%M%S)
BACKUP_PATH="/home/data/Backups/$(basename $SRC_DB_PATH).$TS"

API="https://${APP_NAME}.scm.azurewebsites.net/api/command"
CMD="mkdir -p /home/data/Backups && cp '$SRC_DB_PATH' '$BACKUP_PATH' && ls -l /home/data/Backups"

echo "Backing up $SRC_DB_PATH -> $BACKUP_PATH on ${APP_NAME} via Kudu..."
curl -s -X POST -u "$KUDU_USER:$KUDU_PWD" -H "Content-Type: application/json" -d "{\"command\":\"$CMD\"}" "$API" | jq .

echo "Backup complete: $BACKUP_PATH"
