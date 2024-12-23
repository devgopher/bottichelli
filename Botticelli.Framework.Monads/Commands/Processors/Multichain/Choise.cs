using Microsoft.Extensions.Options;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

/// <summary>
/// Choise is a multivalue type, that can hold only one initialized value
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class Choise<T1, T2> : IChoise
{
    protected short Valid = 0;
    private readonly T1? _item1;
    private readonly T2? _item2;

    public Choise() => None = true;

    /// <summary>
    /// Choise is a multivalue type, that can hold only one initialized value
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public Choise(T1? item1, T2? item2)
    {
        _item1 = item1;
        _item2 = item2;
        
        None = _item1 != null && _item2 != null;
    }

    public bool None { get; set; } = true;

    public virtual void ValidateOptions()
    {
        if (_item1 != null)
            ++Valid;
        
        if (_item2 != null)
            ++Valid;
        
        FinalValidCheck();
    }

    public virtual object? GetValue()
    {
        if (_item1 != null)
            return _item1;
        
        return _item2;
    }

    protected void FinalValidCheck()
    {
        if (Valid != 1)
            throw new ArgumentException("Choise must contain only one value!");
    }

    /// <summary>
    /// Command Id
    /// </summary>
    public Guid Id { get; }
}

/// <inheritdoc />
public class Choise<T1, T2, T3> : Choise<T1, T2>
{
    private readonly T3? _item3;

    /// <inheritdoc />
    public Choise(T1? item1, T2? item2, T3? item3) : base(item1, item2)
    {
        _item3 = item3;
        
        None = None && item3 != null;
    }

    public override void ValidateOptions()
    {
        base.ValidateOptions();
        
        if (_item3 != null)
            ++Valid;

        FinalValidCheck();
    }
    
    public override object? GetValue()
    {
        var baseVal = base.GetValue();
        
        return baseVal ?? _item3;
    }
}

/// <inheritdoc />
public class Choise<T1, T2, T3, T4> : Choise<T1, T2, T3>
{
    private readonly T4? _item4;

    /// <inheritdoc />
    public Choise(T1? item1, T2? item2, T3? item3, T4? item4) : base(item1, item2, item3)
    {
        _item4 = item4;
        
        None = None && item4 != null;
    }

    public override void ValidateOptions()
    {
        if (_item4 != null)
            ++Valid;

        FinalValidCheck();
    }
    
    public override object? GetValue()
    {
        var baseVal = base.GetValue();
        
        return baseVal ?? _item4;
    }
}