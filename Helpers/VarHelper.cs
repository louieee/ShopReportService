namespace ReportService.Helpers
{
    public class VarHelper
    {
        public const string DefaultOrgId = "DEFAULT";
        public const string DefaultCountryId = "234";
        public const string DefaultFullname = "INCOMPLETE_PROFILE";
        public const string DefaultRiderId = "132578783383556671";// "RAVEN";
        public const int DefaultRideOrder = 1;
        public const string DefaultSurname = "INCOMPLETE_PROFILE";
        public const string DefaultFirstname = "INCOMPLETE_PROFILE";
        public const string DefaultIdnum = "INCOMPLETE_PROFILE";
        public const string DefaultTelephone = "2348000000000";

        public enum ResponseStatus
        {
            ERROR,
            SUCCESS,
            PENDING,
            WARNING
        }

        public enum RequestStatus
        {
            PENDING,
            ACCEPTED,
            REJECTED
        }

        public enum UserStatus
        {
            ENABLED,
            DISABLED
        }

        public enum MetaDataTypes
        {
            BANK,
            CARD_TYPE,
            GENDER,
            PAYMENT_TYPE,
            VEHICLE_TYPE,
            HANDLING,
            CONTENT_TYPE,
            PICKUP_TYPE,
            RIDE_STATUS,
            PAYER,
            OCCUPATION,
            RELATIONSHIP
        }
        public enum TransactionStatus
        {
            BOOKING,
            BOOKED,
            IN_TRANSIT,
            DELIVERED,
            CANCELLED
        }

    }
}
