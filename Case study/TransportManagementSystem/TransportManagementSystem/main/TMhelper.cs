using entity;
using TransportManagementSystem.dao;
using System;
using System.Collections.Generic;
using myexceptions;
using Microsoft.Data.SqlClient;

public class TMhelper
{
    ITransportManagementService service;

    public TMhelper(ITransportManagementService service)
    {
        this.service = service;
    }
    private readonly string _connectionString = "Server=localhost;Database=TransportManagementDB;Integrated Security=True;TrustServerCertificate=True";

    public void AddVehicle()
    {
        Console.WriteLine("Enter Vehicle Model:");
        string model = Console.ReadLine();
        Console.WriteLine("Enter Vehicle Capacity:");
        decimal capacity = Convert.ToDecimal(Console.ReadLine());
        Console.WriteLine("Enter Vehicle Type (e.g., Truck, Bus, Van):");
        string type = Console.ReadLine();
        Console.WriteLine("Enter Vehicle Status (Available, On Trip, Maintenance):");
        string status = Console.ReadLine();

        Vehicle vehicle = new Vehicle(model, capacity, type, status);

        bool isAdded = service.AddVehicle(vehicle);
        if (isAdded)
        {
            Console.WriteLine("Vehicle added successfully.");
        }
        else
        {
            Console.WriteLine("Failed to add the vehicle.");
        }
    }


    public void UpdateVehicle()
    {
        try
        {
            Console.WriteLine("Enter Vehicle ID to update:");
            int vehicleId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter New Vehicle Model:");
            string model = Console.ReadLine();
            Console.WriteLine("Enter New Vehicle Capacity:");
            decimal capacity = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Enter New Vehicle Type:");
            string type = Console.ReadLine();
            Console.WriteLine("Enter New Vehicle Status:");
            string status = Console.ReadLine();

            Vehicle vehicle = new Vehicle(vehicleId, model, capacity, type, status);

            bool isUpdated = service.UpdateVehicle(vehicle);
            if (isUpdated)
            {
                Console.WriteLine("Vehicle updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update vehicle.");
            }
        }
        catch(BookingNotFoundException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred: " + ex.Message);
        }
    }


    public void DeleteVehicle()
    {
        Console.WriteLine("Enter Vehicle ID to delete:");
        int vehicleId = Convert.ToInt32(Console.ReadLine());
        Vehicle vehicle = new Vehicle { VehicleID = vehicleId };
        bool isDeleted = service.DeleteVehicle(vehicle);
        if (isDeleted)
        {
            Console.WriteLine("Vehicle deleted successfully.");
        }
        else
        {
            Console.WriteLine("Failed to delete vehicle.");
        }
    }


    public void ScheduleTrip()
    {
        Console.WriteLine("Enter Vehicle ID for the trip:");
        int vehicleId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter Route ID for the trip:");
        int routeId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter Departure Date (YYYY-MM-DD HH:mm):");
        string departureDateInput = Console.ReadLine();
        DateTime departureDate;
        while (!DateTime.TryParse(departureDateInput, out departureDate))
        {
            Console.WriteLine("Invalid departure date format. Please enter the date in the format YYYY-MM-DD HH:mm:");
            departureDateInput = Console.ReadLine();
        }

        Console.WriteLine("Enter Arrival Date (YYYY-MM-DD HH:mm):");
        string arrivalDateInput = Console.ReadLine();
        DateTime arrivalDate;
        while (!DateTime.TryParse(arrivalDateInput, out arrivalDate))
        {
            Console.WriteLine("Invalid arrival date format. Please enter the date in the format YYYY-MM-DD HH:mm:");
            arrivalDateInput = Console.ReadLine();
        }
        Trip trip = new Trip(vehicleId, routeId, departureDate, arrivalDate);
        bool isScheduled = service.ScheduleTrip(trip);
        if (isScheduled)
        {
            Console.WriteLine("Trip scheduled successfully.");
        }
        else
        {
            Console.WriteLine("Failed to schedule trip.");
        }
    }


    public void CancelTrip()
    {
        Console.WriteLine("Enter Trip ID to cancel:");
        int tripId = Convert.ToInt32(Console.ReadLine());
        bool isCancelled = service.CancelTrip(tripId);
        if (isCancelled)
        {
            Console.WriteLine("Trip canceled successfully.");
        }
        else
        {
            Console.WriteLine("Failed to cancel trip.");
        }
    }


    public void BookTrip()
    {
        Console.WriteLine("Enter Passenger ID:");
        int passengerId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter Trip ID to book:");
        int tripId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter Booking Date (YYYY-MM-DD):");
        string bookingDateInput = Console.ReadLine();
        DateTime bookingDate;

        while (!DateTime.TryParse(bookingDateInput, out bookingDate))
        {
            Console.WriteLine("Invalid date format. Please enter the date in the format YYYY-MM-DD:");
            bookingDateInput = Console.ReadLine();
        }

        // Create the booking with the date
        Booking booking = new Booking(passengerId, tripId, bookingDate);

        bool isBooked = service.BookTrip(booking);
        if (isBooked)
        {
            Console.WriteLine("Trip booked successfully.");
        }
        else
        {
            Console.WriteLine("Failed to book trip.");
        }
    }

   
    public void CancelBooking()
    {
        Console.WriteLine("Enter Booking ID to cancel:");
        int bookingId = Convert.ToInt32(Console.ReadLine());

        try
        {
            bool isCancelled = service.CancelBooking(bookingId);
            if (isCancelled)
            {
                Console.WriteLine("Booking cancelled successfully status updated");
            }
            else
            {
                Console.WriteLine("Failed to update booking status.");
            }
        }
        catch (BookingNotFoundException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred: " + ex.Message);
        }
    }


    
    public void AllocateDriver()
    {

        Console.Write("Enter Driver ID: ");
        int driverId = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Trip ID to allocate the driver: ");
        int tripId = Convert.ToInt32(Console.ReadLine());

        bool isAllocated = service.AllocateDriver(tripId, driverId);

        if (isAllocated)
        {
            Console.WriteLine(" Driver allocated successfully.");
        }
        else
        {
            Console.WriteLine(" Failed to allocate driver. Either the driver is unavailable or the trip is invalid.");
        }
    }
       
    


   
    public void DeallocateDriver()
    {
        Console.WriteLine("Enter Trip ID to deallocate the driver:");
        int tripId = Convert.ToInt32(Console.ReadLine());

        bool isDeallocated = service.DeallocateDriver(tripId);
        if (isDeallocated)
        {
            Console.WriteLine("Driver deallocated successfully.");
        }
        else
        {
            Console.WriteLine("Failed to deallocate driver.");
        }
    }

    public void ViewBookingsByPassenger()
    {
        Console.WriteLine("Enter Passenger ID:");
        string passengerInput = Console.ReadLine();

        if (int.TryParse(passengerInput, out int passengerId))
        {
            Passenger passenger = new Passenger { PassengerID = passengerId };

            List<Booking> bookings = service.GetBookingsByPassenger(passenger);

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found for this passenger.");
            }
            else
            {
                foreach (var booking in bookings)
                {
                    Console.WriteLine($"Booking ID: {booking.BookingID}");
                    Console.WriteLine($"Trip ID: {booking.TripID}");
                    Console.WriteLine($"Booking Date: {booking.BookingDate}");
                    Console.WriteLine($"Status: {booking.Status}");
                    Console.WriteLine("-----------------------------");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid Passenger ID.");
        }
    }

    public void ViewBookingsByTrip()
    {
        Console.WriteLine("Enter Trip ID:");
        string tripInput = Console.ReadLine();

        if (int.TryParse(tripInput, out int tripId))
        {
            Trip trip = new Trip { TripID = tripId };

            List<Booking> bookings = service.GetBookingsByTrip(trip);

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found for this trip.");
            }
            else
            {
                foreach (var booking in bookings)
                {
                    Console.WriteLine($"Booking ID: {booking.BookingID}");
                    Console.WriteLine($"Passenger ID: {booking.PassengerID}");
                    Console.WriteLine($"Booking Date: {booking.BookingDate}");
                    Console.WriteLine($"Status: {booking.Status}");
                    Console.WriteLine("-----------------------------");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid Trip ID.");
        }
    }

    public void ViewAvailableDrivers()
    {
     
        List<Driver> availableDrivers = service.GetAvailableDrivers();

      
        if (availableDrivers.Count == 0)
        {
            Console.WriteLine("No available drivers found.");
        }
        else
        {
            foreach (var driver in availableDrivers)
            {
                Console.WriteLine($"Driver ID: {driver.DriverID}");
                Console.WriteLine($"Name: {driver.Name}");
                Console.WriteLine($"Phone: {driver.PhoneNumber}");
                Console.WriteLine($"License: {driver.LicenseNumber}");
                Console.WriteLine($"Status: {driver.Status}");
                Console.WriteLine("-----------------------------");
            }
        }
    }

    public void viewAllRoutes()
    {
        service.GetAllRoutes();  
    }

   
    public void ViewAllPassengers()
    {
        service.GetAllPassengers();
    }

}



