using System.ComponentModel.DataAnnotations;

namespace DemoApi.Application.Models
{
    public class ProductViewModel : BaseViewModel
    {
        #region Properties

        public uint Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public double Weight { get; set; }

        #endregion
    }
}