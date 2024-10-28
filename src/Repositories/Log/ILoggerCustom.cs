﻿using System;

namespace PetProject.Repositories
{
    public interface ILoggerCustom
    {
        void LogInfo(string message);
        void LogError(string message, Exception ex);
    }
}
