using System.ComponentModel.DataAnnotations;

namespace Demo.Api.ViewModels
{
    public class UserViewModel
    {
        #region Properties

        public uint Id { get; set; }

        [Required(ErrorMessage = "Campo Name é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo E-mail é obrigatório")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        #endregion
    }
}