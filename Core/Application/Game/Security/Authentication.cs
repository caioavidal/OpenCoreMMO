using System;
using System.Text;

public class Authentication
{
    public string Account { get; private set; }
    public string Password { get; private set; }

    public Login(byte[] loginData)
    {
        LoadAccount(loginData);
    }

    private void LoadAccount(byte[] loginData)
    {
        var accountSize = BitConverter.ToUInt16(loginData[16..18]);
        Account = Encoding.UTF8.GetString(loginData[18..(18 + accountSize)]);
    }

    private void Login(){
        
    }
}