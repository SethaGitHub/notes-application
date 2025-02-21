#!/bin/bash

echo "SQL Server is ready."

dotnet ef database update

exec dotnet NoteAPI.dll
