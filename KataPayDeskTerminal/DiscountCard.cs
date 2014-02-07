using System;
using System.Xml.Serialization;

namespace KataPayDeskTerminal
{
    [Serializable]
    public class DiscountCard
    {
        public string CardId;
        public ushort DiscountPercent;

        [XmlIgnore]
        public double DiscountMultiplicator
        {
            get { return 1 - Convert.ToDouble(DiscountPercent) / 100; }
        }

        public double PurchasesSum;
    }
}
