using System.Text.Json;
using Botticelli.Pay.Models;
using Botticelli.Shared.Utils;

namespace Botticelli.Pay.Utils;

public static class CurrencySelector
{
    static readonly Dictionary<string, Currency> Currencies;
    
    static CurrencySelector()
    {
        if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "Currencies", "currencies.json")))
            throw new FileNotFoundException("currencies.json could not be found!");
        
        var currenciesJson = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Currencies"));

        Currencies = JsonSerializer.Deserialize<Dictionary<string, Currency>>(currenciesJson)!;
        
        Currencies.NotNullOrEmpty();
    }

    public static Currency SelectCurrency(string currency3Letters) => Currencies[currency3Letters];
}