# Compilacion, instalacion y uso en AutoCAD

Este documento describe como compilar el plugin `Sapal.Cad.Plugin`, donde se genera el archivo DLL y como cargarlo en AutoCAD 2027.

## Requisitos

- AutoCAD 2027 instalado.
- SDK de .NET 10.
- Repositorio local en:

```text
C:\Users\pvargas\Desktop\Plugin_Obras
```

## SDK de .NET 10 portable

En esta maquina se uso el SDK portable descargado y extraido en:

```text
C:\Users\pvargas\Downloads\dotnet-sdk-10.0.300-win-x64
```

El ejecutable usado para compilar es:

```text
C:\Users\pvargas\Downloads\dotnet-sdk-10.0.300-win-x64\dotnet.exe
```

Nota: este SDK fue descargado como archivo `.zip`, por lo que no se instala como un programa tradicional. Se usa directamente desde la carpeta extraida.

## Compilar el plugin

Abrir PowerShell en la carpeta del repositorio:

```powershell
cd C:\Users\pvargas\Desktop\Plugin_Obras
```

Ejecutar la compilacion:

```powershell
& "C:\Users\pvargas\Downloads\dotnet-sdk-10.0.300-win-x64\dotnet.exe" build .\src\Sapal.Cad.Plugin\Sapal.Cad.Plugin.csproj /p:AutoCADApiPath="C:\Program Files\Autodesk\AutoCAD 2027"
```

Una compilacion correcta debe terminar con:

```text
0 Errores
```

Pueden aparecer advertencias de referencias internas entre AutoCAD y .NET. Mientras no existan errores, el DLL se genera correctamente.

## Ubicacion del plugin compilado

El archivo principal del plugin es:

```text
C:\Users\pvargas\Desktop\Plugin_Obras\src\Sapal.Cad.Plugin\bin\Debug\net10.0-windows\Sapal.Cad.Plugin.dll
```

Ese archivo `.dll` es el que se carga en AutoCAD.

## Cargar el plugin en AutoCAD

1. Abrir AutoCAD 2027.
2. En la linea de comandos escribir:

```text
NETLOAD
```

3. Seleccionar el archivo:

```text
C:\Users\pvargas\Desktop\Plugin_Obras\src\Sapal.Cad.Plugin\bin\Debug\net10.0-windows\Sapal.Cad.Plugin.dll
```

4. Confirmar la carga del plugin.

## Carpeta confiable en AutoCAD

Si AutoCAD bloquea el DLL por seguridad, agregar la carpeta del plugin a las rutas confiables.

Ruta sugerida:

```text
C:\Users\pvargas\Desktop\Plugin_Obras\src\Sapal.Cad.Plugin\bin\Debug\net10.0-windows
```

En AutoCAD:

1. Ejecutar `OPTIONS`.
2. Ir a la pestana `Files`.
3. Abrir `Trusted Locations`.
4. Agregar la ruta anterior.
5. Volver a ejecutar `NETLOAD`.

## Verificar carga del plugin

Despues de cargar el DLL, ejecutar:

```text
SAPAL_CONFIG
```

Si el plugin esta cargado correctamente, AutoCAD mostrara un mensaje indicando que se verifico la configuracion `SAPAL_RED`.

## Comandos disponibles

### SAPAL_CONFIG

Prepara el dibujo:

- Registra el AppID `SAPAL_RED`.
- Crea capas recomendadas si no existen:
  - `SAPAL_AGUA`
  - `SAPAL_DRENAJE`
  - `SAPAL_PLUVIAL`
  - `SAPAL_POZOS_DRENAJE`

### SAPAL_CAPTURAR_LINEA

Permite seleccionar una linea o polilinea y capturar datos tecnicos.

Flujo:

1. Ejecutar `SAPAL_CAPTURAR_LINEA`.
2. Seleccionar una entidad tipo Line o Polyline.
3. Capturar los datos solicitados en la linea de comandos.
4. El plugin calcula `longitud_dibujo`.
5. Guarda los datos como XDATA bajo `SAPAL_RED`.
6. Mueve la entidad a la capa correspondiente segun `tipo_elemento`.

Tipos aceptados:

```text
LINEA_AGUA
LINEA_DRENAJE
LINEA_AGUA_PLUVIAL
```

### SAPAL_CAPTURAR_POZO

Permite seleccionar un bloque o punto y capturar datos tecnicos de un pozo de drenaje.

Flujo:

1. Ejecutar `SAPAL_CAPTURAR_POZO`.
2. Seleccionar un bloque o punto.
3. Capturar los datos solicitados.
4. Guarda los datos como XDATA bajo `SAPAL_RED`.
5. Mueve la entidad a la capa `SAPAL_POZOS_DRENAJE`.

### SAPAL_CONSULTAR

Permite consultar los datos guardados en una entidad.

Flujo:

1. Ejecutar `SAPAL_CONSULTAR`.
2. Seleccionar una entidad.
3. AutoCAD muestra los valores XDATA asociados a `SAPAL_RED`.

### SAPAL_VALIDAR

Recorre el espacio modelo y genera un resumen:

- Total de entidades con XDATA `SAPAL_RED`.
- Total de lineas.
- Total de pozos.
- Entidades sin `id_elemento`.
- Tipos invalidos.
- Identificadores duplicados.

### SAPAL_LIMPIAR_XDATA

Permite limpiar los datos XDATA `SAPAL_RED` de una entidad seleccionada.

Flujo:

1. Ejecutar `SAPAL_LIMPIAR_XDATA`.
2. Seleccionar la entidad.
3. Confirmar con `Si`.
4. El plugin limpia los datos `SAPAL_RED` de esa entidad.

## Flujo recomendado de prueba

1. Abrir AutoCAD 2027.
2. Cargar el DLL con `NETLOAD`.
3. Ejecutar `SAPAL_CONFIG`.
4. Dibujar una polilinea.
5. Ejecutar `SAPAL_CAPTURAR_LINEA`.
6. Capturar un `id_elemento`, por ejemplo:

```text
AG-000001
```

7. Seleccionar `LINEA_AGUA`.
8. Capturar el resto de campos.
9. Ejecutar `SAPAL_CONSULTAR` sobre la misma polilinea.
10. Ejecutar `SAPAL_VALIDAR`.

## Notas

- El plugin guarda los datos tecnicos como XDATA dentro de la entidad CAD.
- El AppID usado es `SAPAL_RED`.
- El DXF exportado desde AutoCAD debe conservar esa informacion XDATA.
- En esta fase la captura se realiza por linea de comandos, no por formulario.
