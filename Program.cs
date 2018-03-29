using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DecoratorBoardGameConsoleApp
{
    static class Constants
    {
        public const int PLAYERSTARTCOL = 0;
        public const int PLAYERSTARTROW = 0;

        public const int BOARDSIZE = 4;
        public const int BOARDCOLSIZE = BOARDSIZE;
        public const int BOARDROWSIZE = BOARDCOLSIZE;

        public const int SQUARESIZE = 15;
    }

    public enum Rotation { LEFT = 0, RIGHT = 1, ROTATIONMAX = 2 };
    public enum CompassPoints { NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3, COMPASSMAX = 4 };

    public abstract class Player
    {
        public virtual void Turn(Rotation rotate) { }

        public string name;
        public CompassPoints facingDirection;

        public int colPosition;
        public int rowPosition;

        public Player()
        { }

        ~Player()
        { }

        public void PlayerTurn(Player p, Rotation rotate)
        {
            int compassPointMax = (int)CompassPoints.COMPASSMAX;
            int facingDirection = (int)p.facingDirection;
            switch (rotate)
            {
                case Rotation.LEFT:
                    p.facingDirection = (CompassPoints)((facingDirection - 1 + compassPointMax) % compassPointMax);
                    break;
                case Rotation.RIGHT:
                    p.facingDirection = (CompassPoints)((facingDirection + 1 + compassPointMax) % compassPointMax);
                    break;
                default:
                    break;
            }
        }
    };

    class DefaultPlayer : Player
    {
        public override void Turn(Rotation rotate)
        {
            PlayerTurn(this, rotate);
        }

        public DefaultPlayer(string baseName)
        {
            // Set-up new Player at default board position
            facingDirection = CompassPoints.NORTH;
            name = baseName;
            colPosition = 0;
            rowPosition = 0;
        }

        public DefaultPlayer(string baseName, int row, int col)
        {
            // Set-up new Player at given co-ordinates
            facingDirection = CompassPoints.NORTH;
            name.Equals(baseName);
            colPosition = col;
            rowPosition = row;
        }

        ~DefaultPlayer() { }
    }
    public class BoardSquare
    {
        public Player boardSpace;
        public bool northWall;
        public bool southWall;
        public bool westWall;
        public bool eastWall;

        public BoardSquare() { }
        ~BoardSquare() { }
    }

    public class GamesBoard : BoardSquare
    {
        public int GetNextColPosition(List<List<BoardSquare>> board, Player player)
        {
            switch (player.facingDirection)
            {
                case CompassPoints.EAST:

                    if (!(board[player.colPosition][player.rowPosition].eastWall))
                        return (player.colPosition + 1);
                    break;

                case CompassPoints.WEST:
                    if (!(board[player.colPosition][player.rowPosition].westWall))
                        return (player.colPosition - 1);
                    break;

                default:
                    break;
            }
            return player.colPosition;
        }

        public int GetNextRowPosition(List<List<BoardSquare>> board, Player player)
        {
            switch (player.facingDirection)
            {
                case CompassPoints.NORTH:
                    if (!(board[player.colPosition][player.rowPosition].northWall))
                        return (player.rowPosition + 1);
                    break;

                case CompassPoints.SOUTH:
                    if (!(board[player.colPosition][player.rowPosition].southWall))
                        return (player.rowPosition - 1);
                    break;

                default:
                    break;
            }
            return player.rowPosition;
        }

        public void DisplayBoard()
        {
            string name = "";
            for (int rx = (Constants.BOARDROWSIZE - 1); rx >= 0; rx--)
            {
                // Draw NORTH wall
                for (int col = 0; col < (Constants.BOARDCOLSIZE); col++)
                {
                    Console.Write("+");
                    for (int l = 0; l < Constants.SQUARESIZE; l++)
                    {
                        if (board[col][rx].northWall)
                        {
                            Console.Write("-");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                }
                Console.WriteLine("+");

                // Add square padding
                for (int col = 0; col < Constants.BOARDCOLSIZE; col++)
                {
                    if (board[col][rx].boardSpace != null)
                        name = board[col][rx].boardSpace.name;

                    Console.Write(" ");
                    if (board[col][rx].westWall)
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                    int l = 0;
                    while (l < (Constants.SQUARESIZE - 2))
                    {
                        if ((board[col][rx].boardSpace != null) && (l < name.Length))
                        {
                            Console.Write(name[l]);
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        l++;
                    }
                    if (board[col][rx].eastWall)
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();

                // Draw SOUTH wall
                for (int col = 0; col < (Constants.BOARDCOLSIZE); col++)
                {
                    Console.Write("+");
                    for (int l = 0; l < Constants.SQUARESIZE; l++)
                    {
                        if (board[col][rx].southWall)
                        {
                            Console.Write("-");
                            //Console.Write( (l % 10);
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                }
                Console.WriteLine("+");
            }

        }

        public virtual void AddPlayer(Player player)
        {
            board[player.colPosition][player.rowPosition].boardSpace = player;
        }

        public virtual void MovePlayerOneSquare(Player player)
        {
            board[player.colPosition][player.rowPosition].boardSpace = null;

            player.colPosition = GetNextColPosition(board, player);
            player.rowPosition = GetNextRowPosition(board, player);

            Console.WriteLine("Next Col {0} Next Row {1}", player.colPosition, player.rowPosition);

            board[player.colPosition][player.rowPosition].boardSpace = player;
        }

        public List<List<BoardSquare>> board = new List<List<BoardSquare>>();

        public GamesBoard()
        {
            for (int c = 0; c < Constants.BOARDCOLSIZE; c++)
            {
                board.Add(new List<BoardSquare>());
                for (int r = 0; r < Constants.BOARDROWSIZE; r++)
                {
                    BoardSquare tempSquare = new BoardSquare();
                    board[c].Add(tempSquare);
                    board[c][r].boardSpace = null;
                    if (c == 0)
                    {
                        board[c][r].westWall = true;
                    }
                    else
                    {
                        board[c][r].westWall = false;
                    }

                    if (c == (Constants.BOARDCOLSIZE - 1))
                    {
                        board[c][r].eastWall = true;
                    }
                    else
                    {
                        board[c][r].eastWall = false;
                    }

                    if (r == 0)
                    {
                        board[c][r].southWall = true;
                    }
                    else
                    {
                        board[c][r].southWall = false;
                    }

                    if (r == (Constants.BOARDROWSIZE - 1))
                    {
                        board[c][r].northWall = true;
                    }
                    else
                    {
                        board[c][r].northWall = false;
                    }
                }
            }
        }
        ~GamesBoard() { }
    }

    public class DefaultBoard : GamesBoard
    {
        public DefaultBoard() { }
        ~DefaultBoard() { }
    }

    public class BoardDecoration : GamesBoard
    {
        public GamesBoard prevDeco;

        public override void AddPlayer(Player player) { prevDeco.AddPlayer(player); }
        public override void MovePlayerOneSquare(Player player)
        {
            board[player.colPosition][player.rowPosition].boardSpace = null;

            player.colPosition = GetNextColPosition(board, player);
            player.rowPosition = GetNextRowPosition(board, player);

            board[player.colPosition][player.rowPosition].boardSpace = player;

            prevDeco.MovePlayerOneSquare(player);
        }

        public BoardDecoration() { }
        public BoardDecoration(GamesBoard gamesBoard) { this.prevDeco = gamesBoard; }

        ~BoardDecoration() { }
    }

    public class InnerWallBoard : BoardDecoration
    {
        public InnerWallBoard(GamesBoard gb, int colPosition, int rowPosition, CompassPoints wallSide)
        {
            for (int c = 0; c < Constants.BOARDCOLSIZE; c++)
            {
                for (int r = 0; r < Constants.BOARDROWSIZE; r++)
                {
                    board[c][r].boardSpace = gb.board[c][r].boardSpace;
                    board[c][r].northWall = gb.board[c][r].northWall;
                    board[c][r].southWall = gb.board[c][r].southWall;
                    board[c][r].westWall = gb.board[c][r].westWall;
                    board[c][r].eastWall = gb.board[c][r].eastWall;
                }
            }

            this.prevDeco = gb;
            switch (wallSide)
            {
                case CompassPoints.NORTH:
                    gb.board[colPosition][rowPosition].northWall = true;
                    board[colPosition][rowPosition].northWall = true;
                    prevDeco.board[colPosition][rowPosition].northWall = true;
                    break;
                case CompassPoints.SOUTH:
                    gb.board[colPosition][rowPosition].southWall = true;
                    board[colPosition][rowPosition].southWall = true;
                    prevDeco.board[colPosition][rowPosition].southWall = true;
                    break;
                case CompassPoints.WEST:
                    gb.board[colPosition][rowPosition].westWall = true;
                    board[colPosition][rowPosition].westWall = true;
                    prevDeco.board[colPosition][rowPosition].westWall = true;
                    break;
                case CompassPoints.EAST:
                    gb.board[colPosition][rowPosition].eastWall = true;
                    board[colPosition][rowPosition].eastWall = true;
                    prevDeco.board[colPosition][rowPosition].eastWall = true;
                    break;
                default:
                    break;
            }

        }

        public override void MovePlayerOneSquare(Player player)
        {
            board[player.colPosition][player.rowPosition].boardSpace = null;

            player.colPosition = GetNextColPosition(board, player);
            player.rowPosition = GetNextRowPosition(board, player);

            Console.WriteLine("IWB: Next Col {0} Next Row {1}", player.colPosition, player.rowPosition);

            board[player.colPosition][player.rowPosition].boardSpace = player;

        }

    }

    public class PlayerDecoration : Player
    {
        public Player prevDeco;

        // Mandatory base class methods (that were virtual):
        public override void Turn(Rotation rotate)
        {
            prevDeco.Turn(rotate);
        }

        // CONSTRUCTOR:
        // Constructor based upon a previous player
        public PlayerDecoration(Player p)
        {
            // Attach to previous decoration
            this.prevDeco = p;
        }

        // Construction of new player at default board co-ordiantes (0,0) with given Name
        public PlayerDecoration(string basename)
        {
            // Set-up new Player at default board position
            facingDirection = CompassPoints.NORTH;
            name = basename;
            colPosition = 0;
            rowPosition = 0;
        }

        // Construction of new player at board co-ordiantes given board co-ordinates with given Name
        public PlayerDecoration(string basename, int row, int col)
        {
            // Set-up new Player at given co-ordinates
            facingDirection = CompassPoints.NORTH;
            name = basename;
            colPosition = col;
            rowPosition = row;
        }

        ~PlayerDecoration() { }
    };

    public class EnhancedPlayer : PlayerDecoration
    {
        public int playerSpeed = 1;

        public EnhancedPlayer(Player p) : base(p)
        {
            colPosition = p.colPosition;
            rowPosition = p.rowPosition;
            facingDirection = p.facingDirection;
            name = p.name;
            playerSpeed = 1;
            this.prevDeco = p;
        }

        public EnhancedPlayer(string basename, int row, int col) : base(basename, row, col)
        {
            colPosition = col;
            rowPosition = row;
            facingDirection = CompassPoints.NORTH;
            name = basename;
            playerSpeed = 1;
            this.prevDeco = base.prevDeco;
        }

        public EnhancedPlayer(string basename, int row, int col, int speed) : base(basename, row, col)
        {
            facingDirection = CompassPoints.NORTH;
            name = basename;
            colPosition = col;
            rowPosition = row;
            playerSpeed = speed;
            this.prevDeco = base.prevDeco;
        }

        public override void Turn(Rotation rotate)
        {
            prevDeco.PlayerTurn(this, rotate);

            prevDeco.PlayerTurn(prevDeco, rotate);
        }

        ~EnhancedPlayer() { }
    };


    class EnhancedSquare : BoardSquare
    {
        public EnhancedPlayer enhancedBoardSpace;
    };

    class EnhancedBoard : BoardDecoration
    {
        public void AddPlayer(EnhancedPlayer ePlayer)
        {
            eBoard[ePlayer.colPosition][ePlayer.rowPosition].enhancedBoardSpace = ePlayer;
            eBoard[ePlayer.colPosition][ePlayer.rowPosition].boardSpace = ePlayer;
            board[ePlayer.colPosition][ePlayer.rowPosition].boardSpace = ePlayer;
            prevDeco.AddPlayer(ePlayer);
        }

        public void MovePlayerOneSquare(EnhancedPlayer ePlayer)
        {
            int i = 0;
            while (i < ePlayer.playerSpeed)
            {

                ePlayer.playerSpeed = ePlayer.playerSpeed + GetPowerNextSquare(eBoard, ePlayer);

                eBoard[ePlayer.colPosition][ePlayer.rowPosition].boardSpace = null;
                eBoard[ePlayer.colPosition][ePlayer.rowPosition].enhancedBoardSpace = null;
                board[ePlayer.colPosition][ePlayer.rowPosition].boardSpace = null;

                ePlayer.colPosition = GetNextColPosition(board, ePlayer);
                ePlayer.rowPosition = GetNextRowPosition(board, ePlayer);

                eBoard[ePlayer.colPosition][ePlayer.rowPosition].boardSpace = ePlayer;
                eBoard[ePlayer.colPosition][ePlayer.rowPosition].enhancedBoardSpace = ePlayer;
                board[ePlayer.colPosition][ePlayer.rowPosition].boardSpace = ePlayer;

                prevDeco.MovePlayerOneSquare(ePlayer.prevDeco);

                i++;
            }
        }

        public int GetPowerNextSquare(List<List<EnhancedSquare>> eBoard, EnhancedPlayer ePlayer)
        {
            int nextCol = ePlayer.colPosition;
            int nextRow = ePlayer.rowPosition;
            int powerPoints = 0;

            switch (ePlayer.facingDirection)
            {
                case CompassPoints.NORTH:
                    if (!board[ePlayer.colPosition][ePlayer.rowPosition].northWall)
                    {
                        nextRow++;
                    }
                    else
                    {
                        return powerPoints;
                    }
                    break;

                case CompassPoints.SOUTH:
                    if (!board[ePlayer.colPosition][ePlayer.rowPosition].southWall)
                    {
                        nextRow--;
                    }
                    else
                    {
                        return powerPoints;
                    }
                    break;

                case CompassPoints.WEST:
                    if (!board[ePlayer.colPosition][ePlayer.rowPosition].westWall)
                    {
                        nextCol--;
                    }
                    else
                    {
                        return powerPoints;
                    }
                    break;

                case CompassPoints.EAST:
                    if (!board[ePlayer.colPosition][ePlayer.rowPosition].eastWall)
                    {
                        nextCol++;
                    }
                    else
                    {
                        return powerPoints;
                    }
                    break;

                default:
                    break;
            }

            if (eBoard[nextCol][nextRow].enhancedBoardSpace != null)
            {
                powerPoints = eBoard[nextCol][nextRow].enhancedBoardSpace.playerSpeed;
            }
            return powerPoints;
        }

        public List<List<EnhancedSquare>> eBoard = new List<List<EnhancedSquare>>();

        public EnhancedBoard() { }
        public EnhancedBoard(GamesBoard gb)
        {
            for (int c = 0; c < Constants.BOARDCOLSIZE; c++)
            {
                eBoard.Add(new List<EnhancedSquare>());
                for (int r = 0; r < Constants.BOARDROWSIZE; r++)
                {
                    EnhancedSquare tempESquare = new EnhancedSquare();
                    eBoard[c].Add(tempESquare);

                    board[c][r].boardSpace = gb.board[c][r].boardSpace;
                    board[c][r].northWall = gb.board[c][r].northWall;
                    board[c][r].southWall = gb.board[c][r].southWall;
                    board[c][r].westWall = gb.board[c][r].westWall;
                    board[c][r].eastWall = gb.board[c][r].eastWall;

                    eBoard[c][r].boardSpace = gb.board[c][r].boardSpace;
                    eBoard[c][r].northWall = gb.board[c][r].northWall;
                    eBoard[c][r].southWall = gb.board[c][r].southWall;
                    eBoard[c][r].westWall = gb.board[c][r].westWall;
                    eBoard[c][r].eastWall = gb.board[c][r].eastWall;
                }
            }
            this.prevDeco = gb;
        }

        ~EnhancedBoard()
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Decorator Board Game Console Applications Using Database");

            /* Create a square playing board  5 x 5 */
            GamesBoard gamesBoard = new DefaultBoard();
            List<DefaultPlayer> playersTable = new List<DefaultPlayer>();

            /* Connect to Database */
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = "Database=acsm_154975d2ca17b0c;Data Source=eu-cdbr-azure-west-b.cloudapp.net;User Id=ba5e91c245b744;Password=255e5cd3";

            /* Load Players From Database */
            MySqlCommand cmdQueryAllPlayers = new MySqlCommand("spQueryAllPlayers", conn);
            try
            {
                Console.WriteLine("Connecting to Player Table");
                cmdQueryAllPlayers.CommandType = CommandType.StoredProcedure;
                cmdQueryAllPlayers.Connection.Open();

                MySqlDataReader reader = cmdQueryAllPlayers.ExecuteReader();

                if(reader.HasRows)
                {
                    Console.WriteLine("Player Name, Facing Direction,Col, Row");
                    CompassPoints rFacingDirection = CompassPoints.NORTH;
                    while (reader.Read())
                    {
                        Console.WriteLine("{0}, {1}, {2}, {3}", reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3) );
                        
                        switch (reader.GetString(1).ToUpper())
                        {
                            case "SOUTH":
                                {
                                    rFacingDirection = CompassPoints.SOUTH;
                                }
                                break;
                            case "WEST":
                                {
                                    rFacingDirection = CompassPoints.WEST;
                                }
                                break;
                            case "EAST":
                                {
                                    rFacingDirection = CompassPoints.EAST;
                                }
                                break;

                            default:
                                rFacingDirection = CompassPoints.NORTH;
                                break;
                        }

                        playersTable.Add(new DefaultPlayer(reader.GetString(0)) { name = reader.GetString(0), facingDirection= rFacingDirection , colPosition=reader.GetInt32(2), rowPosition=reader.GetInt32(3)});

                    }
                }
                else
                {
                    Console.WriteLine("No Players Found.");
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            cmdQueryAllPlayers.Connection.Close();
            Console.WriteLine("Player Table Connection closed.");

            /* Load Board From Database */
            MySqlCommand cmdQueryAllBoardSquares = new MySqlCommand("spQueryAllBoardSquares", conn);
            try
            {
                int index = 0;

                Console.WriteLine("Connecting to Board Sqaures Table...");
                cmdQueryAllBoardSquares.CommandType = CommandType.StoredProcedure;
                cmdQueryAllBoardSquares.Connection.Open();

                MySqlDataReader reader = cmdQueryAllBoardSquares.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("Player Name, Facing Direction,Col, Row, North Wall, South Wall, West Wall, East Wall");
                    while (reader.Read())
                    {
                        string nullString = "NULL";
                        if (reader.IsDBNull(0))
                        {
                            /* Empty Board Square */
                            Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", nullString, nullString, reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7));
                        }
                        else
                        {
                            /* Find player in the players table */
                            index = playersTable.FindIndex(a => a.name.Equals(reader.GetString(0)));
                            gamesBoard.board[reader.GetInt32(2)][reader.GetInt32(3)].boardSpace = playersTable[index];

                            /* Board Square with Player */
                            Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7));
                        }
                        /* Add database board square to Games Board */
                        gamesBoard.board[reader.GetInt32(2)][reader.GetInt32(3)].northWall = Convert.ToBoolean(reader.GetInt32(4));
                        gamesBoard.board[reader.GetInt32(2)][reader.GetInt32(3)].southWall = Convert.ToBoolean(reader.GetInt32(5));
                        gamesBoard.board[reader.GetInt32(2)][reader.GetInt32(3)].westWall = Convert.ToBoolean(reader.GetInt32(6));
                        gamesBoard.board[reader.GetInt32(2)][reader.GetInt32(3)].eastWall = Convert.ToBoolean(reader.GetInt32(7));
                    }
                }
                else
                {
                    Console.WriteLine("No Board Squares Found.");
                }
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            cmdQueryAllBoardSquares.Connection.Close();
            Console.WriteLine("Connection to Board Squares Table closed.");
            gamesBoard.DisplayBoard();

            /* Add player upon the board at 0,0 facing North 
            Player playerOne = new DefaultPlayer("Player 1");
            gamesBoard.AddPlayer(playerOne);
            gamesBoard.DisplayBoard();

            /* Add inner walls to the game board*/
            gamesBoard = new InnerWallBoard(gamesBoard, 2, 2, CompassPoints.NORTH);
            gamesBoard = new InnerWallBoard(gamesBoard, 2, 2, CompassPoints.EAST);
            gamesBoard.DisplayBoard();

            /* Move player East by one square */
            playersTable[0].Turn(Rotation.RIGHT);
            gamesBoard.MovePlayerOneSquare(playersTable[0]);
            gamesBoard.DisplayBoard();

            /* Move player North by ten squares */
            playersTable[0].Turn(Rotation.LEFT);
            for (int i = 0; i < 10; i++)
            {
                gamesBoard.MovePlayerOneSquare(playersTable[0]);
                gamesBoard.DisplayBoard();
            }
            gamesBoard.DisplayBoard();

            /* Move player East by ten squares */
            playersTable[0].Turn(Rotation.RIGHT);
            for (int i = 0; i < 10; i++)
            {
                gamesBoard.MovePlayerOneSquare(playersTable[0]);
            }
            gamesBoard.DisplayBoard();

            /* Make Enhanced Games Board*/
            EnhancedBoard enhancedGamesBoard = new EnhancedBoard(gamesBoard);

            /* Add Power Item to the board */
            Console.WriteLine("\nAdd Power Cookie to the new board");
            EnhancedPlayer cookieOne = new EnhancedPlayer("Cookie 1", 0, 1, 1);

            enhancedGamesBoard.AddPlayer(cookieOne);
            enhancedGamesBoard.DisplayBoard();

            /* Make player 2 to eat cookies */
            EnhancedPlayer enhancedPlayerOne = new EnhancedPlayer(playersTable[0]);
            enhancedGamesBoard.AddPlayer(enhancedPlayerOne);
            enhancedGamesBoard.DisplayBoard();

            /* Move player west by four squares */
            enhancedPlayerOne.Turn(Rotation.LEFT);
            enhancedPlayerOne.Turn(Rotation.LEFT);
            for (int i = 0; i < 4; i++)
            {
                enhancedGamesBoard.MovePlayerOneSquare(enhancedPlayerOne);
            }
            gamesBoard.DisplayBoard();
            enhancedGamesBoard.DisplayBoard();

            /* Move player 1 South by three square */
            enhancedPlayerOne.Turn(Rotation.LEFT);
            for (int i = 0; i < 3; i++)
            {
                enhancedGamesBoard.MovePlayerOneSquare(enhancedPlayerOne);
            }
            enhancedGamesBoard.MovePlayerOneSquare(enhancedPlayerOne);
            enhancedGamesBoard.DisplayBoard();

            /* Move player 1 East by one square */
            enhancedPlayerOne.Turn(Rotation.LEFT);
            for (int i = 0; i < 1; i++)
            {
                enhancedGamesBoard.MovePlayerOneSquare(enhancedPlayerOne);
            }
            enhancedGamesBoard.MovePlayerOneSquare(enhancedPlayerOne);
            enhancedGamesBoard.DisplayBoard();

            /* Update database players table */
            MySqlCommand cmdUpdatePlayer = new MySqlCommand("spUpdatePlayer", conn);
            try
            {
                Console.WriteLine("Connecting to Player Table");
                cmdUpdatePlayer.CommandType = CommandType.StoredProcedure;
                cmdUpdatePlayer.Connection.Open();

                string mFacingDirection = "NORTH";
                foreach(DefaultPlayer p in playersTable)
                {
                    cmdUpdatePlayer.Parameters.AddWithValue("@m_playername", p.name);
                    switch (p.facingDirection)
                    {
                        case CompassPoints.EAST:
                            mFacingDirection = "EAST";
                            break;
                        case CompassPoints.SOUTH:
                            mFacingDirection = "SOUTH";
                            break;
                        case CompassPoints.WEST:
                            mFacingDirection = "WEST";
                            break;
                        default:
                            mFacingDirection = "NORTH";
                            break;
                    }
                    cmdUpdatePlayer.Parameters.AddWithValue("@m_facingdirection", mFacingDirection);
                    cmdUpdatePlayer.Parameters.AddWithValue("@m_colposition", p.colPosition);
                    cmdUpdatePlayer.Parameters.AddWithValue("@m_rowposition", p.rowPosition);

                    cmdUpdatePlayer.ExecuteNonQuery();
                    cmdUpdatePlayer.Parameters.Clear();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            cmdUpdatePlayer.Connection.Close();
            Console.WriteLine("Player Table Connection closed.");

            /* Update database players board squares */
            MySqlCommand cmdUpdateBoardSquare = new MySqlCommand("spUpdateBoardSquare", conn);
            try
            {
                Console.WriteLine("Connecting to Board Sqaures Table...");
                cmdUpdateBoardSquare.CommandType = CommandType.StoredProcedure;
                cmdUpdateBoardSquare.Connection.Open();

                for (int r= 0; r < Constants.BOARDROWSIZE; r++ )
                {
                    for (int c=0; c < Constants.BOARDCOLSIZE; c++)
                    {
                        if (gamesBoard.board[c][r].boardSpace != null)
                        {
                            cmdUpdateBoardSquare.Parameters.AddWithValue("@m_playername", gamesBoard.board[c][r].boardSpace);
                        }
                        else
                        {
                            cmdUpdateBoardSquare.Parameters.AddWithValue("@m_playername", null);
                        }
                        cmdUpdateBoardSquare.Parameters.AddWithValue("@m_colposition", c);
                        cmdUpdateBoardSquare.Parameters.AddWithValue("@m_rowposition", r);
                        cmdUpdateBoardSquare.Parameters.AddWithValue("@m_northwall", Convert.ToInt32(gamesBoard.board[c][r].northWall));
                        cmdUpdateBoardSquare.Parameters.AddWithValue("@m_southwall", Convert.ToInt32(gamesBoard.board[c][r].southWall));
                        cmdUpdateBoardSquare.Parameters.AddWithValue("@m_westwall", Convert.ToInt32(gamesBoard.board[c][r].westWall));
                        cmdUpdateBoardSquare.Parameters.AddWithValue("@m_eastwall", Convert.ToInt32(gamesBoard.board[c][r].eastWall));

                        cmdUpdateBoardSquare.ExecuteNonQuery();
                        cmdUpdateBoardSquare.Parameters.Clear();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            cmdUpdateBoardSquare.Connection.Close();
            Console.WriteLine("Connection to Board Squares Table closed.");

            Console.WriteLine("\nPress Any Key To Continue . . . ");
            Console.ReadKey();

        }
    }
}
