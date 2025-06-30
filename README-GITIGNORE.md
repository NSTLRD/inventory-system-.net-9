# Git Repository Cleanup Guide

This document explains how to properly clean up binary and generated files from Git tracking while maintaining them in your local filesystem.

## Problem

Despite having a correctly configured `.gitignore` file, Git continues to track binary files (like `.dll`, `.pdb`, etc.) that were committed to the repository before the `.gitignore` rules were implemented.

## Solution

1. The `.gitignore` file is already configured to ignore:
   - All binary files (`*.dll`, `*.exe`, `*.pdb`)
   - Build output folders (`bin/`, `obj/`)
   - Temporary files and logs
   - Test results and coverage reports

2. To untrack already tracked files while keeping them on your filesystem:

   Run the provided batch script:
   ```
   clean-git-index.bat
   ```

   This script executes the following Git commands:
   ```
   git rm --cached -r "**/*.dll" --ignore-unmatch
   git rm --cached -r "**/*.pdb" --ignore-unmatch
   git rm --cached -r "**/bin/**" --ignore-unmatch
   git rm --cached -r "**/obj/**" --ignore-unmatch
   git rm --cached -r "**/*.exe" --ignore-unmatch
   ```

3. After running the script, commit the changes:
   ```
   git commit -m "Remove binary files from Git tracking"
   ```

4. Push the changes to your repository:
   ```
   git push
   ```

## Important Notes

- The `--cached` flag ensures files are only removed from Git tracking, not from your filesystem
- The `--ignore-unmatch` flag prevents errors if no matching files are found
- The `-r` flag allows recursive removal in subfolders

## Future Maintenance

1. Always ensure new binary files or build output are ignored by checking the `.gitignore` file
2. If new types of files need to be ignored, add them to the `.gitignore` file
3. Periodically verify no binary files are being tracked with:
   ```
   git ls-files | grep -E "\.(dll|exe|pdb)$"
   ```

## Verification

After pushing changes, you can verify that binary files are no longer tracked by cloning the repository to a new location and confirming that binary files are not present in the fresh clone.
