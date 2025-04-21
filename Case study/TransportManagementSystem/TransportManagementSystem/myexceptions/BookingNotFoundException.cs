using System;

namespace myexceptions
{
    public class BookingNotFoundException : Exception
    {
        public BookingNotFoundException() : base() { }

        public BookingNotFoundException(string message) : base(message) { }

    }
}
