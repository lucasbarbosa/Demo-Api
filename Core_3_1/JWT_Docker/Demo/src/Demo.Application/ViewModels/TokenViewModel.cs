namespace Demo.Application.ViewModels
{
    public class TokenViewModel : BaseViewModel
    {
        public string AccessToken { get; set; }
        public string ExpiresIn { get; set; }
        public string Created { get; set; }
    }
}