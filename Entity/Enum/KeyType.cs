using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signature.Entity.Enum
{
	public enum KeyType
	{
		[Display(Name = "Pública")]
		PublicKey = 1,
		[Display(Name = "Privada")]
		PrivateKey = 2
	}
}
