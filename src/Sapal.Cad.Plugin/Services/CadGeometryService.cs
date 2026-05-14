using Autodesk.AutoCAD.DatabaseServices;

namespace Sapal.Cad.Plugin.Services
{
    public class CadGeometryService
    {
        public bool IsSupportedLine(Entity entity)
        {
            return entity is Line || entity is Polyline || entity is Polyline2d || entity is Polyline3d;
        }

        public bool IsSupportedPoint(Entity entity)
        {
            return entity is BlockReference || entity is DBPoint;
        }

        public double GetLength(Entity entity)
        {
            var line = entity as Line;
            if (line != null)
            {
                return line.Length;
            }

            var polyline = entity as Polyline;
            if (polyline != null)
            {
                return polyline.Length;
            }

            var polyline2d = entity as Polyline2d;
            if (polyline2d != null)
            {
                return polyline2d.Length;
            }

            var polyline3d = entity as Polyline3d;
            if (polyline3d != null)
            {
                return polyline3d.Length;
            }

            return 0d;
        }
    }
}
