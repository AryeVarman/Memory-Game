using System;
using System.Collections.Generic;

namespace MemoryGame
{
    public class Board<T>
    {
        private const byte k_MinSizeForBoard = 4;
        private const byte k_MaxSizeForBoard = 6;
        private readonly Cell<T>[,] r_CellMatrix;
        private readonly byte r_NumberOfRows;
        private readonly byte r_NumberOfColumn;
        private byte m_PairsRemainingToFind; // how many pair did not been discovered

        public Board(byte i_NumberOfRows, byte i_NumberOfColumn, List<T> i_CellSignList)
        {
            r_NumberOfRows = i_NumberOfRows;
            r_NumberOfColumn = i_NumberOfColumn;

            r_CellMatrix = new Cell<T>[r_NumberOfRows, r_NumberOfColumn];

            m_PairsRemainingToFind = (byte)((r_NumberOfRows * r_NumberOfColumn) / 2);
            shuffleList(i_CellSignList); // shuffle the list to make the board random

            bool v_Exposed = true;
            byte i = 0, j = 0;

            // initializing the board with unexposed cells
            foreach (T variable in i_CellSignList)
            {
                if (j < r_NumberOfColumn)
                {
                    r_CellMatrix[i, j] = new Cell<T>(variable, new Tuple<byte, byte>(i, j), !v_Exposed);
                }
                else
                {
                    i++;
                    j = 0;
                    r_CellMatrix[i, j] = new Cell<T>(variable, new Tuple<byte, byte>(i, j), !v_Exposed);
                }

                j++;
            }
        }

        public static bool IsLegalSizeForBoard(byte i_RowNumber, byte i_CollNumber)
        {
            return i_RowNumber <= k_MaxSizeForBoard && i_RowNumber >= k_MinSizeForBoard &&
                    i_CollNumber <= k_MaxSizeForBoard && i_CollNumber >= k_MinSizeForBoard;
        }

        public static bool IsEvenNumberOfCells(byte i_RowNumber, byte i_CollNumber)
        {
            return (((i_CollNumber * i_RowNumber) % 2) == 0);
        }

        public byte NumberOfRows
        {
            get { return r_NumberOfRows; }
        }

        public byte PairsRemainingToFind
        {
            get { return m_PairsRemainingToFind; }
        }

        public byte NumberOfColumn
        {
            get { return r_NumberOfColumn; }
        }

        public void PairFound()
        {
            if (PairsRemainingToFind > 0)
            {
                m_PairsRemainingToFind--;
            }
        }

        public Cell<T>[,] CellMatrix
        {
            get { return r_CellMatrix; }
        }

        // shuffle the list to make the board random
        private static void shuffleList(List<T> o_list)
        {
            const int k_NumberOfShuffles = 100;
            Random rand = new Random();
            int numberOfShuffles = k_NumberOfShuffles;

            while (numberOfShuffles > 0)
            {
                int firstRandomPlace = rand.Next(o_list.Count);
                int secondRandomPlace = rand.Next(o_list.Count);

                T value = o_list[firstRandomPlace];
                o_list[firstRandomPlace] = o_list[secondRandomPlace];
                o_list[secondRandomPlace] = value;

                numberOfShuffles--;
            }
        }

        public void IsCellAvailableForPicking(
            byte i_RowWanted, byte i_CollWanted, out bool i_IsCellOutOfRange, out bool i_IsCellAlreadyExpose)
        {
            i_IsCellOutOfRange = (i_RowWanted < 0 || i_RowWanted > r_NumberOfRows - 1) || (i_CollWanted < 0 || i_CollWanted > r_NumberOfColumn - 1);

            if (!i_IsCellOutOfRange)
            {
                i_IsCellAlreadyExpose = r_CellMatrix[i_RowWanted, i_CollWanted].IsExposed;
            }
            else
            {
                i_IsCellAlreadyExpose = false;
            }
        }
    }
}