using MySql.Data.MySqlClient;

namespace ChessBrowser
{
    /// <summary>
    /// Creates a query based on the GUI's search results
    /// </summary>
    public class GameSearch
    {
        /// <summary>
        /// Uses the search criteria provided to query the database for the matching games.
        /// </summary>
        /// <param name="conn">The MySqlConnection generated from ChessBrowser.razor.cs</param>
        /// <param name="white">The white player, or "" if none</param>
        /// <param name="black">The black player, or "" if none</param>
        /// <param name="opening">The first move, e.g. "1.e4", or "" if none</param>
        /// <param name="winner">The winner as "W", "B", "D", or "" if none</param>
        /// <param name="useDate">true if the filter includes a date range, false otherwise</param>
        /// <param name="start">The start of the date range</param>
        /// <param name="end">The end of the date range</param>
        /// <param name="showMoves">true if the returned data should include the PGN moves</param>
        /// <returns>The number of results and the string of results</returns>
        public static (int length, string result) Search(MySqlConnection conn, string white, string black, string opening,
            string winner, bool useDate, DateTime start, DateTime end, bool showMoves)
        {
            MySqlCommand SearchCmd = conn.CreateCommand();
            string Final = "";
            int Length = 0;

            //Giant SQL, Uses AND(OR) to only use search criteria that has text in it (or is on)
            SearchCmd.CommandText = @"
                SELECT e.Name AS Event, e.Site AS Site, e.Date AS Date,
                w.Name AS White, w.Elo AS WhiteElo, b.Name AS Black, b.Elo AS BlackElo,
                g.Result AS Result, g.Moves AS Moves
                FROM Games g JOIN Events e ON g.eID = e.eID
                JOIN Players b ON g.BlackPlayer = b.pID
                JOIN Players w ON g.WhitePlayer = w.pID
                WHERE (@White = '' OR w.Name = @White)
                  AND (@Black = '' OR b.Name = @Black)
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
                    //List because the long line of if statements looked bad
                    string[] keys = ["Event", "Site", "Date", "White", "Black", "Result"];
                    Final += "\n";
                    Length++;
                    foreach (string key in keys)
                    {
                        if (key == "White")
                        {
                            Final += "White: " + reader[key].ToString() + " (" + reader["WhiteElo"].ToString() + ")\n";
                        } 

                        else if (key == "Black")
                        {
                            Final += "Black: " + reader[key].ToString() + " (" + reader["BlackElo"].ToString() + ")\n";
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
