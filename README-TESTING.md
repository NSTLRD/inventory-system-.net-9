# Pruebas Unitarias y Cobertura de Código

Este proyecto incluye pruebas unitarias para los microservicios `ms-products` y `ms-inventory`. A continuación, se explica cómo ejecutar estas pruebas y generar informes de cobertura de código.

## Estructura de las Pruebas

- **ms-products/NewTest**: Contiene pruebas para el microservicio de productos.
  - `ProductsControllerTests.cs`: Pruebas para el controlador de productos.
  
- **ms-Inventory/Inventory.Tests.Controllers**: Contiene pruebas para el microservicio de inventario.
  - `Controllers/InventoryControllerTests.cs`: Pruebas para el controlador de inventario.

## Ejecución de Pruebas Unitarias

### Opción 1: Ejecutar todas las pruebas con cobertura

Para ejecutar todas las pruebas y generar informes de cobertura para ambos microservicios:

1. En la raíz del proyecto, ejecute:
   ```powershell
   .\run-all-coverage.ps1
   ```
   
Este script ejecutará todas las pruebas y abrirá los informes de cobertura HTML combinados para ambos microservicios.

### Opción 2: Ejecutar pruebas para un microservicio específico

- Para ejecutar solo las pruebas de productos con cobertura:
  ```powershell
  .\run-products-coverage.ps1
  ```

- Para ejecutar solo las pruebas de inventario con cobertura:
  ```powershell
  .\run-inventory-coverage.ps1
  ```

### Opción 3: Ejecutar solo pruebas sin cobertura

- Para ejecutar solo las pruebas sin generar informes de cobertura, puede usar directamente comandos dotnet test:
  ```powershell
  cd ms-products
  dotnet test NewTest/NewTest.csproj
  
  cd ..\ms-Inventory
  dotnet test Inventory.Tests.Controllers/Inventory.Tests.Controllers.csproj
  ```

## Informes de Cobertura

Los informes de cobertura se generan en los siguientes directorios:

- **ms-products**: `coverage-reports\products\html\index.html`
- **ms-Inventory**: `coverage-reports\inventory\html\index.html`
- **Combinado**: `coverage-reports\combined\html\index.html`

Estos informes muestran:

- El porcentaje total de cobertura de código para las interfaces
- Métricas como cobertura de línea, rama y método
- Información sobre los tests ejecutados y sus resultados


## Herramientas Utilizadas

- **NUnit**: Framework de pruebas
- **Moq**: Biblioteca para crear mocks
- **FluentAssertions**: Biblioteca para aserciones más expresivas
- **Coverlet**: Herramienta para recopilar métricas de cobertura
- **ReportGenerator**: Herramienta para generar informes HTML a partir de datos de cobertura

## Mejores Prácticas Implementadas

1. **Aislamiento**: Pruebas que se ejecutan de forma aislada, utilizando mocks para dependencias externas
2. **Organización**: Estructura de pruebas que refleja la estructura del código
3. **Nomenclatura**: Nombres de pruebas claros que siguen el patrón `[Método]_[Escenario]_[ResultadoEsperado]`
4. **Aserciones**: Uso de FluentAssertions para aserciones más legibles
5. **Cobertura**: Generación de informes detallados de cobertura de código



## Enfoque de Cobertura y Generación de Informes


Los scripts de cobertura generan informes HTML simplificados que muestran:
- Resultados de ejecución de las pruebas
- Cobertura de código para las interfaces
- Recomendaciones para mejorar la cobertura

Consulta el archivo `IMPLEMENTACION-PRUEBAS.md` para obtener más detalles sobre el enfoque de pruebas y los desafíos actuales.
