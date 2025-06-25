using BibliotecasAPI.Utils.Validation;
using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Utils.Validation
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        [DataRow("")]
        [DataRow("           ")]
        [DataRow(null)]
        [DataRow("Felipe")]
        public void IsValid_RetornaExitoso_SiValueNoTienePrimeraLetraMinuscula(string value)
        {
            //Preparación
            PrimeraLetraMayusculaAttribute primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
            ValidationContext validationContext = new ValidationContext(new object());

            //Prueba
            ValidationResult? resultado = primeraLetraMayusculaAttribute.GetValidationResult(value, validationContext);

            //Verificación

            Assert.AreEqual(expected: ValidationResult.Success, actual: resultado);
        }

        [TestMethod]
        [DataRow("felipe")]        
        public void IsValid_RetornaError_SiLaPrimeraLetraMinuscula(string value)
        {
            //Preparación
            PrimeraLetraMayusculaAttribute primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
            ValidationContext validationContext = new ValidationContext(new object());

            //Prueba
            ValidationResult? resultado = primeraLetraMayusculaAttribute.GetValidationResult(value, validationContext);

            //Verificación

            Assert.AreEqual(expected: "La primera letra debe ser mayúscula", actual: resultado!.ErrorMessage);
        }
    }
}
