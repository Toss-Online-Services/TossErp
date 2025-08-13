#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR=$(cd "$(dirname "$0")/.." && pwd)

echo "[1/4] Preparing env"
if [ ! -f "$ROOT_DIR/infra/docker/.env" ]; then
  echo "No .env found, creating from sample..."
  if [ -f "$ROOT_DIR/infra/docker/.env.example" ]; then
    cp "$ROOT_DIR/infra/docker/.env.example" "$ROOT_DIR/infra/docker/.env" || true
  elif [ -f "$ROOT_DIR/infra/docker/env.sample" ]; then
    cp "$ROOT_DIR/infra/docker/env.sample" "$ROOT_DIR/infra/docker/.env" || true
  fi
fi

echo "[2/4] Building containers"
docker compose -f "$ROOT_DIR/infra/docker/docker-compose.yml" build

echo "[3/4] Starting stack"
docker compose -f "$ROOT_DIR/infra/docker/docker-compose.yml" up -d

echo "[4/4] Web dev dependencies"
if command -v npm >/dev/null 2>&1; then
  (cd "$ROOT_DIR/TossErp.Web" && npm ci || true)
fi

echo "Done. Gateway: http://localhost:8080  Web: http://localhost:3000"


