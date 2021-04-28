﻿using System;

namespace Demo.Domain.Interfaces
{
    public interface ILogger
    {
        void LogException(Exception ex);

        void LogException(Exception ex, string message);
    }
}