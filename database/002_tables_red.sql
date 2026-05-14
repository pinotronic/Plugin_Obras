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
