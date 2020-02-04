using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using System.Globalization;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Rt = Autodesk.AutoCAD.Runtime;
using cad = Autodesk.AutoCAD.ApplicationServices.Application;
using AutoPlan;

namespace AutoPlanGen
{
    public class EntryPoint
    {
        [Rt.CommandMethod("StellarGenerator", CommandFlags.Modal)]
        public static void Generator()
        {

            try
            {
                // Принудительно делаем культуру (локализацию) потока русской
                System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru");
                cad.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n Loaded \n");
                DialogWindow formDialog = new DialogWindow();
                Application.ShowModelessWindow(formDialog);
            }
            catch (System.Exception ex)
            {
                Application.ShowAlertDialog(ex.Message);
            }
        }



        /// <summary>
        /// Выбор прямоугольника
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Rectangle SelectRoomArea(Document doc)
        {
            Editor ed = doc.Editor;
            Database db = doc.Database;
            PromptEntityOptions optEnt = new PromptEntityOptions("\n выберите прямоугольник (полилинию), который очерчивает место будущего расположения стелажей \n");
            optEnt.SetRejectMessage("\n Выбранный вами объект не является плоской полилинией \n");
            optEnt.AddAllowedClass(typeof(Polyline), false);

            PromptEntityResult per = ed.GetEntity(optEnt);
            if (per.Status == PromptStatus.OK)
            {
                Transaction tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    DBObject obj = tr.GetObject(per.ObjectId, OpenMode.ForRead);
                    Polyline lwp = obj as Polyline;
                    if (lwp != null)
                    {
                        double MinX, MinY, MaxX, MaxY;
                        MinX = lwp.GetPoint2dAt(0).X;
                        MaxX = lwp.GetPoint2dAt(0).X;
                        MinY = lwp.GetPoint2dAt(0).Y;
                        MaxY = lwp.GetPoint2dAt(0).Y;
                        // modern style Polyline
                        for (int i = 0; i < lwp.NumberOfVertices; i++)
                        {
                            if (lwp.GetPoint2dAt(i).X > MaxX)
                                MaxX = lwp.GetPoint2dAt(i).X;

                            if (lwp.GetPoint2dAt(i).X < MinX)
                                MinX = lwp.GetPoint2dAt(i).X;

                            if (lwp.GetPoint2dAt(i).Y < MinY)
                                MinY = lwp.GetPoint2dAt(i).Y;
                            
                            if (lwp.GetPoint2dAt(i).Y > MaxY)
                                MaxY = lwp.GetPoint2dAt(i).Y;
                        }
                        tr.Commit();
                        return new Rectangle(new Point(MinX, MinY), new Point(MaxX, MaxY));
                    }
                    else
                    {
                        // old style 2D, 3D Polyline
                        Polyline2d p2d = obj as Polyline2d;
                        if (p2d != null)
                        {
                            List<Point> Point2D = new List<Point>();
                            foreach (ObjectId vId in p2d)
                            {
                                Vertex2d v2d = tr.GetObject(vId, OpenMode.ForRead) as Vertex2d;
                                Point2D.Add(new Point(v2d.Position.X, v2d.Position.Y));
                            }
                            double MinX, MinY, MaxX, MaxY;
                            MinX = Point2D.Min(n => n.X);
                            MinY = Point2D.Min(n => n.Y);
                            MaxX = Point2D.Max(n => n.X);
                            MaxY = Point2D.Max(n => n.Y);
                            tr.Commit();
                            return new Rectangle(new Point(MinX, MinY), new Point(MaxX, MaxY));
                        }
                        else
                        {
                            // значит 3d polyline
                            Polyline3d p3d = obj as Polyline3d;
                            List<Point> Point2D = new List<Point>();
                            if (p3d != null)
                            {
                                foreach (ObjectId vId in p2d)
                                {
                                    Vertex2d v2d = tr.GetObject(vId, OpenMode.ForRead) as Vertex2d;
                                    Point2D.Add(new Point(v2d.Position.X, v2d.Position.Y));
                                }
                                double MinX, MinY, MaxX, MaxY;
                                MinX = Point2D.Min(n => n.X);
                                MinY = Point2D.Min(n => n.Y);
                                MaxX = Point2D.Max(n => n.X);
                                MaxY = Point2D.Max(n => n.Y);
                                tr.Commit();
                                return new Rectangle(new Point(MinX, MinY), new Point(MaxX, MaxY));

                            }
                            tr.Commit();
                            return new Rectangle(new Point(0, 0), new Point(0, 0));
                        }
                    }                    
                }
            }
            return new Rectangle(new Point(0,0), new Point(0,0));
        }


        /// <summary>
        /// Method from Kean Walmsley
        /// </summary>
        [CommandMethod("LV")]
        static public void ListVertices()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            PromptEntityOptions optEnt = new PromptEntityOptions("\n выберите прямоугольник (полилинию), который очерчивает место будущего расположения стелажей \n");
            optEnt.SetRejectMessage("\n Выбранный вами объект не является плоской полилинией \n");
            optEnt.AddAllowedClass(typeof(Polyline), false);

            PromptEntityResult per = ed.GetEntity(optEnt);
            if (per.Status == PromptStatus.OK)
            {
                Transaction tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    DBObject obj = tr.GetObject(per.ObjectId, OpenMode.ForRead);
                    // If a "lightweight" (or optimized) polyline
                    Polyline lwp = obj as Polyline;
                    if (lwp != null)
                    {
                        // Use a for loop to get each vertex, one by one
                        int vn = lwp.NumberOfVertices;
                        for (int i = 0; i < vn; i++)
                        {
                            // Could also get the 3D point here
                            Point2d pt = lwp.GetPoint2dAt(i);
                            ed.WriteMessage("\n" + pt.ToString());
                        }
                    }
                    else
                    {
                        // If an old-style, 2D polyline
                        Polyline2d p2d = obj as Polyline2d;
                        if (p2d != null)
                        {
                            // Use foreach to get each contained vertex
                            foreach (ObjectId vId in p2d)
                            {
                                Vertex2d v2d = (Vertex2d)tr.GetObject(vId, OpenMode.ForRead);
                                ed.WriteMessage("\n" + v2d.Position.ToString());
                            }
                        }
                        else
                        {
                            // If an old-style, 3D polyline
                            Polyline3d p3d = obj as Polyline3d;
                            if (p3d != null)
                            {
                                // Use foreach to get each contained vertex
                                foreach (ObjectId vId in p3d)
                                {
                                    PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(vId, OpenMode.ForRead);
                                    ed.WriteMessage("\n" + v3d.Position.ToString());
                                }
                            }
                        }
                    }
                    // Committing is cheaper than aborting
                    tr.Commit();
                }
            }
        }
    }
}
