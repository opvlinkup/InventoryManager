using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Application.DTO.Admin;
using InventoryManager.Application.DTO.User;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public sealed class AdminRepository(InventoryManagerDbContext context, UserManager<User> userManager) : IAdminRepository
{
    public async Task<IReadOnlyList<UserAdminDto>> GetUsersAsync(UserFilterDto filter, CancellationToken ct)
    {
        var roles = context.Set<IdentityRole<Guid>>();
        var userRoles = context.Set<IdentityUserRole<Guid>>();
        
        var adminRoleId = await roles
            .Where(r => r.Name == "Admin")
            .Select(r => r.Id)
            .SingleOrDefaultAsync(ct);

       
        var query = context.Users.AsNoTracking()
            .Select(user => new
            {
                User = user,
                IsAdmin = adminRoleId != Guid.Empty && 
                          userRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == adminRoleId)
            });

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchPattern = $"%{filter.Search.Trim()}%";

          
            query = query.Where(x =>
                (x.User.Email != null && EF.Functions.ILike(x.User.Email, searchPattern)) ||
                (x.User.Name != null && EF.Functions.ILike(x.User.Name, searchPattern)) ||
                (x.User.Surname != null && EF.Functions.ILike(x.User.Surname, searchPattern)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.User.Status == filter.Status.Value);
        }

        if (filter.IsAdmin.HasValue)
        {
            query = query.Where(x => x.IsAdmin == filter.IsAdmin.Value);
        }

        return await query
            .OrderBy(x => x.User.RegisteredAt)
            .Skip(filter.Skip)
            .Take(filter.Take)
            .Select(x => new UserAdminDto
            {
                Id = x.User.Id,
                Email = x.User.Email!,
                Name = x.User.Name,
                Surname = x.User.Surname,
                Status = x.User.Status,
                IsAdmin = x.IsAdmin,
                RegisteredAt = x.User.RegisteredAt
            })
            .ToListAsync(ct);
    }

  
    public Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        return userManager.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> IsUserAdminAsync(Guid userId, CancellationToken ct)
    {
        return context.Set<IdentityUserRole<Guid>>()
            .AnyAsync(ur => ur.UserId == userId && context.Set<IdentityRole<Guid>>()
                                .Any(r => r.Id == ur.RoleId && r.Name == "Admin"), 
                ct);
    }

    public async Task GrantAdminAsync(User user)
    {
        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    public async Task RevokeAdminAsync(User user)
    {
        if (await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.RemoveFromRoleAsync(user, "Admin");
        }
    }

    public Task DeleteUserAsync(User user)
    {
        return userManager.DeleteAsync(user);
    }

    public Task InvalidateSessionsAsync(Guid userId, CancellationToken ct)
    {
        return context.Set<Session>()
            .Where(s => s.UserId == userId)
            .ExecuteDeleteAsync(ct);
    }
}