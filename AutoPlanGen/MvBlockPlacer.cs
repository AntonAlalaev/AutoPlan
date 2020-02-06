// 
// Лохматый класс из VB написанный 7 лет назад
// 
//


using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace AutoPlan
{
    public class MvBlockPlacer : DrawJig
    {
        private Point3d previousCurserPosition;
        private Point3d currentCurserPosition;
        private Entity entityToDrag;
        private Database db = Application.DocumentManager.MdiActiveDocument.Database;
        private Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        private int mPromptCounter;
        private string sampstatus;
        private double rotationadj;
        private string jiginput;
        private string jigmsg;
        private ObjectId retObjID;

        public void StartJig(string MvBlockName, double ScaleX, double ScaleY, double ScaleZ)
        {
            string blockname;
            rotationadj = 0; // Double
            mPromptCounter = 0; // integer
            blockname = MvBlockName;

            // Словарь стилей МВ Блоков
            Autodesk.Aec.DatabaseServices.DictionaryMultiViewBlockDefinition mvbstyledict;

            ObjectId bstyleID;
            Autodesk.Aec.DatabaseServices.MultiViewBlockReference mvb;
            mvb = new Autodesk.Aec.DatabaseServices.MultiViewBlockReference();
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            // Получаем словарь МВ Блоков
            mvbstyledict = new Autodesk.Aec.DatabaseServices.DictionaryMultiViewBlockDefinition(db);
            // Получаем ObjectID заданного стиля MVBlock
            bstyleID = mvbstyledict.GetAt(blockname);
            ed.WriteMessage("\n blockname " + blockname + "\n");
            // Определяем вставляемый блок полученным ObjectID нужного нам стиля
            mvb.BlockDefId = bstyleID;
            // делаем масштаб
            Scale3d NewScale;
            NewScale = new Scale3d(ScaleX, ScaleY, ScaleZ);

            mvb.Scale = NewScale;
            // Даем команду ACAD'у что нужно таскать (drag) блок заданного стиля
            entityToDrag = mvb;
            // Определяем позицию курсора непонятно зачем)
            previousCurserPosition = new Point3d(0, 0, 0);
            sampstatus = "NoChange";
            while (sampstatus == "NoChange")
            {
                Application.DocumentManager.MdiActiveDocument.Editor.Drag(this);
                // SamplerStatus then World draw runs from here
                mvb.Rotation = rotationadj;
            }
            // Таскаем пока статус sampstatus не изменится с NoChange
            mPromptCounter = 1;
            if (sampstatus == "OK")
            {
                using (Transaction myT = tm.StartTransaction())
                {
                    // Dim doc1 As Document = Application.DocumentManager.MdiActiveDocument
                    // Dim dl As DocumentLock = doc1.LockDocument
                    Document doc = Application.DocumentManager.MdiActiveDocument;
                    using (DocumentLock acLckDoc = doc.LockDocument())
                    {
                        BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead, false);
                        BlockTableRecord btr = tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        //(BlockTableRecord)tm.GetObject(bt(BlockTableRecord.ModelSpace), OpenMode.ForWrite, false);
                        btr.AppendEntity(mvb);
                        tm.AddNewlyCreatedDBObject(mvb, true);
                        // Возвращаем значение нового ObjectID для блока
                        retObjID = mvb.ObjectId;
                        myT.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Вставляет МВ-блок в пространство модели и возвращает его ObjectId
        /// </summary>
        /// <param name="doc">Документ Автокада</param>
        /// <param name="MvBlockName">Имя МВ-Блока</param>
        /// <param name="InsertionPoint">Точка вставки</param>
        /// <param name="InsertionScale">Масштаб</param>
        /// <param name="Rotation">Угол поворота</param>
        /// <returns></returns>
        public static ObjectId MvBlockRefInsert(Document doc, string MvBlockName, Point3d InsertionPoint, Scale3d InsertionScale, double Rotation = 0)
        {
            ObjectId returnObjID = new ObjectId();
            // словарь стилей MВ-блоков
            Autodesk.Aec.DatabaseServices.DictionaryMultiViewBlockDefinition mvbstyledict;

            ObjectId bstyleID;
            Autodesk.Aec.DatabaseServices.MultiViewBlockReference mvb = new Autodesk.Aec.DatabaseServices.MultiViewBlockReference();

            Database db = doc.Database;
            using (DocumentLock acLckDoc = doc.LockDocument())
            {
                Transaction tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    // открываем таблицу MB блоков на чтение
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    // Получаем словарь МВ Блоков
                    mvbstyledict = new Autodesk.Aec.DatabaseServices.DictionaryMultiViewBlockDefinition(db);
                    // Получаем ObjectID заданного стиля MVBlock
                    bstyleID = mvbstyledict.GetAt(MvBlockName);
                    if (bstyleID == null)
                    {
                        doc.Editor.WriteMessage("\n MvBlockName is not avialable" + "\n");
                        return returnObjID;
                    }
                    //doc.Editor.WriteMessage("\n MvBlockName is " + "");
                    // Определяем вставляемый блок полученным ObjectID нужного нам стиля
                    mvb.BlockDefId = bstyleID;
                    mvb.Scale = InsertionScale;
                    doc.Editor.WriteMessage("\n Приведенный масштаб вставляемого элемента X:" + mvb.Scale.X + " Y:" + mvb.Scale.Y + " Z:" + mvb.Scale.Z + "\n");
                    mvb.Location = InsertionPoint;
                    mvb.Rotation = Rotation;
                    // открываем пространство модели на запись
                    BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    ms.AppendEntity(mvb);
                    tr.AddNewlyCreatedDBObject(mvb, true);
                    returnObjID = mvb.ObjectId;
                    tr.Commit();
                    tr.Dispose();
                }
            }
            return returnObjID;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jigOpts = new JigPromptPointOptions();
            jigOpts.UserInputControls = (UserInputControls.Accept3dCoordinates | UserInputControls.AcceptOtherInputString);
            if (mPromptCounter == 0)
            {
                jigOpts.Message = "\nУкажите точку вставки ...\n";
                PromptPointResult dres = prompts.AcquirePoint(jigOpts);
                currentCurserPosition = dres.Value;
                if (dres.Status == PromptStatus.Cancel)
                {
                    sampstatus = "Cancel";
                    return SamplerStatus.Cancel;
                }
                if (currentCurserPosition == previousCurserPosition)
                {
                    // If currentCurserPosition <> previousCurserPosition Then
                    sampstatus = "OK";
                    return SamplerStatus.OK;
                }
                if (dres.StringResult != null)
                {
                    // Here put switch case for keywords.
                    jigmsg = dres.StringResult;
                    switch (jigmsg)
                    {
                        case "NAme":
                            {
                                ed.WriteMessage("\nNAme");
                                sampstatus = "NoChange";
                                return SamplerStatus.NoChange;
                            }

                        case "Xscale":
                            {
                                ed.WriteMessage("\nXscale");
                                sampstatus = "NoChange";
                                return SamplerStatus.NoChange;
                            }

                        case "Yscale":
                            {
                                ed.WriteMessage("\nYscale");
                                sampstatus = "NoChange";
                                return SamplerStatus.NoChange;
                            }

                        case "Zscale":
                            {
                                ed.WriteMessage("\nZscale");
                                sampstatus = "NoChange";
                                return SamplerStatus.NoChange;
                            }

                        case "Rotation":
                            {
                                // Convert rad to degree  rotationadj = (jigdbl * 3.14159265358979) / 180;
                                sampstatus = "NoChange";
                                mPromptCounter = 1;
                                return SamplerStatus.NoChange;
                            }

                        case "Match":
                            {
                                ed.WriteMessage("\nMatch");
                                sampstatus = "NoChange";
                                return SamplerStatus.NoChange;
                            }

                        case "Base":
                            {
                                ed.WriteMessage("\nBase");
                                sampstatus = "NoChange";
                                return SamplerStatus.NoChange;
                            }

                        default:
                            {
                                sampstatus = "NoChange";
                                return SamplerStatus.NoChange;
                            }
                    }
                }
                sampstatus = "NoChange";
                return SamplerStatus.NoChange;
            }
            else if (mPromptCounter == 1)
            {
                jigOpts.UserInputControls = (UserInputControls.Accept3dCoordinates | UserInputControls.AcceptOtherInputString);
                jigOpts.Keywords.Clear();
                jigOpts.Message = "\nRotation: ";
                PromptPointResult rres = prompts.AcquirePoint(jigOpts);
                currentCurserPosition = rres.Value;
                if (rres.StringResult != null)
                {
                    jiginput = rres.StringResult.ToString();
                    mPromptCounter = 0;
                    sampstatus = "NoChange";
                    return SamplerStatus.NoChange;
                }
                if (rres.Status == PromptStatus.Error)
                {
                    sampstatus = "Error";
                    return SamplerStatus.NoChange;
                }
                if (rres.Status == PromptStatus.Cancel)
                {
                    sampstatus = "Cancel";
                    return SamplerStatus.Cancel;
                }
                if (rres.Status == PromptStatus.Keyword)
                {
                    sampstatus = "Keyword";
                    return SamplerStatus.NoChange;
                }
                if (rres.Status == PromptStatus.Modeless)
                {
                    sampstatus = "Modeless";
                    return SamplerStatus.NoChange;
                }
                if (rres.Status == PromptStatus.None)
                {
                    sampstatus = "None";
                    return SamplerStatus.NoChange;
                }
                if (rres.Status == PromptStatus.Other)
                {
                    sampstatus = "Other";
                    return SamplerStatus.NoChange;
                }
                sampstatus = "NoChange";
                return SamplerStatus.NoChange;
            }
            else
            {
                sampstatus = "NoChange";
                return SamplerStatus.NoChange;
            }
        }
        protected override bool WorldDraw(WorldDraw draw)
        {
            if (mPromptCounter == 1)
            {
                Vector3d displacementVector = previousCurserPosition.GetVectorTo(currentCurserPosition);
                entityToDrag.TransformBy(Matrix3d.Displacement(displacementVector));
                previousCurserPosition = currentCurserPosition;
                draw.Geometry.Draw(entityToDrag);
            }
            else
            {
                Vector3d displacementVector = previousCurserPosition.GetVectorTo(currentCurserPosition);
                entityToDrag.TransformBy(Matrix3d.Displacement(displacementVector));
                previousCurserPosition = currentCurserPosition;
                draw.Geometry.Draw(entityToDrag);
            }

            return true;
        }
        public ObjectId returnObjID()
        {
            return retObjID;
        }
    }
}