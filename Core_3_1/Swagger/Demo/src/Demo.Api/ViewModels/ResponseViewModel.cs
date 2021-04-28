using System.Collections.Generic;

namespace Demo.Api.ViewModels
{
    public class ResponseViewModel
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