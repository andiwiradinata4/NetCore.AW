@echo off
cd /d "C:\Project\NetCore\AW"
git pull origin
set /p userInput=Open Project Solution? [y/n]:
if /i "%userInput%"=="y" (
	start "" "C:\Project\NetCore\AW\AW.Web\AW.Web.sln"
)
pause