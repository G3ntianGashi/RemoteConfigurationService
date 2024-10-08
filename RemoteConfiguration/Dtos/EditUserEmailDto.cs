namespace DotnetAPI.Dtos
{
    public partial class EditUserEmailDto
    {
        public string OldEmail {get; set;}
        public string NewEmail {get; set;}

        public EditUserEmailDto()
        {
            if (OldEmail == null)
            {
                OldEmail = "";
            }
            if (NewEmail == null)
            {
                NewEmail = "";
            }
        }
    }
}