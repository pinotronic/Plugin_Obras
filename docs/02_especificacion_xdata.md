# Especificacion XDATA

## AppID

```text
SAPAL_RED
```

## Formato

Los valores se guardan como pares `clave=valor`.

Reglas:

- Claves en minusculas, sin acentos y sin espacios.
- Valores sin saltos de linea.
- Valores vacios como cadena vacia o ausentes.
- El extractor debe aceptar ambos casos.

## Campos para lineas

- `id_elemento`
- `tipo_elemento`
- `tipo_red`
- `tipo_linea`
- `material`
- `diametro`
- `profundidad`
- `grosor`
- `longitud_dibujo`
- `longitud_reportada`
- `estado`
- `observaciones`

## Campos para pozos

- `id_elemento`
- `tipo_elemento`
- `material`
- `diametro`
- `profundidad`
- `estado`
- `observaciones`
