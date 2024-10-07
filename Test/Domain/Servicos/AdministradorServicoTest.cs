using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos
{
    [TestClass]
    public class AdministradorServicoTest
    {
        private DbContexto CriarContextoDeText()
        {
            var assemblePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblePath, "..", "..", ".."));
            //Configura o ConfigurationBuild
            var builder =  new ConfigurationBuilder().SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new DbContexto(configuration);
        }
        [TestMethod]
        public void TestandoSalvarAdm()
        {
            //arrange
            var contexto = CriarContextoDeText();
            contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "test@gmail.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";
            
            var admServico = new AdministradorServicos(contexto);

            //act
            admServico.Incluir(adm);

            //assert
            Assert.AreEqual(1, admServico.Todos(1).Count());
        }

        [TestMethod]
        public void TestandoBuscarAdm()
        {
            //arrange
            var contexto = CriarContextoDeText();
            contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "test@gmail.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";
            
            var admServico = new AdministradorServicos(contexto);

            //act
            admServico.Incluir(adm);
            var admDoBanco = admServico.BuscarPorId(adm.Id);

            //assert
            Assert.AreEqual(1, admDoBanco?.Id);
        }
    }
}