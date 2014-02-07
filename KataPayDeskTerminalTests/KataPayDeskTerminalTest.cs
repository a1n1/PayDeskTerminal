using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KataPayDeskTerminal;
using NUnit.Framework;

namespace KataPayDeskTerminalTests
{
    [NUnit.Framework.TestFixture]
    public class KataPayDeskTerminalTest
    {
        const string TestFileWithCorrectPrices = @"TestData/CorrectFileWithPrices.txt";
        const string TestFileDemoPrices = @"TestData/TestDemoPrices.txt";
        const string DiscountCards = @"TestData/discountCards.xml";
        const string DuplicateProduct = @"TestData/DuplicateProduct.txt";
       
        [Test]
        public void Should_Init()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileWithCorrectPrices, ';', DiscountCards);
            const int actualProductCount = 3;
            Assert.That(payDeskTerminal.Products.Count, Is.EqualTo(actualProductCount));
            Assert.That(payDeskTerminal.DiscountCard.CardId, Is.EqualTo("defaultCard"));
            Assert.That(payDeskTerminal.DiscountCard.DiscountPercent, Is.EqualTo(0));
        }

        [Test]
        public void Should_Set_Prices_And_Read_DiscountCard()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileWithCorrectPrices, ';', DiscountCards);
            const int actualProductCount = 3;
            string cardId = "d2345";
            payDeskTerminal.UseDiscountCard(cardId);
            Assert.That(payDeskTerminal.Products.Count, Is.EqualTo(actualProductCount));
            Assert.That(payDeskTerminal.DiscountCard.DiscountPercent,Is.EqualTo(3));
            Assert.That(payDeskTerminal.DiscountCard.CardId, Is.EqualTo(cardId));
        }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection1()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';', String.Empty);
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("B");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("D");
            double actualTotalPrice = 7.25;
            Assert.That(payDeskTerminal.PurchasesValue, Is.EqualTo(actualTotalPrice));
           
        }

        [Test]
        public void Should_Close_Purchase_On_Calculate()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';',DiscountCards);
            payDeskTerminal.UseDiscountCard("eq34rt");
            var cardPurchasesSum = payDeskTerminal.DiscountCard.PurchasesSum;
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("B");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("D");
            payDeskTerminal.Calculate();
            payDeskTerminal.Scan("D");
            payDeskTerminal.Calculate();
            double actualTotalPriceWithoutDiscounts = 7.25;
            Assert.That(payDeskTerminal.DiscountCard.PurchasesSum, Is.EqualTo(cardPurchasesSum + actualTotalPriceWithoutDiscounts));
        }

        [Test]
        public void Should_Not_Read_Another_DiscountCard()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';', DiscountCards);
            payDeskTerminal.UseDiscountCard("eq34rt");
            payDeskTerminal.UseDiscountCard("z56rt");
            Assert.That(payDeskTerminal.DiscountCard.CardId, Is.EqualTo("eq34rt"));

        }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection2()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';', String.Empty);
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("B");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("D");
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("B");
            payDeskTerminal.Scan("A");
            double actualTotalPrice = 13.25;
            Assert.That(payDeskTerminal.PurchasesValue, Is.EqualTo(actualTotalPrice));

        }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection3()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';', String.Empty);
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            double actualTotalPrice = 6.00;
            Assert.That(payDeskTerminal.PurchasesValue, Is.EqualTo(actualTotalPrice));
         }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection_With_DiscountCard()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';',DiscountCards);
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.UseDiscountCard("z56rt");
            var cardPurchasesSum = payDeskTerminal.DiscountCard.PurchasesSum;
            payDeskTerminal.Calculate();
            double actualTotalPrice = 5.95;
            double actualTotalPriceWithoutDiscounts = 6.00;
            Assert.That(payDeskTerminal.PurchasesValue, Is.EqualTo(actualTotalPrice));
            Assert.That(payDeskTerminal.DiscountCard.PurchasesSum, Is.EqualTo(cardPurchasesSum + actualTotalPriceWithoutDiscounts));
        }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection_With_DiscountCard_And_Bulk_And_Simple_Products()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';', DiscountCards);
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("B");
            payDeskTerminal.Scan("B");
            payDeskTerminal.UseDiscountCard("z56rt");
            var cardPurchasesSum = payDeskTerminal.DiscountCard.PurchasesSum;
            payDeskTerminal.Calculate();
            double actualTotalPrice = 11.075;
            double actualTotalPriceWithoutDiscounts = 11.50;
            Assert.That(payDeskTerminal.PurchasesValue, Is.EqualTo(actualTotalPrice));
            Assert.That(payDeskTerminal.DiscountCard.PurchasesSum, Is.EqualTo(cardPurchasesSum + actualTotalPriceWithoutDiscounts));
        }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection_With_Only_Bulk_Quantity()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';', DiscountCards);
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("A");
            payDeskTerminal.UseDiscountCard("z56rt");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("C");
            var cardPurchasesSum = payDeskTerminal.DiscountCard.PurchasesSum;
            payDeskTerminal.Calculate();
            double actualTotalPrice = 8;
            double actualTotalPriceWithoutDiscounts = 8;
            Assert.That(payDeskTerminal.PurchasesValue, Is.EqualTo(actualTotalPrice));
            Assert.That(payDeskTerminal.DiscountCard.PurchasesSum, Is.EqualTo(cardPurchasesSum + actualTotalPriceWithoutDiscounts));
        }

        [Test]
        public void Should_Throw_ProductUniqnessException()
        {
            Assert.Throws<DuplicateProductException>(() => new PayDeskTerminal(DuplicateProduct, ';', String.Empty));
        }

        [Test]
        public void Should_Throw_ProductNotExistException()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';', String.Empty);
            Assert.Throws<ProductNotExistException>(() => payDeskTerminal.Scan("DY"));
        }

        [Test]
        public void Should_Create_Xml_With_DiscountCards()
        {
            List <DiscountCard> discountCards = new List<DiscountCard>
                {
                    new DiscountCard() {CardId = "d2345", DiscountPercent = 3, PurchasesSum = 0},
                    new DiscountCard() {CardId = "e34758", DiscountPercent = 3, PurchasesSum = 235.49},
                    new DiscountCard() {CardId = "z56rt", DiscountPercent = 5, PurchasesSum = 2567.95},
                    new DiscountCard() {CardId = "eq34rt", DiscountPercent = 3, PurchasesSum = 0}
                };
            DiscountCardRepository.WriteDiscountCards(discountCards,"discountCards_init.xml");
        }
        
    }
}
