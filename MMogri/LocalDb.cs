using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;

namespace MMogri.Utils
{
    class LocalDb<T> where T : new()
    {
        SQLiteWrapper wrapper;
        SQLiteConnection dbconn;

        public LocalDb(string path, SQLiteWrapper wrapper)
        {
            this.wrapper = wrapper;

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                SQLiteConnection.CreateFile(path);
            }

            dbconn = new SQLiteConnection("URI=file:" + path);
            dbconn.Open(); //Open connection to the database.

            if (!TableExists())
            {
                CreateDB(path);
            }
        }

        bool TableExists()
        {
            SQLiteCommand cmd = dbconn.CreateCommand();
            cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = @name";
            cmd.Parameters.Add(new SQLiteParameter("@name", wrapper.tableName));

            using (SQLiteDataReader sqlDataReader = cmd.ExecuteReader())
            {
                if (sqlDataReader.Read())
                    return true;
                else
                    return false;
            }
        }

        void CreateDB(string path)
        {
            //dbconn = (SQLiteConnection)new SQLiteConnection( "URI=file:" + path );
            //dbconn.Open();

            SQLiteCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = wrapper.CreateCmd();
            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;
        }

        void ClearTable()
        {
            SQLiteCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = wrapper.DeleteTableCmd();
            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;
        }

        public void AddItem(T t)
        {
            SQLiteCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = wrapper.InsertCmd(SQLiteWrapper.InsertType.IGNORE);
            //Debug.Log( dbcmd.CommandText );

            int n = 0;
            foreach (SQLiteWrapper.SqlItem itm in wrapper.IterateItems())
            {
                switch (itm.type)
                {
                    case SQLiteWrapper.SQLiteType.INTEGER:
                        dbcmd.Parameters.Add(new SQLiteParameter("@param" + n, (int)itm.GetValue(t))); break;
                    case SQLiteWrapper.SQLiteType.TEXT:
                        dbcmd.Parameters.Add(new SQLiteParameter("@param" + n, (string)itm.GetValue(t))); break;
                    case SQLiteWrapper.SQLiteType.REAL:
                        dbcmd.Parameters.Add(new SQLiteParameter("@param" + n, (float)itm.GetValue(t))); break;
                }
                n++;
            }

            dbcmd.ExecuteNonQuery();
            dbcmd.Dispose();
            dbcmd = null;
        }

        public List<T> GetItems(string cmd)
        {
            List<T> list = new List<T>();

            SQLiteCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = wrapper.SelectCmd(cmd);
            //Debug.Log( dbcmd.CommandText );

            SQLiteDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                int n = 0;
                T t = new T();

                foreach (SQLiteWrapper.SqlItem itm in wrapper.IterateItems())
                {
                    object o = null;
                    if (!reader.IsDBNull(n))
                    {
                        switch (itm.type)
                        {
                            case SQLiteWrapper.SQLiteType.INTEGER:
                                o = reader.GetInt32(n); break;
                            case SQLiteWrapper.SQLiteType.TEXT:
                                o = reader.GetString(n); break;
                            case SQLiteWrapper.SQLiteType.REAL:
                                o = reader.GetFloat(n); break;
                        }
                        itm.SetValue(t, o);
                    }
                    n++;
                }
                list.Add(t);
            }
            int affected = reader.FieldCount;

            reader.Close();
            reader = null;

            dbcmd.Dispose();
            dbcmd = null;

            return list;
        }

        public void RemoveItem(T t, string cmd)
        {
            SQLiteCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = wrapper.DeleteCmd(cmd);
            //Debug.Log( dbcmd.CommandText );

            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;
        }

        public void UpdateItem(T t, string cmd)
        {
            SQLiteCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = wrapper.UpdateCmd(cmd);
            //Debug.Log( dbcmd.CommandText );

            int n = 0;
            foreach (SQLiteWrapper.SqlItem itm in wrapper.IterateItems())
            {
                switch (itm.type)
                {
                    case SQLiteWrapper.SQLiteType.INTEGER:
                        dbcmd.Parameters.Add(new SQLiteParameter("@param" + n, (int)itm.GetValue(t))); break;
                    case SQLiteWrapper.SQLiteType.TEXT:
                        dbcmd.Parameters.Add(new SQLiteParameter("@param" + n, (string)itm.GetValue(t))); break;
                    case SQLiteWrapper.SQLiteType.REAL:
                        dbcmd.Parameters.Add(new SQLiteParameter("@param" + n, (float)itm.GetValue(t))); break;
                }
                n++;
            }

            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;
        }
    }
}
