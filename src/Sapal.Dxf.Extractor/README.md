# Sapal.Dxf.Extractor

Extractor Python para leer archivos DXF con XDATA `SAPAL_RED`.

## Objetivo inicial

- Leer entidades compatibles.
- Extraer pares `clave=valor`.
- Validar campos obligatorios.
- Convertir geometria a representacion geoespacial.
- Generar salida JSON o reporte de carga.
- Preparar insercion posterior en PostgreSQL/PostGIS.

## Entidades DXF previstas

- `LINE`
- `LWPOLYLINE`
- `POLYLINE`
- `INSERT`
- `POINT`
