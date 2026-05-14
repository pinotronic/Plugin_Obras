using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Sapal.Cad.Plugin.Models;

namespace Sapal.Cad.Plugin.Services
{
    public class XDataService
    {
        public void EnsureAppId(Database database, Transaction transaction)
        {
            var regAppTable = (RegAppTable)transaction.GetObject(database.RegAppTableId, OpenMode.ForRead);
            if (regAppTable.Has(Catalogs.AppId))
            {
                return;
            }

            regAppTable.UpgradeOpen();
            var record = new RegAppTableRecord { Name = Catalogs.AppId };
            regAppTable.Add(record);
            transaction.AddNewlyCreatedDBObject(record, true);
        }

        public void WriteXData(Entity entity, IDictionary<string, string> values)
        {
            var bufferValues = new List<TypedValue>
            {
                new TypedValue((int)DxfCode.ExtendedDataRegAppName, Catalogs.AppId)
            };

            foreach (var value in values)
            {
                bufferValues.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, value.Key + "=" + Sanitize(value.Value)));
            }

            entity.XData = new ResultBuffer(bufferValues.ToArray());
        }

        public IDictionary<string, string> ReadXData(Entity entity)
        {
            var values = new Dictionary<string, string>();
            using (var buffer = entity.GetXDataForApplication(Catalogs.AppId))
            {
                if (buffer == null)
                {
                    return values;
                }

                foreach (var typedValue in buffer)
                {
                    if (typedValue.TypeCode != (int)DxfCode.ExtendedDataAsciiString || typedValue.Value == null)
                    {
                        continue;
                    }

                    var item = typedValue.Value.ToString();
                    var separatorIndex = item.IndexOf('=');
                    if (separatorIndex <= 0)
                    {
                        continue;
                    }

                    values[item.Substring(0, separatorIndex)] = item.Substring(separatorIndex + 1);
                }
            }

            return values;
        }

        public void ClearXData(Entity entity)
        {
            entity.XData = new ResultBuffer(new TypedValue((int)DxfCode.ExtendedDataRegAppName, Catalogs.AppId));
        }

        private static string Sanitize(string value)
        {
            return (value ?? string.Empty).Replace("\r", " ").Replace("\n", " ");
        }
    }
}
