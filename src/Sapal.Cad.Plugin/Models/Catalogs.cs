using System.Collections.Generic;

namespace Sapal.Cad.Plugin.Models
{
    public static class Catalogs
    {
        public const string AppId = "SAPAL_RED";

        public const string LineaAgua = "LINEA_AGUA";
        public const string LineaDrenaje = "LINEA_DRENAJE";
        public const string LineaAguaPluvial = "LINEA_AGUA_PLUVIAL";
        public const string PozoDrenaje = "POZO_DRENAJE";

        public static readonly ISet<string> TiposElemento = new HashSet<string>
        {
            LineaAgua,
            LineaDrenaje,
            LineaAguaPluvial,
            PozoDrenaje
        };

        public static readonly ISet<string> TiposLinea = new HashSet<string>
        {
            LineaAgua,
            LineaDrenaje,
            LineaAguaPluvial
        };

        public static readonly ISet<string> TiposPozo = new HashSet<string>
        {
            PozoDrenaje
        };

        public static readonly IDictionary<string, string> TipoRedPorTipoElemento = new Dictionary<string, string>
        {
            { LineaAgua, "Agua" },
            { LineaDrenaje, "Drenaje" },
            { LineaAguaPluvial, "Agua Pluvial" }
        };

        public static readonly IDictionary<string, string> CapaPorTipoElemento = new Dictionary<string, string>
        {
            { LineaAgua, "SAPAL_AGUA" },
            { LineaDrenaje, "SAPAL_DRENAJE" },
            { LineaAguaPluvial, "SAPAL_PLUVIAL" },
            { PozoDrenaje, "SAPAL_POZOS_DRENAJE" }
        };

        public static readonly string[] CapasRecomendadas =
        {
            "SAPAL_AGUA",
            "SAPAL_DRENAJE",
            "SAPAL_PLUVIAL",
            "SAPAL_POZOS_DRENAJE"
        };

        public static string ResolveTipoRed(string tipoElemento)
        {
            switch ((tipoElemento ?? string.Empty).Trim().ToUpperInvariant())
            {
                case LineaAgua:
                    return "Agua";
                case LineaDrenaje:
                    return "Drenaje";
                case LineaAguaPluvial:
                    return "Agua Pluvial";
                default:
                    return string.Empty;
            }
        }

        public static string ResolveCapa(string tipoElemento)
        {
            switch ((tipoElemento ?? string.Empty).Trim().ToUpperInvariant())
            {
                case LineaAgua:
                    return "SAPAL_AGUA";
                case LineaDrenaje:
                    return "SAPAL_DRENAJE";
                case LineaAguaPluvial:
                    return "SAPAL_PLUVIAL";
                case PozoDrenaje:
                    return "SAPAL_POZOS_DRENAJE";
                default:
                    return string.Empty;
            }
        }
    }
}
