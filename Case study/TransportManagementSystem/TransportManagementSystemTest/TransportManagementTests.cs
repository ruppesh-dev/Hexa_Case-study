using NUnit.Framework;
using System;
using TransportManagementSystem.dao;
using entity;
using myexceptions;
using Microsoft.Data.SqlClient;

namespace TransportManagementSystem.tests
{
    [TestFixture]
    public class TransportManagementServiceTests
    {
        private ITransportManagementService _service;

        [SetUp]
        public void Setup()
        {
            _service = new TransportManagementService();
        }

       
        [Test]
        public void AllocateDriver_ShouldReturnTrue_WhenTripIsScheduledAndDriverIsAvailable()
        {
            int validTripId = 1;
            int validDriverId = 1;

            string connectionString = "Server=localhost;Database=TransportManagementDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Reset trip to Scheduled and no driver
                string resetTrip = @"UPDATE Trips SET Status = 'Scheduled', DriverID = NULL WHERE TripID = @TripID";
                using (SqlCommand cmd = new SqlCommand(resetTrip, conn))
                {
                    cmd.Parameters.AddWithValue("@TripID", validTripId);
                    cmd.ExecuteNonQuery();
                }

                // Unassign driver from any trips
                string unassignDriver = @"UPDATE Trips SET DriverID = NULL WHERE DriverID = @DriverID";
                using (SqlCommand cmd = new SqlCommand(unassignDriver, conn))
                {
                    cmd.Parameters.AddWithValue("@DriverID", validDriverId);
                    cmd.ExecuteNonQuery();
                }

                // Make driver available
                string resetDriver = @"UPDATE Drivers SET Status = 'Available' WHERE DriverID = @DriverID";
                using (SqlCommand cmd = new SqlCommand(resetDriver, conn))
                {
                    cmd.Parameters.AddWithValue("@DriverID", validDriverId);
                    cmd.ExecuteNonQuery();
                }
            }

            bool result = _service.AllocateDriver(validTripId, validDriverId);
            Assert.IsTrue(result, "Driver should be allocated to the trip.");
        }

       
        [Test]
        public void AddVehicle_ShouldReturnTrue_WhenVehicleIsValid()
        {
            var vehicle = new Vehicle
            {
                Model = "Test model",
                Capacity = 50,
                Type = "Bus",
                Status = "Available"
            };

            bool result = _service.AddVehicle(vehicle);
            Assert.IsTrue(result, "Vehicle should be added.");
        }

        
        [Test]
        public void BookTrip_ShouldReturnTrue_WhenTripAndPassengerExist()
        {
            int validTripId = 1;
            int validPassengerId = 1;
            DateTime bookingDate = DateTime.Now;

            Booking booking = new Booking(validPassengerId, validTripId, bookingDate);

            bool result = _service.BookTrip(booking);
            Assert.IsTrue(result, "Trip should be booked.");
        }

        [Test]
        public void UpdateVehicle_ShouldThrowVehicleNotFoundException_WhenVehicleDoesNotExist()
        {
            var vehicle = new Vehicle
            {
                VehicleID = -999, 
                Model = "Invalid",
                Capacity = 0,
                Type = "None",
                Status = "Unavailable"
            };

            Assert.Throws<VehicleNotFoundException>(() => _service.UpdateVehicle(vehicle));
        }

      
        [Test]
        public void CancelBooking_ShouldThrowBookingNotFoundException_WhenBookingDoesNotExist()
        {
            int invalidBookingId = -999; 
            Assert.Throws<BookingNotFoundException>(() => _service.CancelBooking(invalidBookingId));
        }
    }
}
