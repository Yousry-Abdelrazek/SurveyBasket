using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {

        // UserManager do like DbContext .
        var user = await _userManager.Users
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .AsNoTracking()
            .FirstOrDefaultAsync();


        return Result.Success(user!);
    }

    public async Task<Result> UpdateProfileAsync(string userId , UpdateProfileRequest request)
    {
        //var user = await _userManager.Users
        //    .Where(x => x.Id == userId)
        //    .FirstOrDefaultAsync();

        //user = request.Adapt(user);

        //await _userManager.UpdateAsync(user!);
        // the last code select a lot of columns from database and update all of them but we just need to update first name and last name so we can do like this

        // we will use ExecuteUpdateAsync to update only the columns we need without selecting all columns from database

        await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setter => 
                setter
                    .SetProperty(x => x.FirstName, request.FirstName)
                    .SetProperty(x => x.LastName, request.LastName)
            );

        return Result.Success();

    }


    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
    }
}
