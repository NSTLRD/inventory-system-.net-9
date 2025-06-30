@echo off
echo Cleaning Git index of tracked binary files for both microservices...

echo Cleaning ms-Inventory microservice...
git rm --cached -r "**/*.dll" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**\*.dll" --ignore-unmatch
git rm --cached -r "**/*.pdb" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**\*.pdb" --ignore-unmatch
git rm --cached -r "**/bin/**" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**" --ignore-unmatch
git rm --cached -r "**/obj/**" --ignore-unmatch
git rm --cached -r "ms-Inventory\obj\**" --ignore-unmatch
git rm --cached -r "**/*.exe" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**\*.exe" --ignore-unmatch

echo.
echo Cleaning ms-products microservice...
git rm --cached -r "ms-products\bin\**\*.dll" --ignore-unmatch
git rm --cached -r "ms-products\bin\**\*.pdb" --ignore-unmatch
git rm --cached -r "ms-products\bin\**" --ignore-unmatch
git rm --cached -r "ms-products\obj\**" --ignore-unmatch
git rm --cached -r "ms-products\bin\**\*.exe" --ignore-unmatch

echo.
echo Files have been removed from Git tracking but kept on your local system.
echo Now commit these changes with: git commit -m "Remove binary files from Git tracking for both microservices"
echo.
pause
