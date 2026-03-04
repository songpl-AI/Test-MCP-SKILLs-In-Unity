#!/bin/bash

# OpenMCP Connection Check Script
# Usage: ./check_mcp.sh [timeout_seconds]

PORT=23456
URL="http://localhost:$PORT/api/v1/status"
TIMEOUT=${1:-30} # Default timeout 30s
INTERVAL=2

echo "Checking OpenMCP connection on port $PORT..."

start_time=$(date +%s)

while true; do
    # Try to connect with a short timeout
    response=$(curl -s -o /dev/null -w "%{http_code}" --max-time 2 "$URL")
    
    if [ "$response" == "200" ]; then
        echo "✅ OpenMCP is connected and ready!"
        exit 0
    fi

    current_time=$(date +%s)
    elapsed=$((current_time - start_time))

    if [ $elapsed -ge $TIMEOUT ]; then
        echo "❌ Connection timed out after $TIMEOUT seconds."
        echo "Possible reasons:"
        echo "1. Unity Editor is not running."
        echo "2. Unity is compiling scripts (Domain Reload)."
        echo "3. Unity is paused in debugger."
        echo "4. Port $PORT is occupied."
        exit 1
    fi

    echo "⏳ Waiting for connection... (Elapsed: ${elapsed}s)"
    sleep $INTERVAL
done
