using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace DotnetDemo.Persistence.Types;

public class StringListJsonType : IUserType
{
    private static readonly SqlType[] Types = { SqlTypeFactory.GetString(int.MaxValue) };

    public bool IsMutable => true;

    public Type ReturnedType => typeof(IList<string>);

    public SqlType[] SqlTypes => Types;

    public object? DeepCopy(object? value)
    {
        if (value is IList<string> list)
        {
            return new List<string>(list);
        }

        return new List<string>();
    }

    public object? Disassemble(object? value) => DeepCopy(value);

    public object? Assemble(object? cached, object? owner) => DeepCopy(cached);

    public object? Replace(object? original, object? target, object? owner) => DeepCopy(original);

    public new bool Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        var left = x as IList<string> ?? new List<string>();
        var right = y as IList<string> ?? new List<string>();

        if (left.Count != right.Count)
        {
            return false;
        }

        for (var i = 0; i < left.Count; i++)
        {
            if (!string.Equals(left[i], right[i], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(object x)
    {
        if (x is not IList<string> list)
        {
            return 0;
        }

        unchecked
        {
            return list.Aggregate(17, (current, item) => current * 23 + (item?.GetHashCode() ?? 0));
        }
    }

    public object? NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object? owner)
    {
        ArgumentNullException.ThrowIfNull(names);

        var raw = (string?)NHibernateUtil.String.NullSafeGet(rs, names[0], session, owner);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(raw) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public void NullSafeSet(DbCommand cmd, object? value, int index, ISessionImplementor session)
    {
        var list = value as IList<string> ?? new List<string>();
        var serialized = JsonSerializer.Serialize(list);
        NHibernateUtil.String.NullSafeSet(cmd, serialized, index, session);
    }
}

