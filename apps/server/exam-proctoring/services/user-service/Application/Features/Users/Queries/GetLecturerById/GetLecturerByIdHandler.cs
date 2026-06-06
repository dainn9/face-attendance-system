using BuildingBlocks.Exceptions;
using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Lecturers;

namespace user_service.Application.Features.Users.Queries.GetLecturerById
{
    public class GetLecturerByIdHandler : IRequestHandler<GetLecturerByIdQuery, LecturerDto>
    {
        private readonly IUserReadRepository _userReadRepository;
        public GetLecturerByIdHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }
        public async Task<LecturerDto> Handle(GetLecturerByIdQuery request, CancellationToken cancellationToken)
        => await _userReadRepository.GetLecturerByIdAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Lecturer", request.Id);
    }
}