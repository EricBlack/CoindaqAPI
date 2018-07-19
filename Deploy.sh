#!/bin/bash
echo "[Step1]: pull git code"
git pull origin milestone_project

echo "[Step2]: restore project"
dotnet restore

echo "[Step3]: build project"
dotnet build

echo "[Step4]: restart supervisor service"
/etc/init.d/supervisor restart