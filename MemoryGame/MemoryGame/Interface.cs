using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryGame
{
    public class Interface
    {
        // the format of the game if against computer or against other player
        public enum eGameFormat
        {
            AgainstPlayer = 1,
            AgainstComputer = 2,
        }

        private Board<char> m_boardGame;
        private Player m_player1;
        private Player m_player2;
        private ComputerPlayer<char> m_computerPlayer;
        private eGameFormat m_FormatGame;

        public Interface()
        {
            m_boardGame = null;
            m_player1 = null;
            m_player2 = null;
            m_computerPlayer = null;
        }

        // start the memory game, initialize the players names, if play against the computer and the size of board
        public void StarMemorytGame()
        {
            const int k_WaitingSeconds = 2000;      // the delay time
            const int k_StatrGameScore = 0;

            Console.WriteLine("Welcome to Memory Game!");
            System.Threading.Thread.Sleep(k_WaitingSeconds);

            m_player1 = createPlayer(eFirstOrSecond.First);     // get first player information 

            m_FormatGame = getIndicateFormatOfGame();       // ask the user if to play against other player or against the computer

            // when choose to play against other player we create the second player
            if (m_FormatGame == eGameFormat.AgainstPlayer)
            {
                m_player2 = createPlayer(eFirstOrSecond.Second);
            }

            bool playAgain = true;

            // change to false when the user dont want to continue playing
            while (playAgain)
            {
                m_boardGame = createBoard();

                if (m_FormatGame == eGameFormat.AgainstPlayer)
                {
                    playAgainstOtherPlayer(); // choose to play against other player
                    m_player2.Score = k_StatrGameScore;     // make sure to start a new game with zero points
                }
                else
                {
                    playAgainstTheComputer();  // choose to play against the computer
                }
                m_player1.Score = k_StatrGameScore;     // make sure to start a new game with zero points


                string answer = getAnswerFromUserIfToPlayAgainOrNot();

                if (answer == "no" || answer == "q" || answer == "Q")
                {
                    playAgain = false;
                }
            }
        }

        // playing against other player 
        private void playAgainstOtherPlayer()
        {
            bool endGame = false;

            Ex02.ConsoleUtils.Screen.Clear();

            printBoard();
            while (!endGame)
            {
                if (m_player1.MyTurn)
                {
                    playerMove(ref m_player1);
                    if (m_boardGame.PairsRemainingToFind == 0)
                    {
                        endGame = true;
                    }
                }
                else
                {
                    playerMove(ref m_player2);
                    if (m_boardGame.PairsRemainingToFind == 0)
                    {
                        endGame = true;
                    }
                }
            }

            printWinningMessage();
        }

        // playing against the computer 
        private void playAgainstTheComputer()
        {
            bool endGame = false, hisTurn = true;
            m_computerPlayer = new ComputerPlayer<char>(m_boardGame);
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();

            m_player1.MyTurn = hisTurn;     // make sure that every new game against the computer the player start the game

            while (!endGame)
            {
                if (m_player1.MyTurn)
                {
                    playerMove(ref m_player1);
                    if (m_boardGame.PairsRemainingToFind == 0)
                    {
                        endGame = true;
                    }
                }
                else
                {
                    computerMove();
                    if (m_boardGame.PairsRemainingToFind == 0)
                    {
                        endGame = true;
                    }
                }
            }

            printWinningMessage();
        }

        // create new player
        private Player createPlayer(eFirstOrSecond i_WhichPlayer)
        {
            string playerName = null, whichPlayerStr;
            bool validInput = false, myTurn;
            if (i_WhichPlayer == eFirstOrSecond.First)
            {
                whichPlayerStr = "First";
                myTurn = true;
            }
            else
            {
                whichPlayerStr = "Second";
                myTurn = false;
            }

            Console.WriteLine("{0} player, please enter your name", whichPlayerStr);

            // in loop until we have valid input for the name user
            while (!validInput)
            {
                playerName = Console.ReadLine();
                validInput = Player.checkValidUserName(playerName);
                if (!validInput)
                {
                    Console.WriteLine("Error, name can contain only English letters" +
                                      "\nPlease try again");
                }
            }

            return new Player(playerName, i_WhichPlayer, myTurn);
        }

        // get the format of the game if its against other player or against the computer
        private eGameFormat getIndicateFormatOfGame()
        {
            const string k_FormatGameAgainstPlayer = "1";

            bool validInput = false;
            string indicateFormatOfGame = null;
            eGameFormat gameFormat;
            Console.WriteLine(
               "For playing against other player press '1'" +
                "\nFor playing against the computer press '2'");

            while (!validInput)
            {
                indicateFormatOfGame = Console.ReadLine();
                validInput = checkValidGameFormat(indicateFormatOfGame);
            }

            if (indicateFormatOfGame == k_FormatGameAgainstPlayer)
            {
                gameFormat = eGameFormat.AgainstPlayer;
            }
            else
            {
                gameFormat = eGameFormat.AgainstComputer;
            }

            return gameFormat;
        }

        private bool checkValidGameFormat(string i_IndicateFormatOfGame)
        {
            bool valid = true;
            if (i_IndicateFormatOfGame != "1" && i_IndicateFormatOfGame != "2")
            {
                Console.WriteLine(
                    "Error, please try again" +
                    "\nchoose '1' for playing against other player or press '2' for playing against the computer");
                valid = false;
            }

            return valid;
        }

        private Board<char> createBoard()
        {
            byte rows, columns;

            getRowsAndColumnsForBoardFromUser(out rows, out columns);

            List<char> cellSignList = initializeSignsOfCellSignList(rows, columns);

            return new Board<char>(rows, columns, cellSignList);
        }

        private void getRowsAndColumnsForBoardFromUser(out byte o_Rows, out byte o_Columns)
        {
            string strRows, strColumns;
            bool validInput = false, ligalSize = false, isEven = false;
            o_Rows = byte.MaxValue; // out of range for board
            o_Columns = byte.MaxValue; // out of range for board

            do
            {
                Console.WriteLine("Please enter the board size: (size between 4x4 and 6x6)");
                Console.WriteLine("how many rows should the board have (enter number and press 'Enter')");
                strRows = Console.ReadLine();

                if (strRows.Length != 1 || !isDigit(strRows[0]))    // make sure the user will type only one char
                {
                    System.Console.WriteLine("invalid input");
                    continue;
                }

                Console.WriteLine("how many columns should the board have (enter number and press 'Enter')");
                strColumns = Console.ReadLine();

                if (strColumns.Length != 1 || !isDigit(strColumns[0]))      // make sure the user will type only one char
                {
                    System.Console.WriteLine("invalid input");
                    continue;
                }

                // if wont able to parse the input is not a number
                bool ableParseRows = Byte.TryParse(strRows, out o_Rows);
                bool ableParseColumns = Byte.TryParse(strColumns, out o_Columns);

                validInput = ableParseRows && ableParseColumns;

                ligalSize = Board<char>.IsLegalSizeForBoard(o_Rows, o_Columns);

                isEven = Board<char>.IsEvenNumberOfCells(o_Rows, o_Columns);

                printMessageAboutValidBoard(validInput, ligalSize, isEven);
            }
            while (!(validInput && ligalSize && isEven));
        }

        // initial the signs in every cell in the board
        private List<char> initializeSignsOfCellSignList(byte i_Rows, byte i_Columns)
        {
            const char v_StartSign = 'A';
            List<char> cellSignList = new List<char>(i_Rows * i_Columns);

            for (int i = 0; i < i_Rows * i_Columns / 2; i++)
            {
                cellSignList.Add((char)(v_StartSign + i));
                cellSignList.Add((char)(v_StartSign + i));
            }

            return cellSignList;
        }

        private void printMessageAboutValidBoard(bool i_ValidInput, bool i_LigalSize, bool i_IsEven)
        {
            if (!i_ValidInput)
            {
                Console.WriteLine("Error, input must have only numbers");
            }
            else if (!i_LigalSize)
            {
                Console.WriteLine("Error, input must be between 4 to 6");
            }
            else if (!i_IsEven)
            {
                Console.WriteLine("Error, size of board (rows*columns) must be even");
            }
        }

        private string getAnswerFromUserIfToPlayAgainOrNot()
        {
            string answer;
            bool validInput;

            Console.WriteLine("Would you like to play another game? (yes/no)");

            do
            {
                answer = Console.ReadLine();
                validInput = checkValidAnswerForPlayAgainOrNot(answer);
            }
            while (!validInput);

            return answer;
        }

        private bool checkValidAnswerForPlayAgainOrNot(string i_Answer)
        {
            bool validInput = true;

            i_Answer = i_Answer.ToLower();

            if (i_Answer != "no" && i_Answer != "yes" && i_Answer != "q" && i_Answer != "Q")
            {
                validInput = false;
                Console.WriteLine("Error, please type if you want to play another game (yes/no)");
            }

            return validInput;
        }

        private void printBoard()
        {
            StringBuilder boardStringBuilder = new StringBuilder();

            boardStringBuilder = this.buildStringBuilderBoard();        // build the board in stringBuilder

            Console.Write(boardStringBuilder);
        }

        // make the computer move
        private void computerMove()
        {
            const int k_WaitingSeconds = 2000;      // the delay time
            const bool v_Expose = true;

            Console.WriteLine("Computer turn \ncomputer is choosing his first place to reveal");

            Tuple<Tuple<byte, byte>, Tuple<byte, byte>> computerMove = m_computerPlayer.PlayComputersTurn(); // the two moves
            Tuple<byte, byte> firstComputerMove = computerMove.Item1; // first move add for readability 
            Tuple<byte, byte> secondComputerMove = computerMove.Item2; // second move add for readability 
            char sign1 = m_boardGame.CellMatrix[firstComputerMove.Item1, firstComputerMove.Item2].CellSign;     // first sign
            char sign2 = m_boardGame.CellMatrix[secondComputerMove.Item1, secondComputerMove.Item2].CellSign;   // second sign

            // expose and print the first choice
            m_boardGame.CellMatrix[firstComputerMove.Item1, firstComputerMove.Item2].IsExposed = v_Expose;
            System.Threading.Thread.Sleep(k_WaitingSeconds);
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();

            Console.WriteLine("Computer turn \ncomputer is choosing his second place to reveal");

            // expose and print the second choice
            m_boardGame.CellMatrix[secondComputerMove.Item1, secondComputerMove.Item2].IsExposed = v_Expose;
            System.Threading.Thread.Sleep(k_WaitingSeconds);
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();

            System.Threading.Thread.Sleep(k_WaitingSeconds);

            if (sign1 != sign2)     // if the computer didnt found a pair
            {
                Console.WriteLine("computer had his turn");
                System.Threading.Thread.Sleep(k_WaitingSeconds);
                m_boardGame.CellMatrix[firstComputerMove.Item1, firstComputerMove.Item2].IsExposed = !v_Expose;
                m_boardGame.CellMatrix[secondComputerMove.Item1, secondComputerMove.Item2].IsExposed = !v_Expose;
                Ex02.ConsoleUtils.Screen.Clear();
                printBoard();
                m_player1.MyTurn = !m_player1.MyTurn;   // make the next turn will be the player turn 
            }
            else    // if the computer found a pair
            {
                Console.WriteLine("computer has found a pair and gets a point");
                m_computerPlayer.AddPointToScore();
                m_boardGame.PairFound();
                System.Threading.Thread.Sleep(k_WaitingSeconds); // we use sleep function here to make it more reality for the user
            }
        }

        // make the player move
        private void playerMove(ref Player i_currentlyPlaying)
        {
            const int k_WaitingSeconds = 2000;  // delay time

            Console.WriteLine("Player: " + i_currentlyPlaying.UserName);

            char sign1, sign2;
            byte firstWantedRow, firstWantedColumn, secondWantedRow, secondWantedcolumn;
            bool k_Exposed = true;

            // get the first choice , the sign , expose and print
            getCellFromUser(out firstWantedRow, out firstWantedColumn);
            sign1 = m_boardGame.CellMatrix[firstWantedRow, firstWantedColumn].CellSign;
            m_boardGame.CellMatrix[firstWantedRow, firstWantedColumn].IsExposed = k_Exposed;
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();

            Console.WriteLine("Player: " + i_currentlyPlaying.UserName);

            // get the second choice , the sign , expose and print
            getCellFromUser(out secondWantedRow, out secondWantedcolumn);
            sign2 = m_boardGame.CellMatrix[secondWantedRow, secondWantedcolumn].CellSign;
            m_boardGame.CellMatrix[secondWantedRow, secondWantedcolumn].IsExposed = k_Exposed;
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();

            if (sign1 != sign2)     // if the player didnt found a pair
            {
                Console.WriteLine(i_currentlyPlaying.UserName + " turn has ended");
                System.Threading.Thread.Sleep(k_WaitingSeconds);
                m_boardGame.CellMatrix[firstWantedRow, firstWantedColumn].IsExposed = !k_Exposed;
                m_boardGame.CellMatrix[secondWantedRow, secondWantedcolumn].IsExposed = !k_Exposed;
                Ex02.ConsoleUtils.Screen.Clear();
                printBoard();
                m_player1.MyTurn = !m_player1.MyTurn;
            }
            else        // if the player found a pair
            {
                i_currentlyPlaying.AddPoint();
                m_boardGame.PairFound();

                if (i_currentlyPlaying.Score > 1)
                {
                    Console.WriteLine("Nice choice! " + i_currentlyPlaying.UserName + " have " + i_currentlyPlaying.Score + " points");
                }
                else
                {
                    Console.WriteLine("Nice choice! " + i_currentlyPlaying.UserName + " have " + i_currentlyPlaying.Score + " point");
                }
            }

            // if we are playing against the computer we need to update the player moves to the memory of the computer
            if (m_FormatGame == eGameFormat.AgainstComputer)
            {
                m_computerPlayer.UpDateComputerMemory(firstWantedRow, firstWantedColumn, secondWantedRow, secondWantedcolumn, sign1, sign2);
            }
        }

        // get a valid cell from user or exit from the game with the keys 'Q' or 'q'
        private void getCellFromUser(out byte o_WantedRow, out byte o_WantedColumn)
        {
            const string k_QuitGame1 = "Q";
            const string k_QuitGame2 = "q";
            o_WantedColumn = 0;
            o_WantedRow = 0;
            Console.WriteLine("Choose which cell you want to reveal, For example 'B3' (and then press 'Enter')");

            string playerInput;
            char wantedColumnChar, wantedRowChar;
            bool validChoise = false;

            do
            {
                playerInput = Console.ReadLine();

                if (playerInput == k_QuitGame1 || playerInput == k_QuitGame2)
                {
                    Environment.Exit(0);
                }

                if (playerInput.Length == 2 && isLetter(playerInput[0]) && isDigit(playerInput[1]))
                {
                    wantedColumnChar = playerInput[0];
                    wantedRowChar = playerInput[1];

                    validChoise = checkValidCellChoise(wantedRowChar, wantedColumnChar, out o_WantedRow, out o_WantedColumn);
                }
                else
                {
                    System.Console.WriteLine(
                        "Error,invalid input row must be a number and column must be a English letter" +
                        "\nChoose which cell you want to reveal, For example 'B3' (and then press 'Enter')");
                }
            }
            while (!validChoise);
        }

        private bool checkValidCellChoise(char i_WantedRow, char i_WantedColumn, out byte o_RowNumber, out byte o_columnNumber)
        {
            bool legalInput =
                translateBoardLocationToNumbers(i_WantedRow, i_WantedColumn, out o_RowNumber, out o_columnNumber);

            bool isCellOutOfRange = false, isCellAllreadyExpose = false;

            if (legalInput)
            {
                m_boardGame.IsCellAvailableForPicking(o_RowNumber, o_columnNumber, out isCellOutOfRange, out isCellAllreadyExpose);
                printMessageAboutValidChoosenCell(isCellOutOfRange, isCellAllreadyExpose);
            }

            return !(isCellOutOfRange || isCellAllreadyExpose) && legalInput;
        }

        private void printMessageAboutValidChoosenCell(bool i_IsCellOutOfRange, bool i_IsCellAlreadyExpose)
        {
            if (i_IsCellOutOfRange)
            {
                const char k_Startletter = 'A';

                char endLetter = (char)(k_Startletter + m_boardGame.NumberOfColumn - 1);
                string rowStr = m_boardGame.NumberOfRows.ToString();

                Console.WriteLine("Error, input out of range, row must be between 1 to " + rowStr + " and column must be between A to " + endLetter);
            }
            else if (i_IsCellAlreadyExpose)
            {
                Console.WriteLine("Error, this sell already expose");
            }

            if (i_IsCellOutOfRange || i_IsCellAlreadyExpose)
            {
                Console.WriteLine("Please try again, Choose which cell you want to reveal, For example 'B3'");
            }
        }

        private void printWinningMessage()
        {
            Console.WriteLine();

            Console.WriteLine("The game is ended");

            if (m_FormatGame == eGameFormat.AgainstPlayer)
            {
                Console.WriteLine(m_player1.UserName + " have " + m_player1.Score + " points");
                Console.WriteLine(m_player2.UserName + " have " + m_player2.Score + " points");

                if (m_player1.Score > m_player2.Score)
                {
                    Console.WriteLine("Congratulation, " + m_player1.UserName + " won the game!");
                }
                else if (m_player1.Score < m_player2.Score)
                {
                    Console.WriteLine("Congratulation, " + m_player2.UserName + " won the game!");
                }
                else
                {
                    Console.WriteLine("The game ended in a tie!");
                }
            }
            else
            {
                Console.WriteLine(m_player1.UserName + " have " + m_player1.Score + " points");
                Console.WriteLine("The computer have " + m_computerPlayer.Score + " points");

                if (m_player1.Score > m_computerPlayer.Score)
                {
                    Console.WriteLine("Congratulation, " + m_player1.UserName + " won the game!");
                }
                else if (m_player1.Score < m_computerPlayer.Score)
                {
                    Console.WriteLine("Unfortunately, the computer won the game!");
                }
                else
                {
                    Console.WriteLine("The game ended in a tie!");
                }
            }

            Console.WriteLine();
        }

        // change from the string user input to byte number  
        private bool translateBoardLocationToNumbers(char i_RowInChar, char i_ColumnInChar, out byte o_RowNumber, out byte o_columnNumber)
        {
            bool canTranslateInput = true;

            if (isDigit(i_RowInChar))
            {
                o_RowNumber = (byte)(i_RowInChar - '1');
            }
            else
            {
                canTranslateInput = false;
                o_RowNumber = 0;
            }

            if (canTranslateInput && isLetter(i_ColumnInChar))
            {
                o_columnNumber = (byte)(char.ToLower(i_ColumnInChar) - 'a');
            }
            else
            {
                canTranslateInput = false;
                o_columnNumber = 0;
            }

            return canTranslateInput;
        }

        // build the board with a stringBuilder 
        private StringBuilder buildStringBuilderBoard()
        {
            StringBuilder sapareteLineBuilder = new StringBuilder();
            StringBuilder boardStringBuilder = new StringBuilder();

            buildUpFrame(ref boardStringBuilder);
            buildSeparateLine(ref boardStringBuilder);

            for (int i = 0; i < m_boardGame.NumberOfRows; i++)
            {
                buildOneLine(ref boardStringBuilder, i);
                buildSeparateLine(ref boardStringBuilder);
            }

            return boardStringBuilder;
        }

        // build in the string builder the columns letters
        private void buildUpFrame(ref StringBuilder i_BoardStringBuilder)
        {
            const char k_StartColumn = 'A';
            const string k_Spaces4 = "    ";
            const string k_Spaces3 = "   ";

            int i;
            char columnSign;
            i_BoardStringBuilder.Append(k_Spaces4);

            for (i = 0; i < m_boardGame.NumberOfColumn - 1; i++)
            {
                columnSign = (char)(k_StartColumn + i);
                i_BoardStringBuilder.Append(columnSign);
                i_BoardStringBuilder.Append(k_Spaces3);
            }

            columnSign = (char)(k_StartColumn + i);
            i_BoardStringBuilder.Append(columnSign);
        }

        private void buildSeparateLine(ref StringBuilder i_BoardStringBuilder)
        {
            const char k_separateLine = '=';
            const string k_Spaces2ForSapereteLine = "  ";

            i_BoardStringBuilder.Append('\n');
            i_BoardStringBuilder.Append(k_Spaces2ForSapereteLine);
            i_BoardStringBuilder.Append(k_separateLine, (m_boardGame.NumberOfColumn * 4) + 1);
            i_BoardStringBuilder.Append('\n');
        }

        // build one line of the board in the stringBuilder
        private void buildOneLine(ref StringBuilder i_BoardStringBuilder, int i_RowNum)
        {
            const char k_SeparateColumn = '|';
            const char k_Space1 = ' ';

            char rowNumChar = (char)(i_RowNum + 1 + '0');

            i_BoardStringBuilder.Append(rowNumChar);
            i_BoardStringBuilder.Append(k_Space1);
            i_BoardStringBuilder.Append(k_SeparateColumn);

            for (byte i = 0; i < m_boardGame.NumberOfColumn; i++)
            {
                i_BoardStringBuilder.Append(k_Space1);

                // print space or the sign depend on the current state of expose  
                if (m_boardGame.CellMatrix[i_RowNum, i].IsExposed)
                {
                    i_BoardStringBuilder.Append(m_boardGame.CellMatrix[i_RowNum, i].CellSign);
                }
                else
                {
                    i_BoardStringBuilder.Append(k_Space1);
                }

                i_BoardStringBuilder.Append(k_Space1);
                i_BoardStringBuilder.Append(k_SeparateColumn);
            }
        }

        private bool isLetter(char i_Character)
        {
            return (i_Character >= 'a' && i_Character <= 'z') || (i_Character >= 'A' && i_Character <= 'Z');
        }

        private bool isDigit(char i_Character)
        {
            return i_Character >= '0' && i_Character <= '9';
        }
    }
}
