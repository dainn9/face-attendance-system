using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.Subject;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Subjects.Commands.UpdateSubject
{
    public class UpdateSubjectHandler : IRequestHandler<UpdateSubjectCommand>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public UpdateSubjectHandler(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            var subject = await _subjectRepository.FindAsync(request.SubjectId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Subject), request.SubjectId);

            var normalizedCode = request.Code.Trim().ToUpperInvariant();

            if (await _subjectRepository.ExistsByCodeAsync(normalizedCode, request.SubjectId, cancellationToken))
                throw new BusinessRuleViolationException($"Subject with code '{request.Code}' already exists.", ErrorCodes.SubjectCodeAlreadyExists);

            subject.Update(
                request.FacultyId,
                request.Name,
                normalizedCode,
                request.Credits,
                _clock.UtcNow
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}