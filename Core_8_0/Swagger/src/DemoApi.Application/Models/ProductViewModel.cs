namespace DemoApi.Application.Models
{
    public class ProductViewModel : BaseViewModel
    {
        #region Properties

        public uint Id { get; set; }

        public string Name { get; set; }

        public double Weight { get; set; }

        #endregion
    }
}