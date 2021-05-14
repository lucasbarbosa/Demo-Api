using System.Collections.Generic;

namespace Demo.Application.ViewModels
{
    public class ResponseViewModel : BaseViewModel
    {
        #region Constructors

        public ResponseViewModel()
        {
            Errors = new List<string>();
        }

        #endregion

        #region Properties

        public bool Success { get; set; }

        public object Data { get; set; }

        public IList<string> Errors { get; set; }

        #endregion
    }
}