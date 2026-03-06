using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace ChessBrowser
{
    public class PgnInserter
    {
        /// <summary>
        /// The MySqlConnection generated from ChessBrowser.razor.cs
        /// </summary>
        private MySqlConnection conn;
        /// <summary>
        /// The different SQLCommands created to insert into the database.
        /// Used multiple instead of one big one as that was easier to modify when editing.
        /// </summary>
        private MySqlCommand EventCmd, PlayerCmd, GameCmd, PIDCmd, EIDCmd;
        /// <summary>
        /// Creates the commands used to insert pgn files into the database.
        /// Uses prepared statements to prevent injection attacks.
        /// </summary>
        /// <param name="conn"> The MySqlConnection generated from ChessBrowser.razor.cs </param>
        public PgnInserter(MySqlConnection conn) {
            this.conn = conn;
            EventCmd = conn.CreateCommand();
            EventCmd.CommandText = @"INSERT IGNORE INTO Events (Name, Site, Date)
                                     VALUES (@Name, @Site, @Date);";

            PlayerCmd = conn.CreateCommand();
            PlayerCmd.CommandText = @"INSERT INTO Players (Name, Elo)
                                      VALUES (@Name, @Elo)
                                      ON DUPLICATE KEY UPDATE Elo = IF(@Elo > Elo, @Elo, Elo);"; //Makes Elo higher of the two values

            GameCmd = conn.CreateCommand();
            GameCmd.CommandText = @"INSERT INTO Games (Round, Result, Moves, BlackPlayer, WhitePlayer, eID)
                                    VALUES (@Round, @Result, @Moves, @BlackPlayer, @WhitePlayer, @EID);";

            PIDCmd = conn.CreateCommand();
            PIDCmd.CommandText = @"SELECT pID FROM Players WHERE Name = @Name;";

            EIDCmd = conn.CreateCommand();
            EIDCmd.CommandText = @"SELECT eID FROM Events WHERE Name=@Name AND Site=@Site AND Date=@Date;";
        }
        public void Insert (ChessGame game) 
        {
            //For Event
            EventCmd.Parameters.Clear();
            EventCmd.Parameters.AddWithValue("@Name", game.EventName);
            EventCmd.Parameters.AddWithValue("@Site", game.Site);
            EventCmd.Parameters.AddWithValue("@Date", game.EventDate);
            EventCmd.ExecuteNonQuery();

            //For White Player
            PlayerCmd.Parameters.Clear();
            PlayerCmd.Parameters.AddWithValue("@Name",game.WhitePlayer);
            PlayerCmd.Parameters.AddWithValue("@Elo",game.WhiteElo);
            PlayerCmd.ExecuteNonQuery();

            //For Black Player
            PlayerCmd.Parameters.Clear();
            PlayerCmd.Parameters.AddWithValue("@Name", game.BlackPlayer);
            PlayerCmd.Parameters.AddWithValue("@Elo", game.BlackElo);
            PlayerCmd.ExecuteNonQuery();

            //For the Game
            GameCmd.Parameters.Clear();
            GameCmd.Parameters.AddWithValue("@Round",game.Round);
            GameCmd.Parameters.AddWithValue("@Result",game.Result);
            GameCmd.Parameters.AddWithValue("@Moves",game.Moves);
            PIDCmd.Parameters.Clear(); //Used to get pID from database, runs after creating players to insure pID exists
            PIDCmd.Parameters.AddWithValue("@Name", game.BlackPlayer);
            int.TryParse(PIDCmd.ExecuteScalar().ToString(), out int BlackPID);
            GameCmd.Parameters.AddWithValue("@BlackPlayer",BlackPID);
            PIDCmd.Parameters.Clear();
            PIDCmd.Parameters.AddWithValue("@Name", game.WhitePlayer);
            int.TryParse(PIDCmd.ExecuteScalar().ToString(), out int WhitePID);
            GameCmd.Parameters.AddWithValue("@WhitePlayer", WhitePID);

            EIDCmd.Parameters.Clear();
            EIDCmd.Parameters.AddWithValue("@Name",game.EventName);
            EIDCmd.Parameters.AddWithValue("@Site", game.Site);
            EIDCmd.Parameters.AddWithValue("@Date", game.EventDate);
            int.TryParse(EIDCmd.ExecuteScalar().ToString(), out int EID);
            GameCmd.Parameters.AddWithValue("@EID", EID);
            GameCmd.ExecuteNonQuery();
        }
    }
}
