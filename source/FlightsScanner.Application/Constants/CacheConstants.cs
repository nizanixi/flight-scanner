namespace FlightsScanner.Application.Constants;

public static class CacheConstants
{
    public const int CACHE_LIMIT = 100;

    #region Airport codes

    public const int SLIDING_EXPIRATION_FOR_IATA_CODES_IN_SECONDS = 60;

    public const int ABSOLUTE_EXPIRATION_FOR_IATA_CODES_IN_MINUTES = 1;

    public const int IATA_CODE_CACHE_SIZE = 1;

    #endregion

    #region Flights

    public const int SLIDING_EXPIRATION_FOR_FLIGHTS_IN_SECONDS = 60;

    public const int ABSOLUTE_EXPIRATION_FOR_FLIGHTS_CODES_IN_MINUTES = 1;

    public const int FLIGHT_ITEM_CACHE_SIZE = 10;

    #endregion

    #region Flights


    public const int SLIDING_EXPIRATION_FOR_AMADEUS_AUTHORIZATION = 300;

    public const int ABSOLUTE_EXPIRATION_FOR_AMADEUS_AUTHORIZATION = 1;

    public const int AMADEUS_AUTHORIZATION_CACHE_SIZE = 1;

    #endregion
}
