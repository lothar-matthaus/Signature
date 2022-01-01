using System.ComponentModel.DataAnnotations;

namespace Signature.Entity.Enum
{
	public enum PersonType
	{
		[Display(Name = "Pessoa Física")]
		Individual = 1,
		[Display(Name = "Pessoa Jurídica")]
		Company = 2
	}
}
