using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private static readonly Random random = new Random();

        private readonly SqlConnection conn =
            new SqlConnection(
                "Server=tcp:musica-db-server.database.windows.net,1433;Initial Catalog=musica;Persist Security Info=False;User ID=musicaAdmin;Password=$u@bD*zP7b5an*-z;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        public Message create_room(string room_name, string location)
        {
            
            var joinCode = RandomString();
            var createNewRoomQuery1 =
                string.Format(
                    "CREATE TABLE \"{0}\" ( round_number INT  IDENTITY (1, 1) NOT NULL, song_choice1 VARCHAR (32)  NOT NULL, song_choice2    VARCHAR (32)  NOT NULL,song_choice3    VARCHAR (32)  NOT NULL, song_choice4    VARCHAR (32)  NOT NULL, image1    VARCHAR (MAX)  NOT NULL,  image2    VARCHAR (MAX)  NOT NULL,  image3    VARCHAR (MAX)  NOT NULL, image4    VARCHAR (MAX)  NOT NULL, artist1    VARCHAR (32)  NOT NULL,artist2    VARCHAR (32)  NOT NULL,artist3    VARCHAR (32)  NOT NULL, artist4    VARCHAR (32)  NOT NULL, votes1    INT   NOT NULL,votes2    INT  NOT NULL, votes3    INT  NOT NULL,votes4    INT  NOT NULL,status    INT NOT NULL, PRIMARY KEY CLUSTERED (round_number ASC));",
                    joinCode);
            var createNewRoomQuery2 =
                string.Format(
                    "INSERT INTO RoomsMaster (room_name, join_code, location) VALUES('{0}', '{1}', '{2}') ",
                    room_name, joinCode, location);
            var createRoomCommand1 = new SqlCommand(createNewRoomQuery1, conn);
            var createRoomCommand2 = new SqlCommand(createNewRoomQuery2, conn);
            conn.Open();
            createRoomCommand1.ExecuteNonQuery();
            createRoomCommand2.ExecuteNonQuery();


            // Fetch strand_id after creation:
            var getIdQuery = string.Format("SELECT room_id, join_code FROM RoomsMaster WHERE join_code='{0}'", joinCode);
            var getId = new SqlCommand(getIdQuery, conn);
            // Use the above SqlCommand object to create a SqlDataReader object.
            var dataTable = new DataTable();
            using (var queryCommandReader = getId.ExecuteReader())
            {
                var haveresult = true;
                while (haveresult)
                {
                    var tempTable = new DataTable();
                    tempTable.Load(queryCommandReader);
                    dataTable.Merge(tempTable);
                    try
                    {
                        queryCommandReader.Read();
                        tempTable = new DataTable();
                        queryCommandReader.NextResult();
                        tempTable.Load(queryCommandReader);
                        dataTable.Merge(tempTable);
                    }
                    catch
                    {
                        haveresult = false;
                    }
                }
            }

            var outMap = new Dictionary<int, Dictionary<string, string>>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                // dataTable.Rows[i][0] = i;
                var rowDict = new Dictionary<string, string>();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                    rowDict.Add(dataTable.Columns[j].ColumnName, dataTable.Rows[i][j].ToString());
                outMap.Add(i, rowDict);
            }
            //return outMap;
            return GetJsonStream(outMap);
        }

        public void destroy_room(string join_code, string room_id)
        {
            var dropQueryROOMSMASTER =
                string.Format(
                    "DELETE FROM RoomsMaster WHERE join_code = '{0}', room_id = '{1}'",
                    join_code, room_id);
            var dropCommandROOMSMASTER = new SqlCommand(dropQueryROOMSMASTER, conn);

            var dropQueryROOMTABLE =
                string.Format(
                    "DROP TABLE '{0}'",
                    join_code);
            var dropCommandROOMTABLE = new SqlCommand(dropQueryROOMTABLE, conn);
            conn.Open();
            dropCommandROOMSMASTER.ExecuteNonQuery();
            dropCommandROOMTABLE.ExecuteNonQuery();
        }

        public Message join_room(string join_code)
        {
            // Fetch strand_id after creation:
            var getIdQuery =
                string.Format(@"IF (SELECT room_size FROM RoomsMaster WHERE join_code = '{0}') < (SELECT max_room_size FROM RoomsMaster WHERE join_code = '{0}')
BEGIN
	UPDATE RoomsMaster SET room_size = room_size + 1 WHERE join_code = '{0}'
	SELECT '0';
END
ELSE
BEGIN
	SELECT '1';
END", join_code);

            var getId = new SqlCommand(getIdQuery, conn);
            conn.Open();

            // Use the above SqlCommand object to create a SqlDataReader object.
            var dataTable = new DataTable();
            using (var queryCommandReader = getId.ExecuteReader())
            {
                var haveresult = true;
                while (haveresult)
                {
                    var tempTable = new DataTable();
                    tempTable.Load(queryCommandReader);
                    dataTable.Merge(tempTable);
                    try
                    {
                        queryCommandReader.Read();
                        tempTable = new DataTable();
                        queryCommandReader.NextResult();
                        tempTable.Load(queryCommandReader);
                        dataTable.Merge(tempTable);
                    }
                    catch
                    {
                        haveresult = false;
                    }
                }
            }

            var outMap = new Dictionary<int, Dictionary<string, string>>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                // dataTable.Rows[i][0] = i;
                var rowDict = new Dictionary<string, string>();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                    rowDict.Add(dataTable.Columns[j].ColumnName, dataTable.Rows[i][j].ToString());
                outMap.Add(i, rowDict);
            }
            //return outMap;
            return GetJsonStream(outMap);
        }

        public Message get_location(string join_code)
        {
            // Fetch strand_id after creation:
            var getIdQuery =
                string.Format("SELECT location FROM RoomsMaster WHERE join_code = '{0}'", join_code);

            var getId = new SqlCommand(getIdQuery, conn);
            conn.Open();

            // Use the above SqlCommand object to create a SqlDataReader object.
            var dataTable = new DataTable();
            using (var queryCommandReader = getId.ExecuteReader())
            {
                var haveresult = true;
                while (haveresult)
                {
                    var tempTable = new DataTable();
                    tempTable.Load(queryCommandReader);
                    dataTable.Merge(tempTable);
                    try
                    {
                        queryCommandReader.Read();
                        tempTable = new DataTable();
                        queryCommandReader.NextResult();
                        tempTable.Load(queryCommandReader);
                        dataTable.Merge(tempTable);
                    }
                    catch
                    {
                        haveresult = false;
                    }
                }
            }

            var outMap = new Dictionary<int, Dictionary<string, string>>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                // dataTable.Rows[i][0] = i;
                var rowDict = new Dictionary<string, string>();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                    rowDict.Add(dataTable.Columns[j].ColumnName, dataTable.Rows[i][j].ToString());
                outMap.Add(i, rowDict);
            }
            //return outMap;
            return GetJsonStream(outMap);
        }

        public void close_current_round(string join_code, string room_id)
        {
            var query = string.Format("UPDATE \"{0}\" SET status = 0", join_code);


            var queryCommand = new SqlCommand(query, conn);
            conn.Open();
            queryCommand.ExecuteNonQuery();
        }

        public Message start_next_round(string join_code, string room_id, string song_choice1, string song_choice2,
            string song_choice3, string song_choice4, string image1, string image2, string image3, string image4,
            string artist1, string artist2, string artist3, string artist4)
        {
            var getIdQuery =
                string.Format(
                    "INSERT INTO \"{0}\" (song_choice1, song_choice2, song_choice3, song_choice4, image1, image2, image3, image4, artist1, artist2, artist3, artist4) VALUES('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
                    join_code, song_choice1, song_choice2, song_choice3, song_choice4, image1, image2, image3, image4,
                    artist1, artist2, artist3, artist4);
            var getId = new SqlCommand(getIdQuery, conn);
            conn.Open();
            // Use the above SqlCommand object to create a SqlDataReader object.
            var dataTable = new DataTable();
            using (var queryCommandReader = getId.ExecuteReader())
            {
                var haveresult = true;
                while (haveresult)
                {
                    var tempTable = new DataTable();
                    tempTable.Load(queryCommandReader);
                    dataTable.Merge(tempTable);
                    try
                    {
                        queryCommandReader.Read();
                        tempTable = new DataTable();
                        queryCommandReader.NextResult();
                        tempTable.Load(queryCommandReader);
                        dataTable.Merge(tempTable);
                    }
                    catch
                    {
                        haveresult = false;
                    }
                }
            }

            var outMap = new Dictionary<int, Dictionary<string, string>>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                // dataTable.Rows[i][0] = i;
                var rowDict = new Dictionary<string, string>();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                    rowDict.Add(dataTable.Columns[j].ColumnName, dataTable.Rows[i][j].ToString());
                outMap.Add(i, rowDict);
            }
            //return outMap;
            return GetJsonStream(outMap);
        }

        public Message vote(string join_code, string choice_number)
        {
            //SELECT MAX(column_name) FROM table_name;
            var getIdQuery =
                string.Format(
                    "IF (SELECT status FROM \"{0}\" WHERE status = 1) > 0 BEGIN UPDATE \"{0}\" SET votes{1} = votes{1} + 1 WHERE status = 1 SELECT '0';END ELSE BEGIN SELECT '1';END",
                    join_code, choice_number);

            var getId = new SqlCommand(getIdQuery, conn);
            conn.Open();

            // Use the above SqlCommand object to create a SqlDataReader object.
            var dataTable = new DataTable();
            using (var queryCommandReader = getId.ExecuteReader())
            {
                var haveresult = true;
                while (haveresult)
                {
                    var tempTable = new DataTable();
                    tempTable.Load(queryCommandReader);
                    dataTable.Merge(tempTable);
                    try
                    {
                        queryCommandReader.Read();
                        tempTable = new DataTable();
                        queryCommandReader.NextResult();
                        tempTable.Load(queryCommandReader);
                        dataTable.Merge(tempTable);
                    }
                    catch
                    {
                        haveresult = false;
                    }
                }
            }

            var outMap = new Dictionary<int, Dictionary<string, string>>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                // dataTable.Rows[i][0] = i;
                var rowDict = new Dictionary<string, string>();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                    rowDict.Add(dataTable.Columns[j].ColumnName, dataTable.Rows[i][j].ToString());
                outMap.Add(i, rowDict);
            }
            //return outMap;
            return GetJsonStream(outMap);
        }

        public Message get_vote_status(string join_code, string room_id)
        {

            var getVoteQuery =
                string.Format(
                    "SELECT votes1, votes2, votes3, votes4 FROM [{0}] WHERE round_number = (SELECT MAX(round_number) FROM  [{0}])", join_code);
     
            var getId = new SqlCommand(getVoteQuery, conn);
            conn.Open();

            // Use the above SqlCommand object to create a SqlDataReader object.
            var dataTable = new DataTable();
            using (var queryCommandReader = getId.ExecuteReader())
            {
                var haveresult = true;
                while (haveresult)
                {
                    var tempTable = new DataTable();
                    tempTable.Load(queryCommandReader);
                    dataTable.Merge(tempTable);
                    try
                    {
                        queryCommandReader.Read();
                        tempTable = new DataTable();
                        queryCommandReader.NextResult();
                        tempTable.Load(queryCommandReader);
                        dataTable.Merge(tempTable);
                    }
                    catch
                    {
                        haveresult = false;
                    }
                }
            }

            var outMap = new Dictionary<int, Dictionary<string, string>>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                // dataTable.Rows[i][0] = i;
                var rowDict = new Dictionary<string, string>();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                    rowDict.Add(dataTable.Columns[j].ColumnName, dataTable.Rows[i][j].ToString());
                outMap.Add(i, rowDict);
            }
            //return outMap;
            return GetJsonStream(outMap);
        }

        public Message get_choices(string join_code)
        {
            var getChoicesQuery =
               string.Format(
                   "IF (SELECT status FROM \"{0}\" WHERE round_number = (SELECT MAX(round_number) FROM  \"{0}\")) != 0 BEGIN SELECT * FROM \"{0}\" WHERE round_number = (SELECT MAX(round_number) FROM  \"{0}\") SELECT '0' END ELSE BEGIN SELECT '1' END", join_code);
            var getId = new SqlCommand(getChoicesQuery, conn);
            conn.Open();

            // Use the above SqlCommand object to create a SqlDataReader object.
            var dataTable = new DataTable();
            using (var queryCommandReader = getId.ExecuteReader())
            {
                var haveresult = true;
                while (haveresult)
                {
                    var tempTable = new DataTable();
                    tempTable.Load(queryCommandReader);
                    dataTable.Merge(tempTable);
                    try
                    {
                        queryCommandReader.Read();
                        tempTable = new DataTable();
                        queryCommandReader.NextResult();
                        tempTable.Load(queryCommandReader);
                        dataTable.Merge(tempTable);
                    }
                    catch
                    {
                        haveresult = false;
                    }
                }
            }

            var outMap = new Dictionary<int, Dictionary<string, string>>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                // dataTable.Rows[i][0] = i;
                var rowDict = new Dictionary<string, string>();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                    rowDict.Add(dataTable.Columns[j].ColumnName, dataTable.Rows[i][j].ToString());
                outMap.Add(i, rowDict);
            }
            //return outMap;
            return GetJsonStream(outMap);
        }

        #region don't touch

        public static string RandomString()
        {
            const string chars = "ABCDEF0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static Message GetJsonStream(object obj)
        {
            //Serialize JSON.NET
            var jsonSerialized = JsonConvert.SerializeObject(obj, Formatting.Indented);
            //Create memory stream
            var memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(jsonSerialized));
            //Set position to 0
            memoryStream.Position = 0;

            //return Message
            return WebOperationContext.Current.CreateStreamResponse(memoryStream, "application/json");
        }

        #endregion
    }
}