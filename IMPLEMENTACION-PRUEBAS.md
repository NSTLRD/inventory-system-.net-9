# Implementación de Pruebas Unitarias y Cobertura

Este documento proporciona una guía para la implementación de pruebas unitarias y cobertura de código para los microservicios ms-products y ms-inventory.

## Estructura del Proyecto de Pruebas

### ms-products/NewTest
- ProductsControllerTests.cs - Pruebas para los endpoints de productos
- Mocks e interfaces para realizar pruebas de controladores

### ms-Inventory/Inventory.Tests.Controllers
- Controllers/InventoryControllerTests.cs - Pruebas para el controlador de inventario
- Mocks e interfaces para realizar pruebas de controladores

## Patrones y Prácticas Implementadas

1. **Mocking con Moq**
   - Se utilizan mocks para aislar las pruebas de las dependencias externas
   - Ejemplo: `_mediatorMock`, `_currencyServiceMock`, `_loggerMock`, etc.

2. **Aserciones Fluidas con FluentAssertions**
   - Sintaxis más legible y expresiva
   - Ejemplo: `result.Should().BeOfType<OkObjectResult>()`

3. **Estructura AAA (Arrange-Act-Assert)**
   - Arrange: Preparar los datos y mocks
   - Act: Ejecutar la acción a probar
   - Assert: Verificar el resultado

4. **Utilización de Interfaces para Compatibilidad**
   - Interfaz `IMediator` para abstraer la comunicación con MediatR
   - Interfaces para DTOs y modelos para facilitar el testing entre versiones de .NET

5. **SetUp con NUnit**
   - Método `[SetUp]` para inicializar las dependencias antes de cada prueba
   - `[TestFixture]` para marcar las clases de prueba

## Cobertura de Código

Se ha configurado la generación de informes de cobertura utilizando scripts de PowerShell:
- **Coverlet**: Para recopilar métricas durante la ejecución de pruebas
- **ReportGenerator**: Para generar informes HTML visuales

### Scripts de Automatización

Se han creado tres scripts de PowerShell para ejecutar pruebas y generar informes de cobertura:

1. `run-products-coverage.ps1`: Ejecuta pruebas y genera informe para ms-products
2. `run-inventory-coverage.ps1`: Ejecuta pruebas y genera informe para ms-inventory
3. `run-all-coverage.ps1`: Ejecuta ambos scripts anteriores y genera un informe combinado

### Ejecutar Pruebas con Cobertura

Para ejecutar las pruebas con cobertura, simplemente utilice uno de los scripts de PowerShell desde la raíz del proyecto:

```powershell
# Para ejecutar todas las pruebas y generar informes de cobertura
.\run-all-coverage.ps1

# Para ejecutar solo las pruebas de productos
.\run-products-coverage.ps1

# Para ejecutar solo las pruebas de inventario
.\run-inventory-coverage.ps1
```

Los informes de cobertura se generarán automáticamente y se abrirán en su navegador predeterminado.

## Enfoque de Pruebas y Desafíos

### Desafío de Compatibilidad entre .NET 9 y Pruebas

El proyecto principal utiliza .NET 9, pero debido a limitaciones de compatibilidad con las herramientas de prueba, los proyectos de prueba se configuraron en .NET 7. Para resolver este problema:

1. Se crearon proyectos de interfaces (`Products.Interfaces` e `Inventory.Interfaces`) utilizando `netstandard2.0`
2. Las pruebas se centran en verificar el comportamiento de los controladores a través de estas interfaces
3. Los informes de cobertura reflejan la cobertura de las interfaces

### Estructura de Proyectos de Prueba

- **NewTest** (para ms-products): Dirigido a .NET 7, utiliza NUnit, Moq y FluentAssertions
- **Inventory.Tests.Controllers** (para ms-inventory): Dirigido a .NET 7, utiliza NUnit, Moq y FluentAssertions

## Resultados Esperados

Los informes de cobertura generados muestran:
- Porcentaje de cobertura para los proyectos de interfaces
- Número de pruebas ejecutadas y su resultado
- Sugerencias para mejorar la cobertura

## Solución de Problemas

Si experimentas problemas con los scripts de cobertura:

1. Asegúrate de tener instalado ReportGenerator como herramienta global:
```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

2. Verifica que los proyectos de prueba se compilen correctamente:
```powershell
cd ms-products
dotnet build NewTest/NewTest.csproj

cd ../ms-Inventory
dotnet build Inventory.Tests.Controllers/Inventory.Tests.Controllers.csproj
```

3. Si los informes no se generan correctamente, verifica la existencia de los directorios de salida:
```powershell
# Estos directorios deben existir:
# .\coverage-reports\products\html
# .\coverage-reports\inventory\html
# .\coverage-reports\combined\html
```

