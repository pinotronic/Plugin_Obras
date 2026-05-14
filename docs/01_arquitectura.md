# Arquitectura

El sistema separa la captura CAD, el intercambio DXF, la extraccion de datos y la persistencia geoespacial.

## Componentes

- Plugin AutoCAD: permite capturar, consultar, editar, validar y limpiar datos XDATA en entidades CAD.
- XDATA: almacena pares `clave=valor` bajo el AppID `SAPAL_RED`.
- DXF: funciona como medio de intercambio entre AutoCAD y procesos externos.
- Extractor DXF: lee entidades compatibles, interpreta XDATA, valida datos y genera registros.
- PostgreSQL/PostGIS: almacena la informacion estructurada y geometria final.

## Flujo

```text
Dibujo CAD -> XDATA en entidades -> Exportacion DXF -> Lectura Python -> Validacion -> PostGIS
```

## Principios

- El dato tecnico debe viajar dentro de la entidad CAD.
- Las capas ayudan a organizar, pero no son la fuente principal del dato.
- La validacion debe ocurrir lo antes posible.
- El extractor debe continuar aunque una entidad sea invalida.
