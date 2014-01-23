using System;
using System.Collections.Generic;

namespace KataPayDeskTerminal
{

    public class ProductNotExistException : KeyNotFoundException
    {
        public ProductNotExistException()
        {
        }
        public ProductNotExistException(string message)
            : base(message)
        {
        }
        public ProductNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class PricesValidationException : ArgumentException
    {
        public PricesValidationException()
        {

        }

        public PricesValidationException(string message)
            : base(message)
        {

        }
        public PricesValidationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
    public class DuplicateProductException : ArgumentException
    {
        public DuplicateProductException()
        {

        }

        public DuplicateProductException(string message)
            : base(message)
        {

        }
        public DuplicateProductException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
