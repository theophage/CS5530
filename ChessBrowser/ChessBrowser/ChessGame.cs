namespace ChessBrowser
{
    public class ChessGame
    {
        /// <summary>
        /// The name of the event the game was played at
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// The location of the event
        /// </summary>
        public string Site { get; set; }
        /// <summary>
        /// The round the game was played in
        /// </summary>
        public string Round { get; set; }
        /// <summary>
        /// The name of the white player (Last, First)
        /// </summary>
        public string WhitePlayer { get; set; }
        /// <summary>
        /// The name of the black player (Last, First)
        /// </summary>
        public string BlackPlayer { get; set; }
        /// <summary>
        /// The result of the game (W, B, or D)
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// The ELO of the white player at the time
        /// </summary>
        public uint WhiteElo { get; set; }
        /// <summary>
        /// The ELO of the black player at the time
        /// </summary>
        public uint BlackElo { get; set; }
        /// <summary>
        /// The official date of the event
        /// </summary>
        public DateTime EventDate { get; set; }
        /// <summary>
        /// The moves that were played in the game
        /// </summary>
        public string Moves { get; set; }

        /// <summary>
        /// A representation of a chess game
        /// </summary>
        /// <param name="eventName">The name of the event the game was played at</param>
        /// <param name="site">The location of the event</param>
        /// <param name="round">The round the game was played in</param>
        /// <param name="whitePlayer">The name of the white player (Last, First)</param>
        /// <param name="blackPlayer">The name of the black player (Last, First)</param>
        /// <param name="result">The result of the game (W, B, or D)</param>
        /// <param name="whiteElo">The ELO of the white player at the time</param>
        /// <param name="blackElo">The ELO of the black player at the time</param>
        /// <param name="eventDate">The official date of the event</param>
        /// <param name="moves">The moves that were played in the game</param>
        public ChessGame(
            string eventName, 
            string site, 
            string round, 
            string whitePlayer, 
            string blackPlayer, 
            string result, 
            uint whiteElo, 
            uint blackElo, 
            DateTime eventDate,
            string moves)
        {
            EventName = eventName;
            Site = site;
            Round = round;
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
            Result = result;
            WhiteElo = whiteElo;
            BlackElo = blackElo;
            EventDate = eventDate;
            Moves = moves;
        }
    }
}
