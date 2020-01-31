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
    }
}
