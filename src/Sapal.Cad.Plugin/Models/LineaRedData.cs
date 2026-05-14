using System.Collections.Generic;
using System.Globalization;

namespace Sapal.Cad.Plugin.Models
{
    public class LineaRedData
    {
        public string IdElemento { get; set; }
        public string TipoElemento { get; set; }
        public string TipoRed { get; set; }
        public string TipoLinea { get; set; }
        public string Material { get; set; }
        public decimal? Diametro { get; set; }
        public decimal? Profundidad { get; set; }
        public decimal? Grosor { get; set; }
        public decimal LongitudDibujo { get; set; }
        public decimal? LongitudReportada { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "id_elemento", IdElemento ?? string.Empty },
                { "tipo_elemento", TipoElemento ?? string.Empty },
                { "tipo_red", TipoRed ?? string.Empty },
                { "tipo_linea", TipoLinea ?? string.Empty },
                { "material", Material ?? string.Empty },
                { "diametro", FormatNullable(Diametro) },
                { "profundidad", FormatNullable(Profundidad) },
                { "grosor", FormatNullable(Grosor) },
                { "longitud_dibujo", LongitudDibujo.ToString(CultureInfo.InvariantCulture) },
                { "longitud_reportada", FormatNullable(LongitudReportada) },
                { "estado", Estado ?? string.Empty },
                { "observaciones", Observaciones ?? string.Empty }
            };
        }

        public static LineaRedData FromDictionary(IDictionary<string, string> values)
        {
            return new LineaRedData
            {
                IdElemento = Get(values, "id_elemento"),
                TipoElemento = Get(values, "tipo_elemento"),
                TipoRed = Get(values, "tipo_red"),
                TipoLinea = Get(values, "tipo_linea"),
                Material = Get(values, "material"),
                Diametro = ParseNullableDecimal(Get(values, "diametro")),
                Profundidad = ParseNullableDecimal(Get(values, "profundidad")),
                Grosor = ParseNullableDecimal(Get(values, "grosor")),
                LongitudDibujo = ParseNullableDecimal(Get(values, "longitud_dibujo")) ?? 0m,
                LongitudReportada = ParseNullableDecimal(Get(values, "longitud_reportada")),
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
