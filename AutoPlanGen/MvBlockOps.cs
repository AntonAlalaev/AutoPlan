// 
// Лохматый код, преобразованный из VB написанный 7 лет назад
// требует конкретного такого рефакторинга
// 
// 


// From JigSaw
using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Aec.DatabaseServices;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Collections.Specialized;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using ObjectIdCollection = Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using AecDb = Autodesk.Aec.DatabaseServices;
using System.Windows.Forms;

namespace AutoPlan
{

    public class MvBlockOps
    {
        // By me
        private ObjectId PlacedBlockObjID;
        private string SourceDirectoryDWGPath;

        public void DefineSourcePath(string SourceDirectoryPath)
        {
            SourceDirectoryDWGPath = SourceDirectoryPath;
        }

        public void PlaceMvBlock(string MvBlockName, string SourceFileName, Document doc)
        {
            Scale3d Scale = new Scale3d();
            CloneMvBlock(MvBlockName, SourceDirectoryDWGPath, SourceFileName, ref Scale, doc);
            // Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
            MvBlockPlacer Placer1 = new MvBlockPlacer();
            Placer1.StartJig(MvBlockName, Scale.X, Scale.Y, Scale.Z);
            PlacedBlockObjID = Placer1.returnObjID();
        }

        public ObjectId ReturnObjectID()
        {
            return PlacedBlockObjID;
        }
        public static void CloneMvBlock(string MVBlockName, string SourceDirectoryDWGPath, string SourceFileName, ref Scale3d scale, Document doc)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            // MsgBox(SourceDirectoryDWGPath)
            if (SourceDirectoryDWGPath == "")
            {
                MessageBox.Show("Source Directory DWG Path not defined!!!");
                //Interaction.MsgBox("Source Directory DWG Path not defined!!!");
                return;
            }
            // сначала надо проверить есть ли такой блок уже или нет


            try
            {
                Database dbDestination = doc.Database;
                string SourcePath = SourceDirectoryDWGPath + @"\" + SourceFileName;
                Autodesk.AutoCAD.DatabaseServices.Database dbSource = new Database(false, true);
                ed.WriteMessage("\n Открываю файл с заданным блоком");
                try
                {
                    dbSource.ReadDwgFile(SourcePath, System.IO.FileShare.Read, true, "");
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    MessageBox.Show("Не могу открыть файл" + ex.Message);
                }
                using (DocumentLock acLocDoc = doc.LockDocument())
                {

                    // Dim MvBlockRef As MultiViewBlockReference
                    using (Transaction t = dbSource.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = t.GetObject(dbSource.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord btr = t.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead, false) as BlockTableRecord;

                        foreach (ObjectId id in btr)
                        {
                            try
                            {
                                Entity ent = t.GetObject(id, OpenMode.ForRead) as Entity;
                                string entType = ent.GetType().ToString();
                                if (entType == "Autodesk.Aec.DatabaseServices.MultiViewBlockReference")
                                {
                                    MultiViewBlockReference MvBlockRef = t.GetObject(id, OpenMode.ForRead) as MultiViewBlockReference;
                                    scale = MvBlockRef.Scale;
                                    //ed.WriteMessage("Получил масштаб X:" + MvBlockRef.Scale.X + " Y:" + MvBlockRef.Scale.Y + " Z:" + MvBlockRef.Scale.Z + "\n");
                                }
                            }
                            catch (Autodesk.AutoCAD.Runtime.Exception ex)
                            {
                                ed.WriteMessage("Не могу найти элемент!!!" + ex.Message);
                            }
                        }
                        t.Commit();
                        t.Dispose();
                    }
                    // get the source dictionary
                    DictionaryMultiViewBlockDefinition dictStyle = new DictionaryMultiViewBlockDefinition(dbSource);
                    // get the list of style ids that you want to import
                    // (1) if you want to import everything, use this.
                    // the list of ids in the style dictionary.
                    // ObjectIdCollection objCollectionSrc = dictStyle.Records
                    // (2) if you want to import a specific style, use this.
                    // we assume you know the name of style you want to import. 

                    Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection objCollectionSrc = new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
                    objCollectionSrc.Add(dictStyle.GetAt(MVBlockName));

                    //Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection objCollectionSrc = dictStyle.Records;

                    // now use CloningHelper class to import styles.  
                    // there are four options for merge type:
                    // Normal     = 0, // no overwrite
                    // Overwrite  = 1, // this is default.
                    // Unique     = 2, // rename it if the same name exists.
                    // Merge      = 3  // no overwrite + add overlapping as anonymous
                    Autodesk.Aec.ApplicationServices.Utility.CloningHelper helpme = new Autodesk.Aec.ApplicationServices.Utility.CloningHelper();
                    helpme.MergeType = Autodesk.Aec.ApplicationServices.Utility.DictionaryRecordMergeBehavior.Normal;
                    helpme.Clone(dbSource, dbDestination, objCollectionSrc, dictStyle.RecordType, true);
                    helpme.Dispose();
                    dbSource.Dispose();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Не могу открыть файл со стеллажами!!!");
                ed.WriteMessage("Error in CloneHelper " + ex.Message + "\n");
                return;
            }
        }
        public void CreatePropSetDefs(string propSetDefName, StringCollection RowNames, Document doc)
        {
            // Here we are creating a propertysetdefinition with PropsetDefsName
            // It must contain RowNames and Row Count

            Database db = doc.Database;
            Editor ed = doc.Editor;
            // The name of the property set def.
            // Could be for objects or styles. Hard coding for simplicity.
            // This will be the key in the dictionary
            // Dim propSetDefName As String = "ACADoorObjects"
            // Dim propSetDefName As String = "ACANetDoorStyles"
            if (RowNames.Count == 0)
            {
                // There are empty rows
                MessageBox.Show("Error! Cannot create a empty propertysetdef!!");
                return;
            }
            try
            {
                // (1) create prop set def
                PropertySetDefinition propSetDef = new PropertySetDefinition();
                propSetDef.SetToStandard(db);
                propSetDef.SubSetDatabaseDefaults(db);
                // alternatively, you can use dictionary's NewEntry
                // Dim dictPropSetDef = New DictionaryPropertySetDefinitions(db)
                // Dim propSetDef As PropertySetDefinition =
                // dictPropSetDef.NewEntry()
                // General tab
                propSetDef.Description = "Описание парметров стеллажа. Создано автоматически";
                // Applies To tab
                // apply to objects or styles. True if style, False if objects
                bool isStyle = false;
                var appliedTo = new StringCollection();
                appliedTo.Add("AecDbMvBlockRef"); // apply to a MVBlock Reference
                                                  // appliedTo.Add("AecDBMVBlockRef") ' apply to a MVBlock Reference
                                                  // appliedTo.Add("AecDbDoor")       ' apply to a door object 
                                                  // appliedTo.Add("AecDbDoorStyle") ' apply to a door style
                                                  // apply to more than one object type, add more here.
                                                  // appliedTo.Add("AecDbWindow")   
                propSetDef.SetAppliesToFilter(appliedTo, isStyle);
                // propSetDef.AppliesToAll = True
                // Definition tab
                // (2) we can add a set of property definitions. 
                // We first make a container to hold them.
                // This is the main part. A property set definition can contain
                // a set of property definition.
                // (2.1) let's first add manual property.
                // Here we use text type
                PropertyDefinition propDefManual = new PropertyDefinition();
                // = New PropertyDefinition()
                // Adding a Manual propertysetdef
                int i;
                for (i = 0; i <= RowNames.Count - 1; i++)
                {
                    propDefManual = new PropertyDefinition();
                    propDefManual.SetToStandard(db);
                    propDefManual.SubSetDatabaseDefaults(db);
                    // MsgBox("Row name:" & RowNames.Item(i))
                    // MsgBox(RowNames.Item(i))
                    propDefManual.Name = RowNames[i];
                    propDefManual.Description = "Параметры стеллажа";
                    propDefManual.DataType = Autodesk.Aec.PropertyData.DataType.Text;
                    // propDefManual.Name = "ACAManualProp"
                    // propDefManual.Description = "Manual property by ACA.NET"
                    // propDefManual.DataType = Autodesk.Aec.PropertyData.DataType.Text
                    propDefManual.DefaultData = "0";
                    // add to the prop set def
                    propSetDef.Definitions.Add(propDefManual);
                }
                propSetDef.IsVisible = true;
                // (2.2) let's add another one, automatic one this time
                // Dim propDefAutomatic = New PropertyDefinition()
                // propDefAutomatic.SetToStandard(db)
                // propDefAutomatic.SubSetDatabaseDefaults(db)
                // propDefAutomatic.Name = "ACAWidth"
                // propDefAutomatic.Description = "Automatic property by ACA.NET"
                // propDefAutomatic.SetAutomaticData("AecDbDoor", "Width")
                // add to the prop set def
                // propSetDef.Definitions.Add(propDefAutomatic)
                // similarly, add one with height
                // propDefAutomatic = New PropertyDefinition()
                // propDefAutomatic.SetToStandard(db)
                // propDefAutomatic.SubSetDatabaseDefaults(db)
                // propDefAutomatic.Name = "ACAHeight"
                // propDefAutomatic.Description = "Automatic property by ACA.NET"
                // propDefAutomatic.SetAutomaticData("AecDbDoor", "Height")
                // add to the prop set def
                // propSetDef.Definitions.Add(propDefAutomatic)
                // (3)  finally add the prop set def to the database

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // check the name
                    var dictPropSetDef = new DictionaryPropertySetDefinitions(db);
                    if (dictPropSetDef.Has(propSetDefName, tr))
                    {
                        ed.WriteMessage("error - the property set defintion already exists: " + propSetDefName + "\n");
                        // try to delete?
                        tr.Abort();
                        return;
                    }
                    using (DocumentLock acLckDoc = doc.LockDocument())
                    {
                        dictPropSetDef.AddNewRecord(propSetDefName, propSetDef);
                        tr.AddNewlyCreatedDBObject(propSetDef, true);
                        tr.Commit();
                        tr.Dispose();
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("error in CreatePropSetDef: " + ex.ToString() + "\n");
                return;
            }
            ed.WriteMessage("property set definition " + propSetDefName + " is successfully created." + "\n");
        }
        // Attach a property set to an object.
        /// <summary>
        /// Attach a property set to an object.
        /// </summary>
        /// <param name="ObjID">ObjectId Элемента</param>
        /// <param name="propSetDefName">Имя PropertySet'а</param>
        public void AttachPropSetDef(ObjectId ObjID, string propSetDefName)
        {
            // MsgBox(ObjID.ToString)
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            var dictPropSetDef = new DictionaryPropertySetDefinitions(db);
            ObjectId idPropSetDef = Utils.findStyle(dictPropSetDef, propSetDefName);

            if (idPropSetDef == null/* TODO Change to default(_) if this is not a reference type */ )
            {
                MessageBox.Show("Ошибка, определение property set definition не обнаружено!");
                //Interaction.MsgBox("Ошибка, определение property set definition не обнаружено!");
                return;
            }
            else
            {
                ed.WriteMessage("\n" + "Property Set " + propSetDefName + " ObjectID: " + idPropSetDef.ToString() + "\n");
                ed.WriteMessage("target object " + ObjID.ToString() + "\n");
            }
            // If we come here, we have a prop set def id and an object id. 
            // (3) Attach the given property set to the given object.
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // imports AcObject = Autodesk.AutoCAD.DatabaseServices.DBObject
                    // PropertyDataServices provide a convenient method to do
                    // the actual work.
                    using (DocumentLock acLckDoc = doc.LockDocument())
                    {
                        Autodesk.AutoCAD.DatabaseServices.DBObject obj = tr.GetObject(ObjID, OpenMode.ForWrite, false, true);
                        PropertyDataServices.AddPropertySet(obj, idPropSetDef);
                        tr.Commit();
                        tr.Dispose();
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("error in AttachPropertySet: " + ex.ToString() + "\n");
                return;
            }
            ed.WriteMessage("Property Set successfully addet to object " + ObjID.ToString() + " ObjectType is " + ObjID.ObjectClass.Name + "\n");
        }

        // Getting Object ID from selection
        public Autodesk.AutoCAD.DatabaseServices.ObjectId GetObjIDbySelection()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptEntityOptions optEnt = new PromptEntityOptions("\n" + "Select an AEC object" + "\n");
            optEnt.SetRejectMessage("\n" + "Selected entity is NOT an AEC object, try again...\n");
            // "Geo" is the base class for AEC object.
            // Use this if you want to apply to all the AEC objects.
            optEnt.AddAllowedClass(typeof(Geo), false);
            PromptEntityResult resEnt = ed.GetEntity(optEnt);
            if (resEnt.Status != PromptStatus.OK)
            {
                ed.WriteMessage("Selection error - aborting\n");
                return resEnt.ObjectId;
            }
            return resEnt.ObjectId;
        }
        public bool SetValueFromPropertySetByName(string psetname, string pname, Autodesk.AutoCAD.DatabaseServices.DBObject dbobj, object NewValue)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            bool findany = false;
            // Dim trId As ObjectIdCollection =Autodesk .Aec.PropertyData .DatabaseServices .PropertyDataServices .GetPropertySet(d
            ObjectIdCollection setIds = Autodesk.Aec.PropertyData.DatabaseServices.PropertyDataServices.GetPropertySets(dbobj);
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    //ObjectId psId;
                    foreach (ObjectId psId in setIds) // setids is all property sets
                    {
                        Autodesk.Aec.PropertyData.DatabaseServices.PropertySet pset = (PropertySet)tr.GetObject(psId, OpenMode.ForWrite, false, false); // As AecPropDb.PropertySet
                        if (pset.PropertySetDefinitionName == psetname)
                        {
                            int pid; // have to create this object to place the PropertyNameToId somewhere
                            pid = pset.PropertyNameToId(pname); // propertynametoid gives the id for the psetdef
                            ed.WriteMessage("\n Property ID " + pid + " Property Name " + pset.PropertyIdToName(pid) + "\n");
                            using (DocumentLock acLckDoc = doc.LockDocument()) // 
                            {
                                // pset.PropertySetData.
                                pset.SetAt(pid, NewValue);
                            }
                            findany = true;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("error in SetValueFromPropertySetByName: " + ex.ToString() + "\n");
                //ed.WriteMessage("error in SetValueFromPropertySetByName: " + ex.ToString + Constants.vbCrLf);
                return findany;
            }
            return findany;
        }
        /// <summary>
        /// Установка значений PropertySet по ObjectId
        /// </summary>
        /// <param name="psetname">Наименование propertySet</param>
        /// <param name="pname">Наименование property</param>
        /// <param name="ObjID">ObjectId</param>
        /// <param name="NewValue">Значение property</param>
        public void setValueFromOBJID(string psetname, string pname, ObjectId ObjID, object NewValue)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            bool bolres;
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // imports AcObject = Autodesk.AutoCAD.DatabaseServices.DBObject
                    // PropertyDataServices provide a convenient method to do
                    // the actual work.
                    using (DocumentLock acLckDoc = doc.LockDocument())
                    {
                        Autodesk.AutoCAD.DatabaseServices.DBObject obj = tr.GetObject(ObjID, OpenMode.ForRead, false, false);
                        // PropertyDataServices.AddPropertySet(obj, idPropSetDef)
                        bolres = SetValueFromPropertySetByName(psetname, pname, obj, NewValue);
                        ed.WriteMessage("\nResult of setvalue from OBJ ID is " + bolres + "\n");
                        tr.Commit();
                        tr.Dispose();
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("\nerror in get obj " + ex.ToString() + "\n");
                return;
            }
            ed.WriteMessage("\nDBObject Successfuly founded" + ObjID.ToString() + " ObjectType is " + ObjID.ObjectClass.Name + "\n");
        }
        /// <summary>
        /// ПРисваивает значение Property
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="Props"></param>
        public void setProperties(ObjectId ObjID, BlockAttributes Props)
        {
            if (Props.Count == 0)
                return;
            //long i;
            for (int i = 0; i < Props.Count; i++)
                setValueFromOBJID(Props.Name, Props.ParNames[i], ObjID, Props.ParValues[i]);
        }

        public BlockAttributes getBlockAttributes(System.Data.DataTable Tbl, ParTable Pars)
        {
            BlockAttributes retVal = new BlockAttributes();
            if (Tbl.Rows.Count == 0)
            {
                MessageBox.Show("\nError there are empty Datatable!!!\n");
                //Interaction.MsgBox(Constants.vbCrLf + "Error there are empty Datatable!!!" + Constants.vbCrLf);
                return retVal;
            }
            //long i;
            // значения в таблице
            // ////////////////////
            // Dim ParID As Integer
            for (int i = 0; i < Tbl.Rows.Count; i++)
                // Поиск кода параметра по имени
                // ParID = Pars.getIDParfromName(Tbl.Rows(i).Item(0).ToString)
                // retVal.Add(ParID, Tbl.Rows(i).Item(1).ToString)                
                retVal.Add(Tbl.Rows[i].ItemArray[0].ToString(), Tbl.Rows[i].ItemArray[1].ToString());
            return retVal;
        }
        public string getMVBNameByObjID(ObjectId objID)
        {
            string retValue = "";
            if (objID.ObjectClass.Name != "AecDbMvBlockRef")
            {
                retValue = "Error. Not AecDbMvBlockRef";
                return retValue;
            }
            // Dim mvb As Autodesk.Aec.DatabaseServices.MultiViewBlockReference
            // mvb.BlockDefId = objID
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Autodesk.Aec.DatabaseServices.MultiViewBlockReference mvb;
            MultiViewBlockDefinition def;
            // Dim tm As Autodesk.AutoCAD.DatabaseServices.TransactionManager = db.TransactionManager
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    using (DocumentLock acLckDoc = doc.LockDocument())
                    {
                        mvb = tr.GetObject(objID, OpenMode.ForRead) as MultiViewBlockReference;
                        def = tr.GetObject(mvb.StyleId, OpenMode.ForRead) as MultiViewBlockDefinition;
                        retValue = def.Name;
                        tr.Commit();
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("An error occured at getmvblockname " + ex.ToString() + "\n");
                retValue = "An error occured at getmvblockname";
            }
            return retValue;
        }
        public BlockAttributes GetAttributes(ObjectId MvbObjID)
        {
            BlockAttributes attr = null/* TODO Change to default(_) if this is not a reference type */;
            string blName = getMVBNameByObjID(MvbObjID);
            if (blName == "Error. Not AecDbMvBlockRef" | blName == "An error occured at getmvblockname")
                return attr;
            attr = GetAttributesFromObject(MvbObjID, blName);
            if (attr.Count == 0)
                attr = null/* TODO Change to default(_) if this is not a reference type */;
            return attr;
        }
        public BlockAttributes[] getChekedAttributesList(ObjectId[] ObjIDArr)
        {
            BlockAttributes[] Res = null;
            if (ObjIDArr.Count() == 0)
                return Res;
            BlockAttributes Tmp = new BlockAttributes();
            Res = new BlockAttributes[ObjIDArr.Count() - 1 + 1];
            int j = 0;
            for (int i = 0; i <= ObjIDArr.Count() - 1; i++)
            {
                Tmp = GetAttributes(ObjIDArr[i]);
                // MsgBox(" Attribute TMP is " & Tmp.ParNames(0) & " Reccount " & Tmp.Count & " j=" & j & " i=" & i)
                if (Tmp != null)
                {
                    Res[j] = Tmp;
                    j = j + 1;
                }
            }
            if (j == 0)
            {
                Res = null;
                return Res;
            }

            var oldRes = Res;
            Res = new BlockAttributes[j - 1 + 1];
            if (oldRes != null)
                Array.Copy(oldRes, Res, Math.Min(j - 1 + 1, oldRes.Length));
            return Res;
        }


        public StellarAttributesList GroupAttributesList(BlockAttributes[] UngroupedList)
        {
            StellarAttributesList Res = new StellarAttributesList();
            if (UngroupedList.Count() == 0 | UngroupedList == null)
                return null/* TODO Change to default(_) if this is not a reference type */;
            int ResCount = 0;
            // Заполняем первую запись массива       
            Res.Add(UngroupedList[0], 1);
            ResCount = ResCount + 1;
            if (UngroupedList.Count() > 1)
            {
                // Остальные сравниваем
                for (int i = 1; i <= UngroupedList.Count() - 1; i++)
                {
                    // MsgBox("Отладка i=" & i & " UngroupedList.Count =" & UngroupedList.Count)
                    int rCount = ResCount - 1;
                    bool addFlag = false;
                    for (int j = 0; j <= rCount; j++)
                    {
                        // MsgBox("unlist(" & i & ")=" & UngroupedList(i).Name & " res.getstellar(" & j & ")=" & Res.GetStellar(j).Name & _
                        // " is " & compareAttributes(UngroupedList(i), Res.GetStellar(j)))
                        if (compareAttributes(UngroupedList[i], Res.GetStellar(j)))
                        {
                            Res.incAmount(j);
                            addFlag = false;
                            break;
                        }
                        else
                            addFlag = true;
                    }
                    if (addFlag == true)
                    {
                        Res.Add(UngroupedList[i], 1);
                        ResCount = ResCount + 1;
                    }
                }
            }
            return Res;
        }
        public bool compareAttributes(BlockAttributes Attr1, BlockAttributes Attr2)
        {
            // Сравниваем имена
            if (Attr1.Name != Attr2.Name)
                return false;
            // Сравниваем количество
            if (Attr1.Count != Attr2.Count)
                return false;
            // Сортируем
            Attr1 = sortAttributes(Attr1);
            Attr2 = sortAttributes(Attr2);
            // Сравниваем содержимое
            for (int i = 0; i <= Attr1.Count - 1; i++)
            {
                // If String.Compare(Attr1.ParNames(i), Attr2.ParNames(i)) <> 0 Then
                if (Attr1.ParNames[i] != Attr2.ParNames[i])
                    return false;
                if (Attr1.ParValues[i] != Attr2.ParValues[i])
                    return false;
            }
            return true;
        }
        public BlockAttributes sortAttributes(BlockAttributes Attr)
        {
            BlockAttributes sorted = Attr;
            if (Attr.Count == 0)
                return sorted;
            for (int i = 0; i < sorted.Count; i++)
            {
                for (int j = 0; j < sorted.Count - 1; j++)
                {
                    // sorted.Name(j)
                    if (string.Compare(sorted.ParNames[j], sorted.ParNames[j + 1]) > 0)
                    {
                        // If CInt(sorted.ParNames(j)) > CInt(sorted.ParNames(j + 1)) Then
                        string UName, UValue;
                        UName = sorted.ParNames[j];
                        UValue = sorted.ParValues[j];
                        sorted.ParNames[j] = sorted.ParNames[j + 1];
                        sorted.ParValues[j] = sorted.ParValues[j + 1];
                        sorted.ParNames[j + 1] = UName;
                        sorted.ParValues[j + 1] = UValue;
                    }
                }
            }
            return sorted;
        }
        public BlockAttributes GetAttributesFromObject(ObjectId MvBlockObjID, string PropSetDefName)
        {
            BlockAttributes attr = new BlockAttributes();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Получаем DBObject
            Autodesk.AutoCAD.DatabaseServices.DBObject dbObj;
            Autodesk.Aec.PropertyData.DatabaseServices.PropertySet PropSet;
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    dbObj = tr.GetObject(MvBlockObjID, OpenMode.ForRead, false, false);
                    tr.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("\nCannot get DBObject. Error " + ex.ToString() + "\n");
                return attr;
            }
            // Ищем PropertSetDefinitionID по Имени
            var dictPropSetDef = new DictionaryPropertySetDefinitions(db);
            ObjectId idPropSetDef = Utils.findStyle(dictPropSetDef, PropSetDefName);

            if (idPropSetDef.IsNull)
            {
                ed.WriteMessage("Error!!! Current drawing do not have this propertysetdefinition: " + PropSetDefName + "\n");
                return attr;
            }
            // Получаем setID
            ObjectId setID = new ObjectId()/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                setID = Autodesk.Aec.PropertyData.DatabaseServices.PropertyDataServices.GetPropertySet(dbObj, idPropSetDef);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("\nError!!! Selected MVBlock don't have Propsetdef named: " + PropSetDefName + " " + ex.Message + "\n");
                return attr;
            }

            // Получаем properyset
            // Dim propCollection As Autodesk.Aec.PropertyData.DatabaseServices.PropertySetDataCollection
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    PropSet = tr.GetObject(setID, OpenMode.ForRead, false, false) as PropertySet;
                    // ed.WriteMessage("PsetName is " & pset.Name & " Pset.toString " & pset.PropertySetData.ToString & " Item " & pset.PropertySetData.Item(0).GetData.ToString)
                    tr.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("Cannot get PropertySet. Error " + ex.ToString() + "\n");
                return attr;
            }

            Autodesk.Aec.PropertyData.DatabaseServices.PropertySetDefinition PropSetDef;
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    PropSetDef = tr.GetObject(idPropSetDef, OpenMode.ForRead, false, false) as PropertySetDefinition;
                    tr.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("Cannot get Propsetdef. Error " + ex.ToString() + "\n");
                return attr;
            }

            for (int i = 0; i <= PropSetDef.Definitions.Count - 1; i++)
                attr.Add(PropSetDef.Definitions[i].Name, PropSet.PropertySetData[i].GetData().ToString());
            attr.Name = PropSetDefName;
            return attr;
        }
        public Autodesk.AutoCAD.DatabaseServices.ObjectId[] GetObjectsFromSelection(string PromptString = "Select AEC Objects", string ObjTypeDXFName = "AEC_MVBLOCK_REF")
        {
            Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                // set up the entity prompt options, note that we only want a selected object
                PromptSelectionOptions prOptions = new PromptSelectionOptions();
                TypedValue[] values = new[] { new TypedValue((int)DxfCode.Start, ObjTypeDXFName) };
                SelectionFilter selFilter = new SelectionFilter(values);
                // prompt the user to select an objects
                PromptSelectionResult prResult = ed.GetSelection(prOptions, selFilter);
                if (prResult.Status == PromptStatus.Cancel)
                    return null;
                SelectionSet selSet = prResult.Value;
                // ed.WriteMessage("Selected " & selSet.GetObjectIds().Count() & " object")
                return selSet.GetObjectIds();
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ed.WriteMessage("Error!!! Cannot get an selection. " + e.Message + "\n");
                return null;
            }
        }
    }

    // And here is the helper functions, Utils.findStyle() I used in the above code: 

    public class Utils
    {
        // Helper function: findStyle().
        // Find a style (or dictionary record) with the given name
        // from the given dictionary, and return its object id. 
        public static ObjectId findStyle(AecDb.Dictionary dict, string key)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            // The id of the style we are looking for. Return value
            ObjectId id = new ObjectId(); /* TODO Change to default(_) if this is not a reference type */;
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // Do we have a property set definition with the given name? 
                    if (!dict.Has(key, tr))
                    {
                        // If not, return
                        ed.WriteMessage("cannot find the style: " + key + "\n");
                        return id/* TODO Change to default(_) if this is not a reference type */;
                    }
                    tr.Commit();
                }
                // Get the id of property set definition from the name
                id = dict.GetAt(key);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("error in findPropertySetDef: " + ex.ToString() + "\n");
            }
            return id;
        }
    }

}