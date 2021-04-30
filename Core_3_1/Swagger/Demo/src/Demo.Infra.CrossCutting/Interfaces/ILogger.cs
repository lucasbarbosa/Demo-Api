using System;

namespace Demo.Infra.CrossCutting.Interfaces
{
    public interface ILogger
    {
        void LogException(Exception ex);

        void LogException(Exception ex, string message);
    }
}