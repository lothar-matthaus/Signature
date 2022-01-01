using System.ComponentModel.DataAnnotations;

namespace Signature.Entity.Enum
{
	public enum MessageType
	{
		[Display(Name = "Assinado")]
		Signed = 1,
		[Display(Name = "Não assinado")]
		Unsigned = 2
	}
}
