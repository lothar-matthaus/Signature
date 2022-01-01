using System.ComponentModel.DataAnnotations;

namespace Signature.Entity.Enum
{
	public enum MessageType
	{
		[Display(Name = "Assinada")]
		Signed = 1,
		[Display(Name = "Não assinada")]
		Unsigned = 2
	}
}
