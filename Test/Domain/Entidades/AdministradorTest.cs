using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropiedades()
    {
        //arrange
        var adm = new Administrador();

        //act
        adm.Id = 1;
        adm.Email = "teste@gmail.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        //assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@gmail.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }
}