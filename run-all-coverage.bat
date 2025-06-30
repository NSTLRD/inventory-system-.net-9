@echo off
echo Ejecutando pruebas de ambos microservicios con cobertura...

set outputDir=.\coverage-reports\combined

:: Crear directorio para reportes si no existe
if not exist "%outputDir%" mkdir "%outputDir%"

:: Ejecutar scripts individuales
echo Ejecutando tests de ms-products...
call run-products-coverage.bat

echo Ejecutando tests de ms-Inventory...
call run-inventory-coverage.bat

:: Generar informe combinado
echo Generando informe combinado...

:: Crear lista de archivos de cobertura
set reportFiles=

for /r ".\coverage-reports" %%i in (coverage.cobertura.xml) do (
    if defined reportFiles (
        set "reportFiles=!reportFiles!;%%i"
    ) else (
        set "reportFiles=%%i"
    )
)

:: Generar informe HTML combinado
if defined reportFiles (
    reportgenerator -reports:"%reportFiles%" -targetdir:"%outputDir%\html" -reporttypes:Html;HtmlSummary -title:"Cobertura de Pruebas - Sistema de Inventario"

    echo Informe combinado de cobertura generado en: %outputDir%\html\index.html
    
    :: Abrir el informe HTML
    start "" "%outputDir%\html\index.html"
) else (
    echo ERROR: No se encontraron archivos de cobertura. Asegurate de que los tests se ejecutaron correctamente.
)
