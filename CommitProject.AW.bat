@echo off
cd /d "C:\Project\NetCore\AW"
git add .
set /p userInput=Enter your commit message:
git commit -m "%userInput%"
git pull origin
git push origin
pause