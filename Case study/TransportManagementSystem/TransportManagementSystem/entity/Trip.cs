namespace entity
{
    public class Trip
    {
        public int TripID { get; set; }
        public int VehicleID { get; set; }
        public int RouteID { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
    
       
        public Trip() { }

  
        public Trip( int vehicleID, int routeID, DateTime departureDate, DateTime arrivalDate)
        {
            
            VehicleID = vehicleID;
            RouteID = routeID;
            DepartureDate = departureDate;
            ArrivalDate = arrivalDate;
           
        }
        
    }
}
