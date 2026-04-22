namespace auth_service.Application.Abstractions.Seed
{
    public interface IUserSeeder
    {
        Task SeedAsync();
    }
}