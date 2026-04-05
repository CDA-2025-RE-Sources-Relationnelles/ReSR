using FluentResponse.Interfaces;

namespace ReSR.Application.Services.Users.Definitions;

/// <summary>
/// A service for handling user friendships.
/// </summary>
public interface IFriendshipService {

    /// <summary>
    /// Tries to set/unset a like on user profile. If both users mutually like each others, they become friends.
    /// </summary>
    /// <returns>A successful response if the like was set/unset.</returns>
    /// <param name="byUserId">The identifier of the user liking/unliking the profile.</param>
    /// <param name="userId">The identifier of the user being liked/unliked.</param>
    /// <param name="value">`true` if the profile should be liked, `false` if it should be unliked.</param>
    public Task<IResponse> TrySetProfileLikeAsync(Id byUserId, Id userId, bool value = true);
}
