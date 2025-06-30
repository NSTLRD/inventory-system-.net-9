@echo off
echo Ejecutando pruebas de ms-Inventory con cobertura...

set projectPath=.\ms-Inventory
set testProject=.\ms-Inventory\Inventory.Tests\Inventory.Tests.Controllers
set outputDir=.\coverage-reports\inventory

:: Crear directorio para reportes si no existe
if not exist "%outputDir%" mkdir "%outputDir%"

:: Limpiar proyecto
dotnet clean %projectPath%

:: Restaurar dependencias
dotnet restore %projectPath%

:: Ejecutar tests con coverlet para generar el informe de cobertura
dotnet test %testProject% --collect:"XPlat Code Coverage" --results-directory:%outputDir% --verbosity:normal

:: Buscar el archivo de cobertura
for /r "%outputDir%" %%i in (coverage.cobertura.xml) do set coverageFile=%%i

:: Generar informe HTML usando ReportGenerator
if defined coverageFile (
    reportgenerator -reports:"%coverageFile%" -targetdir:"%outputDir%\html" -reporttypes:Html;HtmlSummary

    echo Informe de cobertura generado en: %outputDir%\html\index.html
    
    :: Abrir el informe HTML
    start "" "%outputDir%\html\index.html"
) else (
    echo ERROR: No se encontro el archivo de cobertura. Asegurate de que los tests se ejecutaron correctamente.
)
