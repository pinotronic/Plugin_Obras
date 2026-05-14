# Prompts Codex

## Crear estructura base

Lee el archivo `SPEC_CODEX_PLUGIN_AUTOCAD_SAPAL_RED.md` y crea la estructura inicial del repositorio.

Crea las carpetas indicadas, el archivo `.github/instructions/memory.instruction.md`, los documentos base en `docs`, los scripts SQL en `database` y los README iniciales.

No implementes todavia logica compleja. Solo estructura, documentacion base y archivos iniciales.

## Crear plugin AutoCAD base

Implementa el proyecto C# `Sapal.Cad.Plugin` para AutoCAD .NET API.

Debe incluir comandos, modelos, servicio XDATA y validaciones basicas.

## Crear extractor DXF

Crea el proyecto Python `Sapal.Dxf.Extractor` usando `ezdxf`, lectura de XDATA y salida JSON inicial.
