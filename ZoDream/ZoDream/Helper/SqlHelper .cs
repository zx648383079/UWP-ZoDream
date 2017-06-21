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

namespace ZoDream.Helper
{
    public class SqlHelper
    {

        const string DB_PATH = "Filename=zodream.db";

        public static void CreateDatabase()
        {
            SqliteEngine.UseWinSqlite3();
            using (var conn = new SqliteConnection(DB_PATH))
            {
                conn.Open();
                var tableCommand = "CREATE TABLE IF NOT EXISTS `Book` (`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `Name`	VARCHAR(200) NOT NULL, `Index`	INTEGER DEFAULT 0, `Count`	INTEGER DEFAULT 0, `Thumb`	VARCHAR(200), `ReadTime` DATETIME, `UpdateTime`	DATETIME, `LasetChapter` INT, `Author` VARCHAR(40), `Description` VARCHAR(200), `Kind` VARCHAR(20), `IsLocal` BOOL);" +
                    "CREATE TABLE IF NOT EXISTS `BookChapter` (`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `BookId` INT NOT NULL, `Name`VARCHAR(200) NOT NULL, `Content` TEXT, `Position` INT(6) DEFAULT 99, `Url` VARCHAR(200));";
                SqliteCommand createTable = new SqliteCommand(tableCommand, conn);
                try
                {
                    createTable.ExecuteReader();
                }
                catch (SqliteException e)
                {
                    //Do nothing
                }

            }
        }

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
        // Insert the new contact in the Contacts table. 
        public void Insert()
        {
            SqliteEngine.UseWinSqlite3();
            using (var conn = new SqliteConnection(DB_PATH))
            {
                conn.Open();
                String tableCommand = "CREATE TABLE IF NOT EXISTS `Book` (`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `Name`	VARCHAR(200) NOT NULL, `Index`	INTEGER DEFAULT 0, `Count`	INTEGER DEFAULT 0, `Thumb`	VARCHAR(200), `ReadTime` DATETIME, `UpdateTime`	DATETIME )";
                SqliteCommand createTable = new SqliteCommand(tableCommand, conn);
                try
                {
                    createTable.ExecuteReader();
                }
                catch (SqliteException e)
                {
                    //Do nothing
                }

            }
        }
   
        //Delete specific contact   
        public void DeleteContact(int Id)
        {
            SqliteEngine.UseWinSqlite3();
            using (var conn = new SqliteConnection(DB_PATH))
            {
                conn.Open();

            }
        }
    }
}
