﻿using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

public class ConfigDecryptJsonConfig : Config<ConfigDecryptJsonConfig>
{
    public string FirstName { get; set; }
    public int Age { get; set; }
    public string Password { get; set; }
    public string Password2 { get; set; }

    public string PasswordDecrypt { get; set; }
    public string PasswordDecrypted { get; set; }

    [ConfigDecrypt(nameof(Password))]
    public string PasswordDecByAttrib { get; set; }

    [ConfigDecrypt(nameof(Password2))]
    public string Password2DecByAttrib { get; set; }
}