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
