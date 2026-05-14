# Plugin AutoCAD SAPAL Red

Proyecto para capturar informacion tecnica de infraestructura sobre entidades CAD, conservarla como XDATA en DWG/DXF y cargarla posteriormente a PostgreSQL/PostGIS.

## Flujo general

```text
AutoCAD -> Plugin SAPAL Red -> DWG/DXF -> Extractor DXF -> PostgreSQL/PostGIS -> Web
```

## Componentes

- `src/Sapal.Cad.Plugin`: plugin C# para AutoCAD .NET API.
- `src/Sapal.Dxf.Extractor`: extractor Python para leer DXF y preparar/cargar registros.
- `database`: scripts SQL para PostgreSQL/PostGIS.
- `docs`: documentacion tecnica y operativa.
- `samples`: archivos DXF y reportes de ejemplo.

## AppID XDATA

Todos los datos tecnicos se guardan bajo el AppID:

```text
SAPAL_RED
```

## Estado

Estructura inicial del repositorio creada a partir de `SPEC_CODEX_PLUGIN_AUTOCAD_SAPAL_RED.md`.
