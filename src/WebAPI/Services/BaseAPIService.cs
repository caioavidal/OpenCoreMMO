using AutoMapper;

namespace NeoServer.Web.API.Services;

public class BaseAPIService
{
    #region private members

    protected readonly IMapper _mapper;

    #endregion private members

    #region constructors

    public BaseAPIService(IMapper mapper)
    {
        _mapper = mapper;
    }

    #endregion constructors
}
