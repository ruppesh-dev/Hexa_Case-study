using entity;
using Microsoft.Data.SqlClient;
using myexceptions;

namespace TransportManagementSystem.dao
{
    public class TransportManagementService : ITransportManagementService
    {
        private readonly string _connectionString = "Server=localhost;Database=TransportManagementDB;Integrated Security=True;TrustServerCertificate=True";

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public bool AddVehicle(Vehicle vehicle)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    string query = "INSERT INTO Vehicles (Model, Capacity, Type, Status) VALUES (@Model, @Capacity, @Type, @Status)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Model", vehicle.Model);
                    cmd.Parameters.AddWithValue("@Capacity", vehicle.Capacity);
                    cmd.Parameters.AddWithValue("@Type", vehicle.Type);
                    cmd.Parameters.AddWithValue("@Status", vehicle.Status);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding vehicle: {ex.Message}");
                return false;
            }
        }

        public bool UpdateVehicle(Vehicle vehicle)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    string query = "UPDATE Vehicles SET Model = @Model, Capacity = @Capacity, Type = @Type, Status = @Status WHERE VehicleID = @VehicleID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Model", vehicle.Model);
                    cmd.Parameters.AddWithValue("@Capacity", vehicle.Capacity);
                    cmd.Parameters.AddWithValue("@Type", vehicle.Type);
                    cmd.Parameters.AddWithValue("@Status", vehicle.Status);
                    cmd.Parameters.AddWithValue("@VehicleID", vehicle.VehicleID);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new myexceptions.VehicleNotFoundException($"Vehicle with ID {vehicle.VehicleID} not found.");

                    return true;
                }
            }
            catch (VehicleNotFoundException) { throw; }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating vehicle: {ex.Message}");
                return false;
            }
        }


        public bool DeleteVehicle(Vehicle vehicle)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    string query = "DELETE FROM Vehicles WHERE VehicleID = @VehicleID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@VehicleID", vehicle.VehicleID);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting vehicle: {ex.Message}");
                return false;
            }
        }

        public bool ScheduleTrip(Trip trip)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO Trips (VehicleID, RouteID, DepartureDate, ArrivalDate, Status)
                         VALUES (@VehicleID, @RouteID, @DepartureDate, @ArrivalDate, 'Scheduled')";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", trip.VehicleID);
                    cmd.Parameters.AddWithValue("@RouteID", trip.RouteID);
                    cmd.Parameters.AddWithValue("@DepartureDate", trip.DepartureDate);
                    cmd.Parameters.AddWithValue("@ArrivalDate", trip.ArrivalDate);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


        public bool CancelTrip(int tripId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string deleteTripQuery = @"DELETE FROM Trips WHERE TripID = @TripID";
                using (SqlCommand cmd = new SqlCommand(deleteTripQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TripID", tripId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }




        public bool BookTrip(Booking booking)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    string query = "INSERT INTO Bookings (TripID, PassengerID, BookingDate, Status) VALUES (@TripID, @PassengerID, @BookingDate, 'Confirmed')";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@TripID", booking.TripID);
                    cmd.Parameters.AddWithValue("@PassengerID", booking.PassengerID);
                    cmd.Parameters.AddWithValue("@BookingDate", booking.BookingDate);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error booking trip: {ex.Message}");
                return false;
            }
        }

        public bool CancelBooking(int bookingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Check if the booking exists
                string checkBookingQuery = @"SELECT COUNT(*) FROM Bookings WHERE BookingID = @BookingID";
                using (SqlCommand checkCmd = new SqlCommand(checkBookingQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@BookingID", bookingId);
                    int bookingCount = (int)checkCmd.ExecuteScalar();

                    if (bookingCount == 0)
                    {
            
                        throw new BookingNotFoundException("Booking ID not found.");
                    }
                }

                string updateBookingQuery = @"UPDATE Bookings SET Status = 'Cancelled' WHERE BookingID = @BookingID";
                using (SqlCommand cmd = new SqlCommand(updateBookingQuery, conn))

                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }




        public bool AllocateDriver(int tripId, int driverId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

               
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    //  trip exists, is scheduled, and has no driver
                    string checkTripQuery = @"
                SELECT COUNT(*) FROM Trips 
                WHERE TripID = @TripID 
                  AND LTRIM(RTRIM(Status)) = 'Scheduled' 
                  AND DriverID IS NULL";

                    using (SqlCommand checkTripCmd = new SqlCommand(checkTripQuery, conn, transaction))
                    {
                        checkTripCmd.Parameters.AddWithValue("@TripID", tripId);
                        int tripCount = (int)checkTripCmd.ExecuteScalar();

                        if (tripCount == 0)
                        {
                            Console.WriteLine("Trip is either not scheduled or already has a driver.");
                            transaction.Rollback();
                            return false;
                        }
                    }

                    //  driver is available
                    string checkDriverQuery = @"
                SELECT COUNT(*) FROM Drivers 
                WHERE DriverID = @DriverID 
                  AND LTRIM(RTRIM(Status)) = 'Available'";

                    using (SqlCommand checkDriverCmd = new SqlCommand(checkDriverQuery, conn, transaction))
                    {
                        checkDriverCmd.Parameters.AddWithValue("@DriverID", driverId);
                        int driverCount = (int)checkDriverCmd.ExecuteScalar();

                        if (driverCount == 0)
                        {
                            Console.WriteLine("Driver is not available.");
                            transaction.Rollback();
                            return false;
                        }
                    }

                    // Update the trip to assign the driver and mark as 'In Progress'
                    string updateTripQuery = @"
                UPDATE Trips 
                SET DriverID = @DriverID, Status = 'In Progress' 
                WHERE TripID = @TripID";

                    using (SqlCommand updateTripCmd = new SqlCommand(updateTripQuery, conn, transaction))
                    {
                        updateTripCmd.Parameters.AddWithValue("@DriverID", driverId);
                        updateTripCmd.Parameters.AddWithValue("@TripID", tripId);
                        updateTripCmd.ExecuteNonQuery();
                    }

                    // Update the driver's status to 'On Trip'
                    string updateDriverQuery = @"
                UPDATE Drivers 
                SET Status = 'On Trip' 
                WHERE DriverID = @DriverID";

                    using (SqlCommand updateDriverCmd = new SqlCommand(updateDriverQuery, conn, transaction))
                    {
                        updateDriverCmd.Parameters.AddWithValue("@DriverID", driverId);
                        updateDriverCmd.ExecuteNonQuery();
                    }

                   
                    transaction.Commit();
                    Console.WriteLine("Driver successfully allocated.");
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Exception during driver allocation: " + ex.Message);
                    return false;
                }
            }
        }





        public bool DeallocateDriver(int tripId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Get the driver ID assigned to the trip
                    string getDriverQuery = @"
                SELECT DriverID FROM Trips 
                WHERE TripID = @TripID AND DriverID IS NOT NULL";

                    int? driverId = null;

                    using (SqlCommand getDriverCmd = new SqlCommand(getDriverQuery, conn, transaction))
                    {
                        getDriverCmd.Parameters.AddWithValue("@TripID", tripId);
                        object result = getDriverCmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            transaction.Rollback();
                            return false;
                        }
                        driverId = Convert.ToInt32(result);
                    }

                    //  Remove driver from trip
                    string updateTrip = @"
                UPDATE Trips 
                SET DriverID = NULL, Status = 'Scheduled' 
                WHERE TripID = @TripID";

                    using (SqlCommand updateTripCmd = new SqlCommand(updateTrip, conn, transaction))
                    {
                        updateTripCmd.Parameters.AddWithValue("@TripID", tripId);
                        updateTripCmd.ExecuteNonQuery();
                    }

                    //  Update driver status to 'Available'
                    string updateDriver = @"
                UPDATE Drivers 
                SET Status = 'Available' 
                WHERE DriverID = @DriverID";

                    using (SqlCommand updateDriverCmd = new SqlCommand(updateDriver, conn, transaction))
                    {
                        updateDriverCmd.Parameters.AddWithValue("@DriverID", driverId);
                        updateDriverCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public List<Booking> GetBookingsByPassenger(Passenger passenger)
        {
            List<Booking> bookings = new List<Booking>();

            try
            {
                using (var connection = GetConnection())
                {
                    string query = "SELECT * FROM Bookings WHERE PassengerID = @PassengerID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@PassengerID", passenger.PassengerID);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No bookings found for this passenger.");
                        return bookings;
                    }

                    while (reader.Read())
                    {
                        Booking booking = new Booking
                        {
                            BookingID = reader.GetInt32(0),
                            TripID = reader.GetInt32(1),
                            PassengerID = reader.GetInt32(2),
                            BookingDate = reader.GetDateTime(3),
                            Status = reader.GetString(4)
                        };
                        bookings.Add(booking);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting bookings by passenger: {ex.Message}");
            }

            return bookings;
        }
        public List<Booking> GetBookingsByTrip(Trip trip)
        {
            List<Booking> bookings = new List<Booking>();

            try
            {
                using (var connection = GetConnection())
                {
                    string query = "SELECT * FROM Bookings WHERE TripID = @TripID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@TripID", trip.TripID);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No bookings found for the given trip.");
                        return bookings; 
                    }

                    while (reader.Read())
                    {
                        Booking booking = new Booking
                        {
                            BookingID = Convert.ToInt32(reader["BookingID"]),
                            TripID = Convert.ToInt32(reader["TripID"]),
                            PassengerID = Convert.ToInt32(reader["PassengerID"]),
                            BookingDate = Convert.ToDateTime(reader["BookingDate"]),
                            Status = reader["Status"].ToString()
                        };
                        bookings.Add(booking);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting bookings by trip: {ex.Message}");
            }

            return bookings;
        }
        public List<Driver> GetAvailableDrivers()
        {
            List<Driver> availableDrivers = new List<Driver>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT DriverID, FirstName, LastName, Age, Gender, PhoneNumber, LicenseNumber, Status FROM Drivers WHERE Status = 'Available'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No available drivers found.");
                            return availableDrivers; 
                        }

                        while (reader.Read())
                        {
                            Driver driver = new Driver
                            {
                                DriverID = reader.GetInt32(0),
                                Name = reader.GetString(1) + " " + reader.GetString(2),
                                Age = reader.GetInt32(3),
                                Gender = reader.GetString(4),
                                PhoneNumber = reader.GetString(5),
                                LicenseNumber = reader.GetString(6),
                                Status = reader.GetString(7)
                            };

                            availableDrivers.Add(driver);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting available drivers: {ex.Message}");
            }

            return availableDrivers;
        }


        public void GetAllRoutes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT RouteID, StartDestination, EndDestination, Distance FROM Routes";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No routes found.");
                        return;
                    }

                    Console.WriteLine("\n--- Route List ---");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Route ID: {reader.GetInt32(0)}");
                        Console.WriteLine($"Start Destination: {reader.GetString(1)}");
                        Console.WriteLine($"End Destination: {reader.GetString(2)}");
                        Console.WriteLine($"Distance: {reader.GetDecimal(3)} km");
                        Console.WriteLine("-----------------------------");
                    }
                }
            }
        }

        public void GetAllPassengers()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT PassengerID, FirstName, Gender, Age, Email, PhoneNumber FROM Passengers";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No passengers found.");
                        return;
                    }

                    Console.WriteLine("\n--- Passenger List ---");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Gender: {reader.GetString(2)}, Age: {reader.GetInt32(3)}, Email: {reader.GetString(4)}, Phone: {reader.GetString(5)}");
                        Console.WriteLine("-----------------------------");
                    }
                }
            }
        }



    }
}

