namespace Main.Common.Response;

public enum ErrorCode
{
    None = 00,
    ValidationError = 01,

    DataBaseError = 02,
    ItemAlreadyExists = 03,
    Conflict = 409,

    InternalServerError = 500,
    NotFound = 501,
}
