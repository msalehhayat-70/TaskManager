#!/bin/bash
# ── Run TaskManager locally (no Docker, no database server needed) ──────────
set -e

echo "Starting TaskManager..."
echo "Using SQLite database (auto-created as src/taskmanager.db)"
echo ""

cd "$(dirname "$0")/src"

# Set development environment
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://localhost:5000

dotnet run

echo ""
echo "App running at: http://localhost:5000"
echo "Default login:  admin@taskmanager.com / Admin@123"
