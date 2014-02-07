using System.Collections.Generic;

namespace KataPayDeskTerminal
{
    public class Product
    {
        public string ProductName;
        public double PricePerUnit;
        public KeyValuePair<int, double> PackPrice;
        public bool PackPriceAvailable = true;
    }
}
