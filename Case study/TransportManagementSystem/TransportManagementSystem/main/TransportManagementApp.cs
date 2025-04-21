using TransportManagementSystem.dao;
using entity;
using myexceptions;
using System;

namespace main
{
    public class TransportManagementApp
    {
        static void Main(string[] args)
        {
            ITransportManagementService service = new TransportManagementService();

            
            TMhelper tmhelper = new TMhelper(service);

            while (true)
            {
                Console.WriteLine("\n--- Transport Management System ---");
                Console.WriteLine("1. Add Vehicle");
                Console.WriteLine("2. Update Vehicle");
                Console.WriteLine("3. Delete Vehicle");
                Console.WriteLine("4. Schedule Trip");
                Console.WriteLine("5. Cancel Trip");
                Console.WriteLine("6. Book Trip");
                Console.WriteLine("7. Cancel Booking");
                Console.WriteLine("8. Allocate Driver");
                Console.WriteLine("9. Deallocate Driver");
                Console.WriteLine("10. Get Bookings By Passenger");
                Console.WriteLine("11. Get Bookings By Trip");
                Console.WriteLine("12. Get Available Drivers");
                Console.WriteLine("13. View All Routes");
                Console.WriteLine("14. View All Passangers");
                Console.WriteLine("15. Exit");
                Console.WriteLine("Select an option:");

                string option = Console.ReadLine();

                try
                {
                    switch (option)
                    {
                        case "1":
                            tmhelper.AddVehicle();
                            break;

                        case "2":
                            tmhelper.UpdateVehicle();
                            break;

                        case "3":
                            tmhelper.DeleteVehicle();
                            break;

                        case "4":
                            tmhelper.ScheduleTrip();
                            break;

                        case "5":
                            tmhelper.CancelTrip();
                            break;

                        case "6":
                            tmhelper.BookTrip();
                            break;

                        case "7":
                            tmhelper.CancelBooking();
                            break;

                        case "8":
                            tmhelper.AllocateDriver();
                            break;

                        case "9":
                            tmhelper.DeallocateDriver();
                            break;

                        case "10":
                            tmhelper.ViewBookingsByPassenger();
                            break;

                        case "11":
                            tmhelper.ViewBookingsByTrip();
                            break;

                        case "12":
                            tmhelper.ViewAvailableDrivers();
                            break;
                       
                        case "13":
                            tmhelper.viewAllRoutes();
                            break;
                        case "14":
                            tmhelper.ViewAllPassengers();
                            break;
                        case "15":
                            Console.WriteLine("Exiting the system.");
                            return;

                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
