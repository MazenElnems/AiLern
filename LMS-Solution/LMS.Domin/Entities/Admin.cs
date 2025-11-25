using LMS.Domin.Enums;

namespace LMS.Domin.Entities;

public class Admin : ApplicationUser
{
    public AdminLevels AdminLevel { get; set; }
}
