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
        const string DuplicateProduct = @"TestData/DuplicateProduct.txt";
        [Test]
        public void Should_Set_Prices()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileWithCorrectPrices, ';');
            const int actualProductCount = 3;
            Assert.That(payDeskTerminal.Products.Count, Is.EqualTo(actualProductCount));
        }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection1()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';');
            payDeskTerminal.Scan("A");
            payDeskTerminal.Scan("B");
            payDeskTerminal.Scan("C");
            payDeskTerminal.Scan("D");
            double actualTotalPrice = 7.25;
            Assert.That(payDeskTerminal.PurchasesValue, Is.EqualTo(actualTotalPrice));
           
        }

        [Test]
        public void Should_Correct_CalculateTotal_ProductCollection2()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';');
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
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';');
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
        public void Should_Throw_ProductUniqnessException()
        {
            Assert.Throws<DuplicateProductException>(() => new PayDeskTerminal(DuplicateProduct, ';'));
        }

        [Test]
        public void Should_Throw_ProductNotExistException()
        {
            var payDeskTerminal = new PayDeskTerminal(TestFileDemoPrices, ';');
            Assert.Throws<ProductNotExistException>(() => payDeskTerminal.Scan("DY"));
        }
    }
}
