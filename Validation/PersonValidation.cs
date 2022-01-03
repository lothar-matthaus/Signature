using System;
using Signature.Entity;

namespace Signature.Validation {
    static class PersonValidation {
        public static bool ValidatePersonData(Person person) {
            if (person.Name.Length == 0)
                throw new Exception("O nome da pessoa não é valido.\n");
            if (person.DocumentNumber.Length != 11)
               if (person.DocumentNumber.Length != 14)
                    throw new Exception("O documento não é válido.\n");
            return true;
        }
    }
}