namespace Main.Feature.IpGeoLocation.Endpoints;

using FluentValidation;
public sealed class PaginationParam
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

}
public class PaginationParamValidator : AbstractValidator<PaginationParam>
{
    public PaginationParamValidator()
    {


    }
}