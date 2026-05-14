using System;
using System.Collections.Generic;
using System.Linq;
using Sapal.Cad.Plugin.Models;

namespace Sapal.Cad.Plugin.Validation
{
    public class SapalDataValidator
    {
        public ValidationResult ValidateLinea(LineaRedData data)
        {
            var result = new ValidationResult();

            ValidateRequired(result, data.IdElemento, "id_elemento");
            ValidateRequired(result, data.TipoElemento, "tipo_elemento");
            ValidateRequired(result, data.TipoRed, "tipo_red");

            if (!string.IsNullOrWhiteSpace(data.TipoElemento) && !Catalogs.TiposLinea.Contains(data.TipoElemento))
            {
                result.AddError("tipo_elemento no corresponde a una linea soportada.");
            }

            var expectedTipoRed = Catalogs.ResolveTipoRed(data.TipoElemento);
            if (!string.IsNullOrWhiteSpace(expectedTipoRed) &&
                !string.Equals(data.TipoRed, expectedTipoRed, StringComparison.OrdinalIgnoreCase))
            {
                result.AddError("tipo_red no corresponde al tipo_elemento seleccionado.");
            }

            if (data.LongitudDibujo <= 0)
            {
                result.AddError("longitud_dibujo debe ser mayor que cero.");
            }

            return result;
        }

        public ValidationResult ValidatePozo(PozoDrenajeData data)
        {
            var result = new ValidationResult();

            ValidateRequired(result, data.IdElemento, "id_elemento");
            ValidateRequired(result, data.TipoElemento, "tipo_elemento");

            if (!string.IsNullOrWhiteSpace(data.TipoElemento) && !Catalogs.TiposPozo.Contains(data.TipoElemento))
            {
                result.AddError("tipo_elemento no corresponde a un pozo soportado.");
            }

            return result;
        }

        public ValidationResult ValidateUniqueIds(IEnumerable<IDictionary<string, string>> xdataRecords)
        {
            var result = new ValidationResult();
            var duplicated = xdataRecords
                .Select(record => Get(record, "id_elemento"))
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .GroupBy(id => id, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            foreach (var id in duplicated)
            {
                result.AddError("id_elemento duplicado: " + id);
            }

            return result;
        }

        private static void ValidateRequired(ValidationResult result, string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result.AddError(fieldName + " es obligatorio.");
            }
        }

        private static string Get(IDictionary<string, string> values, string key)
        {
            string value;
            return values.TryGetValue(key, out value) ? value : string.Empty;
        }
    }
}
