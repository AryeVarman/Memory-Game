using System;
using System.Collections.Generic;

namespace MemoryGame
{
    public class ComputerPlayer<T>
    {
        // private readonly eFirstOrSecond r_FirstOrSecond;
        private Dictionary<T, MemoryInformation> m_Memory;
        private List<Tuple<byte, byte>> m_UnknownLocationsOnBoard; // list for places on the board the computer dont know 
        private Board<T> m_GameBoard;
        //private bool m_MyTurn;
        private byte m_Score;

        public ComputerPlayer(Board<T> i_Board)
        {
            m_Memory = new Dictionary<T, MemoryInformation>();
            m_GameBoard = i_Board;
            m_Score = 0;

            m_UnknownLocationsOnBoard = new List<Tuple<byte, byte>> { Capacity = m_GameBoard.NumberOfColumn * m_GameBoard.NumberOfRows };

            for (byte i = 0; i < m_GameBoard.NumberOfRows; i++)
            {
                for (byte j = 0; j < m_GameBoard.NumberOfColumn; j++)
                {
                    m_UnknownLocationsOnBoard.Add(new Tuple<byte, byte>(i, j));
                }
            }
        }

        public List<Tuple<byte, byte>> UnknownLocationsOnBoard
        {
            get { return m_UnknownLocationsOnBoard; }
        }

        public byte Score
        {
            get { return m_Score; }

            set { m_Score = value; }
        }

        public Tuple<Tuple<byte, byte>, Tuple<byte, byte>> PlayComputersTurn()
        {
            bool donePlaying = false;
            Tuple<Tuple<byte, byte>, Tuple<byte, byte>> placesToRevealOnBoard = CheckIfCanUnfoldCouple(); // checking if the computer know where a pair is

            if (placesToRevealOnBoard != null) // if the computer know where a couple is: 
            {
                donePlaying = true;
            }

            if (!donePlaying) // gets in this only if the computer dont know where there is a pair
            {
                Tuple<byte, byte> firstChosenLocationOnBoard = chooseCellRandomlyFromList(); // choosing a location on the board randomly
                m_UnknownLocationsOnBoard.Remove(firstChosenLocationOnBoard); // remove location from the unknown locations list

                T sign1 = m_GameBoard.CellMatrix[firstChosenLocationOnBoard.Item1, firstChosenLocationOnBoard.Item2].CellSign; // save the locations sign

                Tuple<byte, byte> secondChosenLocationOnBoard = CheckIfKnowWhereMatchingSignIs(sign1); // check if computer know where is the matching sign is

                if (secondChosenLocationOnBoard != null) // if he know where matchin sign is: 
                {
                    placesToRevealOnBoard =
                        new Tuple<Tuple<byte, byte>, Tuple<byte, byte>>(
                            firstChosenLocationOnBoard, secondChosenLocationOnBoard); // gets the locations on the board in Tuples
                    donePlaying = true;
                }

                if (!donePlaying) // does the following if didnt found couple to reveal
                {
                    AddNewLocationOfSignToMemory(sign1, firstChosenLocationOnBoard); // add's first location to computer memory

                    placesToRevealOnBoard =
                        new Tuple<Tuple<byte, byte>, Tuple<byte, byte>>(
                            firstChosenLocationOnBoard, chooseCellRandomlyFromList()); // choosing second location on the board randomly and set Tuple of Tuples for location

                    m_UnknownLocationsOnBoard.Remove(placesToRevealOnBoard.Item2); // remove location from the unknown locations list

                    T sign2 = m_GameBoard
                        .CellMatrix[placesToRevealOnBoard.Item2.Item1, placesToRevealOnBoard.Item2.Item2].CellSign; // save the second locations sign

                    if (sign1.Equals(sign2)) // if the two signs we found match
                    {
                        m_Memory.Remove(sign1); // remove first location from memory because we found its match
                    }
                    else
                    {
                        AddNewLocationOfSignToMemory(sign2, placesToRevealOnBoard.Item2);
                    }
                }
            }

            return placesToRevealOnBoard;
        }

        // checks if the computer knows locations of pair of signs, if yes return in a form of Tuple of Tuples otherwise return null
        internal Tuple<Tuple<byte, byte>, Tuple<byte, byte>> CheckIfCanUnfoldCouple()
        {
            Tuple<Tuple<byte, byte>, Tuple<byte, byte>> locationPairOnBoard = null;

            Tuple<byte, byte> firstLocation = null;
            Tuple<byte, byte> secondLocation = null;
            bool foundPair = false;

            // go trough the memory and check if know where there is a pair
            foreach (var variable in m_Memory)
            {
                if (variable.Value.FoundCount == 2)
                {
                    firstLocation = variable.Value.FirstLocationOfSign;
                    secondLocation = variable.Value.SecondLocationOfSign;
                    foundPair = true;

                    m_Memory.Remove(variable.Key);
                    break;
                }
            }

            if (foundPair)
            {
                locationPairOnBoard = new Tuple<Tuple<byte, byte>, Tuple<byte, byte>>(firstLocation, secondLocation);
            }

            return locationPairOnBoard;
        }

        // check if the computer know where the second sign is in the board, if yes return Tuple<byte, byte> of location otherwise return null
        internal Tuple<byte, byte> CheckIfKnowWhereMatchingSignIs(T i_FirstSignFoundInTurn)
        {
            Tuple<byte, byte> matchingSignLocationOnBoard = null;

            if (m_Memory.ContainsKey(i_FirstSignFoundInTurn))
            {
                matchingSignLocationOnBoard = m_Memory[i_FirstSignFoundInTurn].FirstLocationOfSign;
                m_Memory.Remove(i_FirstSignFoundInTurn);
            }

            return matchingSignLocationOnBoard;
        }

        // add new location of sign to the computer memory
        public void AddNewLocationOfSignToMemory(T i_Sign, byte i_LocationRow, byte i_LocationColl)
        {
            Tuple<byte, byte> locationOnBored = new Tuple<byte, byte>(i_LocationRow, i_LocationColl);

            m_UnknownLocationsOnBoard.Remove(locationOnBored); // remove from unknown location list

            if (m_Memory.ContainsKey(i_Sign)) // if we know where the pair to the sign is we add the new location to the MemoryInformation box
            {
                m_Memory[i_Sign].AddMemoryLocation(locationOnBored);
            }
            else
            {
                m_Memory.Add(i_Sign, new MemoryInformation(locationOnBored));
            }
        }

        public void AddNewLocationOfSignToMemory(T i_Sign, Tuple<byte, byte> i_LocationOnBoard)
        {
            if (m_Memory.ContainsKey(i_Sign)) // if we know where the pair to the sign is we add the new location to the MemoryInformation box
            {
                if (!m_Memory[i_Sign].FirstLocationOfSign.Equals(i_LocationOnBoard))
                {
                    m_Memory[i_Sign].AddMemoryLocation(i_LocationOnBoard);
                }
            }
            else
            {
                m_Memory.Add(i_Sign, new MemoryInformation(i_LocationOnBoard));
            }
        }

        // make the computer know about the moves of the player
        public void UpDateComputerMemory(
            byte i_FirstWantedRow, byte i_FirstWantedColumn, byte i_SecondWantedRow, byte i_SecondWantedColumn, T i_Sign1, T i_Sign2)
        {
            Tuple<byte, byte> firstLocation = new Tuple<byte, byte>(i_FirstWantedRow, i_FirstWantedColumn);
            Tuple<byte, byte> secondLocation = new Tuple<byte, byte>(i_SecondWantedRow, i_SecondWantedColumn);

            if (UnknownLocationsOnBoard.Contains(firstLocation))
            {
                UnknownLocationsOnBoard.Remove(firstLocation);
            }

            if (UnknownLocationsOnBoard.Contains(secondLocation))
            {
                UnknownLocationsOnBoard.Remove(secondLocation);
            }

            if (!i_Sign1.Equals(i_Sign2))
            {
                AddNewLocationOfSignToMemory(i_Sign1, firstLocation); // need to check if location is already in memory
                AddNewLocationOfSignToMemory(i_Sign2, secondLocation);
            }
            else
            {
                RemoveSignFromMemory(i_Sign1);
            }
        }

        public void RemoveSignFromMemory(T i_Sign)
        {
            if (m_Memory.ContainsKey(i_Sign))
            {
                m_Memory.Remove(i_Sign);
            }
        }

        private Tuple<byte, byte> chooseCellRandomlyFromList()
        {
            Tuple<byte, byte> placeOnBoard;
            Random rand = new Random();

            if (m_UnknownLocationsOnBoard.Count != 0)
            {
                int indexChoseRandomly = rand.Next(m_UnknownLocationsOnBoard.Count);
                placeOnBoard = m_UnknownLocationsOnBoard[indexChoseRandomly];
            }
            else
            {
                placeOnBoard = findFirstOpenPlaceOnBoard();
            }

            return placeOnBoard;
        }

        private Tuple<byte, byte> findFirstOpenPlaceOnBoard()
        {
            Tuple<byte, byte> placeOnBoard = null;

            for (byte i = 0; i < m_GameBoard.NumberOfRows; i++)
            {
                for (byte j = 0; j < m_GameBoard.NumberOfColumn; j++)
                {
                    if (!m_GameBoard.CellMatrix[i, j].IsExposed)
                    {
                        placeOnBoard = new Tuple<byte, byte>(i, j);
                    }
                }
            }

            return placeOnBoard;
        }

        public void AddPointToScore()
        {
            m_Score++;
        }

        internal class MemoryInformation
        {
            private byte m_FoundCount;
            private Tuple<byte, byte> m_FirstLocationOfSign;
            private Tuple<byte, byte> m_SecondLocationOfSign;

            internal MemoryInformation(Tuple<byte, byte> i_FirstLocation)
            {
                m_FoundCount = 1; // MemoryInformation created after finding first appearance of new sign
                m_FirstLocationOfSign = i_FirstLocation;
                m_SecondLocationOfSign = null; // didn't found second appearance yet
            }

            internal MemoryInformation(byte i_FirstLocationRow, byte i_FirstLocationColl)
            {
                m_FoundCount = 1; // MemoryInformation created after finding first appearance of new sign

                Tuple<byte, byte> FirstLocation = new Tuple<byte, byte>(i_FirstLocationRow, i_FirstLocationColl);
                m_FirstLocationOfSign = FirstLocation; // creates new location Tuple from the input to the c'tor

                m_SecondLocationOfSign = null; // didn't found second appearance yet
            }

            internal void AddMemoryLocation(Tuple<byte, byte> i_Location)
            {
                m_FoundCount++;
                m_SecondLocationOfSign = i_Location;
            }

            public byte FoundCount
            {
                get { return m_FoundCount; }
            }

            public Tuple<byte, byte> FirstLocationOfSign
            {
                get { return m_FirstLocationOfSign; }
            }

            public Tuple<byte, byte> SecondLocationOfSign
            {
                get { return m_SecondLocationOfSign; }
            }
        }
    }
}
