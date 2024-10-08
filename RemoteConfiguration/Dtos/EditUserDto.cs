namespace DotnetAPI.Dtos
{
    public partial class EditUserDto
    {
        public int UserId {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public bool Active {get; set;}

        public EditUserDto()
        {
            if (FirstName == null)
            {
                FirstName = "";
            }
            if (LastName == null)
            {
                LastName = "";
            }
        }
    }
}