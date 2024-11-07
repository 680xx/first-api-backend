using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace first_api_backend.Models;

public class AppUser:IdentityUser
{
    [PersonalData]
    [Column(TypeName ="varchar(150)")]
    public string FullName { get; set; }
    
    [PersonalData]
    [Column(TypeName = "varchar(10)")]
    public string Gender { get; set; }
    
    [PersonalData]
    public DateOnly DOB { get; set; }
    
    [PersonalData]
    public int? LibararyID { get; set; }
}