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
                    switch (line)
                    {
                        case string s when s.StartsWith("[Event "):
                            eventName = ParseTag("Event", s);
                            break;

                        case string s when s.StartsWith("[Site "):
                            site = ParseTag("Site", s);
                            break;

                        case string s when s.StartsWith("[Round "):
                            round = ParseTag("Round", s);
                            break;

                        case string s when s.StartsWith("[White "):
                            whitePlayer = ParseTag("White", s);
                            break;

                        case string s when s.StartsWith("[Black "):
                            blackPlayer = ParseTag("Black", s);
                            break;

                        case string s when s.StartsWith("[Result "):
                            result = ParseResult(s.Split('"')[1]);
                            break;

                        case string s when s.StartsWith("[WhiteElo "):
                            whiteElo = UInt32.Parse(s.Split('"')[1]);
                            break;

                        case string s when s.StartsWith("[BlackElo "):
                            blackElo = UInt32.Parse(s.Split('"')[1]);
                            break;

                        case string s when s.StartsWith("[EventDate "):
                            eventDate = ParseDate(s.Split('"')[1]);
                            break;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line)) // if it's not a tag or empty, it's a list of moves
                {
                    lastLineWasMoves = true;
                    // add to the moves string
                    if (moves == "")
                    {
                        moves = Sanitize(line.Trim());
                    }
                    else // add a space before the next line of moves
                    {
                        moves += " " + Sanitize(line.Trim());
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
    string[] parts = input.Split('.');
    if (!int.TryParse(parts[0], out int year) ||
        !int.TryParse(parts[1], out int month) ||
        !int.TryParse(parts[2], out int day))
    {
        return new DateTime(1000,01,01);//Updated, initially tried 0000's as per instructions, but that didn't work
                                        //After google search found minimum could be 1000-01-01 which proceeded to work.
    }
    return new DateTime(year, month, day);
}

        /// <summary>
        /// Sanitizes a string for MySQL. Escapes " and \
        /// </summary>
        /// <param name="input">The string to sanitize</param>
        /// <returns>The sanitized string</returns>
        private static string Sanitize(string input)
        {
            string sanitized = "";
            foreach (char c in input)
            {
                if (c == '"' || c == '\\')
                {
                    sanitized += @"\";
                }
                sanitized += c;
            }
            return sanitized;
        }

        /// <summary>
        /// Parses a sanitized tag from a tag line from the PGN file
        /// </summary>
        /// <param name="tagName">The tag name of the line being parsed</param>
        /// <param name="line">The full line containing the tag</param>
        /// <returns>The sanitized tag</returns>
        private static string ParseTag(string tagName, string line)
        {
            string tagLine = line.Trim();
            // get substring from a line like [tagName "tag"]
            string tag = tagLine.Substring(tagName.Length + 3, tagLine.Length - tagName.Length - 5);
            return Sanitize(tag);
        }

        /// <summary>
        /// Gets the result of the game as a W, B, or D from the PGN format (1-0, 0-1, or 1/2-1/2)
        /// </summary>
        /// <param name="result">The result tag section from the result line</param>
        /// <returns>The result: "W", "B", or "D"</returns>
        private static string ParseResult(string result)
        {
            if (result == "1-0")
            {
                return "W";
            }
            else if (result == "0-1")
            {
                return "B";
            }
            else
            {
                return "D";
            }
        }
    }
}
