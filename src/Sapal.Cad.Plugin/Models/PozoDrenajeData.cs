using System.Collections.Generic;
using System.Globalization;

namespace Sapal.Cad.Plugin.Models
{
    public class PozoDrenajeData
    {
        public string IdElemento { get; set; }
        public string TipoElemento { get; set; }
        public string Material { get; set; }
        public decimal? Diametro { get; set; }
        public decimal? Profundidad { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "id_elemento", IdElemento ?? string.Empty },
                { "tipo_elemento", TipoElemento ?? string.Empty },
                { "material", Material ?? string.Empty },
                { "diametro", FormatNullable(Diametro) },
                { "profundidad", FormatNullable(Profundidad) },
                { "estado", Estado ?? string.Empty },
                { "observaciones", Observaciones ?? string.Empty }
            };
        }

        public static PozoDrenajeData FromDictionary(IDictionary<string, string> values)
        {
            return new PozoDrenajeData
            {
                IdElemento = Get(values, "id_elemento"),
                TipoElemento = Get(values, "tipo_elemento"),
                Material = Get(values, "material"),
                Diametro = ParseNullableDecimal(Get(values, "diametro")),
                Profundidad = ParseNullableDecimal(Get(values, "profundidad")),
                Estado = Get(values, "estado"),
                Observaciones = Get(values, "observaciones")
            };
        }

        private static string Get(IDictionary<string, string> values, string key)
        {
            string value;
            return values.TryGetValue(key, out value) ? value : string.Empty;
        }

        private static string FormatNullable(decimal? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        private static decimal? ParseNullableDecimal(string value)
        {
            decimal parsed;
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed) ? parsed : (decimal?)null;
        }
    }
}
