# Git Cleanup Manual Commands

## Git Bash (MINGW) Commands

Since you're using Git Bash, use these Unix-style commands:

```bash
# You're already in the correct directory
# ~/workspace-C#-net-asp/ms-inventory-sistem

# Clean both microservices (ms-Inventory and ms-products)

# Remove DLL files from Git tracking (not from filesystem)
git rm --cached -r "**/*.dll" --ignore-unmatch
git rm --cached -r "ms-Inventory/bin/**/*.dll" --ignore-unmatch
git rm --cached -r "ms-products/bin/**/*.dll" --ignore-unmatch

# Remove PDB files from Git tracking
git rm --cached -r "**/*.pdb" --ignore-unmatch
git rm --cached -r "ms-Inventory/bin/**/*.pdb" --ignore-unmatch
git rm --cached -r "ms-products/bin/**/*.pdb" --ignore-unmatch

# Remove bin directories from Git tracking
git rm --cached -r "**/bin/**" --ignore-unmatch
git rm --cached -r "ms-Inventory/bin/**" --ignore-unmatch
git rm --cached -r "ms-products/bin/**" --ignore-unmatch

# Remove obj directories from Git tracking
git rm --cached -r "**/obj/**" --ignore-unmatch
git rm --cached -r "ms-Inventory/obj/**" --ignore-unmatch
git rm --cached -r "ms-products/obj/**" --ignore-unmatch

# Remove EXE files from Git tracking
git rm --cached -r "**/*.exe" --ignore-unmatch
git rm --cached -r "ms-Inventory/bin/**/*.exe" --ignore-unmatch
git rm --cached -r "ms-products/bin/**/*.exe" --ignore-unmatch

# Commit the changes
git commit -m "Remove binary files from Git tracking for both microservices"

# Push to remote repository
git push
```

## Windows CMD or PowerShell Commands

If you switch to Windows CMD or PowerShell, use these commands instead:

```cmd
# Navigate to your project directory
cd C:\Users\starling\workspace-C#-net-asp\ms-inventory-sistem

# Clean both microservices (ms-Inventory and ms-products)

# Remove DLL files from Git tracking (not from filesystem)
git rm --cached -r "**/*.dll" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**\*.dll" --ignore-unmatch
git rm --cached -r "ms-products\bin\**\*.dll" --ignore-unmatch

# Remove PDB files from Git tracking
git rm --cached -r "**/*.pdb" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**\*.pdb" --ignore-unmatch
git rm --cached -r "ms-products\bin\**\*.pdb" --ignore-unmatch

# Remove bin directories from Git tracking
git rm --cached -r "**/bin/**" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**" --ignore-unmatch
git rm --cached -r "ms-products\bin\**" --ignore-unmatch

# Remove obj directories from Git tracking
git rm --cached -r "**/obj/**" --ignore-unmatch
git rm --cached -r "ms-Inventory\obj\**" --ignore-unmatch
git rm --cached -r "ms-products\obj\**" --ignore-unmatch

# Remove EXE files from Git tracking
git rm --cached -r "**/*.exe" --ignore-unmatch
git rm --cached -r "ms-Inventory\bin\**\*.exe" --ignore-unmatch
git rm --cached -r "ms-products\bin\**\*.exe" --ignore-unmatch

# Commit the changes
git commit -m "Remove binary files from Git tracking for both microservices"

# Push to remote repository
git push
```

These commands will untrack the binary files from Git while keeping them in your local filesystem.
