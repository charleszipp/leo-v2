namespace Leo.Core.Api.Interfaces
{
    public interface IAuthorizedRequest : IRequest
    {
        string UserId { get; set; }
        string Token { get; set; }
    }
}