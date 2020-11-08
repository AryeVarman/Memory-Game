using System;

namespace MemoryGame
{
    public class Cell<T>
    {
        private readonly T r_CellSign;
        private readonly Tuple<byte, byte> r_Location;
        private bool m_IsExposed;

        public Cell(T i_CellSign, Tuple<byte, byte> i_Location, bool i_IsExposed)
        {
            r_CellSign = i_CellSign;
            r_Location = i_Location;
            this.IsExposed = i_IsExposed;
        }

        public bool IsExposed
        {
            get { return m_IsExposed; }

            set { m_IsExposed = value; }
        }

        public T CellSign
        {
            get { return r_CellSign; }
        }

        public Tuple<byte, byte> Location
        {
            get { return r_Location; }
        }
    }
}