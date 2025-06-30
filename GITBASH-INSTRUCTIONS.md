# Quick Instructions for Git Bash Users

Since you're using Git Bash (MINGW64), follow these steps to clean up your repository for both microservices (ms-Inventory and ms-products):

1. Make sure you're in the repository directory:
   ```
   pwd
   ```
   The output should show you're in `/c/Users/starling/workspace-C#-net-asp/ms-inventory-sistem`

2. Run these commands one by one to clean **both microservices**:

   ```
   # Clean ms-Inventory microservice
   
   # Remove DLL files from Git tracking (not from filesystem)
   git rm --cached -r "**/*.dll" --ignore-unmatch
   git rm --cached -r "ms-Inventory/bin/**/*.dll" --ignore-unmatch
   
   # Remove PDB files from Git tracking
   git rm --cached -r "**/*.pdb" --ignore-unmatch
   git rm --cached -r "ms-Inventory/bin/**/*.pdb" --ignore-unmatch
   
   # Remove bin directories from Git tracking
   git rm --cached -r "**/bin/**" --ignore-unmatch
   git rm --cached -r "ms-Inventory/bin/**" --ignore-unmatch
   
   # Remove obj directories from Git tracking
   git rm --cached -r "**/obj/**" --ignore-unmatch
   git rm --cached -r "ms-Inventory/obj/**" --ignore-unmatch
   
   # Clean ms-products microservice
   
   # Remove DLL files from Git tracking
   git rm --cached -r "ms-products/bin/**/*.dll" --ignore-unmatch
   
   # Remove PDB files from Git tracking
   git rm --cached -r "ms-products/bin/**/*.pdb" --ignore-unmatch
   
   # Remove bin directories from Git tracking
   git rm --cached -r "ms-products/bin/**" --ignore-unmatch
   
   # Remove obj directories from Git tracking
   git rm --cached -r "ms-products/obj/**" --ignore-unmatch
   
   # Remove EXE files from Git tracking for both microservices
   git rm --cached -r "**/*.exe" --ignore-unmatch
   git rm --cached -r "ms-Inventory/bin/**/*.exe" --ignore-unmatch
   git rm --cached -r "ms-products/bin/**/*.exe" --ignore-unmatch
   ```

3. Commit the changes:
   ```
   git commit -m "Remove binary files from Git tracking for both microservices"
   ```

4. Push to the remote repository:
   ```
   git push
   ```

You can also try to make the bash script executable and run it (which includes all the commands for both microservices):
```
chmod +x clean-git-cache.sh
./clean-git-cache.sh
```

5. Verify the results with:
   ```
   git status
   ```
   
   This should show that the binary files are no longer tracked.
