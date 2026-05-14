# Sapal.Cad.Plugin

Plugin C# para AutoCAD .NET API.

## Compilacion

El proyecto apunta por defecto a AutoCAD 2024:

```text
C:\Program Files\Autodesk\AutoCAD 2024
```

Si AutoCAD esta instalado en otra ruta, compilar indicando `AutoCADApiPath`:

```powershell
dotnet msbuild .\src\Sapal.Cad.Plugin\Sapal.Cad.Plugin.csproj /p:AutoCADApiPath="C:\Program Files\Autodesk\AutoCAD 2024"
```

El plugin resultante se carga en AutoCAD con `NETLOAD`.

## Comandos previstos

- `SAPAL_CONFIG`
- `SAPAL_CAPTURAR_LINEA`
- `SAPAL_CAPTURAR_POZO`
- `SAPAL_CONSULTAR`
- `SAPAL_VALIDAR`
- `SAPAL_LIMPIAR_XDATA`

## Organizacion

- `Commands`: comandos expuestos a AutoCAD.
- `Models`: modelos de datos tecnicos.
- `Services`: servicios para AppID, XDATA y operaciones CAD.
- `Validation`: validaciones de catalogos, campos y geometria.
- `UI`: formularios y ventanas de captura.
