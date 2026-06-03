using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.Subject;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Subjects.Commands.CreateSubject
{
    public class CreateSubjectHandler : IRequestHandler<CreateSubjectCommand, Guid>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public CreateSubjectHandler(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task<Guid> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            if (await _subjectRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
                throw new BusinessRuleViolationException($"Subject with code '{request.Code}' already exists.", ErrorCodes.SubjectCodeAlreadyExists);

            var subject = Subject.Create(
                request.Name,
                request.Code,
                request.Credits,
                _clock.UtcNow
            );

            _subjectRepository.Add(subject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return subject.Id;
        }
    }
}