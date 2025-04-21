namespace entity
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public string Model { get; set; }
        public decimal Capacity { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

       
        public Vehicle() { }

   
        public Vehicle(int vehicleID, string model, decimal capacity, string type, string status)
        {
            VehicleID = vehicleID;
            Model = model;
            Capacity = capacity;
            Type = type;
            Status = status;
        }
        public Vehicle( string model, decimal capacity, string type, string status)
        {
            
            Model = model;
            Capacity = capacity;
            Type = type;
            Status = status;
        }
    }
}


