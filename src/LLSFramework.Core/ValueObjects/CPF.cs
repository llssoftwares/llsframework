namespace LLSFramework.Core.ValueObjects;

/// <summary>
/// Represents a CPF (Cadastro de Pessoas Físicas) value object.
/// Encapsulates formatting, normalization, and validation logic for Brazilian individual taxpayer identifiers.
/// </summary>
public class CPF(string value) : ValueObject
{
    // Stores only the numeric digits of the CPF value.
    private readonly string _numericOnly = value.NumericOnly() ?? string.Empty;

    /// <summary>
    /// Gets the CPF formatted as "###.###.###-##".
    /// </summary>
    public string Formatted => _numericOnly.ApplyMask("###.###.###-##") ?? string.Empty;

    /// <summary>
    /// Gets the normalized CPF, containing only numeric digits.
    /// </summary>
    public string Normalized => _numericOnly;

    /// <summary>
    /// Validates the CPF according to official rules.
    /// Checks digit calculation, length, and repeated digits.
    /// </summary>
    /// <returns>True if the CPF is valid; otherwise, false.</returns>
    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(_numericOnly)) return false;

        // CPF must have 11 digits and not be a sequence of the same digit
        if (_numericOnly!.Length != 11 || _numericOnly.Distinct().Count() == 1) return false;

        var totalDigit1 = 0;
        var totalDigit2 = 0;

        var cpfArray = new int[11];

        // Convert each character to an integer
        for (var i = 0; i < _numericOnly.Length; i++)
            cpfArray[i] = int.Parse(Convert.ToString(_numericOnly[i]));

        // Calculate the first and second check digits
        for (var position = 0; position < cpfArray.Length - 2; position++)
        {
            totalDigit1 += cpfArray[position] * (10 - position);
            totalDigit2 += cpfArray[position] * (11 - position);
        }

        var mod1 = totalDigit1 % 11;
        mod1 = mod1 < 2 ? 0 : 11 - mod1;

        if (cpfArray[9] != mod1) return false;

        totalDigit2 += mod1 * 2;

        var mod2 = totalDigit2 % 11;
        mod2 = mod2 < 2 ? 0 : 11 - mod2;

        return cpfArray[10] == mod2;
    }

    /// <summary>
    /// Provides the components used to determine equality for this value object.
    /// Only the normalized CPF is considered for equality.
    /// </summary>
    /// <returns>An enumerable containing the normalized CPF.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Normalized;
    }
}