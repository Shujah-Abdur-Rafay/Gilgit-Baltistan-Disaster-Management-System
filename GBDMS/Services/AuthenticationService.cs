using GBDMS.Data;
using GBDMS.Repository;
using GBDMS.Repository.IRepository;
using Microsoft.JSInterop;

namespace GBDMS.Services
{
    public class AuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJSRuntime _jsRuntime;
        private readonly ToastService _toastService;

        public event Action? OnAuthenticationStateChanged;

        private User? _currentUser;
        private string _currentUserRole = "Guest"; // Guest, Privileged, SuperAdmin

        public AuthenticationService(IUserRepository userRepository, IJSRuntime jsRuntime, ToastService toastService)
        {
            _userRepository = userRepository;
            _jsRuntime = jsRuntime;
            _toastService = toastService;
        }
        
        public User? CurrentUser => _currentUser;
        public string CurrentUserRole => _currentUserRole;
        public bool IsLoggedIn => _currentUser != null;
        public bool IsPrivileged => _currentUserRole == "Privileged" || _currentUserRole == "SuperAdmin";
        public bool IsSuperAdmin => _currentUserRole == "SuperAdmin";
        public bool IsGuest => _currentUserRole == "Guest";
        
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                // Check for super admin credentials
                if (email == "shujah@gbdms.com" && password == "Test@123")
                {
                    _currentUser = new User
                    {
                        Id = -1, // Special ID for super admin
                        Username = "Super Admin",
                        Email = "shujah@gbdms.com",
                        Role = "SuperAdmin",
                        IsActive = true
                    };
                    _currentUserRole = "SuperAdmin";
                    OnAuthenticationStateChanged?.Invoke();
                    return true;
                }
                
                // Check regular user credentials
                var user = await _userRepository.LoginAsync(email, password);
                if (user != null && user.IsActive)
                {
                    _currentUser = user;
                    _currentUserRole = user.Role == "Admin" ? "SuperAdmin" : "Privileged";
                    OnAuthenticationStateChanged?.Invoke();
                    return true;
                }
                
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task LogoutAsync()
        {
            _currentUser = null;
            _currentUserRole = "Guest";
            OnAuthenticationStateChanged?.Invoke();
            await Task.CompletedTask;
        }
        
        public async Task<bool> ShowLoginPromptAsync(string message = "This feature requires a privileged account. Please login to continue.")
        {
            try
            {
                // Show green toast notification instead of confirm dialog
                await _toastService.ShowInfo(message);
                return false; // Don't auto-login, just show the message
            }
            catch
            {
                return false;
            }
        }
        
        public bool CanAccessFeature(string feature)
        {
            return feature.ToLower() switch
            {
                "model" => IsPrivileged,
                "admin" => IsSuperAdmin,
                "facilities_full" => IsPrivileged,
                "survival_guidelines_edit" => IsSuperAdmin,
                "home_advanced" => IsPrivileged,
                "add_edit_delete" => IsPrivileged,
                "super_admin_only" => IsSuperAdmin,
                _ => true // Default allow for basic features
            };
        }
        
        public async Task<bool> CheckAccessAndPromptAsync(string feature, string? customMessage = null)
        {
            if (CanAccessFeature(feature))
            {
                return true;
            }
            
            string message = customMessage ?? GetDefaultMessage(feature);
            bool shouldLogin = await ShowLoginPromptAsync(message);
            
            return false; // Always return false since we're just showing prompt
        }
        
        private string GetDefaultMessage(string feature)
        {
            return feature.ToLower() switch
            {
                "model" => "The AI Model feature requires a privileged account. Please login to access this feature.",
                "admin" => "Admin panel access requires super admin privileges. Please login with super admin credentials.",
                "facilities_full" => "Full facilities management requires a privileged account. Please login to continue.",
                "survival_guidelines_edit" => "Managing survival guidelines requires super admin privileges. Please login with super admin credentials.",
                "home_advanced" => "Advanced features require a privileged account. Please login to continue.",
                _ => "This feature requires a privileged account. Please login to continue."
            };
        }
    }
}
