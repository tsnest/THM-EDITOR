using System;
namespace ConsoleApp1
{
    public class Flags32
    {
        private uint current_flags = 0;
        public void Set(uint flags, bool set)
        {
            if (set)
                current_flags |= flags;
            else
                current_flags &= ~flags;
        }
        public uint Get()
        {
            return current_flags;
        }

        public Flags32() { }
    }
    public class CBase
    {
        public CBase() { }

        public void R_ASSERT(bool expr, string text)
        {
            if (!expr)
                throw new InvalidOperationException(text);
        }
    }
}