using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // để mặc định
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
