namespace entity
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int TripID { get; set; }
        public int PassengerID { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }

       
        public Booking() { }

    
        public Booking(  int passengerID, int tripID, DateTime bookingDate)
        {
            
            TripID = tripID;
            PassengerID = passengerID;
            BookingDate = bookingDate;
           
        }
    }
}
