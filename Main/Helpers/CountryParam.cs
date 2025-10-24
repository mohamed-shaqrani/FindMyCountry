using FluentValidation;

namespace Main.Helpers;

public sealed class CountryParam
{
    private const int MaxPageSize = 30;
    private int _pageSize = 5;
    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber <= 0 ? 1 : _pageNumber;
        set => _pageNumber = value <= 0 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? CountryCode { get; set; }
}
public class UserParamValidator : AbstractValidator<CountryParam>
{
    public UserParamValidator()
    {


    }
}