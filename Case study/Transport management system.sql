CREATE DATABASE TransportManagementDB;
USE TransportManagementDB;

CREATE TABLE Vehicles (
    VehicleID INT PRIMARY KEY IDENTITY(1,1),
    Model VARCHAR(255) NOT NULL,
    Capacity DECIMAL(10, 2) NOT NULL,
    Type VARCHAR(50) NOT NULL CHECK (Type IN ('Truck', 'Van', 'Bus')),
    Status VARCHAR(50) NOT NULL CHECK (Status IN ('Available', 'On Trip', 'Maintenance'))
);

CREATE TABLE Routes (
    RouteID INT PRIMARY KEY IDENTITY(1,1),
    StartDestination VARCHAR(255) NOT NULL,
    EndDestination VARCHAR(255) NOT NULL,
    Distance DECIMAL(10, 2) NOT NULL
);

CREATE TABLE Trips (
    TripID INT PRIMARY KEY IDENTITY(1,1),
    VehicleID INT,
    RouteID INT,
    DepartureDate DATETIME NOT NULL,
    ArrivalDate DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL CHECK (Status IN ('Scheduled', 'In Progress', 'Completed', 'Cancelled')),
    TripType VARCHAR(50) DEFAULT 'Freight' CHECK (TripType IN ('Freight', 'Passenger')),
    MaxPassengers INT,
    FOREIGN KEY (VehicleID) REFERENCES Vehicles(VehicleID) ON DELETE CASCADE,
    FOREIGN KEY (RouteID) REFERENCES Routes(RouteID) ON DELETE CASCADE
);

CREATE TABLE Passengers (
    PassengerID INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(255) NOT NULL,
    Gender VARCHAR(50) CHECK (Gender IN ('Male', 'Female', 'Other')),
    Age INT CHECK (Age > 0),
    Email VARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(50) NOT NULL
);

CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    TripID INT,
    PassengerID INT,
    BookingDate DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL CHECK (Status IN ('Confirmed', 'Cancelled', 'Completed')),
    FOREIGN KEY (TripID) REFERENCES Trips(TripID) ON DELETE CASCADE,
    FOREIGN KEY (PassengerID) REFERENCES Passengers(PassengerID) ON DELETE CASCADE
);
CREATE TABLE Drivers (
    DriverID INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    Age INT NOT NULL CHECK (Age > 0),
    Gender VARCHAR(50) CHECK (Gender IN ('Male', 'Female')),
    PhoneNumber VARCHAR(50) NOT NULL,
    LicenseNumber VARCHAR(50) NOT NULL,
    Status VARCHAR(50) NOT NULL CHECK (Status IN ('Available', 'On Trip', 'Off-Duty'))
);


INSERT INTO Vehicles (Model, Capacity, Type, Status)
VALUES
('Ashok Leyland', 12000, 'Truck', 'Available'),
('Tata Ace', 1500, 'Van', 'On Trip'),
('Eicher Pro', 60, 'Bus', 'Maintenance'),
('Mahindra Bolero', 2500, 'Van', 'Available'),
('BharatBenz', 10000, 'Truck', 'On Trip'),
('Swaraj Mazda', 50, 'Bus', 'Available'),
('Force Traveller', 1500, 'Van', 'Maintenance'),
('Tata Starbus', 45, 'Bus', 'Available'),
('Isuzu D-Max', 1800, 'Truck', 'On Trip'),
('Maruti Eeco', 1000, 'Van', 'Available');

INSERT INTO Routes (StartDestination, EndDestination, Distance)
VALUES
('Chennai', 'Madurai', 460),
('Coimbatore', 'Tiruchirapalli', 210),
('Salem', 'Erode', 60),
('Vellore', 'Chennai', 130),
('Thanjavur', 'Kumbakonam', 40),
('Tirunelveli', 'Nagercoil', 90),
('Trichy', 'Karur', 75),
('Puducherry', 'Chennai', 150),
('Dindigul', 'Madurai', 70),
('Kanchipuram', 'Vellore', 60);

INSERT INTO Passengers (FirstName, Gender, Age, Email, PhoneNumber)
VALUES
('Arun', 'Male', 32, 'arun.kumar@example.com', '9876543210'),
('Priya', 'Female', 28, 'priya.ramesh@example.com', '9876543211'),
('Karthik', 'Male', 40, 'karthik.vijay@example.com', '9876543212'),
('Meena', 'Female', 35, 'meena.sundar@example.com', '9876543213'),
('Suresh', 'Male', 29, 'suresh.kumar@example.com', '9876543214'),
('Divya', 'Female', 25, 'divya.bala@example.com', '9876543215'),
('Muthu', 'Male', 45, 'muthu.raj@example.com', '9876543216'),
('Anitha', 'Female', 38, 'anitha.selvi@example.com', '9876543217'),
('Vikram', 'Male', 30, 'vikram.venkat@example.com', '9876543218'),
('Lakshmi', 'Female', 33, 'lakshmi.narayan@example.com', '9876543219');

INSERT INTO Trips (VehicleID, RouteID, DepartureDate, ArrivalDate, Status, TripType, MaxPassengers)
VALUES
(1, 1, '2025-04-10 08:00:00', '2025-04-10 16:00:00', 'Scheduled', 'Freight', NULL),
(2, 2, '2025-04-12 06:00:00', '2025-04-12 10:00:00', 'Scheduled', 'Passenger', 40),
(3, 3, '2025-04-15 09:00:00', '2025-04-15 11:00:00', 'Scheduled', 'Passenger', 60),
(4, 4, '2025-04-18 05:00:00', '2025-04-18 07:30:00', 'Scheduled', 'Freight', NULL),
(5, 5, '2025-04-20 11:00:00', '2025-04-20 12:30:00', 'Scheduled', 'Passenger', 50),
(6, 6, '2025-04-22 13:00:00', '2025-04-22 15:30:00', 'Scheduled', 'Passenger', 45),
(7, 7, '2025-04-25 07:00:00', '2025-04-25 08:30:00', 'Scheduled', 'Freight', NULL),
(8, 8, '2025-04-28 12:00:00', '2025-04-28 14:30:00', 'Scheduled', 'Passenger', 50),
(9, 9, '2025-04-30 10:00:00', '2025-04-30 11:30:00', 'Scheduled', 'Freight', NULL),
(10, 10, '2025-05-02 09:00:00', '2025-05-02 10:30:00', 'Scheduled', 'Passenger', 40);

INSERT INTO Bookings (TripID, PassengerID, BookingDate, Status)
VALUES
(2, 1, '2025-03-24 12:00:00', 'Confirmed'),
(5, 2, '2025-03-25 14:00:00', 'Confirmed'),
(8, 3, '2025-03-26 10:00:00', 'Cancelled'),
(3, 4, '2025-03-27 11:30:00', 'Confirmed'),
(6, 5, '2025-03-28 09:00:00', 'Completed'),
(10, 6, '2025-03-29 13:00:00', 'Confirmed'),
(1, 7, '2025-03-30 08:00:00', 'Cancelled'),
(4, 8, '2025-03-31 07:00:00', 'Confirmed'),
(7, 9, '2025-04-01 15:00:00', 'Completed'),
(9, 10, '2025-04-02 10:30:00', 'Confirmed');

INSERT INTO Drivers (FirstName, LastName, Age, Gender, PhoneNumber, LicenseNumber, Status)
VALUES
('Raj', 'Kumar', 45, 'Male', '9888776655', 'TN01-123456', 'Available'),
('Suresh', 'Ravi', 35, 'Male', '9888776644', 'TN02-654321', 'On Trip'),
('Lakshmi', 'Krishnan', 40, 'Female', '9888776633', 'TN03-112233', 'Available'),
('Muthu', 'Rajendran', 38, 'Male', '9888776622', 'TN04-998877', 'Off-Duty'),
('Arul', 'Sundaram', 50, 'Male', '9888776611', 'TN05-223344', 'Available'),
('Srinivasan', 'Venkatesan', 32, 'Male', '9888776600', 'TN06-334455', 'On Trip'),
('Meena', 'Sundar', 30, 'Female', '9888776599', 'TN07-445566', 'Off-Duty'),
('Vikram', 'Subramanian', 42, 'Male', '9888776588', 'TN08-556677', 'Available'),
('Anitha', 'Balu', 28, 'Female', '9888776577', 'TN09-667788', 'On Trip'),
('Karthik', 'Natarajan', 33, 'Male', '9888776566', 'TN10-778899', 'Available');

select * from Vehicles
select * from Routes

select * from Bookings
select * from Passengers
select * from Trips
select * from Drivers

