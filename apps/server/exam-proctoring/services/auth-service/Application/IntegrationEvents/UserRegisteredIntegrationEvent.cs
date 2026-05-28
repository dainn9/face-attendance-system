// using SharedKernel.Core.Enums;

// namespace auth_service.Application.IntegrationEvents
// {
//     /// <summary>
//     /// Integration event published when a new user is registered. This event is used to create a user profile in the internal user service.
//     /// </summary>
//     public record UserRegisteredIntegrationEvent(
//         Guid UserId,
//         string FullName,
//         Gender Gender,
//         DateOnly DateOfBirth,
//         string Email,
//         UserRole Role,
//         string? StudentCode,
//         string? ClassCode,
//         string? FacultyCode
//     );
// }