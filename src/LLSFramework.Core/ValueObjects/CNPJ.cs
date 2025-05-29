namespace LLSFramework.Core.ValueObjects;

/// <summary>
/// Represents a CNPJ (Cadastro Nacional da Pessoa Jurídica) value object.
/// Encapsulates formatting, normalization, and validation logic for Brazilian company identifiers.
/// </summary>
public class CNPJ(string value) : ValueObject
{
    // Stores only the numeric digits of the CNPJ value.
    private readonly string _numericOnly = value.NumericOnly() ?? string.Empty;

    /// <summary>
    /// Gets the CNPJ formatted as "##.###.###/####-##".
    /// </summary>
    public string Formatted => _numericOnly.ApplyMask("##.###.###/####-##") ?? string.Empty;

    /// <summary>
    /// Gets the normalized CNPJ, containing only numeric digits.
    /// </summary>
    public string Normalized => _numericOnly;

    /// <summary>
    /// Validates the CNPJ according to official rules.
    /// Checks digit calculation and structure.
    /// </summary>
    /// <returns>True if the CNPJ is valid; otherwise, false.</returns>
    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(_numericOnly)) return false;

        const string cnpjWeightPattern = "6543298765432";
        var digits = new int[14];
        var sum = new int[2];
        sum[0] = 0;
        sum[1] = 0;
        var result = new int[2];
        result[0] = 0;
        result[1] = 0;
        var cnpjOk = new bool[2];
        cnpjOk[0] = false;
        cnpjOk[1] = false;

        try
        {
            int nrDig;
            // Parse each digit and calculate the sums for check digits
            for (nrDig = 0; nrDig < 14; nrDig++)
            {
                digits[nrDig] = int.Parse(_numericOnly.Substring(nrDig, 1));

                if (nrDig <= 11)
                    sum[0] += digits[nrDig] * int.Parse(cnpjWeightPattern.Substring(nrDig + 1, 1));

                if (nrDig <= 12)
                    sum[1] += digits[nrDig] * int.Parse(cnpjWeightPattern.Substring(nrDig, 1));
            }

            // Calculate and validate the two check digits
            for (nrDig = 0; nrDig < 2; nrDig++)
            {
                result[nrDig] = sum[nrDig] % 11;

                cnpjOk[nrDig] = result[nrDig] == 0 || result[nrDig] == 1 
                    ? digits[12 + nrDig] == 0 
                    : digits[12 + nrDig] == 11 - result[nrDig];
            }

            return cnpjOk[0] && cnpjOk[1];
        }
        catch
        {
            // Return false if any parsing or calculation fails
            return false;
        }
    }

    /// <summary>
    /// Provides the components used to determine equality for this value object.
    /// Only the normalized CNPJ is considered for equality.
    /// </summary>
    /// <returns>An enumerable containing the normalized CNPJ.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Normalized;
    }
}