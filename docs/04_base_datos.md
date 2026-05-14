# Base de datos

La base destino es PostgreSQL con PostGIS.

## Esquema

```sql
CREATE SCHEMA IF NOT EXISTS infraestructura;
```

## Tablas

- `infraestructura.red_lineal`: redes de agua, drenaje y agua pluvial.
- `infraestructura.pozo_drenaje`: pozos de drenaje.

## SRID

El SRID inicial recomendado es:

```text
32614
```

## Scripts

Los scripts estan en:

- `database/001_schema_infraestructura.sql`
- `database/002_tables_red.sql`
- `database/003_indexes.sql`
