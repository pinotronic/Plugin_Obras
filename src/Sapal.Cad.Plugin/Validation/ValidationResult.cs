using System.Collections.Generic;
using System.Linq;

namespace Sapal.Cad.Plugin.Validation
{
    public class ValidationResult
    {
        private readonly List<string> _errors = new List<string>();

        public bool IsValid
        {
            get { return !_errors.Any(); }
        }

        public IReadOnlyList<string> Errors
        {
            get { return _errors; }
        }

        public void AddError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                _errors.Add(message);
            }
        }
    }
}
