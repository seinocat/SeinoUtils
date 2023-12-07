@echo off

if "%1" == "" (
echo %1
echo No Tags, Exiting...
exit /b 1
)

git subtree push --prefix=Unity/Assets/SeinoUtils https://github.com/seinocat/SeinoUtils.git upm

for /f "delims=" %%i in ('git log -n 1 --pretty^=format:%%H upm') do set commit_hash=%%i
echo upm hash: %commit_hash%

git tag -a %1 -m '%2' %commit_hash%
git push origin %1

echo %1 release  success