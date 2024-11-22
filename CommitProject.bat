@echo off
cd /d "D:\My Project\NETCore\AW"
git add .
set /p userInput=Enter your commit message:
git commit -m "%userInput%"
git pull origin
git push origin
pause