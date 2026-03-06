using MySql.Data.MySqlClient;

namespace ChessBrowser
{
    public class GameSearch
    {
        public static (int length, string result) Search(MySqlConnection conn, string white, string black, string opening,
            string winner, bool useDate, DateTime start, DateTime end, bool showMoves)
        {
            MySqlCommand SearchCmd = conn.CreateCommand();
            string Final = "";
            int Length = 0;

            SearchCmd.CommandText = @"
                SELECT e.Name AS Event, e.Site AS Site, e.Date AS Date,
                pw.Name AS White, pw.Elo AS WhiteElo, pb.Name AS Black, pb.Elo AS BlackElo,
                g.Result AS Result, g.Moves AS Moves
                FROM Games g JOIN Events e ON g.eID = e.eID
                JOIN Players pb ON g.BlackPlayer = pb.pID
                JOIN Players pw ON g.WhitePlayer = pw.pID
                WHERE (@White = '' OR pw.Name LIKE CONCAT('%', @White, '%'))
                  AND (@Black = '' OR pb.Name LIKE CONCAT('%', @Black, '%'))
                  AND (@Opening = '' OR g.Moves LIKE CONCAT(@Opening, '%'))
                  AND (@Winner = '' OR g.Result = @Winner)
                  AND (@UseDate = 0 OR e.Date BETWEEN @Start AND @End)
            ";

            SearchCmd.Parameters.AddWithValue("@White",white);
            SearchCmd.Parameters.AddWithValue("@Black",black);
            SearchCmd.Parameters.AddWithValue("@Opening",opening);
            SearchCmd.Parameters.AddWithValue("@Winner",winner);
            SearchCmd.Parameters.AddWithValue("@UseDate",useDate);
            SearchCmd.Parameters.AddWithValue("@Start",start);
            SearchCmd.Parameters.AddWithValue("@End",end);

            using (MySqlDataReader reader = SearchCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string[] keys = ["Event", "Site", "Date", "White", "Black", "Result"];
                    Final += "\n";
                    Length++;
                    foreach (string key in keys)
                    {
                        if (key == "White")
                        {
                            Final += "White: " + reader[key].ToString() + "(" + reader["WhiteElo"].ToString() + ")\n";
                        } 

                        else if (key == "Black")
                        {
                            Final += "Black: " + reader[key].ToString() + "(" + reader["BlackElo"].ToString() + ")\n";
                        } 
                        
                        else
                        {
                            Final += key + ": " + reader[key].ToString() + "\n";
                        }
                    }
                    if(showMoves)
                    {
                        Final += reader["Moves"].ToString() + "\n";
                    }
                }
            }

            return (Length, Final);
        }
    }
}
