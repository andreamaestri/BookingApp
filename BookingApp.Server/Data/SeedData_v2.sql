-- ==========================================================================
-- Mock Data Script for Cornwall Holiday Bookings (Updated Schema)
-- Target Database: SQL Server
-- Database Name: BookingApp
-- ==========================================================================

-- Set Database Context
USE BookingApp;
GO

-- ==========================================================================
-- Clear Existing Data
-- ==========================================================================
PRINT 'Clearing existing data...';
-- Delete in an order that respects potential foreign key dependencies
DELETE FROM Reviews;
DELETE FROM Bookings;
DELETE FROM AvailabilityPeriods;
DELETE FROM Guests;
DELETE FROM Accommodations;
GO

-- ==========================================================================
-- Reset Identity Columns
-- ==========================================================================
PRINT 'Resetting identity columns...';
DBCC CHECKIDENT ('Reviews', RESEED, 0);
DBCC CHECKIDENT ('Bookings', RESEED, 0);
DBCC CHECKIDENT ('AvailabilityPeriods', RESEED, 0);
DBCC CHECKIDENT ('Guests', RESEED, 0);
DBCC CHECKIDENT ('Accommodations', RESEED, 0);
GO

-- ==========================================================================
-- Insert Accommodations (10 Properties)
-- ==========================================================================
PRINT 'Inserting Accommodations...';
INSERT INTO Accommodations (
    Title, Name, Description, Type, AddressLine1, Town, PostCode, Latitude, Longitude,
    Bedrooms, Bathrooms, MaxOccupancy, HasSeaView, Amenities, ImageUrls,
    BasePricePerNight, OwnerId, ReviewCount, CreatedDate, LastModifiedDate
)
VALUES
-- Type: 1=Cottage/House, 2=Apartment/Loft/Studio, 3=Lodge/Chalet/Retreat
-- Amenities/ImageUrls are JSON strings
('Charming Sea View Cottage', 'Sea View Cottage', 'Charming cottage with stunning sea views.', 1, '1 Cliff Road', 'St Ives', 'TR26 1AB', 50.2108, -5.4806, 2, 1, 4, 1, '["Wifi", "Parking", "Sea view", "Washing machine"]', '["stives_cottage1.jpg", "stives_cottage2.jpg"]', 135.71, 1, 0, GETDATE(), GETDATE()),
('Modern Beach Apartment', 'Fistral Beach Apartment', 'Modern apartment near Fistral Beach.', 2, '52 Beach Road', 'Newquay', 'TR7 1DY', 50.4172, -5.0747, 2, 1, 4, 1, '["Wifi", "Balcony", "Beach access", "Dishwasher"]', '["newquay_apt1.jpg", "newquay_apt2.jpg"]', 144.00, 1, 0, GETDATE(), GETDATE()),
('Historic Harbour House', 'Harbour House', 'Characterful house overlooking the harbour.', 1, '3 Harbour Street', 'Mousehole', 'TR19 6PL', 50.0833, -5.5333, 3, 2, 6, 1, '["Wifi", "Parking", "Garden", "Pet friendly"]', '["mousehole_house1.jpg", "mousehole_house2.jpg"]', 136.00, 1, 0, GETDATE(), GETDATE()),
('Cosy Fisherman''s Loft', 'Fisherman''s Loft', 'Cosy loft conversion in a historic fishing village.', 2, '12 Polperro Way', 'Polperro', 'PL13 2RJ', 50.3305, -4.5182, 1, 1, 2, 1, '["Wifi", "Sea view"]', '["polperro_loft1.jpg", "polperro_loft2.jpg"]', 127.14, 1, 0, GETDATE(), GETDATE()),
('Quiet Padstow Hideaway', 'Padstow Hideaway', 'A quiet retreat near Padstow harbour.', 1, '7 Harbour Road', 'Padstow', 'PL28 8BY', 50.5432, -4.9360, 2, 2, 4, 0, '["Wifi", "Parking", "Patio", "BBQ"]', '["padstow_hideaway1.jpg", "padstow_hideaway2.jpg"]', 137.50, 1, 0, GETDATE(), GETDATE()),
('Relaxing Coastal Retreat', 'Coastal Retreat', 'Relaxing property on the Penzance coast.', 3, '25 Promenade', 'Penzance', 'TR18 4HG', 50.1183, -5.5378, 3, 2, 6, 1, '["Wifi", "Garden", "Sea view", "Parking"]', '["penzance_retreat1.jpg", "penzance_retreat2.jpg"]', 117.14, 1, 0, GETDATE(), GETDATE()),
('Stylish Marina Apartment', 'Falmouth Marina View', 'Apartment overlooking Falmouth Marina.', 2, '45 Marina Court', 'Falmouth', 'TR11 3XY', 50.1531, -5.0644, 2, 1, 4, 1, '["Wifi", "Balcony", "Marina view", "Parking"]', '["falmouth_marina1.jpg", "falmouth_marina2.jpg"]', 157.14, 1, 0, GETDATE(), GETDATE()),
('Central Truro Apartment', 'Truro City Apartment', 'Convenient apartment in the heart of Truro.', 2, '12 Cathedral Street', 'Truro', 'TR1 2QR', 50.2632, -5.0510, 1, 1, 2, 0, '["Wifi", "City center location"]', '["truro_apt1.jpg", "truro_apt2.jpg"]', 116.67, 1, 0, GETDATE(), GETDATE()),
('Bude Surfer''s Lodge', 'Bude Surfer''s Lodge', 'Lodge ideal for surfers near Bude beaches.', 3, '7 Surf Lane', 'Bude', 'EX23 8SD', 50.8295, -4.5515, 3, 2, 6, 1, '["Wifi", "Beach access", "Surfboard storage", "Parking"]', '["bude_lodge1.jpg", "bude_lodge2.jpg"]', 121.43, 1, 0, GETDATE(), GETDATE()),
('Picturesque Valley Cottage', 'Looe Valley Cottage', 'Picturesque cottage in the Looe Valley.', 1, '5 Valley Road', 'Looe', 'PL13 1FA', 50.3514, -4.4544, 2, 1, 4, 0, '["Wifi", "Garden", "Valley view", "Pet friendly"]', '["looe_cottage1.jpg", "looe_cottage2.jpg"]', 120.00, 1, 0, GETDATE(), GETDATE());
GO

-- ==========================================================================
-- Insert Guests (15 Guests)
-- ==========================================================================
PRINT 'Inserting Guests...';
INSERT INTO Guests (Name, Email, PhoneNumber)
VALUES
('Alice Johnson', 'alice.j@email.com', '07700900001'),
('Bob Williams', 'bob.w@email.com', '07700900002'),
('Charlie Brown', 'charlie.b@email.com', '07700900003'),
('Diana Davis', 'diana.d@email.com', '07700900004'),
('Ethan Miller', 'ethan.m@email.com', '07700900005'),
('Fiona Wilson', 'fiona.w@email.com', '07700900006'),
('George Moore', 'george.m@email.com', '07700900007'),
('Hannah Taylor', 'hannah.t@email.com', '07700900008'),
('Ian Anderson', 'ian.a@email.com', '07700900009'),
('Jessica Thomas', 'jessica.t@email.com', '07700900010'),
('Kevin Jackson', 'kevin.j@email.com', '07700900011'),
('Laura White', 'laura.w@email.com', '07700900012'),
('Michael Harris', 'michael.h@email.com', '07700900013'),
('Nora Martin', 'nora.m@email.com', '07700900014'),
('Oliver Thompson', 'oliver.t@email.com', '07700900015');
GO

-- ==========================================================================
-- Insert Bookings
-- ==========================================================================
PRINT 'Inserting Bookings...';
INSERT INTO Bookings (
    AccommodationId, 
    GuestId, 
    CheckInDate, 
    CheckOutDate, 
    NumberOfGuests,
    TotalPrice, 
    IsPaid,
    Status, 
    BookingDate
)
VALUES
(1, 1, '2025-07-22', '2025-07-29', 2, 950.00, 1, 1, '2025-03-10'),
(2, 2, '2025-08-05', '2025-08-10', 3, 720.00, 0, 0, '2025-04-01'),
(3, 3, '2024-09-15', '2024-09-20', 4, 680.00, 1, 3, '2024-07-01'),
(4, 4, '2025-06-10', '2025-06-17', 2, 890.00, 1, 1, '2025-01-15'),
(5, 5, '2025-05-01', '2025-05-05', 2, 550.00, 1, 2, '2025-02-20'),
(6, 6, '2024-11-05', '2024-11-12', 3, 820.00, 1, 3, '2024-08-15'),
(7, 7, '2025-09-01', '2025-09-08', 4, 1100.00, 1, 1, '2025-05-15'),
(8, 8, '2024-12-20', '2024-12-23', 2, 350.00, 1, 3, '2024-10-01'),
(9, 9, '2025-07-14', '2025-07-21', 3, 850.00, 0, 0, '2025-04-08'),
(10, 10, '2025-10-10', '2025-10-15', 2, 600.00, 1, 1, '2025-06-20');
GO

-- ==========================================================================
-- Insert Availability Periods
-- ==========================================================================
PRINT 'Inserting Availability Periods...';
-- Creating availability periods for each property
INSERT INTO AvailabilityPeriods (
    AccommodationId,
    StartDate,
    EndDate,
    IsAvailable,
    MinimumStayNights
)
VALUES
-- Summer 2025 for all accommodations
(1, '2025-05-01', '2025-09-30', 1, 3),
(2, '2025-05-01', '2025-09-30', 1, 3),
(3, '2025-05-01', '2025-09-30', 1, 3),
(4, '2025-05-01', '2025-09-30', 1, 3),
(5, '2025-05-01', '2025-09-30', 1, 3),
(6, '2025-05-01', '2025-09-30', 1, 3),
(7, '2025-05-01', '2025-09-30', 1, 3),
(8, '2025-05-01', '2025-09-30', 1, 3),
(9, '2025-05-01', '2025-09-30', 1, 3),
(10, '2025-05-01', '2025-09-30', 1, 3);
GO

PRINT 'Mock data insertion complete.';
GO
