using System.ComponentModel.DataAnnotations;

namespace Demo.Application.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        #region Properties

        [Range(1, uint.MaxValue, ErrorMessage = "Id deve ser um valor entre 1 e 4294967295")]
        [Required(ErrorMessage = "Campo Id é obrigatório")]
        public uint Id { get; set; }

        [Required(ErrorMessage = "Campo Name é obrigatório")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Weight deve ser maior que 0")]
        public double Weight { get; set; }

        #endregion
    }
}