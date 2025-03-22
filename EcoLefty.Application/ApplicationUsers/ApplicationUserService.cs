using AutoMapper;
using EcoLefty.Domain.Contracts;

namespace EcoLefty.Application.ApplicationUsers;

public class ApplicationUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApplicationUserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


}
