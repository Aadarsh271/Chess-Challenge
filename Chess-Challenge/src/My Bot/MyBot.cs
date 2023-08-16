using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    static double[] qTable;
    //static bool firstTurn = true;
    static bool isWhite = false;

    public MyBot()
    {
        initQTable();
    }

    public void initQTable()
    {
        // Technically speaking, a Q-Table requires an entry for each possible state
        // And a very large list of actions
        // A traditional QTable is impractical for chess
        //qTable = new double
    }

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        isWhite = board.IsWhiteToMove;
        Random rng = new();
        Move chosenMove = moves[rng.Next(moves.Length)];
        int bestReward = -100000;
        foreach (Move move in moves)
        {
            if(MoveIsCheckmate(board, move))
            {
                return move;
            }
            int r = computeReward(board, move);
            if(r > bestReward)
            {
                bestReward = r;
                chosenMove = move;
            }

        }
        return chosenMove;

    }

    int computeReward(Board board, Move move)
    {
        int reward = 0;
        //bool isWhite = false;
        board.MakeMove(move);
        int[] pieceRewards = { 0, 100, 300, 300, 500, 900, 10000 };
        // { 0, 200, 400, 500, 700, 100, 3000 };
        //if(board.IsInCheck())
        //{
        //    reward += 10000;
        //}
        foreach (PieceList pieceList in board.GetAllPieceLists() )
        {
            foreach (Piece piece in pieceList)
            {
                bool myPiece = (piece.IsWhite == isWhite);
                Square s = piece.Square;
                /* For each piece, based on its position give it a score
                 * Write functions for each piece that determine the strength of its position
                 * Offensive pieces prefer to be in the center/ends of the board
                 * King preferse to be in a defensive position
                 * Pawns prefer having pawns on their flanks
                 * Offensive pieces prefer positions where they threaten their opponent,
                 * but are not threatened themselves
                 * Each opposing piece should probably have a significant cost, so as to represent
                 * the reward of taking a piece
                 * Reward values can be taught, btw!!!!
                 */
                if (myPiece) { 
                    switch (piece.PieceType)
                    {
                        case PieceType.Pawn:
                            //reward += pawnReward(board, s);
                            break;
                        case PieceType.Queen:
                            reward += queenReward(board, s);
                            break;
                    }
                } else
                {
                    reward -= pieceRewards[(int)piece.PieceType];
                }
                
            }
        }
        board.UndoMove(move);
        return reward;
    }
    // So we are on a square, and need to compute how good this pawn's spot is
    int pawnReward(Board board, Square square)
    {
        int ret = 1;
        const int FLANK_FACTOR = 5;
        for(int i = -1; i < 2; i+=2)
        {
            for(int j = -1; j<2; j+=2)
            {
                try
                {
                    ret += FLANK_FACTOR * (board.GetPiece(new Square(square.File + i, square.Rank + j)).IsPawn ? 1 : 0);
                }
                catch(Exception e)
                {

                }
            }
        }
        int rank = relativeRank(square.Rank);
        ret += rank == 8 ? rank * 2 : rank; 
        return ret;
    }

    int queenReward(Board board, Square square)
    {
        int ret = 200;
        return ret;
    }

    int relativeRank(int rank)
    {
        bool isWhite = true;
        return isWhite ? rank : 8 - rank;
    }

    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}