#!/bin/bash

# Limpiar cualquier artefacto de build previo
echo "Limpiando proyectos..."
cd ms-Inventory
dotnet clean
cd ../ms-products
dotnet clean
cd ..

# Construir las imágenes
echo "Construyendo imágenes Docker..."
docker-compose build --no-cache

echo "Proceso finalizado"
