using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Sapal.Cad.Plugin.Models;

namespace Sapal.Cad.Plugin.Services
{
    public class LayerService
    {
        public void EnsureRecommendedLayers(Database database, Transaction transaction)
        {
            foreach (var layerName in Catalogs.CapasRecomendadas)
            {
                EnsureLayer(database, transaction, layerName);
            }
        }

        public void MoveEntityToCatalogLayer(Entity entity, string tipoElemento)
        {
            var layerName = Catalogs.ResolveCapa(tipoElemento);
            if (!string.IsNullOrWhiteSpace(layerName))
            {
                entity.Layer = layerName;
            }
        }

        private static void EnsureLayer(Database database, Transaction transaction, string layerName)
        {
            var layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);
            if (layerTable.Has(layerName))
            {
                return;
            }

            layerTable.UpgradeOpen();
            var record = new LayerTableRecord
            {
                Name = layerName,
                Color = Color.FromColorIndex(ColorMethod.ByAci, 7)
            };

            layerTable.Add(record);
            transaction.AddNewlyCreatedDBObject(record, true);
        }
    }
}
