@echo off
echo Limpiando proyectos...
cd ms-Inventory
dotnet clean
cd ..\ms-products
dotnet clean
cd ..

echo Construyendo imágenes Docker...
docker-compose build --no-cache

echo Proceso finalizado
