# Flujo operativo

## Preparacion del dibujo

1. Abrir el dibujo en AutoCAD.
2. Cargar el plugin SAPAL Red.
3. Ejecutar `SAPAL_CONFIG`.
4. Confirmar AppID, capas y unidades.

## Captura de lineas

1. Dibujar o seleccionar una linea o polilinea.
2. Ejecutar `SAPAL_CAPTURAR_LINEA`.
3. Capturar datos tecnicos.
4. Guardar XDATA en la entidad.
5. Consultar con `SAPAL_CONSULTAR` si se requiere verificar.

## Captura de pozos

1. Seleccionar bloque o punto.
2. Ejecutar `SAPAL_CAPTURAR_POZO`.
3. Capturar datos tecnicos.
4. Guardar XDATA en la entidad.

## Validacion y exportacion

1. Ejecutar `SAPAL_VALIDAR`.
2. Corregir faltantes o duplicados.
3. Exportar el dibujo a DXF.
4. Ejecutar el extractor DXF.
5. Revisar reporte de carga.
