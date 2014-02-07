using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KataPayDeskTerminal
{
    public static class DiscountCardRepository
    {
        public static List<DiscountCard> GetDiscountCards(string pathToCardStorage)
        {
            List<DiscountCard> discountCards;
            XmlSerializer deserializer = new XmlSerializer(typeof(List<DiscountCard>));
            using (TextReader textReader = new StreamReader(pathToCardStorage))
            {
                discountCards = (List<DiscountCard>)deserializer.Deserialize(textReader);
            }
            return discountCards;
        }
        
        public static void WriteDiscountCards(List<DiscountCard> discountCards, string pathToCardStorage)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<DiscountCard>));
            using (var textWriter = new StreamWriter(pathToCardStorage))
            {
                serializer.Serialize(textWriter, discountCards);
            }
        }
    }
}
