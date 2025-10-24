namespace Main.Common.Response;

public enum ErrorCode
{
    None = 00,
    ValidationError = 01,

    DataBaseError = 02,
    ItemAlreadyExists = 03,
    InternalServerError = 500,


}
