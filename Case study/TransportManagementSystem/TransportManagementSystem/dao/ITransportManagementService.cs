using entity;
using System;
using System.Collections.Generic;

namespace TransportManagementSystem.dao
{
    public interface ITransportManagementService
    {   // Asked method in case study
        bool AddVehicle(Vehicle vehicle);
        bool UpdateVehicle(Vehicle vehicle);
        bool DeleteVehicle(Vehicle vehicle);
        bool ScheduleTrip(Trip trip);
        bool CancelTrip(int tripId);
        bool BookTrip(Booking booking);
        bool CancelBooking(int bookingId);
        bool AllocateDriver(int tripId, int driverId);
        bool DeallocateDriver(int tripId);
        public List<Booking> GetBookingsByTrip(Trip trip);
        public List<Booking> GetBookingsByPassenger(Passenger passenger);
        public List<Driver> GetAvailableDrivers();
        // Additional methods for my reference
        public void GetAllRoutes();
        public void GetAllPassengers();
    }
}
