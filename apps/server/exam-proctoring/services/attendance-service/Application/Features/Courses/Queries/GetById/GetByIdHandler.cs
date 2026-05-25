using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Exceptions;
using MediatR;

namespace attendance_service.Application.Features.Courses.Queries.GetById
{
    public class GetByIdHandler : IRequestHandler<GetByIdQuery, CourseDto>
    {
        private readonly ICourseReadRepository _courseReadRepository;

        public GetByIdHandler(ICourseReadRepository courseReadRepository) => _courseReadRepository = courseReadRepository;

        public async Task<CourseDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        => await _courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new EntityNotFoundException("Course", request.CourseId);
    }
}