#!/bin/bash

export $(cat .env | xargs)

echo "Running migrations..."

dotnet ef migrations add InitialCreate
dotnet ef database update
