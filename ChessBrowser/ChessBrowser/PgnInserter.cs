using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace ChessBrowser
{
    public class PgnInserter
    {
        private MySqlConnection conn;
        private MySqlCommand EventCmd, PlayerCmd, GameCmd, PIDCmd;
        public PgnInserter(MySqlConnection conn) {
            this.conn = conn;
            EventCmd = conn.CreateCommand();
            EventCmd.CommandText = @"INSERT IGNORE INTO Events (Name, Site, Date)
                                     VALUES (@Name, @Site, @Date);";

            PlayerCmd = conn.CreateCommand();
            PlayerCmd.CommandText = @"INSERT INTO Players (Name, Elo)
                                      VALUES (@Name, @Elo)
                                      ON DUPLICATE KEY UPDATE Elo = IF(@Elo > Elo, @Elo, Elo);";

            GameCmd = conn.CreateCommand();
            GameCmd.CommandText = @"INSERT IGNORE INTO Games (Round, Result, Moves, BlackPlayer, WhitePlayer)
                                    VALUES (@Round, @Result, @Moves, @BlackPlayer, @WhitePlayer);";

            PIDCmd = conn.CreateCommand();
            PIDCmd.CommandText = @"SELECT pID FROM Players WHERE Name = @Name";
        }
        public void Insert (ChessGame game) 
        {
            EventCmd.Parameters.Clear();
            EventCmd.Parameters.AddWithValue("@Name", game.EventName);
            EventCmd.Parameters.AddWithValue("@Site", game.Site);
            EventCmd.Parameters.AddWithValue("@Date", game.EventDate);
            EventCmd.ExecuteNonQuery();

            PlayerCmd.Parameters.Clear();
            PlayerCmd.Parameters.AddWithValue("@Name",game.WhitePlayer);
            PlayerCmd.Parameters.AddWithValue("@Elo",game.WhiteElo);
            PlayerCmd.ExecuteNonQuery();

            PlayerCmd.Parameters.Clear();
            PlayerCmd.Parameters.AddWithValue("@Name", game.BlackPlayer);
            PlayerCmd.Parameters.AddWithValue("@Elo", game.BlackElo);
            PlayerCmd.ExecuteNonQuery();

            GameCmd.Parameters.Clear();
            GameCmd.Parameters.AddWithValue("@Round",game.Round);
            GameCmd.Parameters.AddWithValue("@Result",game.Result);
            GameCmd.Parameters.AddWithValue("@Moves",game.Moves);
            PIDCmd.Parameters.Clear();
            PIDCmd.Parameters.AddWithValue("@Name", game.BlackPlayer);
            int.TryParse(PIDCmd.ExecuteScalar().ToString(), out int BlackPID);
            GameCmd.Parameters.AddWithValue("@BlackPlayer",BlackPID);
            PIDCmd.Parameters.Clear();
            PIDCmd.Parameters.AddWithValue("@Name", game.WhitePlayer);
            int.TryParse(PIDCmd.ExecuteScalar().ToString(), out int WhitePID);
            GameCmd.Parameters.AddWithValue("@WhitePlayer", WhitePID);
            GameCmd.ExecuteNonQuery();
        }
    }
}
