using System;

namespace myexceptions
{
    public class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException() : base() { }

        public VehicleNotFoundException(string message) : base(message) { }

 }
    }
