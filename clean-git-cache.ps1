# Script para eliminar archivos binarios del índice de Git sin eliminarlos del sistema de archivos

Write-Host "Eliminando archivos .dll del índice de Git..." -ForegroundColor Cyan

# Eliminar archivos .dll de ms-Inventory
git rm --cached -r "ms-Inventory/bin/Debug/net9.0/**/*.dll"

# Eliminar archivos .dll de ms-products
git rm --cached -r "ms-products/bin/Debug/net9.0/**/*.dll"
git rm --cached -r "bin/Debug/net9.0/**/*.dll"

# Eliminar archivos relacionados
git rm --cached -r "ms-Inventory/bin/Debug/net9.0/**/*.exe"
git rm --cached -r "ms-products/bin/Debug/net9.0/**/*.exe"
git rm --cached -r "bin/Debug/net9.0/**/*.exe"

git rm --cached -r "ms-Inventory/bin/Debug/net9.0/**/*.json"
git rm --cached -r "ms-products/bin/Debug/net9.0/**/*.json"
git rm --cached -r "bin/Debug/net9.0/**/*.json"

git rm --cached -r "ms-Inventory/bin/Debug/net9.0/**/*.pdb"
git rm --cached -r "ms-products/bin/Debug/net9.0/**/*.pdb"
git rm --cached -r "bin/Debug/net9.0/**/*.pdb"

# Eliminar los archivos de obj
git rm --cached -r "ms-Inventory/obj/Debug/net9.0/ms-Inventory.dll"
git rm --cached -r "ms-Inventory/obj/Debug/net9.0/ms-Inventory.pdb"
git rm --cached -r "ms-Inventory/obj/Debug/net9.0/ref/ms-Inventory.dll"
git rm --cached -r "ms-Inventory/obj/Debug/net9.0/refint/ms-Inventory.dll"

git rm --cached -r "ms-products/obj/Debug/net9.0/ms-products.dll"
git rm --cached -r "ms-products/obj/Debug/net9.0/ms-products.pdb"
git rm --cached -r "ms-products/obj/Debug/net9.0/ref/ms-products.dll"
git rm --cached -r "ms-products/obj/Debug/net9.0/refint/ms-products.dll"

git rm --cached -r "obj/Debug/net9.0/ms-products.dll"
git rm --cached -r "obj/Debug/net9.0/ms-products.pdb"
git rm --cached -r "obj/Debug/net9.0/ref/ms-products.dll"
git rm --cached -r "obj/Debug/net9.0/refint/ms-products.dll"

Write-Host "Limpieza completada. Ahora puedes realizar un commit para confirmar estos cambios." -ForegroundColor Green
Write-Host "Usa 'git status' para verificar que se han eliminado correctamente del seguimiento." -ForegroundColor Yellow
