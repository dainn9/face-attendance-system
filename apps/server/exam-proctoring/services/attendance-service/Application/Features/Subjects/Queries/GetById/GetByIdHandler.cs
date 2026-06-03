using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Exceptions;
using MediatR;

namespace attendance_service.Application.Features.Subjects.Queries.GetById
{
    public class GetByIdHandler : IRequestHandler<GetByIdQuery, SubjectDto>
    {
        private readonly ISubjectReadRepository _subjectReadRepository;

        public GetByIdHandler(ISubjectReadRepository subjectReadRepository) => _subjectReadRepository = subjectReadRepository;

        public async Task<SubjectDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        => await _subjectReadRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new EntityNotFoundException("Subject", request.SubjectId);
    }
}