-- ==========================================================================
-- Mock Data Script for Cornwall Holiday Bookings (Updated Schema)
-- Target Database: SQL Server
-- Database Name: BookingApp
-- ==========================================================================

-- Set Database Context
USE BookingApp;
GO

-- ==========================================================================
-- Drop Foreign Key Constraints (Optional but recommended for clean reset)
-- ==========================================================================
-- Uncomment and modify based on your actual constraint names if needed
-- ALTER TABLE Reviews DROP CONSTRAINT FK_Reviews_Bookings;
-- ALTER TABLE Reviews DROP CONSTRAINT FK_Reviews_Guests;
-- ALTER TABLE Reviews DROP CONSTRAINT FK_Reviews_Accommodations;
-- ALTER TABLE Bookings DROP CONSTRAINT FK_Bookings_Accommodations;
-- ALTER TABLE Bookings DROP CONSTRAINT FK_Bookings_Guests;
-- ALTER TABLE AvailabilityPeriods DROP CONSTRAINT FK_AvailabilityPeriods_Accommodations;
-- GO

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
-- Insert Accommodations (52 Properties)
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
('Picturesque Valley Cottage', 'Looe Valley Cottage', 'Picturesque cottage in the Looe Valley.', 1, '5 Valley Road', 'Looe', 'PL13 1FA', 50.3514, -4.4544, 2, 1, 4, 0, '["Wifi", "Garden", "Valley view", "Pet friendly"]', '["looe_cottage1.jpg", "looe_cottage2.jpg"]', 120.00, 1, 0, GETDATE(), GETDATE()),
('Fowey River View House', 'Fowey River View', 'Property with beautiful views of the Fowey River.', 1, '10 Riverside', 'Fowey', 'PL23 1JA', 50.3360, -4.6340, 3, 2, 5, 1, '["Wifi", "Balcony", "River view", "Parking"]', '["fowey_house1.jpg", "fowey_house2.jpg"]', 140.00, 1, 0, GETDATE(), GETDATE()),
('Mevagissey Harbour Rest', 'Mevagissey Harbour Rest', 'Comfortable stay near Mevagissey harbour.', 1, '9 Harbour Place', 'Mevagissey', 'PL26 6QQ', 50.2700, -4.7800, 2, 1, 4, 1, '["Wifi", "Harbour view", "Pet friendly"]', '["meva_rest1.jpg", "meva_rest2.jpg"]', 142.00, 1, 0, GETDATE(), GETDATE()),
('Tintagel Castle View Flat', 'Tintagel Castle View', 'Accommodation with views towards Tintagel Castle.', 2, '1 Castle Road', 'Tintagel', 'PL34 0DA', 50.6650, -4.7550, 2, 1, 3, 1, '["Wifi", "Sea view", "Parking"]', '["tintagel_flat1.jpg", "tintagel_flat2.jpg"]', 145.00, 1, 0, GETDATE(), GETDATE()),
('Port Isaac Fisherman''s Cottage', 'Port Isaac Fisherman''s Cottage', 'Traditional cottage in Port Isaac.', 1, '4 Fore Street', 'Port Isaac', 'PL29 3RB', 50.5940, -4.8320, 3, 1, 5, 1, '["Wifi", "Sea view", "Character property"]', '["portisaac_cottage1.jpg", "portisaac_cottage2.jpg"]', 150.00, 1, 0, GETDATE(), GETDATE()),
('Perranporth Beach House', 'Perranporth Beach House', 'House located close to Perranporth beach.', 1, '2 Beach Walk', 'Perranporth', 'TR6 0DP', 50.3450, -5.1550, 4, 2, 8, 1, '["Wifi", "Garden", "Beach access", "Parking", "BBQ"]', '["perranporth_house1.jpg", "perranporth_house2.jpg"]', 131.43, 1, 0, GETDATE(), GETDATE()),
('Marazion Mount View Apt', 'Marazion Mount View', 'Property offering views of St Michael''s Mount.', 2, '8 Market Place', 'Marazion', 'TR17 0AR', 50.1240, -5.4750, 2, 1, 4, 1, '["Wifi", "Sea view", "Balcony"]', '["marazion_apt1.jpg", "marazion_apt2.jpg"]', 125.00, 1, 0, GETDATE(), GETDATE()),
('Charlestown Harbour Loft', 'Charlestown Harbour Loft', 'Stylish loft near the historic Charlestown harbour.', 2, '1 Pier House', 'Charlestown', 'PL25 3NJ', 50.3310, -4.7580, 2, 1, 3, 1, '["Wifi", "Harbour view", "Parking"]', '["charlestown_loft1.jpg", "charlestown_loft2.jpg"]', 150.00, 1, 0, GETDATE(), GETDATE()),
('St Agnes Coastal Barn', 'St Agnes Coastal Barn', 'Converted barn on the St Agnes coast.', 1, '3 Beacon Drive', 'St Agnes', 'TR5 0NU', 50.3120, -5.2050, 3, 2, 6, 1, '["Wifi", "Parking", "Garden", "Sea view", "Pet friendly"]', '["stagnes_barn1.jpg", "stagnes_barn2.jpg"]', 125.71, 1, 0, GETDATE(), GETDATE()),
('Mousehole Artist Studio', 'Mousehole Artist Studio', 'Inspiring studio space in Mousehole.', 2, '15 Chapel Street', 'Mousehole', 'TR19 6SE', 50.0845, -5.5345, 1, 1, 2, 0, '["Wifi", "Artistic decor"]', '["mousehole_studio1.jpg", "mousehole_studio2.jpg"]', 155.00, 1, 0, GETDATE(), GETDATE()),
('Sennen Cove Retreat', 'Sennen Cove Retreat', 'Peaceful retreat near Sennen Cove.', 3, '6 Cove Road', 'Sennen Cove', 'TR19 7DG', 50.0750, -5.7000, 3, 2, 5, 1, '["Wifi", "Parking", "Sea view", "Garden"]', '["sennen_retreat1.jpg", "sennen_retreat2.jpg"]', 141.43, 1, 0, GETDATE(), GETDATE()),
('Quaint Cadgwith Cottage', 'Cadgwith Cove Cottage', 'Quaint cottage in Cadgwith Cove.', 1, '2 The Square', 'Cadgwith Cove', 'TR12 7LZ', 50.0180, -5.1780, 2, 1, 4, 1, '["Wifi", "Sea view", "Pet friendly", "Character property"]', '["cadgwith_cottage1.jpg", "cadgwith_cottage2.jpg"]', 146.00, 1, 0, GETDATE(), GETDATE()),
('Coastal Path Annexe', 'Coverack Coastal Path Annexe', 'Annexe located near the coastal path.', 2, '10 Chynhalls Road', 'Coverack', 'TR12 6SY', 50.0190, -5.1000, 1, 1, 2, 1, '["Wifi", "Parking", "Sea view", "Garden access"]', '["coverack_annexe1.jpg", "coverack_annexe2.jpg"]', 133.33, 1, 0, GETDATE(), GETDATE()),
('Lizard Point Lookout Lodge', 'Lizard Point Lookout', 'Accommodation with views at Lizard Point.', 3, 'Lighthouse Road', 'Lizard Point', 'TR12 7NJ', 49.9590, -5.2100, 4, 2, 7, 1, '["Wifi", "Parking", "Sea view", "Balcony", "Hot tub"]', '["lizard_lookout1.jpg", "lizard_lookout2.jpg"]', 164.29, 1, 0, GETDATE(), GETDATE()),
('Riverside Helford Lodge', 'Helford River Lodge', 'Lodge situated near the Helford River.', 3, '4 Riverside', 'Helford', 'TR12 6JU', 50.0950, -5.1400, 3, 2, 6, 0, '["Wifi", "Parking", "Garden", "River view", "Pet friendly"]', '["helford_lodge1.jpg", "helford_lodge2.jpg"]', 162.50, 1, 0, GETDATE(), GETDATE()),
('Roseland Peninsula Place', 'Roseland Peninsula Place', 'A place to stay on the Roseland Peninsula.', 1, '5 Churchtown Road', 'St Just in Roseland', 'TR2 5JD', 50.1950, -5.0200, 3, 1, 5, 0, '["Wifi", "Parking", "Garden"]', '["roseland_place1.jpg", "roseland_place2.jpg"]', 137.14, 1, 0, GETDATE(), GETDATE()),
('Historic Miner''s Cottage', 'St Just Miner''s Cottage', 'Historic miner''s cottage in St Just.', 1, '11 Market Square', 'St Just', 'TR19 7HF', 50.1250, -5.6800, 2, 1, 4, 0, '["Wifi", "Character property", "Parking nearby"]', '["stjust_cottage1.jpg", "stjust_cottage2.jpg"]', 118.00, 1, 0, GETDATE(), GETDATE()),
('Zennor Coastal Haven', 'Zennor Coastal Haven', 'Coastal property near Zennor.', 1, '2 Coast Road', 'Zennor', 'TR26 3BY', 50.1900, -5.5600, 4, 2, 7, 1, '["Wifi", "Parking", "Garden", "Sea view", "Pet friendly"]', '["zennor_haven1.jpg", "zennor_haven2.jpg"]', 130.00, 1, 0, GETDATE(), GETDATE()),
('Porthcurno Beach View Apt', 'Porthcurno Beach View', 'Stunning views over Porthcurno Beach.', 2, '3 The Valley', 'Porthcurno', 'TR19 6JX', 50.0450, -5.6550, 3, 2, 6, 1, '["Wifi", "Parking", "Balcony", "Sea view", "Beach access"]', '["porthcurno_apt1.jpg", "porthcurno_apt2.jpg"]', 178.57, 1, 0, GETDATE(), GETDATE()),
('Lamorna Cove Studio', 'Lamorna Cove Studio', 'Studio apartment near Lamorna Cove.', 2, '9 Lamorna Cove', 'Lamorna Cove', 'TR19 6XN', 50.0600, -5.5700, 1, 1, 2, 1, '["Wifi", "Parking", "Sea view"]', '["lamorna_studio1.jpg", "lamorna_studio2.jpg"]', 134.00, 1, 0, GETDATE(), GETDATE()),
('Newlyn Harbour Apartment', 'Newlyn Harbour Apartment', 'Apartment overlooking Newlyn Harbour.', 2, '14 The Strand', 'Newlyn', 'TR18 5HJ', 50.1020, -5.5500, 2, 1, 4, 1, '["Wifi", "Harbour view", "Parking nearby"]', '["newlyn_apt1.jpg", "newlyn_apt2.jpg"]', 122.00, 1, 0, GETDATE(), GETDATE()),
('Polzeath Surfer''s Rest', 'Polzeath Surfer''s Rest', 'Ideal base for surfing at Polzeath.', 1, '18 Atlantic Terrace', 'Polzeath', 'PL27 6TB', 50.5680, -4.9150, 3, 2, 6, 1, '["Wifi", "Parking", "Beach access", "Sea view", "Surfboard storage"]', '["polzeath_rest1.jpg", "polzeath_rest2.jpg"]', 132.86, 1, 0, GETDATE(), GETDATE()),
('Luxury Rock Estuary View', 'Rock Estuary View', 'Premium property with estuary views in Rock.', 1, '2 Ferry Point', 'Rock', 'PL27 6LD', 50.5500, -4.9000, 4, 3, 8, 1, '["Wifi", "Parking", "Garden", "Estuary view", "Hot tub", "Pool"]', '["rock_view1.jpg", "rock_view2.jpg"]', 185.71, 1, 0, GETDATE(), GETDATE()),
('Boscastle Harbour House', 'Boscastle Harbour House', 'House located near Boscastle Harbour.', 1, '6 The Harbour', 'Boscastle', 'PL35 0AG', 50.6900, -4.6950, 3, 1, 5, 1, '["Wifi", "Parking", "Harbour view", "Character property"]', '["boscastle_house1.jpg", "boscastle_house2.jpg"]', 140.00, 1, 0, GETDATE(), GETDATE()),
('Crackington Haven Cottage', 'Crackington Haven Cottage', 'Cottage near the beach at Crackington Haven.', 1, '1 Haven Road', 'Crackington Haven', 'EX23 0JG', 50.7500, -4.6300, 3, 2, 6, 1, '["Wifi", "Parking", "Garden", "Beach access", "Pet friendly"]', '["crackington_cottage1.jpg", "crackington_cottage2.jpg"]', 120.00, 1, 0, GETDATE(), GETDATE()),
('Widemouth Bay Retreat', 'Widemouth Bay Retreat', 'Retreat close to Widemouth Bay.', 3, '4 Marine Drive', 'Widemouth Bay', 'EX23 0AH', 50.8000, -4.5600, 2, 1, 4, 1, '["Wifi", "Parking", "Sea view", "Beach access"]', '["widemouth_retreat1.jpg", "widemouth_retreat2.jpg"]', 170.00, 1, 0, GETDATE(), GETDATE()),
('Gorran Haven Fisherman''s Loft', 'Gorran Haven Fisherman''s Loft', 'Loft conversion in Gorran Haven.', 2, '11 Church Street', 'Gorran Haven', 'PL26 6JG', 50.2400, -4.8000, 2, 1, 3, 1, '["Wifi", "Sea view", "Parking nearby"]', '["gorran_loft1.jpg", "gorran_loft2.jpg"]', 112.86, 1, 0, GETDATE(), GETDATE()),
('Pentewan Sands Apartment', 'Pentewan Sands Apartment', 'Apartment near Pentewan Sands.', 2, '2 Beach Court', 'Pentewan', 'PL26 6BT', 50.2900, -4.7850, 2, 1, 4, 1, '["Wifi", "Balcony", "Sea view", "Beach access"]', '["pentewan_apt1.jpg", "pentewan_apt2.jpg"]', 140.00, 1, 0, GETDATE(), GETDATE()),
('Unique Veryan Round House', 'Veryan Round House', 'Unique round house accommodation in Veryan.', 1, 'Pendower Road', 'Veryan', 'TR2 5QL', 50.2300, -4.9000, 3, 2, 5, 0, '["Wifi", "Parking", "Garden", "Character property"]', '["veryan_roundhouse1.jpg", "veryan_roundhouse2.jpg"]', 154.00, 1, 0, GETDATE(), GETDATE()),
('Portloe Harbour View Cottage', 'Portloe Harbour View', 'Property overlooking the harbour at Portloe.', 1, '5 The Lugger', 'Portloe', 'TR2 5RD', 50.2100, -4.9400, 2, 1, 4, 1, '["Wifi", "Sea view", "Harbour view", "Parking nearby"]', '["portloe_cottage1.jpg", "portloe_cottage2.jpg"]', 134.29, 1, 0, GETDATE(), GETDATE()),
('St Mawes Castle Retreat', 'St Mawes Castle Retreat', 'Luxury retreat near St Mawes Castle.', 3, 'Castle Drive', 'St Mawes', 'TR2 5DE', 50.1580, -5.0180, 4, 3, 8, 1, '["Wifi", "Parking", "Garden", "Sea view", "Pool", "Hot tub"]', '["stmawes_retreat1.jpg", "stmawes_retreat2.jpg"]', 200.00, 1, 0, GETDATE(), GETDATE()),
('Flushing Quayside Cottage', 'Flushing Quayside Cottage', 'Cottage located on the quayside in Flushing.', 1, '3 Trefusis Road', 'Flushing', 'TR11 5TY', 50.1600, -5.0680, 3, 1, 5, 1, '["Wifi", "River view", "Parking nearby", "Pet friendly"]', '["flushing_cottage1.jpg", "flushing_cottage2.jpg"]', 162.00, 1, 0, GETDATE(), GETDATE()),
('Mylor Bridge Waterside House', 'Mylor Bridge Waterside', 'Waterside property near Mylor Bridge.', 1, '7 Lemon Hill', 'Mylor Bridge', 'TR11 5NA', 50.1800, -5.0900, 3, 2, 6, 1, '["Wifi", "Parking", "Garden", "River view", "Boat mooring"]', '["mylor_house1.jpg", "mylor_house2.jpg"]', 152.00, 1, 0, GETDATE(), GETDATE()),
('Gwithian Dunes Chalet', 'Gwithian Dunes Chalet', 'Chalet set amongst the dunes at Gwithian.', 3, '15 Mexico Towans', 'Gwithian', 'TR27 5BU', 50.2200, -5.3900, 2, 1, 4, 1, '["Wifi", "Parking", "Beach access", "Sea view"]', '["gwithian_chalet1.jpg", "gwithian_chalet2.jpg"]', 124.29, 1, 0, GETDATE(), GETDATE()),
('Hayle Estuary Lodge', 'Hayle Estuary Lodge', 'Lodge overlooking the Hayle Estuary.', 3, '22 North Quay', 'Hayle', 'TR27 4BL', 50.1850, -5.4200, 2, 1, 4, 1, '["Wifi", "Parking", "Estuary view", "Balcony"]', '["hayle_lodge1.jpg", "hayle_lodge2.jpg"]', 135.00, 1, 0, GETDATE(), GETDATE()),
('Luxury Carbis Bay Apartment', 'Carbis Bay Beach Apartment', 'Premium apartment near Carbis Bay beach.', 2, '1 Ocean View', 'Carbis Bay', 'TR26 2NP', 50.2000, -5.4850, 3, 2, 6, 1, '["Wifi", "Parking", "Balcony", "Sea view", "Beach access", "Pool"]', '["carbis_apt1.jpg", "carbis_apt2.jpg"]', 214.29, 1, 0, GETDATE(), GETDATE()),
('Lelant Saltings View Cottage', 'Lelant Saltings View', 'Accommodation with views over Lelant Saltings.', 1, '4 Station Hill', 'Lelant', 'TR26 3DL', 50.1920, -5.4650, 2, 1, 4, 1, '["Wifi", "Parking", "Garden", "Estuary view"]', '["lelant_cottage1.jpg", "lelant_cottage2.jpg"]', 138.00, 1, 0, GETDATE(), GETDATE()),
('Praa Sands Coastal Cottage', 'Praa Sands Coastal Cottage', 'Cottage located near Praa Sands.', 1, '1 Beach Road', 'Praa Sands', 'TR20 9TQ', 50.0950, -5.4100, 3, 2, 6, 1, '["Wifi", "Parking", "Garden", "Sea view", "Beach access"]', '["praa_cottage1.jpg", "praa_cottage2.jpg"]', 128.57, 1, 0, GETDATE(), GETDATE()),
('Porthleven Harbour Rest', 'Porthleven Harbour Rest', 'Restful accommodation by Porthleven Harbour.', 1, '10 Commercial Road', 'Porthleven', 'TR13 9JD', 50.0850, -5.3150, 2, 1, 4, 1, '["Wifi", "Harbour view", "Parking nearby"]', '["porthleven_rest1.jpg", "porthleven_rest2.jpg"]', 122.86, 1, 0, GETDATE(), GETDATE()),
('Gunwalloe Church Cove Haven', 'Gunwalloe Church Cove Haven', 'Haven near the iconic Church Cove.', 1, '3 Church Cove Lane', 'Gunwalloe', 'TR12 7QE', 50.0550, -5.2800, 3, 1, 5, 1, '["Wifi", "Parking", "Garden", "Sea view", "Pet friendly"]', '["gunwalloe_haven1.jpg", "gunwalloe_haven2.jpg"]', 148.00, 1, 0, GETDATE(), GETDATE()),
('Mullion Cove Lookout', 'Mullion Cove Lookout', 'Property with views over Mullion Cove.', 3, '1 Cove Hill', 'Mullion Cove', 'TR12 7EP', 50.0080, -5.2600, 2, 1, 4, 1, '["Wifi", "Parking", "Sea view", "Balcony"]', '["mullion_lookout1.jpg", "mullion_lookout2.jpg"]', 126.00, 1, 0, GETDATE(), GETDATE()),
('Compact St Ives Studio', 'St Ives Studio Flat', 'Compact studio flat in St Ives.', 2, '2b Fore Street', 'St Ives', 'TR26 1HE', 50.2140, -5.4780, 1, 1, 2, 0, '["Wifi", "Central location"]', '["stives_studio1.jpg", "stives_studio2.jpg"]', 110.00, 1, 0, GETDATE(), GETDATE()),
('Newquay Penthouse View', 'Newquay Penthouse View', 'Penthouse with extensive views in Newquay.', 2, 'Penthouse, Atlantic Heights', 'Newquay', 'TR7 1QJ', 50.4150, -5.0850, 3, 2, 6, 1, '["Wifi", "Parking", "Balcony", "Sea view", "Hot tub"]', '["newquay_penthouse1.jpg", "newquay_penthouse2.jpg"]', 157.14, 1, 0, GETDATE(), GETDATE());
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
-- Insert Bookings (52 Bookings)
-- ==========================================================================
-- Note: AccommodationId is based on the order of insertion above (1-52).
-- GuestId is assigned pseudo-randomly from the 15 guests created.
-- NumberOfGuests is random but <= MaxOccupancy for the Accommodation.
-- IsPaid is random (weighted towards 1 for Confirmed/Completed).
-- Status: 0=Pending, 1=Confirmed, 2=Cancelled, 3=Completed
PRINT 'Inserting Bookings...';
INSERT INTO Bookings (
    AccommodationId, GuestId, CheckInDate, CheckOutDate, NumberOfGuests, TotalPrice, IsPaid, Status, BookingDate
)
VALUES
(1, 1, '2025-07-22', '2025-07-29', 2, 950.00, 1, 1, '2025-03-10'), -- MaxOcc: 4
(2, 2, '2025-08-05', '2025-08-10', 4, 720.00, 0, 0, '2025-04-01'), -- MaxOcc: 4
(3, 3, '2024-09-15', '2024-09-20', 5, 680.00, 1, 3, '2024-07-01'), -- MaxOcc: 6
(4, 4, '2025-06-10', '2025-06-17', 2, 890.00, 1, 1, '2025-01-15'), -- MaxOcc: 2
(5, 5, '2025-05-01', '2025-05-05', 3, 550.00, 1, 2, '2025-02-20'), -- MaxOcc: 4
(6, 6, '2024-11-05', '2024-11-12', 4, 820.00, 1, 3, '2024-08-15'), -- MaxOcc: 6
(7, 7, '2025-09-01', '2025-09-08', 3, 1100.00, 1, 1, '2025-05-15'), -- MaxOcc: 4
(8, 8, '2024-12-20', '2024-12-23', 2, 350.00, 1, 3, '2024-10-01'), -- MaxOcc: 2
(9, 9, '2025-07-14', '2025-07-21', 5, 850.00, 0, 0, '2025-04-08'), -- MaxOcc: 6
(10, 10, '2025-10-10', '2025-10-15', 4, 600.00, 1, 1, '2025-06-20'), -- MaxOcc: 4
(11, 11, '2025-08-18', '2025-08-25', 4, 980.00, 0, 2, '2025-03-01'), -- MaxOcc: 5
(12, 12, '2024-08-01', '2024-08-06', 3, 710.00, 1, 3, '2024-05-10'), -- MaxOcc: 4
(13, 13, '2025-09-22', '2025-09-26', 2, 580.00, 1, 1, '2025-07-05'), -- MaxOcc: 3
(14, 14, '2025-07-01', '2025-07-08', 5, 1050.00, 1, 1, '2025-02-11'), -- MaxOcc: 5
(15, 15, '2024-10-05', '2024-10-12', 6, 920.00, 1, 3, '2024-06-01'), -- MaxOcc: 8
(16, 1, '2025-11-01', '2025-11-05', 3, 500.00, 0, 0, '2025-04-05'), -- MaxOcc: 4
(17, 2, '2025-08-11', '2025-08-16', 2, 750.00, 1, 1, '2025-04-02'), -- MaxOcc: 3
(18, 3, '2024-07-10', '2024-07-17', 5, 880.00, 1, 3, '2024-03-15'), -- MaxOcc: 6
(19, 4, '2025-06-01', '2025-06-05', 2, 620.00, 1, 2, '2025-01-10'), -- MaxOcc: 2
(20, 5, '2025-09-15', '2025-09-22', 4, 990.00, 1, 1, '2025-05-20'), -- MaxOcc: 5
(21, 6, '2024-09-02', '2024-09-07', 3, 730.00, 1, 3, '2024-06-18'), -- MaxOcc: 4
(22, 7, '2025-10-01', '2025-10-04', 2, 400.00, 1, 1, '2025-07-11'), -- MaxOcc: 2
(23, 8, '2025-08-20', '2025-08-27', 6, 1150.00, 0, 0, '2025-04-07'), -- MaxOcc: 7
(24, 9, '2024-11-15', '2024-11-19', 4, 650.00, 1, 3, '2024-09-01'), -- MaxOcc: 6
(25, 10, '2025-07-25', '2025-08-01', 5, 960.00, 1, 1, '2025-03-05'), -- MaxOcc: 5
(26, 11, '2025-06-20', '2025-06-25', 3, 590.00, 1, 1, '2025-02-15'), -- MaxOcc: 4
(27, 12, '2024-08-10', '2024-08-17', 6, 910.00, 1, 3, '2024-04-20'), -- MaxOcc: 7
(28, 13, '2025-08-08', '2025-08-15', 5, 1250.00, 1, 1, '2025-01-25'), -- MaxOcc: 6
(29, 14, '2025-09-05', '2025-09-10', 2, 670.00, 0, 2, '2025-06-01'), -- MaxOcc: 2
(30, 15, '2024-10-20', '2024-10-25', 4, 610.00, 1, 3, '2024-07-15'), -- MaxOcc: 4
(31, 1, '2025-07-12', '2025-07-19', 5, 930.00, 1, 1, '2025-03-18'), -- MaxOcc: 6
(32, 2, '2025-08-22', '2025-08-29', 7, 1300.00, 0, 0, '2025-04-08'), -- MaxOcc: 8
(33, 3, '2024-09-25', '2024-09-29', 4, 560.00, 1, 3, '2024-07-02'), -- MaxOcc: 5
(34, 4, '2025-06-05', '2025-06-12', 6, 840.00, 1, 1, '2025-01-20'), -- MaxOcc: 6
(35, 5, '2025-07-07', '2025-07-11', 3, 680.00, 1, 1, '2025-03-22'), -- MaxOcc: 4
(36, 6, '2024-11-01', '2024-11-08', 2, 790.00, 1, 3, '2024-08-10'), -- MaxOcc: 3
(37, 7, '2025-08-01', '2025-08-06', 4, 700.00, 1, 2, '2025-04-01'), -- MaxOcc: 4
(38, 8, '2025-09-10', '2025-09-15', 4, 770.00, 1, 1, '2025-05-25'), -- MaxOcc: 5
(39, 9, '2024-07-20', '2024-07-27', 3, 940.00, 1, 3, '2024-04-15'), -- MaxOcc: 4
(40, 10, '2025-10-15', '2025-10-22', 6, 1400.00, 1, 1, '2025-06-10'), -- MaxOcc: 8
(41, 11, '2025-08-04', '2025-08-09', 4, 810.00, 0, 0, '2025-04-06'), -- MaxOcc: 5
(42, 12, '2024-12-01', '2024-12-06', 5, 760.00, 1, 3, '2024-09-20'), -- MaxOcc: 6
(43, 13, '2025-07-18', '2025-07-25', 3, 870.00, 1, 1, '2025-03-12'), -- MaxOcc: 4
(44, 14, '2025-09-08', '2025-09-12', 4, 540.00, 1, 1, '2025-05-01'), -- MaxOcc: 4
(45, 15, '2024-08-20', '2024-08-27', 5, 1500.00, 1, 3, '2024-02-10'), -- MaxOcc: 6
(46, 1, '2025-06-15', '2025-06-20', 3, 690.00, 0, 2, '2025-02-01'), -- MaxOcc: 4
(47, 2, '2025-08-14', '2025-08-21', 6, 900.00, 1, 1, '2025-04-03'), -- MaxOcc: 6
(48, 3, '2024-10-10', '2024-10-17', 4, 860.00, 1, 3, '2024-07-25'), -- MaxOcc: 4
(49, 4, '2025-09-20', '2025-09-25', 4, 740.00, 1, 1, '2025-06-15'), -- MaxOcc: 5
(50, 5, '2025-11-10', '2025-11-15', 3, 630.00, 0, 0, '2025-04-08'), -- MaxOcc: 4
(51, 6, '2026-01-05', '2026-01-10', 2, 550.00, 1, 1, '2025-10-15'), -- MaxOcc: 2
(52, 7, '2024-06-10', '2024-06-17', 5, 1100.00, 1, 3, '2024-01-20'); -- MaxOcc: 6
GO

-- ==========================================================================
-- Insert Availability Periods
-- ==========================================================================
-- Creating a default summer availability period for each property
PRINT 'Inserting Availability Periods...';

-- Declare variables for loop
DECLARE @AccommodationId INT = 1;
DECLARE @MaxAccommodationId INT = 52; -- Total number of accommodations inserted

-- Loop through each accommodation
WHILE @AccommodationId <= @MaxAccommodationId
BEGIN
    INSERT INTO AvailabilityPeriods (
        AccommodationId, StartDate, EndDate, IsAvailable, MinimumStayNights
    )
    VALUES
    -- Summer 2025
    (@AccommodationId, '2025-05-01', '2025-09-30', 1, CASE WHEN @AccommodationId % 5 = 0 THEN 7 ELSE 3 END), -- Vary min stay slightly
    -- Autumn 2025 (Example of another period)
    (@AccommodationId, '2025-10-01', '2025-12-20', 1, 2),
    -- Spring 2026 (Example)
    (@AccommodationId, '2026-03-01', '2026-04-30', 1, 3);

    -- Increment counter
    SET @AccommodationId = @AccommodationId + 1;
END;
GO

-- ==========================================================================
-- Recreate Foreign Key Constraints (Optional)
-- ==========================================================================
-- Uncomment and modify based on your actual constraint names if needed
-- PRINT 'Recreating foreign key constraints...';
-- ALTER TABLE Reviews WITH CHECK ADD CONSTRAINT FK_Reviews_Bookings FOREIGN KEY(BookingId) REFERENCES Bookings (Id);
-- ALTER TABLE Reviews CHECK CONSTRAINT FK_Reviews_Bookings;
-- ALTER TABLE Reviews WITH CHECK ADD CONSTRAINT FK_Reviews_Guests FOREIGN KEY(GuestId) REFERENCES Guests (Id);
-- ALTER TABLE Reviews CHECK CONSTRAINT FK_Reviews_Guests;
-- ALTER TABLE Reviews WITH CHECK ADD CONSTRAINT FK_Reviews_Accommodations FOREIGN KEY(AccommodationId) REFERENCES Accommodations (Id);
-- ALTER TABLE Reviews CHECK CONSTRAINT FK_Reviews_Accommodations;
-- ALTER TABLE Bookings WITH CHECK ADD CONSTRAINT FK_Bookings_Accommodations FOREIGN KEY(AccommodationId) REFERENCES Accommodations (Id);
-- ALTER TABLE Bookings CHECK CONSTRAINT FK_Bookings_Accommodations;
-- ALTER TABLE Bookings WITH CHECK ADD CONSTRAINT FK_Bookings_Guests FOREIGN KEY(GuestId) REFERENCES Guests (Id);
-- ALTER TABLE Bookings CHECK CONSTRAINT FK_Bookings_Guests;
-- ALTER TABLE AvailabilityPeriods WITH CHECK ADD CONSTRAINT FK_AvailabilityPeriods_Accommodations FOREIGN KEY(AccommodationId) REFERENCES Accommodations (Id);
-- ALTER TABLE AvailabilityPeriods CHECK CONSTRAINT FK_AvailabilityPeriods_Accommodations;
-- GO

PRINT 'Mock data insertion complete.';
GO
