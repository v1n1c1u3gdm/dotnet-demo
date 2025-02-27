using System.Text.Json;

namespace api.Extensions;

public class LowerCaseNamingPolicy: JsonNamingPolicy
{
  public override string ConvertName(string name)
  {
      if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
          return name;

      return name.ToLower();
  }
}