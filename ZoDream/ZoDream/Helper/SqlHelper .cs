using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Model;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Internal;
using System.Data;
using System.Collections;

namespace ZoDream.Helper
{
    public class SqlHelper
    {

        const string DB_PATH = "Filename=zodream.db";

        private static SqliteConnection _conn = null;
        /// <summary>  
        /// 连接对象  
        /// </summary>  
        public static SqliteConnection Conn
        {
            get
            {
                if (_conn == null)
                {
                    _conn = new SqliteConnection(DB_PATH);
                    //SqliteEngine.UseWinSqlite3();
                }
                return _conn;
            }
            set { _conn = value; }
        }

        public static void CreateDatabase()
        {
            Conn.Open();
            CreateTable<Book>(new string[]{
                "`Name`	VARCHAR(200) NOT NULL",
                "`Index`	INTEGER DEFAULT 0",
                "`Count`	INTEGER DEFAULT 0",
                "`Thumb`	VARCHAR(200)",
                "`ReadTime` DATETIME",
                "`UpdateTime`	DATETIME",
                "`LastChapter` INT",
                "`Author` VARCHAR(40)",
                "`Description` VARCHAR(200)",
                "`Kind` VARCHAR(20)",
                "`IsLocal` BOOL",
                "`Url` VARCHAR(200)"
            });
            CreateTable<BookChapter>(new string[] {
                "`BookId` INT NOT NULL",
                "`Name`VARCHAR(200) NOT NULL",
                "`Content` TEXT",
                "`Position` INT(6) DEFAULT 99",
                "`Url` VARCHAR(200)"
            });
            CreateTable<FavoriteUrl>(new string[] {
                "`Name`VARCHAR(200) NOT NULL",
                "`Url`VARCHAR(200) NOT NULL",
            });
            CreateTable<HistoryUrl>(new string[] {
                "`Name`VARCHAR(200) NOT NULL",
                "`Url`VARCHAR(200) NOT NULL",
                "`CreateTime` DATETIME",
            });
            CreateTable<BookRule>(new string[] {
                "`Host`VARCHAR(200) NOT NULL",
                "`NameStart`VARCHAR(200)",
                "`NameEnd`VARCHAR(200)",
                "`AuthorStart`VARCHAR(200)",
                "`AuthorEnd`VARCHAR(200)",
                "`CoverStart`VARCHAR(200)",
                "`CoverEnd`VARCHAR(200)",
                "`DescriptionStart`VARCHAR(200)",
                "`DescriptionEnd`VARCHAR(200)",
                "`ListStart`VARCHAR(200)",
                "`ListEnd`VARCHAR(200)",
                "`TitleStart`VARCHAR(200)",
                "`TitleEnd`VARCHAR(200)",
                "`ContentStart`VARCHAR(200)",
                "`ContentEnd`VARCHAR(200)",
            });
            Conn.Close();
        }

        #region CreateCommand(commandText,SQLiteParameter[])  
        /// <summary>  
        /// 创建命令  
        /// </summary>  
        /// <param name="connection">连接</param>  
        /// <param name="commandText">语句</param>  
        /// <param name="commandParameters">语句参数.</param>  
        /// <returns>SQLite Command</returns>  
        public static SqliteCommand CreateCommand(string commandText, params SqliteParameter[] commandParameters)
        {
            var cmd = new SqliteCommand(commandText, Conn);
            if (commandParameters.Length > 0)
            {
                foreach (var parm in commandParameters)
                {
                    cmd.Parameters.Add(parm);
                }
            }
            return cmd;
        }
        #endregion


        #region CreateParameter(parameterName,parameterType,parameterValue)  
        /// <summary>  
        /// 创建参数  
        /// </summary>  
        /// <param name="parameterName">参数名</param>  
        /// <param name="parameterType">参数类型</param>  
        /// <param name="parameterValue">参数值</param>  
        /// <returns>返回创建的参数</returns>  
        public static SqliteParameter CreateParameter(string parameterName, DbType parameterType, object parameterValue)
        {
            var parameter = new SqliteParameter()
            {
                DbType = parameterType,
                ParameterName = parameterName,
                Value = parameterValue
            };
            return parameter;
        }
        #endregion

        public static int ExecteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqliteParameter[] commandParameters)
        {
            SqliteEngine.UseWinSqlite3();
            var cmd = new SqliteCommand();
            using (var conn = new SqliteConnection(connectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecteNonQuery(CommandType cmdType, string cmdText, params SqliteParameter[] commandParameters)
        {
            return ExecteNonQuery(DB_PATH, cmdType, cmdText, commandParameters);
        }


        public static int ExecteNonQueryText(string cmdText, params SqliteParameter[] commandParameters)
        {
            return ExecteNonQuery(CommandType.Text, cmdText, commandParameters);
        }


        private static void PrepareCommand(SqliteCommand cmd, SqliteConnection conn, SqliteTransaction trans, CommandType cmdType, string cmdText, SqliteParameter[] cmdParms)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
                
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //判断是否需要事物处理
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
                
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (var parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
                    
            }
        }

        public static SqliteDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqliteParameter[] commandParameters)
        {
            var cmd = new SqliteCommand();
            var conn = new SqliteConnection(connectionString);
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqliteDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static SqliteDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqliteParameter[] commandParameters)
        {
            return ExecuteReader(DB_PATH, cmdType, cmdText, commandParameters);
        }

        public static SqliteDataReader ExecuteReader(string cmdText, params SqliteParameter[] commandParameters)
        {
            return ExecuteReader(DB_PATH, CommandType.Text, cmdText, commandParameters);
        }

        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int CreateTable<T>(string sql)
        {
            var table = typeof(T).Name;
            return CreateCommand($"CREATE TABLE IF NOT EXISTS `{table}` (`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, {sql});" ).ExecuteNonQuery();
        }

        public static int CreateTable<T>(IList<string> args)
        {
            return CreateTable<T>(string.Join(", ", args));
        }

        /// <summary>
        /// 插入语句
        /// </summary>
        /// <typeparam name="T">表名</typeparam>
        /// <param name="columns">插入的列</param>
        /// <param name="tags">标签或值</param>
        /// <param name="parameters">标签对应的值</param>
        /// <returns>最后插入的值</returns>
        public static int Insert<T>(string columns, string tags, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"INSERT INTO {table} ({columns}) VALUES ({tags});", parameters).ExecuteNonQuery();
        }

        /// <summary>
        /// 返回插入的自增id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int InsertId<T>(string columns, string tags, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return Convert.ToInt32(CreateCommand($"INSERT INTO {table} ({columns}) VALUES ({tags});select last_insert_rowid();", parameters).ExecuteScalar());
        }

        public static int Insert<T>(string tags, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"INSERT INTO {table} VALUES ({tags});", parameters).ExecuteNonQuery();
        }

        /// <summary>
        /// 插入时，某条记录不存在则插入，存在则更新 id 会改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int Replace<T>(string columns, string tags, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"REPLACE INTO {table} ({columns}) VALUES ({tags});", parameters).ExecuteNonQuery();
        }
        /// <summary>
        /// 插入 如果存在忽略
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int InsertOrIgnore<T>(string columns, string tags, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"INSERT OR IGNORE INTO {table} ({columns}) VALUES ({tags});", parameters).ExecuteNonQuery();
        }
        /// <summary>
        /// 返回自增id 0 表示失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int InsertOrIgnoreId<T>(string columns, string tags, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return Convert.ToInt32(CreateCommand($"INSERT OR IGNORE INTO {table} ({columns}) VALUES ({tags});select last_insert_rowid();", parameters).ExecuteScalar());
        }

        public static int Update<T>(string sql, string where, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"UPDATE {table} SET {sql} WHERE {where};", parameters).ExecuteNonQuery();
        }

        public static int Update<T>(IList<string> args, string where, params SqliteParameter[] parameters)
        {
            return Update<T>(string.Join(", ", args), where, parameters);
        }

        public static int Update<T>(IList<string> args, int id, params SqliteParameter[] parameters)
        {
            return Update<T>(args, $"Id = {id}", parameters);
        }

        public static int Delete<T>(string where)
        {
            var table = typeof(T).Name;
            return CreateCommand($"DELETE FROM {table} WHERE {where};").ExecuteNonQuery();
        }

        public static int Delete<T>(int id)
        {
            return Delete<T>($"Id = {id}");
        }

        public static SqliteDataReader Select<T>(string field, string sql, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"SELECT {field} FROM {table} {sql};", parameters).ExecuteReader();
        }

        public static SqliteDataReader Select<T>(string sql, params SqliteParameter[] parameters)
        {
            return Select<T>("*", sql, parameters);
        }

        public static SqliteDataReader Select<T>(int id)
        {
            return Select<T>($"WHERE Id = {id} LIMIT 1");
        }

        public static SqliteDataReader Select<T>()
        {
            return Select<T>("");
        }

        public static T First<T>(string field, string where, params SqliteParameter[] parameters)
        {
            using (var reader = Select<T>(field, $"WHERE {where}  LIMIT 1", parameters))
            {
                reader.Read();
                if (reader.HasRows)
                {
                    return (T)Activator.CreateInstance(typeof(T), reader);
                }
            }
            return default(T);
        }

        public static T First<T>( string where, params SqliteParameter[] parameters)
        {
            return First<T>("*", where, parameters);
        }

        /// <summary>
        /// 获取第一行第一列的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="feild"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object Find<T>(string feild, string where, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"SELECT {feild} FROM {table} WHERE {where} LIMIT 1;", parameters).ExecuteScalar();
        }

        public static object Find<T>(string feild, params SqliteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return CreateCommand($"SELECT {feild} FROM {table} LIMIT 1;", parameters).ExecuteScalar();
        }

        public static object Find<T>(string feild, int id)
        {
            return Find<T>(feild, $"Id = {id}");
        }
    }
}
