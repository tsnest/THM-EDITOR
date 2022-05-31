using System;
namespace ConsoleApp1
{
    public class Flags32
    {
        private uint current_flags = 0;
        public void Add(uint flags, bool set)
        {
            if (set)
                current_flags |= flags;
            else
                current_flags &= ~flags;
        }
        public void Set(uint flags)
        {
            current_flags = flags;
        }
        public uint Get()
        {
            return current_flags;
        }
        public bool Test(uint flag)
        {
            return (current_flags & flag) != 0;
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