using System.ComponentModel.DataAnnotations;
using Micro.Handlers;
using Micro.Identity;
using Micro.Security.Encryption;
using Micro.Time;
using Microsoft.Extensions.Logging;
using VideoHub.Services.Users.Core.Entities;
using VideoHub.Services.Users.Core.Exceptions;
using VideoHub.Services.Users.Core.Repositories;

namespace VideoHub.Services.Users.Core.Commands.Handlers;

internal sealed class SignUpHandler : ICommandHandler<SignUp>
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();
    private static readonly string DefaultRole = Role.Default;
    
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;
    private readonly IIdGen _idGen;
    private readonly ILogger<SignUpHandler> _logger;

    public SignUpHandler(IUserRepository userRepository, IRoleRepository roleRepository,
        IUserSubscriptionRepository userSubscriptionRepository, IPasswordManager passwordManager,
        IClock clock,IIdGen idGen, ILogger<SignUpHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userSubscriptionRepository = userSubscriptionRepository;
        _passwordManager = passwordManager;
        _clock = clock;
        _idGen = idGen;
        _logger = logger;
    }

    public async Task HandleAsync(SignUp command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || !EmailAddressAttribute.IsValid(command.Email))
        {
            throw new InvalidEmailException();
        }
        
        if (string.IsNullOrWhiteSpace(command.Username))
        {
            throw new InvalidUsernameException();
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            throw new MissingPasswordException();
        }

        var email = command.Email.ToLowerInvariant();
        if (await _userRepository.GetByEmailAsync(email) is not null)
        {
            throw new EmailInUseException(email);
        }
        
        var username = command.Username.ToLowerInvariant();
        if (await _userRepository.GetByUsernameAsync(username) is not null)
        {
            throw new UsernameInUseException(username);
        }

        var roleName = string.IsNullOrWhiteSpace(command.Role) ? DefaultRole : command.Role.ToLowerInvariant();
        var role = await _roleRepository.GetAsync(roleName);
        if (role is null)
        {
            throw new RoleNotFoundException(roleName);
        }

        var now = _clock.Current();
        var password = _passwordManager.Secure(command.Password);
        var user = new User
        {
            Id = command.UserId,
            Email = email,
            Username = username,
            Password = password,
            Role = role,
            CreatedAt = now
        };
        await _userRepository.AddAsync(user);
        var subscription = CreateDefaultSubscription(user.Id);
        await _userSubscriptionRepository.AddAsync(subscription);
        _logger.LogInformation($"User with ID: '{user.Id}' has signed up.");
    }

    private UserSubscription CreateDefaultSubscription(long userId)
        => new()
        {
            Id = _idGen.Create(),
            UserId = userId,
            SizeLimit = 100_000_000,
            LengthLimit = 10,
            VideosLimit = 5,
            Version = 1
        };
}