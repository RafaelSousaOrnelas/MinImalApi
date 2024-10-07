using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class VeiculosTeste
    {
        [TestMethod]
        public void TestarGetSetPropiedades()
        {
            var veiculo = new Veiculos();

            veiculo.Id = 1;
            veiculo.Nome = "Teste";
            veiculo.Marca = "Ford";
            veiculo.Ano = 2000;
            
            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("Ford", veiculo.Marca);
            Assert.AreEqual("Teste", veiculo.Nome);
            Assert.AreEqual(2000, veiculo.Ano);
        }
    }
}