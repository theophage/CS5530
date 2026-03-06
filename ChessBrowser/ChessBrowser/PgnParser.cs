namespace ChessBrowser
{
    public class PgnParser
    {
        /// <summary>
        /// Parses an array of lines in PGN file format and returns a list of ChessGame objects
        /// </summary>
        /// <param name="PGNFileLines">A list of lines in PGN file format</param>
        /// <returns>The list of ChessGame objects parsed from the lines</returns>
        public static List<ChessGame> ParseLines(string[] PGNFileLines)
        {
            List<ChessGame> games = [];

            // storage variables
            string eventName = "";
            string site = "";
            string round = "";
            string whitePlayer = "";
            string blackPlayer = "";
            string result = "";
            uint whiteElo = 0;
            uint blackElo = 0;
            DateTime eventDate = new();
            string moves = "";

            // context variable
            bool lastLineWasMoves = false;

            foreach (string line in PGNFileLines)
            {
                if (line.StartsWith("["))
                {
                    lastLineWasMoves = false;
                    if (line.StartsWith("[Event "))
                    {
                        eventName = line.Split('"')[1]; // get only what's inside the quotation marks
                    }
                    else if (line.StartsWith("[Site "))
                    {
                        site = line.Split('"')[1];
                    }
                    else if (line.StartsWith("[Round "))
                    {
                        round = line.Split('"')[1];
                    }
                    else if (line.StartsWith("[White "))
                    {
                        whitePlayer = line.Split('"')[1];
                    }
                    else if (line.StartsWith("[Black "))
                    {
                        blackPlayer = line.Split('"')[1];
                    }
                    else if (line.StartsWith("[Result "))
                    {
                        result = line.Split('"')[1];
                    }
                    else if (line.StartsWith("[WhiteElo "))
                    {
                        whiteElo = UInt32.Parse(line.Split('"')[1]); // parse uint
                    }
                    else if (line.StartsWith("[BlackElo "))
                    {
                        blackElo = UInt32.Parse(line.Split('"')[1]); // parse uint
                    }
                    else if (line.StartsWith("[EventDate "))
                    {
                        eventDate = ParseDate(line.Split('"')[1]); // use helper date parser
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line)) // if it's not a tag or empty, it's a list of moves
                {
                    lastLineWasMoves = true;
                    // add to the moves string
                    if (moves == "")
                    {
                        moves = line;
                    }
                    else // add a space before the next line of moves
                    {
                        moves += " " + line;
                    }
                }
                else // empty line
                {
                    if (lastLineWasMoves) // empty line after moves means game is complete
                    {
                        // add the game to the list
                        games.Add(new ChessGame(
                            eventName,
                            site,
                            round,
                            whitePlayer,
                            blackPlayer,
                            result,
                            whiteElo,
                            blackElo,
                            eventDate,
                            moves));
                        // reset storage variables for next game
                        eventName = "";
                        site = "";
                        round = "";
                        whitePlayer = "";
                        blackPlayer = "";
                        result = "";
                        whiteElo = 0;
                        blackElo = 0;
                        eventDate = new();
                        moves = "";
                    }
                    lastLineWasMoves = false;
                    // does nothing if following tags or empty line
                }
            }
            // add the last game if the file doesn't end with an empty line
            if (lastLineWasMoves)
            {
                games.Add(new ChessGame(
                        eventName,
                        site,
                        round,
                        whitePlayer,
                        blackPlayer,
                        result,
                        whiteElo,
                        blackElo,
                        eventDate,
                        moves));
            }

            // done parsing
            return games;
        }

        /// <summary>
        /// Helper method to parse a date from the PGN file date format
        /// </summary>
        /// <param name="input">The input date in yyyy.mm.dd format</param>
        /// <returns>The output DateTime object matching the input</returns>
        private static DateTime ParseDate(string input)
        {
            string year = input.Split('.')[0];
            string month = input.Split('.')[1];
            string day = input.Split('.')[2];

            DateTime date = DateTime.Parse($"{month}/{day}/{year}");

            return date;
        }
    }
}
