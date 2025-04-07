namespace BookingApp.Server.Model
{
    /// <summary>
    /// Defines the primary types of holiday accommodation available in Cornwall.
    /// Keep this list focused on broad categories relevant to the region.
    /// </summary>
    public enum AccommodationType
    {
        Cottage,
        Apartment,
        Flat,
        House,
        Bungalow,
        Farmhouse,
        Lodge,
        Studio,
        Other       // Catch-all, details in Description.
    }

    /// <summary>
    /// Represents the lifecycle status of a booking.
    /// </summary>
    public enum BookingStatus
    {
        Pending,     // Awaiting confirmation/payment.
        Confirmed,   // Secured, guest expected.
        Cancelled,   // Booking cancelled before stay.
        Completed,   // Stay finished successfully.
        NoShow       // Guest failed to arrive.
    }

    /// <summary>
    /// Key amenities/features often seen in Cornwall holiday lets.
    /// Using an Enum collection is concise and queryable.
    /// </summary>
    public enum AmenityType // Renamed from FeatureType for clarity
    {
        Wifi,
        Parking,          // Consider variants if needed: OffRoadParking, GarageParking
        PetFriendly,
        Kitchen,          // Could be broken down (e.g., BasicKitchen, FullKitchen) if necessary.
        WashingMachine,
        Dishwasher,
        SwimmingPool,     // Specify Private or Shared if needed.
        HotTub,
        Garden,           // Specify Private or Shared if needed.
        WoodBurner,
        EVCharging,
        AirConditioning,
        Heating,          // Assumed standard, but explicit mention can be useful.
        SurfboardStorage, // Relevant for coastal properties.
        BBQ,
        Accessible        // Consider dedicated accessibility features/ratings if needed.
    }
}