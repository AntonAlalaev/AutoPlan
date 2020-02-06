// 
// Лохматый класс, конвертирован из VB, котрый был написан почти 7 лет назад
// С ним особо не разбирался, возможно требует рефакторинга
// 


using System;
using System.Collections.Specialized;
using AutoPlan;
using System.Data.OleDb;
using System.Windows;
using System.Data;

namespace AutoPlanGen
{
    public class StellarDataLink
    {
        private string ConnectionString;
        public StellarDataLink()
        {
            StellarEnvironment Environment = new StellarEnvironment();
            ConnectionString = Environment.getConnectionString;
        }
        public StringCollection getStellarList(string Parameters = "")
        {
            StringCollection stlist = new StringCollection();
            return stlist;
        }

        /// <summary>
        /// Заполняет параметры по имени стеллажа из базы
        /// </summary>
        /// <param name="StellarName"></param>
        /// <param name="StellarParTable"></param>
        public void FillParameters(string StellarName, ref ParTable StellarParTable)
        {
            // Очищаем таблицу
            // Создаем соединение
            if (StellarParTable.RecordCount > 0)
                StellarParTable.Clear();
            try
            {
                OleDbConnection dbs = new OleDbConnection(ConnectionString);
                dbs.Open();
                // MsgBox(Err.Number)
                OleDbCommand sqlCom = new OleDbCommand();
                System.Data.DataTable DT = new System.Data.DataTable();
                OleDbDataAdapter DA = new OleDbDataAdapter();
                sqlCom = new OleDbCommand("SELECT Параметры.IDПараметр, ПараметрСписок.Параметр, " + "Параметры.Штатно, ПараметрСписок.Минимально, ПараметрСписок.Максимально " + "FROM ПараметрСписок INNER JOIN (IDСтеллаж INNER JOIN Параметры ON " + "IDСтеллаж.ID = Параметры.IDСтеллаж) ON ПараметрСписок.IDПараметр = " + "Параметры.IDПараметр " + "WHERE IDСтеллаж.Имя='" + StellarName + "' " + "ORDER BY ПараметрСписок.Параметр", dbs);
                sqlCom.ExecuteNonQuery();
                DA = new OleDbDataAdapter(sqlCom);
                // Очищаем таблицу
                DT.Clear();
                // Заполняем таблицу
                DA.Fill(DT);
                // Закрываем соединение
                dbs.Close();
                StellarParTable.LoadTable(DT);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \n Проверьте правильность пути к файлу базы данных! \n" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Преобразует атрибуты в табличное представление
        /// </summary>
        /// <param name="Attributes"></param>
        /// <returns></returns>
        public System.Data.DataTable AttributesToTableView(BlockAttributes Attributes)
        {
            System.Data.DataTable ResultTable = new System.Data.DataTable();
            long aCount = Attributes.Count;
            if (aCount == 0)
            {
                MessageBox.Show("Error!!! There are empty attributes in AttributesToTableView!!!");
                return ResultTable;
            }
            string BlockName;
            BlockName = Attributes.Name;
            // теперь надо законнектиться к базе и вытащить оттуда имена аттрибутов
            // можно это сделать существующей процедурой
            ParTable Param = new ParTable();
            FillParameters(BlockName, ref Param);
            string resParName;

            ResultTable.Columns.Add("Имя");
            ResultTable.Columns.Add("Значение");

            for (int i = 0; i <= aCount - 1; i++)
            {
                resParName = Param.getNameFromID(Convert.ToInt64(Attributes.ParNames[i]));
                ResultTable.Rows.Add(new string[] { resParName, Attributes.ParValues[i] });
            }
            return ResultTable;
        }

        /// <summary>
        /// Возвращает перечень заказчиков в виде таблицы
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomersTable()
        {
            DataTable Res = new DataTable();
            try
            {
                OleDbConnection dbs = new OleDbConnection(ConnectionString);
                dbs.Open();
                // MsgBox(Err.Number)
                OleDbCommand sqlCom = new OleDbCommand();
                DataTable DT = new System.Data.DataTable();
                OleDbDataAdapter DA = new OleDbDataAdapter();
                sqlCom = new OleDbCommand("SELECT Customer.CustomerID, Customer.CustomerName FROM Customer ORDER BY Customer.CustomerName", dbs);
                sqlCom.ExecuteNonQuery();
                DA = new OleDbDataAdapter(sqlCom);
                DA.Fill(Res);
                // Закрываем соединение
                dbs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \n Проверьте правильность пути к файлу базы данных!\n" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // MsgBox(Res.Rows.Count)
            return Res;
        }

        /// <summary>
        /// Возвращает перечень заказчиков
        /// </summary>
        /// <returns></returns>
        public string[] getCustomerList()
        {
            string[] Res = null;
            DataTable CustomerDataTable = GetCustomersTable();
            if (CustomerDataTable.Rows.Count == 0)
                // Пустой список заказчиков
                return Res;
            Res = new string[CustomerDataTable.Rows.Count - 1 + 1];
            for (int i = 0; i <= CustomerDataTable.Rows.Count - 1; i++)
                Res[i] = CustomerDataTable.Rows[i].ItemArray[1].ToString();
            //Res[i] = CustomerDataTable.Rows[i].Item[1].ToString;
            return Res;
        }

        /// <summary>
        /// Возвращает список заказчиков из таблицы
        /// </summary>
        /// <param name="CustomerDataTable"></param>
        /// <returns></returns>
        public string[] GetCustomerListFromTable(DataTable CustomerDataTable)
        {
            string[] Res = null;
            if (CustomerDataTable.Rows.Count == 0)
                // Пустой список заказчиков
                return Res;
            Res = new string[CustomerDataTable.Rows.Count - 1 + 1];
            for (int i = 0; i <= CustomerDataTable.Rows.Count - 1; i++)
                Res[i] = CustomerDataTable.Rows[i].ItemArray[1].ToString();
            //Res[i] = CustomerDataTable.Rows(i).Item(1).ToString;
            return Res;
        }
        /// <summary>
        /// Получает имя заказчика по ID
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public string getCustomerNameByID(string CustomerID)
        {
            DataTable CustomerDataTable = GetCustomersTable();
            string Res = null;
            if (CustomerDataTable.Rows.Count == 0)
                // Пустой список заказчиков
                return Res;
            for (int i = 0; i <= CustomerDataTable.Rows.Count - 1; i++)
            {
                if (CustomerDataTable.Rows[i].ItemArray[0].ToString() == CustomerID)
                    Res = CustomerDataTable.Rows[i].ItemArray[1].ToString();
            }
            return Res;
        }
        /// <summary>
        /// Получает ID заказчика по имени
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public string getCustomerIDByName(string CustomerName)
        {
            DataTable CustomerDataTable = GetCustomersTable();
            string Res = null;
            if (CustomerDataTable.Rows.Count == 0)
                // Пустой список заказчиков
                return Res;
            for (int i = 0; i <= CustomerDataTable.Rows.Count - 1; i++)
            {
                if (CustomerDataTable.Rows[i].ItemArray[1].ToString() == CustomerName)
                    Res = CustomerDataTable.Rows[i].ItemArray[0].ToString();
            }
            return Res;
        }
        /// <summary>
        /// ПОлучает список проектов
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns></returns>
        public string[] GetProjectList(string Customer)
        {
            string[] res = null;
            DataTable dt = getProjectTable(Customer);
            if (dt.Rows.Count == 0)
                return res;
            res = new string[dt.Rows.Count - 1 + 1];
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
                res[i] = dt.Rows[i].ItemArray[1].ToString();
            return res;
        }
        /// <summary>
        /// ПОлучает ID стеллажа по имени
        /// </summary>
        /// <param name="stellarName">Имя стеллажа</param>
        /// <returns></returns>
        public string getStellarID(string stellarName)
        {
            OleDbConnection dbs = new OleDbConnection(ConnectionString);
            try
            {
                dbs.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \n Проверьте правильность пути к файлу базы данных!\n Ошибка " + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            OleDbCommand sqlCom = new OleDbCommand();
            System.Data.DataTable DT = new System.Data.DataTable();
            OleDbDataAdapter DA = new OleDbDataAdapter();
            sqlCom = new OleDbCommand("SELECT ID FROM IDСтеллаж WHERE Имя='" + stellarName + "'", dbs);
            sqlCom.ExecuteNonQuery();
            DA = new OleDbDataAdapter(sqlCom);
            DA.Fill(DT);
            // Закрываем соединение
            dbs.Close();
            if (DT.Rows.Count == 0)
                return null;
            return DT.Rows[0].ItemArray[0].ToString();
            //DT.Rows[0].Item[0];       
        }

        /// <summary>
        /// Получает ID проекта по нимаенованию заказчика и наименованию проекта
        /// </summary>
        /// <param name="CustomerName">Наименование закзачика</param>
        /// <param name="ProjectName">Наименованеие проекта</param>
        /// <returns></returns>
        public string getProjectId(string CustomerName, string ProjectName)
        {
            OleDbConnection dbs = new OleDbConnection(ConnectionString);
            try
            {
                dbs.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \nПроверьте правильность пути к файлу базы данных!\nОшибка" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            //OleDbCommand sqlCom = new OleDbCommand();
            System.Data.DataTable DT = new System.Data.DataTable();
            //OleDbDataAdapter DA = new OleDbDataAdapter();
            OleDbCommand sqlCom = new OleDbCommand("SELECT DISTINCT  Project.ProjectID FROM Customer INNER JOIN Project ON " + "Customer.CustomerID = Project.CustomerID WHERE Customer.CustomerName='" + CustomerName + "' AND Project.Name='" + ProjectName + "'", dbs);
            sqlCom.ExecuteNonQuery();
            OleDbDataAdapter DA = new OleDbDataAdapter(sqlCom);
            DA.Fill(DT);
            // Закрываем соединение
            dbs.Close();
            if (DT.Rows.Count == 0)
                return null;
            return DT.Rows[0].ItemArray[0].ToString();
        }

        /// <summary>
        /// Создает новый проект
        /// </summary>
        /// <param name="CustomerName">Нименование заказчика</param>
        /// <param name="ProjectName">Наименование создаваемого проекта</param>
        /// <returns></returns>
        public string CreateNewProject(string CustomerName, string ProjectName)
        {
            OleDbConnection dbs = new OleDbConnection(ConnectionString);
            try
            {
                dbs.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \nПроверьте правильность пути к файлу базы данных!\nОшибка" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            OleDbCommand sqlCom = new OleDbCommand();
            System.Data.DataTable DT = new System.Data.DataTable();
            OleDbDataAdapter DA = new OleDbDataAdapter();
            // Проверяем еть ли такой проект
            sqlCom = new OleDbCommand("SELECT DISTINCT  Project.ProjectID FROM Customer INNER JOIN Project ON " + "Customer.CustomerID = Project.CustomerID WHERE Customer.CustomerName='" + CustomerName + "' AND Project.Name='" + ProjectName + "'", dbs);
            sqlCom.ExecuteNonQuery();
            DA = new OleDbDataAdapter(sqlCom);
            DA.Fill(DT);
            // Закрываем соединение
            if (DT.Rows.Count > 0)
            {
                dbs.Close();
                return null;
            }
            // MsgBox("CustomerName is " & CustomerName)
            // Определяем есть ли такой заказчик

            string CustomerID = getCustomerIDByName(CustomerName);
            if (CustomerID == null)
            {
                MessageBox.Show("Не могу найти такого заказчика!!!");
                return null;
            }
            // MsgBox("CustomerID is:" & CustomerID)

            sqlCom = new OleDbCommand("INSERT INTO Project (Name, CustomerID) VALUES ('" + ProjectName + "'," + System.Convert.ToString(CustomerID) + ")", dbs);
            sqlCom.ExecuteNonQuery();
            // Теперь получаем ProjectID
            sqlCom = new OleDbCommand("SELECT DISTINCT  Project.ProjectID FROM Customer INNER JOIN Project ON " + "Customer.CustomerID = Project.CustomerID WHERE Customer.CustomerName='" + CustomerName + "' AND Project.Name='" + ProjectName + "'", dbs);
            sqlCom.ExecuteNonQuery();
            DA = new OleDbDataAdapter(sqlCom);
            DT.Clear();
            DA.Fill(DT);
            string ProjectID = DT.Rows[0].ItemArray[0].ToString();
            dbs.Close();
            return ProjectID;
        }

        /// <summary>
        /// ПОлучает таблицу с проектами по заказчику
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns></returns>
        public DataTable getProjectTable(string Customer)
        {
            DataTable Res = new DataTable();
            OleDbConnection dbs = new OleDbConnection(ConnectionString);
            try
            {
                dbs.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \nПроверьте правильность пути к файлу базы данных!\nОшибка" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
            OleDbCommand sqlCom = new OleDbCommand();
            System.Data.DataTable DT = new System.Data.DataTable();
            OleDbDataAdapter DA = new OleDbDataAdapter();
            sqlCom = new OleDbCommand("SELECT Project.ProjectID, Project.Name FROM Project WHERE " + "(Project.CustomerID=(SELECT Customer.CustomerID FROM " + "Customer WHERE CustomerName='" + Customer + "')) AND " + "((Project.type Is Null) OR Project.type =0) " + "ORDER BY Name", dbs);
            sqlCom.ExecuteNonQuery();
            DA = new OleDbDataAdapter(sqlCom);
            DA.Fill(Res);
            // Закрываем соединение
            dbs.Close();
            return Res;
        }

        /// <summary>
        /// Экспорт в БД
        /// </summary>
        /// <param name="stList"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public bool exportToDatabase(StellarAttributesList stList, string ProjectID)
        {
            OleDbConnection dbs = new OleDbConnection(ConnectionString);
            try
            {
                dbs.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \nПроверьте правильность пути к файлу базы данных!\nОшибка" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            OleDbCommand sqlCom = new OleDbCommand();
            System.Data.DataTable DT = new System.Data.DataTable();
            OleDbDataAdapter DA = new OleDbDataAdapter();
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.EditorInput.Editor ed = doc.Editor;
            ed.WriteMessage("\n Пока все нормально, stList.count " + stList.count + " \n");

            for (int i = 0; i <= stList.count - 1; i++)
            {
                string StID = getStellarID(stList.GetStellar(i).Name);
                ed.WriteMessage("\n StID " + StID + " \n");
                DT.Clear();
                // Пишем стеллажи            
                // Таблица Комплектация проекта
                // Поля ПроектID, СтеллажID, Количество
                ed.WriteMessage("\n ПроектID " + ProjectID + " СтеллажID " + StID + " Количество " + stList.getAmount(i) + "\n");
                sqlCom = new OleDbCommand("INSERT INTO КомплектацияПроекта (ПроектID, СтеллажID, Количество) VALUES (" + ProjectID + "," + StID + "," + stList.getAmount(i) + ")", dbs);
                sqlCom.ExecuteNonQuery();
                // После записи ищем номер этой комплектации, он должен быть последним
                sqlCom = new OleDbCommand("SELECT Max(ID) AS Quantity FROM КомплектацияПроекта", dbs);
                sqlCom.ExecuteNonQuery();
                DA = new OleDbDataAdapter(sqlCom);
                DA.Fill(DT);
                // Новый код
                // Сохраняем номер комплектации
                string complID = DT.Rows[0].ItemArray[0].ToString();
                ed.WriteMessage("\n ComplID " + complID + " \n");
                // Пишем параметры

                StellarDataLink tmpTable = new StellarDataLink();
                ParTable Partbl = new ParTable();
                tmpTable.FillParameters(stList.GetStellar(i).Name, ref Partbl);
                ed.WriteMessage("\n Partbl.RecordCount " + Partbl.RecordCount + "\n");
                if (Partbl.RecordCount == 0)
                    continue;

                Partbl.MoveFirst();
                for (var j = 0; j <= Partbl.RecordCount - 1; j++)
                {
                    string ParID = Partbl.ID.ToString();
                    string ParVal = Partbl.DefaultValue.ToString();
                    ed.WriteMessage("\nj= " + j + " complID " + complID + " ParID " + ParID + " ParVal " + ParVal + "\n");
                    sqlCom = new OleDbCommand("INSERT INTO КомплектацияПараметры (КомплектацияID, ПараметрID, КоличествоID) VALUES (" + complID + "," + ParID + "," + ParVal + ")", dbs);
                    sqlCom.ExecuteNonQuery();
                    if (j < Partbl.RecordCount - 1)
                        Partbl.MoveNext();
                }
            }
            // Закрываем соединение
            dbs.Close();
            return true;
        }
        /// <summary>
        /// Возвращает таблицу с существующими в проекте стеллажами
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public System.Data.DataTable getExistPrjStList(string ProjectID)
        {
            System.Data.DataTable resTable = new DataTable();
            OleDbConnection dbs = new OleDbConnection(ConnectionString);
            try
            {
                dbs.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу подключиться к базе! \nПроверьте правильность пути к файлу базы данных!\nОшибка" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return resTable;
            }
            OleDbCommand sqlCom = new OleDbCommand();
            System.Data.DataTable DT = new System.Data.DataTable();
            OleDbDataAdapter DA = new OleDbDataAdapter();
            sqlCom = new OleDbCommand("SELECT IDСтеллаж.Имя, КомплектацияПроекта.Количество " + "FROM КомплектацияПроекта INNER JOIN " + "IDСтеллаж ON КомплектацияПроекта.СтеллажID = IDСтеллаж.ID " + "WHERE КомплектацияПроекта.ПроектID = " + ProjectID + " " + "ORDER BY IDСтеллаж.Имя", dbs);
            sqlCom.ExecuteNonQuery();
            DA = new OleDbDataAdapter(sqlCom);
            DA.Fill(resTable);
            dbs.Close();
            return resTable;
        }
    }
}