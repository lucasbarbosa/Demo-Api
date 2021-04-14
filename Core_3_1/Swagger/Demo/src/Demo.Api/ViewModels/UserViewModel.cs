using System.ComponentModel.DataAnnotations;

namespace Demo.Api.ViewModels
{
    public class UserViewModel
    {
        #region Attributes

        public uint Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        #endregion
    }
}