using System;

namespace Smart_Investments.Services
{
    public class ExceptionService
    {
        public Action<Exception, TypeException> GetException;

        public void NewException(Exception e, TypeException te)
        {
            GetException?.Invoke(e, te);
        }
    }

    public enum TypeException
    {
        Ethernet,
        LocalDataBase
    }
}
