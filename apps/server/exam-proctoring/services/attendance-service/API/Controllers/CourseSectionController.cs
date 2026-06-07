using attendance_service.Application.Features.Enrollments.Commands.RemoveEnrollment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace attendance_service.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiController]
    [Route("api/v1/course-sections")]
    public class CourseSectionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourseSectionController(IMediator mediator) => _mediator = mediator;

        // DELETE: api/v1/course-sections/{courseSectionId}/enrollments/{studentId}
        [HttpDelete("{courseSectionId:guid}/enrollments/{studentId:guid}")]
        public async Task<IActionResult> RemoveEnrollment(Guid courseSectionId, Guid studentId, CancellationToken cancellationToken)
        {
            var command = new RemoveEnrollmentCommand(courseSectionId, studentId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}