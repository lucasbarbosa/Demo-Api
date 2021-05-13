using System.ComponentModel.DataAnnotations;

namespace Demo.Application.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        #region Properties

        [Range(1, uint.MaxValue, ErrorMessage = "Id deve ser um valor entre 1 e 4294967295")]
        [Required(ErrorMessage = "Campo Id é obrigatório")]
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