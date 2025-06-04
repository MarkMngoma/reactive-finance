#!/bin/bash

BUILD_CONFIGURATION="Release"
PROJECT_PATH="Server/Server.csproj"
OUTPUT_DIR="publish"
PORT=5050

echo "Cleaning previous builds..."
rm -rf $OUTPUT_DIR/
dotnet clean

echo "Restoring dependencies..."
dotnet restore $PROJECT_PATH

echo "Building..."
dotnet build $PROJECT_PATH -c $BUILD_CONFIGURATION --no-restore

echo "Publishing..."
dotnet publish $PROJECT_PATH -c $BUILD_CONFIGURATION -o $OUTPUT_DIR --no-build

echo "Starting application..."
ASPNETCORE_URLS="http://*:$PORT" dotnet $OUTPUT_DIR/Server.dll
