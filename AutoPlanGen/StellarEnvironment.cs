// Лохматый класс, который был сконвертирован из VB, написанного 7 лет назад
// требует рефакторинга возможно, не разбирался с классом 
// 


using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoPlan
{
    /// <summary>
    /// Окружение стеллажей
    /// </summary>
    public partial class StellarEnvironment
    {
        /// <summary>
        /// Путь к базе
        /// </summary>
        private string dbPath { get; set; }
        /// <summary>
        /// Путь к шейпам со стеллажами
        /// </summary>
        private string srcPath { get; set; }
        /// <summary>
        /// Конструктор класса базовый
        /// </summary>
        public StellarEnvironment()
        {
            // Пути по умолчанию, если нет ini файла
            dbPath = @"C:\Stellar\client_G.mdb";
            srcPath = @"C:\Stellar\shapes\";
            // Пробуем открыть файл для того чтобы получить пути
            try
            {
                string iniFileName = @"C:\stellar\stellar.ini";
                var fReader = new StreamReader(iniFileName, Encoding.GetEncoding(1251), true);
                string sourceString;
                sourceString = fReader.ReadToEnd();
                fReader.Close();
                string[] srcArr = sourceString.Split('\n', '\r');
                srcArr = srcArr.Where(n => n != "").ToArray();
                if (srcArr.GetUpperBound(0) == 1)
                {
                    dbPath = srcArr[0];
                    srcPath = srcArr[1];
                }
                else
                {
                    MessageBox.Show("Ошибка в файле stellar.ini Пути будут заданы по умолчанию");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не могу открыть stellar.ini Ошибка:" + ex.Message);
                return;
            }
        }
        /// <summary>
        /// Возвращает путь к базе
        /// </summary>
        public string getDBPath
        {
            get
            {
                return dbPath;
            }
        }
        /// <summary>
        /// Возвращает путь к каталогу с шэйпами
        /// </summary>
        public string getSourcePath
        {
            get
            {
                return srcPath;
            }
        }
        /// <summary>
        /// Возвращает строку соединения с БД
        /// </summary>
        public string getConnectionString
        {
            get
            {
                return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbPath + ";User ID=Admin;Password=;";
            }
        }
    }

    /// <summary>
    /// Таблица с параметрами
    /// </summary>
    public partial class ParTable
    {
        private System.Collections.Specialized.StringCollection ParList = new System.Collections.Specialized.StringCollection();
        private long CurPosition;
        private long RecCount;
        public bool EOf;
        public long ID;
        public string Name;
        public int DefaultValue;
        public int MaxValue;
        public int MinValue;
        private long[] IDPar;
        private string[] NamePar;
        private int[] DefValuePar;
        private int[] MaxValuePar;
        private int[] MinValuePar;
        public long RecordCount
        {
            get
            {
                return RecCount;
            }
        }

        public System.Collections.Specialized.StringCollection ParameterList
        {
            get
            {
                return ParList;
            }
        }
        public ParTable()
        {
            CurPosition = 0;
            RecCount = 0;
            EOf = true;
            ID = 0;
            Name = "";
            DefaultValue = 0;
            MaxValue = 0;
            MinValue = 0;
        }
        public int DefaultVal(long i)
        {
            int DefaultValRet = default(int);
            DefaultValRet = DefValuePar[Convert.ToInt32(i)];
            return DefaultValRet;
        }
        public void Clear()
        {
            ParList.Clear();
            CurPosition = 0;
            RecCount = 0;
            EOf = true;
            IDPar = new long[1];
            NamePar = new string[1];
            DefValuePar = new int[1];
            MaxValuePar = new int[1];
            MinValuePar = new int[1];
        }
        public void AddRow(long IDParam, string ParameterName, int DefaultValue, int MinValue, int MaxValue)
        {
            // Increase recordcount    
            RecCount = RecCount + 1;
            var oldIDPar = IDPar;
            IDPar = new long[(RecCount - 1) + 1];
            if (oldIDPar != null)
                Array.Copy(oldIDPar, IDPar, Math.Min((RecCount - 1) + 1, oldIDPar.Length));
            var oldNamePar = NamePar;
            NamePar = new string[(RecCount - 1) + 1];
            if (oldNamePar != null)
                Array.Copy(oldNamePar, NamePar, Math.Min((RecCount - 1) + 1, oldNamePar.Length));
            var oldDefValuePar = DefValuePar;
            DefValuePar = new int[(RecCount - 1) + 1];
            if (oldDefValuePar != null)
                Array.Copy(oldDefValuePar, DefValuePar, Math.Min((RecCount - 1) + 1, oldDefValuePar.Length));
            var oldMaxValuePar = MaxValuePar;
            MaxValuePar = new int[(RecCount - 1) + 1];
            if (oldMaxValuePar != null)
                Array.Copy(oldMaxValuePar, MaxValuePar, Math.Min((RecCount - 1) + 1, oldMaxValuePar.Length));
            var oldMinValuePar = MinValuePar;
            MinValuePar = new int[(RecCount - 1) + 1];
            // redim array
            if (oldMinValuePar != null)
                Array.Copy(oldMinValuePar, MinValuePar, Math.Min((RecCount - 1) + 1, oldMinValuePar.Length));
            // Fill the array     
            IDPar[(RecCount - 1)] = IDParam;
            NamePar[(RecCount - 1)] = ParameterName;
            DefValuePar[(RecCount - 1)] = DefaultValue;
            MaxValuePar[(RecCount - 1)] = MaxValue;
            MinValuePar[(RecCount - 1)] = MinValue;
            // Add ParameterList
            ParList.Add(ParameterName);
        }
        // Comment upper
        public void LoadTable(System.Data.DataTable DT) // Comment on 
        {
            // Comment under
            if (RecCount > 0)
                Clear();
            RecCount = DT.Rows.Count;
            IDPar = new long[(RecordCount - 1) + 1];
            NamePar = new string[(RecordCount - 1) + 1];
            DefValuePar = new int[(RecordCount - 1) + 1];
            MaxValuePar = new int[(RecordCount - 1) + 1];
            MinValuePar = new int[(RecordCount - 1) + 1];
            int i;
            var loopTo = (RecCount - 1);
            for (i = 0; i <= loopTo; i++)
            {
                IDPar[i] = Convert.ToInt64(DT.Rows[i][0]);
                NamePar[i] = DT.Rows[i][1].ToString();
                DefValuePar[i] = Convert.ToInt32(DT.Rows[i][2]);
                MinValuePar[i] = Convert.ToInt32(DT.Rows[i][3]);
                MaxValuePar[i] = Convert.ToInt32(DT.Rows[i][4]);
                ParList.Add(DT.Rows[i][1].ToString());
            }
        }
        public void MoveFirst()
        {
            CurPosition = 0;
            checkEof();
        }
        public void MoveLast()
        {
            if (RecCount > 0)
                CurPosition = RecCount - 1;
            checkEof();
        }
        public void MoveNext()
        {
            if (CurPosition < RecCount)
            {
                CurPosition = CurPosition + 1;
                checkEof();
            }
            else
                MessageBox.Show("Ошибка!!! Количеств записей закончилось!", "Ошибка");
        }
        private void checkEof()
        {
            if (CurPosition >= RecCount)
                EOf = true;
            else
                EOf = false;
            ID = IDPar[CurPosition];
            Name = NamePar[CurPosition];
            DefaultValue = DefValuePar[CurPosition];
            MaxValue = MaxValuePar[CurPosition];
            MinValue = MinValuePar[CurPosition];
        }
        public bool CheckMaxLimit(int Value, long Position)
        {
            bool CheckMaxLimitRet = default(bool);
            if (Position >= RecCount || Position < 0)
            {
                MessageBox.Show("Позиция находится за пределами существующих значений!!!", "Ошибка");
                CheckMaxLimitRet = true;
                return CheckMaxLimitRet;
            }
            if (Value > MaxValuePar[Position])
                CheckMaxLimitRet = true;
            else
                CheckMaxLimitRet = false;
            return CheckMaxLimitRet;
        }
        public bool CheckMinLimit(int Value, long Position)
        {
            bool CheckMinLimitRet = default(bool);
            if (Position >= RecCount || Position < 0)
            {
                MessageBox.Show("Позиция находится за пределами существующих значений!!!", "Ошибка");
                CheckMinLimitRet = true;
                return CheckMinLimitRet;
            }
            if (Value < MinValuePar[Position])
                CheckMinLimitRet = true;
            else
                CheckMinLimitRet = false;
            return CheckMinLimitRet;
        }
        public int CheckParValue(int Value, long Position)
        {
            int CheckParValueRet = default(int);
            if (Position >= RecCount || Position < 0)
            {
                MessageBox.Show("Позиция находится за пределами существующих значений!!!", "Ошибка");
                CheckParValueRet = Value;
                return CheckParValueRet;
            }
            if (Value < MinValuePar[Position])
            {
                //if (MsgBox["Введенное вами число меньше допустимого значения! " + Constants.vbCrLf + "Значение должно находиться в пределах от " + Conversions.ToString(MinValuePar[Conversions.ToInteger(Position)]) + " до " + Conversions.ToString(MaxValuePar[Conversions.ToInteger(Position)]) + "." + Constants.vbCrLf + "Вы уверенны, что хотите продолжить?", Constants.vbYesNo, "Предупреждение"] == Constants.vbYes)
                if (MessageBox.Show("Введенное вами число меньше допустимого значения! \n" +
                    "Значение должно находиться в пределах от " + MinValuePar[Position].ToString() +
                    " до " + MaxValuePar[Position].ToString() + " .\n" + "Вы уверенны, что хотите продолжить?", "Вопрос", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    CheckParValueRet = Value;
                else
                    CheckParValueRet = MinValuePar[Position];
                return CheckParValueRet;
            }
            if (Value > MaxValuePar[Position])
            {
                if (MessageBox.Show("Введенное вами число больше допустимого значения! \n" +
                    "Значение должно находиться в пределах от " + MinValuePar[Position].ToString() +
                    " до " + MaxValuePar[Position].ToString() + " .\n" + "Вы уверенны, что хотите продолжить?", "Вопрос", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    CheckParValueRet = Value;
                else
                    CheckParValueRet = MaxValuePar[Position];
                return CheckParValueRet;
            }
            CheckParValueRet = Value;
            return CheckParValueRet;
        }
        public System.Data.DataTable GetGrid(BlockAttributes attr = null)
        {
            var NewGrid = new System.Data.DataTable();
            NewGrid.Columns.Add("Наименование");
            NewGrid.Columns.Add("Значение");
            if (attr == null)
            {
                for (int i = 0; i < (RecCount); i++)
                    NewGrid.Rows.Add(new string[] { NamePar[i], DefValuePar[i].ToString() });
                return NewGrid;
            }
            for (int i = 0; i < RecordCount; i++)
                NewGrid.Rows.Add(new string[] { NamePar[i], attr.getValueByName(NamePar[i]) });
            return NewGrid;
        }
        public string getNameFromID(long ID)
        {
            string retValue = "Error";
            if (RecCount == 0)
            {
                // MsgBox("Ошибка! В объекте ParTable нет записей!!!")
                retValue = "Ошибка! В объекте ParTable нет записей!!!";
                return retValue;
            }
            int i;
            bool flag;
            flag = false;
            //var loopTo = Conversions.ToInteger(RecCount - 1);
            for (i = 0; i < RecCount; i++)
            {
                if (IDPar[i] == ID)
                {
                    retValue = NamePar[i];
                    flag = true;
                }
            }
            if (flag == false)
            {
                // MsgBox("Ошибка! В объекте ParTable отсутствует параметр с таким именем!!!")
                retValue = "Ошибка! В объекте ParTable отсутствует параметр с таким именем!!!";
                return retValue;
            }
            return retValue;
        }
        public int getIDParfromName(string Name)
        {
            int retValue;
            retValue = 0;
            if (RecCount == 0)
            {
                MessageBox.Show("Ошибка! В объекте ParTable нет записей!!!");
                //MsgBox["Ошибка! В объекте ParTable нет записей!!!"];
                return retValue;
            }
            int i;
            bool flag;
            flag = false;
            //var loopTo = Conversions.ToInteger(RecCount - 1);
            for (i = 0; i < RecCount; i++)
            {
                if ((NamePar[i] ?? "") == (Name ?? ""))
                {
                    retValue = Convert.ToInt32(IDPar[i]);
                    flag = true;
                }
            }
            if (flag == false)
            {
                MessageBox.Show("Ошибка! В объекте ParTable отсутствует параметр с таким именем!!!");
                //MsgBox["Ошибка! В объекте ParTable отсутствует параметр с таким именем!!!"];
                return retValue;
            }
            return retValue;
        }
    }

    public partial class BlockAttributes
    {
        public System.Collections.Specialized.StringCollection ParNames = new System.Collections.Specialized.StringCollection();
        public System.Collections.Specialized.StringCollection ParValues = new System.Collections.Specialized.StringCollection();

        public string Name { get; set; }
        public void Clear()
        {
            ParNames.Clear();
            ParValues.Clear();
        }
        public void Add(string ParameterName, string ParameterValue)
        {
            ParNames.Add(ParameterName);
            ParValues.Add(ParameterValue);
        }
        public long Count
        {
            get
            {
                return ParNames.Count;
            }
        }
        public string getValueByName(string Nme)
        {
            if (ParNames.Count == 0)
            {
                MessageBox.Show("Ошибка!!! Класс BlockAttributes пустой!!!");
                //MsgBox["Ошибка!!! Класс BlockAttributes пустой!!!"];
                return "Error. Class is empty";
            }
            string Val = "Error. Can't find the Value";
            for (int i = 0; i <= ParNames.Count; i++)
            {
                if (this.ParNames[i] == Nme)
                    Val = this.ParValues[i];
            }
            if ((Val ?? "") == "Error. Can't find the Value")
                MessageBox.Show("Ошибка!!! В классе BlockAttributes отсутствует поле с таким именем!!!");
            //MsgBox["Ошибка!!! В классе BlockAttributes отсутствует поле с таким именем!!!"];
            return Val;
        }
    }

    public partial class StellarAttributesList
    {
        private BlockAttributes[] StAttrList;
        private long[] Amnt;

        public StellarAttributesList()
        {
            StAttrList = new BlockAttributes[1];
            Amnt = new long[1];
            count = 0;
        }
        public void Clear()
        {
            StAttrList = new BlockAttributes[1];
            Amnt = new long[1];
            count = 0;
        }
        public long count { get; private set; }
        public void Add(BlockAttributes Stellar, long Amount)
        {
            count = count + 1;
            var oldStAttrList = StAttrList;
            StAttrList = new BlockAttributes[count + 1];
            if (oldStAttrList != null)
                Array.Copy(oldStAttrList, StAttrList, Math.Min(count + 1, oldStAttrList.Length));
            var oldAmnt = Amnt;
            Amnt = new long[count + 1];
            if (oldAmnt != null)
                Array.Copy(oldAmnt, Amnt, Math.Min(count + 1, oldAmnt.Length));
            StAttrList[(count - 1)] = Stellar;
            Amnt[(count - 1)] = Amount;
        }
        public BlockAttributes GetStellar(int i)
        {
            if (i > count - 1)
            {
                MessageBox.Show("Ошибка!!! Нет объекта с таким номером!!!");
                //MsgBox["Ошибка!!! Нет объекта с таким номером!!!"];
                return null;
            }
            return StAttrList[i];
        }
        public long getAmount(int i)
        {
            if (i > count - 1)
            {
                MessageBox.Show("Ошибка!!! Нет объекта с таким номером!!!");
                //MsgBox["Ошибка!!! Нет объекта с таким номером!!!"];
                return default(long);
            }
            return Amnt[i];
        }
        public void editAmount(int i, long NewValue)
        {
            if (i > count - 1)
            {
                MessageBox.Show("Ошибка!!! Нет объекта с таким номером!!!");
                //MsgBox["Ошибка!!! Нет объекта с таким номером!!!"];
                return;
            }
            Amnt[i] = NewValue;
        }
        public void incAmount(int i)
        {
            if (i > count - 1)
            {
                MessageBox.Show("Ошибка!!! Нет объекта с таким номером!!!");
                //MsgBox["Ошибка!!! Нет объекта с таким номером!!!"];
                return;
            }
            Amnt[i] = Amnt[i] + 1;
        }
        public System.Data.DataTable getTable()
        {
            var DT = new System.Data.DataTable();
            DT.Columns.Add("Имя");
            DT.Columns.Add("Количество");
            DT.Columns.Add("Параметр");
            DT.Columns.Add("Значение");
            if (count == 0)
                return DT;
            for (long i = 0; i < count; i++)
            {
                //for (int j = 0, loopTo1 = Conversions.ToInteger(this.GetStellar(Conversions.ToInteger(i)).Count - 1); j <= loopTo1; j++)
                for (int j = 0; j <= GetStellar(Convert.ToInt32(i)).Count; j++)
                    DT.Rows.Add(new string[] { GetStellar(Convert.ToInt32(i)).Name, getAmount(Convert.ToInt32(i)).ToString(), GetStellar(Convert.ToInt32(i)).ParNames[j], GetStellar(Convert.ToInt32(i)).ParValues[j] });
            }
            return DT;
        }
    }
}