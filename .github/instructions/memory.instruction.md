---
applyTo: '**'
---

# Memoria del proyecto

Este repositorio contiene un plugin para AutoCAD orientado a capturar datos tecnicos de infraestructura de SAPAL sobre entidades CAD y transportarlos mediante DXF hacia PostgreSQL/PostGIS.

## Reglas de desarrollo

- Responder y documentar en espanol.
- Priorizar claridad, modularidad y evolucion.
- No dejar codigo incompleto.
- No usar placeholders como "resto del codigo".
- Mantener nombres consistentes.
- Separar comandos, modelos, servicios, validaciones e interfaz.
- Escribir cambios pequenos y verificables.
- Agregar pruebas cuando sea posible.
- Mantener compatibilidad con el flujo AutoCAD -> DXF -> PostgreSQL/PostGIS -> Web.

## Decisiones tecnicas

- El plugin se implementa en C# usando AutoCAD .NET API.
- Los atributos tecnicos se guardan como XDATA.
- El AppID de XDATA es SAPAL_RED.
- El extractor DXF se implementa en Python usando ezdxf.
- La base de datos destino es PostgreSQL/PostGIS.
- El SRID inicial recomendado es EPSG:32614.

## Elementos soportados

- LINEA_AGUA
- LINEA_DRENAJE
- LINEA_AGUA_PLUVIAL
- POZO_DRENAJE
