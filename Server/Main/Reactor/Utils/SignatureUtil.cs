using System.Security.Cryptography;
using System.Text;

namespace Server.Main.Reactor.Utils;

public static class SignatureUtil
{
  public static string CreateHash(this StringBuilder input)
  {
    var inputStringBuilder = new StringBuilder(input.ToString());
    var hash = MD5.HashData(Encoding.ASCII.GetBytes(inputStringBuilder.ToString()));
    var stringBuilder = new StringBuilder();
    for (int i = 0; i < hash.Length; i++)
    {
      stringBuilder.Append(hash[i].ToString("x2"));
    }
    return stringBuilder.ToString();
  }
}
