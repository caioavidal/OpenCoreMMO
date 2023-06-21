using AutoMapper;

namespace NeoServer.Web.API.Services;

public class BaseApiService
{
    #region private members

    protected readonly IMapper Mapper;

    #endregion private members

    #region constructors

    public BaseApiService(IMapper mapper)
    {
        Mapper = mapper;
    }

    #endregion constructors
}