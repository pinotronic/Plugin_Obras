# ESPECIFICACIÓN PARA CODEX
# Plugin AutoCAD SAPAL Red → DXF → PostgreSQL/PostGIS → Web

## 1. Objetivo del proyecto

Desarrollar un plugin para AutoCAD que permita capturar información técnica asociada a elementos dibujados por el departamento de Obras.

El flujo general será:

```text
AutoCAD
  ↓
Plugin SAPAL Red
  ↓
DWG / DXF con datos técnicos en entidades
  ↓
Extractor DXF
  ↓
PostgreSQL / PostGIS
  ↓
API / servicios geográficos
  ↓
Página web
```

El objetivo principal es que los elementos dibujados en AutoCAD no sean únicamente geometría, sino entidades con información técnica estructurada.

## 2. Principios de diseño

### Claridad

El usuario de AutoCAD debe poder seleccionar un objeto, capturar datos en una ventana sencilla y guardar la información sin conocer detalles técnicos internos.

### Modularidad

El proyecto debe separar claramente:

- Plugin AutoCAD.
- Modelo de datos.
- Servicio de escritura y lectura de XDATA.
- Validaciones.
- Extractor DXF.
- Carga a PostgreSQL/PostGIS.
- Documentación.
- Pruebas.

### Evolución

La primera versión debe ser simple y funcional, pero preparada para crecer hacia:

- Catálogos controlados.
- Validación avanzada.
- Exportación GeoJSON.
- Carga directa a PostgreSQL.
- Integración con servicios web.
- Bitácora de cambios.
- Sincronización con SIG.

## 3. Alcance de la primera versión

La primera versión debe permitir:

- Crear comandos dentro de AutoCAD.
- Seleccionar una polilínea o línea para redes.
- Seleccionar un bloque o punto para pozos.
- Capturar datos técnicos.
- Guardar los datos como XDATA en la entidad seleccionada.
- Consultar los datos guardados.
- Validar entidades con datos incompletos.
- Exportar el dibujo como DXF usando las herramientas normales de AutoCAD.
- Crear un extractor independiente que lea el DXF y genere registros para PostgreSQL/PostGIS.

## 4. Tecnología sugerida

### Plugin AutoCAD

- Lenguaje: C#.
- Plataforma: .NET Framework compatible con la versión de AutoCAD instalada.
- API: AutoCAD .NET API.
- Persistencia dentro de entidades: XDATA.
- AppID XDATA: `SAPAL_RED`.

### Extractor DXF

- Lenguaje: Python.
- Librería sugerida: `ezdxf`.
- Base de datos: PostgreSQL con PostGIS.
- Librerías sugeridas:
  - `psycopg2` o `psycopg`
  - `shapely`
  - `pyproj`, si se requiere transformación de coordenadas

### Base de datos

- PostgreSQL.
- PostGIS.
- SRID recomendado: `32614`, salvo que el proyecto indique otro.

## 5. Tipos de elementos

El sistema debe soportar inicialmente estos elementos:

```text
LINEA_AGUA
LINEA_DRENAJE
LINEA_AGUA_PLUVIAL
POZO_DRENAJE
```

## 6. Tipos de geometría esperados

| Elemento | Objeto AutoCAD recomendado | Geometría PostGIS |
|---|---|---|
| LINEA_AGUA | Polyline / LWPOLYLINE | LineString |
| LINEA_DRENAJE | Polyline / LWPOLYLINE | LineString |
| LINEA_AGUA_PLUVIAL | Polyline / LWPOLYLINE | LineString |
| POZO_DRENAJE | BlockReference / INSERT / POINT | Point |

Preferir polilíneas para redes lineales. Evitar líneas sueltas cuando representen tramos completos.

## 7. Campos de datos para líneas

Todas las líneas deberán manejar estos campos:

```text
id_elemento
tipo_elemento
tipo_red
tipo_linea
material
diametro
profundidad
grosor
longitud_dibujo
longitud_reportada
estado
observaciones
```

### Descripción de campos

| Campo | Tipo | Obligatorio | Descripción |
|---|---|---:|---|
| id_elemento | texto | sí | Identificador único del elemento |
| tipo_elemento | texto | sí | LINEA_AGUA, LINEA_DRENAJE o LINEA_AGUA_PLUVIAL |
| tipo_red | texto | sí | Agua, Drenaje o Agua Pluvial |
| tipo_linea | texto | no | Conducción, distribución, colector, atarjea, etc. |
| material | texto | no | PVC, PEAD, concreto, acero, etc. |
| diametro | decimal | no | Diámetro nominal |
| profundidad | decimal | no | Profundidad aproximada |
| grosor | decimal | no | Grosor o espesor si aplica |
| longitud_dibujo | decimal | sí | Longitud calculada desde la geometría CAD |
| longitud_reportada | decimal | no | Longitud capturada o validada en campo |
| estado | texto | no | Existente, proyecto, fuera de servicio, desconocido |
| observaciones | texto | no | Comentarios libres |

## 8. Campos de datos para pozos de drenaje

```text
id_elemento
tipo_elemento
material
diametro
profundidad
estado
observaciones
```

### Descripción de campos

| Campo | Tipo | Obligatorio | Descripción |
|---|---|---:|---|
| id_elemento | texto | sí | Identificador único del pozo |
| tipo_elemento | texto | sí | POZO_DRENAJE |
| material | texto | no | Concreto, mampostería, prefabricado, etc. |
| diametro | decimal | no | Diámetro o dimensión principal |
| profundidad | decimal | no | Profundidad del pozo |
| estado | texto | no | Existente, proyecto, fuera de servicio, desconocido |
| observaciones | texto | no | Comentarios libres |

## 9. Catálogos iniciales

### tipo_elemento

```text
LINEA_AGUA
LINEA_DRENAJE
LINEA_AGUA_PLUVIAL
POZO_DRENAJE
```

### tipo_red

```text
Agua
Drenaje
Agua Pluvial
```

### material

```text
PVC
PEAD
Acero
Concreto
FoFo
Asbesto Cemento
Mamposteria
Otro
Desconocido
```

### estado

```text
Existente
Proyecto
Fuera de servicio
Desconocido
```

### tipo_linea para agua

```text
Conduccion
Distribucion
Alimentacion
Toma
Otro
```

### tipo_linea para drenaje

```text
Colector
Subcolector
Atarjea
Descarga
Otro
```

### tipo_linea para agua pluvial

```text
Colector pluvial
Canalizacion
Boca de tormenta
Descarga pluvial
Otro
```

## 10. Formato XDATA

El plugin debe registrar el AppID:

```text
SAPAL_RED
```

Cada entidad debe guardar sus datos técnicos en XDATA bajo ese AppID.

### Reglas generales

- Todos los valores deben guardarse como pares `clave=valor`.
- Las claves deben estar en minúsculas, sin acentos y sin espacios.
- Usar nombres consistentes.
- No guardar textos con saltos de línea dentro del XDATA.
- Los valores vacíos deben guardarse como cadena vacía o no guardarse, pero el extractor debe poder manejar ambos casos.
- La longitud calculada debe actualizarse cuando se capture o edite la entidad.

### Ejemplo conceptual para línea

```text
APPID: SAPAL_RED
id_elemento=AG-000001
tipo_elemento=LINEA_AGUA
tipo_red=Agua
tipo_linea=Distribucion
material=PVC
diametro=4
profundidad=1.20
grosor=
longitud_dibujo=35.80
longitud_reportada=
estado=Existente
observaciones=Linea capturada desde plano de obras
```

### Ejemplo conceptual para pozo

```text
APPID: SAPAL_RED
id_elemento=PD-000001
tipo_elemento=POZO_DRENAJE
material=Concreto
diametro=1.20
profundidad=2.40
estado=Existente
observaciones=Pozo existente
```

## 11. Comandos del plugin

El plugin debe incluir estos comandos iniciales:

```text
SAPAL_CONFIG
SAPAL_CAPTURAR_LINEA
SAPAL_CAPTURAR_POZO
SAPAL_CONSULTAR
SAPAL_EDITAR
SAPAL_VALIDAR
SAPAL_LIMPIAR_XDATA
```

### SAPAL_CONFIG

Debe verificar o crear la configuración base del dibujo:

- Registrar AppID `SAPAL_RED`.
- Crear capas recomendadas si no existen.
- Confirmar unidades del dibujo.
- Mostrar mensaje de estado.

Capas recomendadas:

```text
SAPAL_AGUA
SAPAL_DRENAJE
SAPAL_PLUVIAL
SAPAL_POZOS_DRENAJE
```

### SAPAL_CAPTURAR_LINEA

Flujo esperado:

1. Solicitar selección de entidad.
2. Aceptar solamente Line, Polyline o LWPOLYLINE.
3. Calcular longitud desde la geometría.
4. Mostrar formulario de captura.
5. Validar datos obligatorios.
6. Guardar XDATA en la entidad.
7. Opcionalmente mover la entidad a una capa según `tipo_elemento`.
8. Mostrar mensaje de confirmación.

### SAPAL_CAPTURAR_POZO

Flujo esperado:

1. Solicitar selección de entidad.
2. Aceptar BlockReference, INSERT o POINT.
3. Mostrar formulario de captura.
4. Validar datos obligatorios.
5. Guardar XDATA en la entidad.
6. Opcionalmente mover la entidad a la capa `SAPAL_POZOS_DRENAJE`.
7. Mostrar mensaje de confirmación.

### SAPAL_CONSULTAR

Flujo esperado:

1. Solicitar selección de entidad.
2. Leer XDATA `SAPAL_RED`.
3. Mostrar los datos en una ventana o en la línea de comandos.
4. Si no tiene datos, informar que la entidad no está registrada.

### SAPAL_EDITAR

Flujo esperado:

1. Solicitar selección de entidad.
2. Leer XDATA existente.
3. Abrir formulario con valores actuales.
4. Permitir modificar.
5. Recalcular longitud si es línea.
6. Guardar XDATA actualizado.

### SAPAL_VALIDAR

Debe recorrer el dibujo y generar un resumen:

- Total de entidades con XDATA `SAPAL_RED`.
- Total de líneas.
- Total de pozos.
- Entidades sin `id_elemento`.
- Identificadores duplicados.
- Entidades con tipo no válido.
- Entidades con geometría no compatible.
- Entidades con datos numéricos inválidos.

### SAPAL_LIMPIAR_XDATA

Debe permitir limpiar XDATA `SAPAL_RED` de una entidad seleccionada, previa confirmación del usuario.

## 12. Validaciones

### Validaciones obligatorias

- `id_elemento` no vacío.
- `tipo_elemento` no vacío.
- `tipo_elemento` dentro del catálogo.
- `id_elemento` único dentro del dibujo.
- Para líneas, geometría tipo línea o polilínea.
- Para pozos, geometría tipo bloque o punto.
- Valores numéricos válidos en:
  - `diametro`
  - `profundidad`
  - `grosor`
  - `longitud_dibujo`
  - `longitud_reportada`

### Validaciones recomendadas

- Si `tipo_elemento = LINEA_AGUA`, entonces `tipo_red = Agua`.
- Si `tipo_elemento = LINEA_DRENAJE`, entonces `tipo_red = Drenaje`.
- Si `tipo_elemento = LINEA_AGUA_PLUVIAL`, entonces `tipo_red = Agua Pluvial`.
- Si `tipo_elemento = POZO_DRENAJE`, entonces no requiere `tipo_red`.

## 13. Estructura sugerida del repositorio

```text
sapal-autocad-red-plugin/
│
├── .github/
│   └── instructions/
│       └── memory.instruction.md
│
├── docs/
│   ├── 01_arquitectura.md
│   ├── 02_especificacion_xdata.md
│   ├── 03_flujo_operativo.md
│   ├── 04_base_datos.md
│   └── 05_prompts_codex.md
│
├── src/
│   ├── Sapal.Cad.Plugin/
│   │   ├── Commands/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Validation/
│   │   ├── UI/
│   │   └── Sapal.Cad.Plugin.csproj
│   │
│   └── Sapal.Dxf.Extractor/
│       ├── sapal_dxf_extractor/
│       │   ├── __init__.py
│       │   ├── config.py
│       │   ├── dxf_reader.py
│       │   ├── models.py
│       │   ├── postgis_writer.py
│       │   ├── validator.py
│       │   └── main.py
│       ├── tests/
│       ├── pyproject.toml
│       └── README.md
│
├── database/
│   ├── 001_schema_infraestructura.sql
│   ├── 002_tables_red.sql
│   └── 003_indexes.sql
│
├── samples/
│   ├── dxf/
│   └── reports/
│
├── README.md
└── SPEC_CODEX_PLUGIN_AUTOCAD_SAPAL_RED.md
```

## 14. Archivo de memoria para Codex

Crear este archivo:

```text
.github/instructions/memory.instruction.md
```

Con este contenido:

```yaml
---
applyTo: '**'
---

# Memoria del proyecto

Este repositorio contiene un plugin para AutoCAD orientado a capturar datos técnicos de infraestructura de SAPAL sobre entidades CAD y transportarlos mediante DXF hacia PostgreSQL/PostGIS.

## Reglas de desarrollo

- Responder y documentar en español.
- Priorizar claridad, modularidad y evolución.
- No dejar código incompleto.
- No usar placeholders como "resto del código".
- Mantener nombres consistentes.
- Separar comandos, modelos, servicios, validaciones e interfaz.
- Escribir cambios pequeños y verificables.
- Agregar pruebas cuando sea posible.
- Mantener compatibilidad con el flujo AutoCAD → DXF → PostgreSQL/PostGIS → Web.

## Decisiones técnicas

- El plugin se implementa en C# usando AutoCAD .NET API.
- Los atributos técnicos se guardan como XDATA.
- El AppID de XDATA es SAPAL_RED.
- El extractor DXF se implementa en Python usando ezdxf.
- La base de datos destino es PostgreSQL/PostGIS.
- El SRID inicial recomendado es EPSG:32614.

## Elementos soportados

- LINEA_AGUA
- LINEA_DRENAJE
- LINEA_AGUA_PLUVIAL
- POZO_DRENAJE
```

## 15. Tablas PostgreSQL/PostGIS

### Esquema

```sql
CREATE SCHEMA IF NOT EXISTS infraestructura;
```

### Tabla de líneas

```sql
CREATE TABLE IF NOT EXISTS infraestructura.red_lineal (
    id BIGSERIAL PRIMARY KEY,
    id_elemento TEXT NOT NULL UNIQUE,
    tipo_elemento TEXT NOT NULL,
    tipo_red TEXT NOT NULL,
    tipo_linea TEXT,
    material TEXT,
    diametro NUMERIC,
    profundidad NUMERIC,
    grosor NUMERIC,
    longitud_dibujo NUMERIC,
    longitud_reportada NUMERIC,
    estado TEXT,
    observaciones TEXT,
    fuente_archivo TEXT,
    fecha_carga TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    geom geometry(LineString, 32614) NOT NULL
);
```

### Tabla de pozos

```sql
CREATE TABLE IF NOT EXISTS infraestructura.pozo_drenaje (
    id BIGSERIAL PRIMARY KEY,
    id_elemento TEXT NOT NULL UNIQUE,
    tipo_elemento TEXT NOT NULL,
    material TEXT,
    diametro NUMERIC,
    profundidad NUMERIC,
    estado TEXT,
    observaciones TEXT,
    fuente_archivo TEXT,
    fecha_carga TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
    geom geometry(Point, 32614) NOT NULL
);
```

### Índices

```sql
CREATE INDEX IF NOT EXISTS red_lineal_geom_gix
ON infraestructura.red_lineal
USING GIST (geom);

CREATE INDEX IF NOT EXISTS pozo_drenaje_geom_gix
ON infraestructura.pozo_drenaje
USING GIST (geom);

CREATE INDEX IF NOT EXISTS red_lineal_tipo_elemento_idx
ON infraestructura.red_lineal (tipo_elemento);

CREATE INDEX IF NOT EXISTS red_lineal_tipo_red_idx
ON infraestructura.red_lineal (tipo_red);

CREATE INDEX IF NOT EXISTS pozo_drenaje_tipo_elemento_idx
ON infraestructura.pozo_drenaje (tipo_elemento);
```

## 16. Extractor DXF

El extractor debe:

1. Recibir la ruta de un archivo DXF.
2. Abrir el archivo.
3. Recorrer entidades compatibles:
   - LINE
   - LWPOLYLINE
   - POLYLINE
   - INSERT
   - POINT
4. Leer XDATA del AppID `SAPAL_RED`.
5. Convertir los pares `clave=valor` a modelo interno.
6. Validar campos obligatorios.
7. Convertir geometría CAD a WKT/WKB.
8. Insertar o actualizar en PostgreSQL/PostGIS.
9. Generar reporte de carga.

### Comportamiento esperado

- Si un `id_elemento` ya existe, hacer actualización.
- Si la geometría cambia, actualizar `geom`.
- Si faltan campos obligatorios, registrar error y continuar.
- Si existe duplicidad dentro del DXF, reportar error.
- No detener toda la carga por un solo elemento inválido.

## 17. Criterios de aceptación

### Plugin AutoCAD

- [ ] El comando `SAPAL_CONFIG` registra el AppID `SAPAL_RED`.
- [ ] El comando `SAPAL_CAPTURAR_LINEA` permite seleccionar una línea o polilínea.
- [ ] El comando `SAPAL_CAPTURAR_LINEA` guarda XDATA en la entidad.
- [ ] El comando `SAPAL_CAPTURAR_POZO` permite seleccionar un bloque o punto.
- [ ] El comando `SAPAL_CAPTURAR_POZO` guarda XDATA en la entidad.
- [ ] El comando `SAPAL_CONSULTAR` muestra los datos guardados.
- [ ] El comando `SAPAL_EDITAR` permite modificar los datos existentes.
- [ ] El comando `SAPAL_VALIDAR` detecta identificadores duplicados.
- [ ] El comando `SAPAL_VALIDAR` detecta campos obligatorios faltantes.
- [ ] El DXF exportado conserva los datos XDATA.

### Extractor DXF

- [ ] Lee entidades con XDATA `SAPAL_RED`.
- [ ] Ignora entidades sin XDATA `SAPAL_RED`.
- [ ] Convierte polilíneas a LineString.
- [ ] Convierte pozos a Point.
- [ ] Valida campos obligatorios.
- [ ] Inserta líneas en `infraestructura.red_lineal`.
- [ ] Inserta pozos en `infraestructura.pozo_drenaje`.
- [ ] Actualiza registros existentes por `id_elemento`.
- [ ] Genera reporte de errores y resumen de carga.

### Base de datos

- [ ] Crea esquema `infraestructura`.
- [ ] Crea tabla `red_lineal`.
- [ ] Crea tabla `pozo_drenaje`.
- [ ] Crea índices espaciales.
- [ ] Respeta SRID `32614`.

## 18. Plan de implementación para Codex

### Fase 1: documentación y estructura

- [ ] Crear estructura del repositorio.
- [ ] Crear `.github/instructions/memory.instruction.md`.
- [ ] Crear documentación inicial.
- [ ] Crear scripts SQL.

### Fase 2: plugin AutoCAD mínimo

- [ ] Crear proyecto C#.
- [ ] Crear clase de comandos.
- [ ] Implementar `SAPAL_CONFIG`.
- [ ] Implementar servicio `XDataService`.
- [ ] Implementar modelos `LineaRedData` y `PozoDrenajeData`.
- [ ] Implementar `SAPAL_CAPTURAR_LINEA` con captura mínima por línea de comandos.
- [ ] Implementar `SAPAL_CONSULTAR`.

### Fase 3: formulario

- [ ] Crear formulario para línea.
- [ ] Crear formulario para pozo.
- [ ] Agregar catálogos.
- [ ] Agregar validaciones.
- [ ] Conectar formularios con XDATA.

### Fase 4: extractor DXF

- [ ] Crear proyecto Python.
- [ ] Leer DXF con `ezdxf`.
- [ ] Leer XDATA.
- [ ] Convertir geometrías.
- [ ] Validar datos.
- [ ] Guardar en PostgreSQL/PostGIS.
- [ ] Generar reporte.

### Fase 5: pruebas

- [ ] Crear DXF de ejemplo.
- [ ] Probar lectura de líneas.
- [ ] Probar lectura de pozos.
- [ ] Probar duplicados.
- [ ] Probar campos faltantes.
- [ ] Probar inserción en PostgreSQL.
- [ ] Probar actualización por `id_elemento`.

## 19. Prompts sugeridos para Codex

### Prompt 1: crear estructura base

```text
Lee el archivo SPEC_CODEX_PLUGIN_AUTOCAD_SAPAL_RED.md y crea la estructura inicial del repositorio.

Crea las carpetas indicadas, el archivo .github/instructions/memory.instruction.md, los documentos base en docs, los scripts SQL en database y los README iniciales.

No implementes todavía lógica compleja. Solo estructura, documentación base y archivos iniciales.
```

### Prompt 2: crear plugin AutoCAD base

```text
Implementa el proyecto C# Sapal.Cad.Plugin para AutoCAD .NET API.

Debe incluir:
- Comandos SAPAL_CONFIG, SAPAL_CAPTURAR_LINEA, SAPAL_CONSULTAR y SAPAL_VALIDAR.
- Modelos LineaRedData y PozoDrenajeData.
- Servicio XDataService para registrar APPID SAPAL_RED, escribir XDATA y leer XDATA.
- Validaciones básicas.

Usa separación por carpetas Commands, Models, Services y Validation.
No dejes código incompleto ni placeholders.
```

### Prompt 3: implementar captura de línea

```text
Implementa SAPAL_CAPTURAR_LINEA.

Requisitos:
- Solicitar selección de entidad.
- Aceptar Line, Polyline o LWPOLYLINE.
- Calcular longitud de la geometría.
- Pedir datos mínimos por línea de comandos si aún no existe formulario.
- Validar id_elemento y tipo_elemento.
- Guardar datos como XDATA bajo APPID SAPAL_RED.
- Permitir consultar después con SAPAL_CONSULTAR.
```

### Prompt 4: crear extractor DXF

```text
Crea el proyecto Python Sapal.Dxf.Extractor.

Requisitos:
- Usar ezdxf.
- Leer entidades LINE, LWPOLYLINE, POLYLINE, INSERT y POINT.
- Leer XDATA del APPID SAPAL_RED.
- Convertir pares clave=valor a modelos internos.
- Validar campos obligatorios.
- Generar salida JSON inicialmente.
- Dejar preparada la capa postgis_writer.py para insertar después en PostgreSQL/PostGIS.
```

### Prompt 5: integrar PostgreSQL/PostGIS

```text
Implementa la carga a PostgreSQL/PostGIS en Sapal.Dxf.Extractor.

Requisitos:
- Usar psycopg o psycopg2.
- Insertar o actualizar por id_elemento.
- Líneas a infraestructura.red_lineal.
- Pozos a infraestructura.pozo_drenaje.
- Usar geometría WKT con SRID 32614.
- Generar reporte con insertados, actualizados, ignorados y errores.
```

## 20. Notas importantes para el desarrollador

- No depender de nombres de capas como fuente principal de datos.
- No depender de textos sueltos en el dibujo.
- No guardar atributos solamente en la página web.
- El dato técnico debe viajar desde AutoCAD hasta PostgreSQL.
- XDATA es la primera opción de transporte porque viaja dentro de las entidades DXF.
- El usuario final del departamento de Obras debe tener una experiencia simple.
- La validación debe hacerse lo antes posible, preferentemente desde el plugin.
- PostgreSQL/PostGIS será la fuente para la página web.
- El DXF funcionará como medio de intercambio.

## 21. Resultado esperado

Al terminar la primera versión, se debe poder hacer lo siguiente:

1. Abrir AutoCAD.
2. Cargar el plugin.
3. Ejecutar `SAPAL_CONFIG`.
4. Dibujar o seleccionar una polilínea.
5. Ejecutar `SAPAL_CAPTURAR_LINEA`.
6. Capturar datos técnicos.
7. Guardar el dibujo.
8. Exportar a DXF.
9. Ejecutar el extractor.
10. Cargar la información a PostgreSQL/PostGIS.
11. Consultar la información desde una página web o visor SIG.
