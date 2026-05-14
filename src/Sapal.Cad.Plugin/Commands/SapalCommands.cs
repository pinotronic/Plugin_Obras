using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Sapal.Cad.Plugin.Models;
using Sapal.Cad.Plugin.Services;
using Sapal.Cad.Plugin.Validation;

namespace Sapal.Cad.Plugin.Commands
{
    public class SapalCommands
    {
        private readonly XDataService _xdataService = new XDataService();
        private readonly LayerService _layerService = new LayerService();
        private readonly CadGeometryService _geometryService = new CadGeometryService();
        private readonly SapalDataValidator _validator = new SapalDataValidator();

        [CommandMethod("SAPAL_CONFIG")]
        public void Config()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;
            var editor = document.Editor;

            using (var transaction = database.TransactionManager.StartTransaction())
            {
                _xdataService.EnsureAppId(database, transaction);
                _layerService.EnsureRecommendedLayers(database, transaction);
                transaction.Commit();
            }

            editor.WriteMessage("\nSAPAL_RED configurado. AppID y capas recomendadas verificados.");
        }

        [CommandMethod("SAPAL_CAPTURAR_LINEA")]
        public void CapturarLinea()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;
            var editor = document.Editor;

            var entityResult = editor.GetEntity("\nSeleccione linea o polilinea SAPAL: ");
            if (entityResult.Status != PromptStatus.OK)
            {
                return;
            }

            using (var transaction = database.TransactionManager.StartTransaction())
            {
                _xdataService.EnsureAppId(database, transaction);
                var entity = transaction.GetObject(entityResult.ObjectId, OpenMode.ForWrite) as Entity;
                if (entity == null || !_geometryService.IsSupportedLine(entity))
                {
                    editor.WriteMessage("\nLa entidad seleccionada no es Line, Polyline, LWPOLYLINE o polilinea compatible.");
                    return;
                }

                var data = ReadLineaData(editor, entity);
                var validation = _validator.ValidateLinea(data);
                if (!validation.IsValid)
                {
                    WriteErrors(editor, validation.Errors);
                    return;
                }

                _xdataService.WriteXData(entity, data.ToDictionary());
                var savedValues = _xdataService.ReadXData(entity);
                if (!savedValues.Any())
                {
                    editor.WriteMessage("\nAdvertencia: AutoCAD no devolvio datos SAPAL_RED despues de guardar.");
                }

                _layerService.MoveEntityToCatalogLayer(entity, data.TipoElemento);
                transaction.Commit();
            }

            editor.WriteMessage("\nDatos SAPAL_RED guardados en la linea.");
        }

        [CommandMethod("SAPAL_CAPTURAR_POZO")]
        public void CapturarPozo()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;
            var editor = document.Editor;

            var entityResult = editor.GetEntity("\nSeleccione bloque o punto SAPAL: ");
            if (entityResult.Status != PromptStatus.OK)
            {
                return;
            }

            using (var transaction = database.TransactionManager.StartTransaction())
            {
                _xdataService.EnsureAppId(database, transaction);
                var entity = transaction.GetObject(entityResult.ObjectId, OpenMode.ForWrite) as Entity;
                if (entity == null || !_geometryService.IsSupportedPoint(entity))
                {
                    editor.WriteMessage("\nLa entidad seleccionada no es BlockReference o POINT.");
                    return;
                }

                var data = ReadPozoData(editor);
                var validation = _validator.ValidatePozo(data);
                if (!validation.IsValid)
                {
                    WriteErrors(editor, validation.Errors);
                    return;
                }

                _xdataService.WriteXData(entity, data.ToDictionary());
                var savedValues = _xdataService.ReadXData(entity);
                if (!savedValues.Any())
                {
                    editor.WriteMessage("\nAdvertencia: AutoCAD no devolvio datos SAPAL_RED despues de guardar.");
                }

                _layerService.MoveEntityToCatalogLayer(entity, data.TipoElemento);
                transaction.Commit();
            }

            editor.WriteMessage("\nDatos SAPAL_RED guardados en el pozo.");
        }

        [CommandMethod("SAPAL_CONSULTAR")]
        public void Consultar()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;
            var editor = document.Editor;

            var entityResult = editor.GetEntity("\nSeleccione entidad SAPAL para consultar: ");
            if (entityResult.Status != PromptStatus.OK)
            {
                return;
            }

            using (var transaction = database.TransactionManager.StartTransaction())
            {
                var entity = transaction.GetObject(entityResult.ObjectId, OpenMode.ForRead) as Entity;
                if (entity == null)
                {
                    editor.WriteMessage("\nNo se pudo leer la entidad seleccionada.");
                    return;
                }

                var values = _xdataService.ReadXData(entity);
                if (!values.Any())
                {
                    editor.WriteMessage("\nLa entidad no tiene datos SAPAL_RED.");
                    if (_xdataService.HasAnyXData(entity))
                    {
                        editor.WriteMessage("\nLa entidad tiene XDATA de otra aplicacion o en formato no reconocido:");
                        foreach (var rawValue in _xdataService.ReadRawXData(entity))
                        {
                            editor.WriteMessage("\n  {0}", rawValue);
                        }
                    }

                    return;
                }

                editor.WriteMessage("\nDatos SAPAL_RED:");
                foreach (var value in values.OrderBy(item => item.Key))
                {
                    editor.WriteMessage("\n  {0}: {1}", value.Key, value.Value);
                }

                transaction.Commit();
            }
        }

        [CommandMethod("SAPAL_VALIDAR")]
        public void Validar()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;
            var editor = document.Editor;
            var records = new List<IDictionary<string, string>>();
            var totalLineas = 0;
            var totalPozos = 0;
            var sinId = 0;
            var tipoInvalido = 0;

            using (var transaction = database.TransactionManager.StartTransaction())
            {
                var blockTable = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord)transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                foreach (ObjectId objectId in modelSpace)
                {
                    var entity = transaction.GetObject(objectId, OpenMode.ForRead) as Entity;
                    if (entity == null)
                    {
                        continue;
                    }

                    var values = _xdataService.ReadXData(entity);
                    if (!values.Any())
                    {
                        continue;
                    }

                    records.Add(values);
                    var idElemento = Get(values, "id_elemento");
                    var tipoElemento = Get(values, "tipo_elemento");

                    if (string.IsNullOrWhiteSpace(idElemento))
                    {
                        sinId++;
                    }

                    if (!Catalogs.TiposElemento.Contains(tipoElemento))
                    {
                        tipoInvalido++;
                    }
                    else if (Catalogs.TiposLinea.Contains(tipoElemento))
                    {
                        totalLineas++;
                    }
                    else if (Catalogs.TiposPozo.Contains(tipoElemento))
                    {
                        totalPozos++;
                    }
                }

                transaction.Commit();
            }

            var uniqueValidation = _validator.ValidateUniqueIds(records);
            editor.WriteMessage("\nResumen SAPAL_VALIDAR");
            editor.WriteMessage("\n  Entidades SAPAL_RED: {0}", records.Count);
            editor.WriteMessage("\n  Lineas: {0}", totalLineas);
            editor.WriteMessage("\n  Pozos: {0}", totalPozos);
            editor.WriteMessage("\n  Sin id_elemento: {0}", sinId);
            editor.WriteMessage("\n  Tipos invalidos: {0}", tipoInvalido);
            editor.WriteMessage("\n  Identificadores duplicados: {0}", uniqueValidation.Errors.Count);
            WriteErrors(editor, uniqueValidation.Errors);
        }

        [CommandMethod("SAPAL_LIMPIAR_XDATA")]
        public void LimpiarXData()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;
            var editor = document.Editor;

            var entityResult = editor.GetEntity("\nSeleccione entidad para limpiar XDATA SAPAL_RED: ");
            if (entityResult.Status != PromptStatus.OK)
            {
                return;
            }

            var confirmation = editor.GetKeywords(new PromptKeywordOptions("\nConfirmar limpieza de XDATA SAPAL_RED [Si/No]: ", "Si No")
            {
                AllowNone = false
            });

            if (confirmation.Status != PromptStatus.OK || confirmation.StringResult != "Si")
            {
                editor.WriteMessage("\nOperacion cancelada.");
                return;
            }

            using (var transaction = database.TransactionManager.StartTransaction())
            {
                var entity = transaction.GetObject(entityResult.ObjectId, OpenMode.ForWrite) as Entity;
                if (entity == null)
                {
                    editor.WriteMessage("\nNo se pudo leer la entidad seleccionada.");
                    return;
                }

                _xdataService.ClearXData(entity);
                transaction.Commit();
            }

            editor.WriteMessage("\nXDATA SAPAL_RED limpiado.");
        }

        private LineaRedData ReadLineaData(Editor editor, Entity entity)
        {
            var tipoElemento = PromptCatalogValue(
                editor,
                "\nTipo de elemento (LINEA_AGUA, LINEA_DRENAJE, LINEA_AGUA_PLUVIAL): ",
                Catalogs.TiposLinea);
            string tipoRed;
            Catalogs.TipoRedPorTipoElemento.TryGetValue(tipoElemento, out tipoRed);

            var data = new LineaRedData
            {
                IdElemento = PromptString(editor, "\nid_elemento: ", true),
                TipoElemento = tipoElemento,
                TipoRed = tipoRed ?? string.Empty,
                TipoLinea = PromptString(editor, "\ntipo_linea: ", false),
                Material = PromptString(editor, "\nmaterial: ", false),
                Diametro = PromptDecimal(editor, "\ndiametro: "),
                Profundidad = PromptDecimal(editor, "\nprofundidad: "),
                Grosor = PromptDecimal(editor, "\ngrosor: "),
                LongitudDibujo = Convert.ToDecimal(_geometryService.GetLength(entity)),
                LongitudReportada = PromptDecimal(editor, "\nlongitud_reportada: "),
                Estado = PromptString(editor, "\nestado: ", false),
                Observaciones = PromptString(editor, "\nobservaciones: ", false)
            };

            editor.WriteMessage("\nlongitud_dibujo calculada: {0}", data.LongitudDibujo.ToString(CultureInfo.InvariantCulture));
            return data;
        }

        private static PozoDrenajeData ReadPozoData(Editor editor)
        {
            return new PozoDrenajeData
            {
                IdElemento = PromptString(editor, "\nid_elemento: ", true),
                TipoElemento = Catalogs.PozoDrenaje,
                Material = PromptString(editor, "\nmaterial: ", false),
                Diametro = PromptDecimal(editor, "\ndiametro: "),
                Profundidad = PromptDecimal(editor, "\nprofundidad: "),
                Estado = PromptString(editor, "\nestado: ", false),
                Observaciones = PromptString(editor, "\nobservaciones: ", false)
            };
        }

        private static string PromptString(Editor editor, string message, bool required)
        {
            while (true)
            {
                var options = new PromptStringOptions(message)
                {
                    AllowSpaces = true
                };

                var result = editor.GetString(options);
                if (result.Status != PromptStatus.OK)
                {
                    return string.Empty;
                }

                if (!required || !string.IsNullOrWhiteSpace(result.StringResult))
                {
                    return result.StringResult;
                }

                editor.WriteMessage("\nEl valor es obligatorio.");
            }
        }

        private static string PromptCatalogValue(Editor editor, string message, ISet<string> allowedValues)
        {
            while (true)
            {
                var result = editor.GetString(new PromptStringOptions(message)
                {
                    AllowSpaces = false
                });

                if (result.Status != PromptStatus.OK)
                {
                    return string.Empty;
                }

                var value = (result.StringResult ?? string.Empty).Trim().ToUpperInvariant();
                if (allowedValues.Contains(value))
                {
                    return value;
                }

                editor.WriteMessage("\nValor no valido. Use uno de: {0}", string.Join(", ", allowedValues));
            }
        }

        private static decimal? PromptDecimal(Editor editor, string message)
        {
            var result = editor.GetString(new PromptStringOptions(message) { AllowSpaces = false });
            if (result.Status != PromptStatus.OK || string.IsNullOrWhiteSpace(result.StringResult))
            {
                return null;
            }

            decimal parsed;
            if (decimal.TryParse(result.StringResult, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            editor.WriteMessage("\nValor numerico invalido. Se guardara vacio.");
            return null;
        }

        private static void WriteErrors(Editor editor, IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                editor.WriteMessage("\n  ERROR: {0}", error);
            }
        }

        private static string Get(IDictionary<string, string> values, string key)
        {
            string value;
            return values.TryGetValue(key, out value) ? value : string.Empty;
        }
    }
}
