from dataclasses import dataclass
from typing import Any


@dataclass(frozen=True)
class ExtractedEntity:
    id_elemento: str
    tipo_elemento: str
    geometry_type: str
    properties: dict[str, Any]
    wkt: str
